// -----------------------------------------------------------------------
// <copyright file="ReadOnlySession.cs" company="Project Contributors">
// Copyright Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using MicroLite.Builder;
using MicroLite.Dialect;
using MicroLite.Driver;
using MicroLite.Logging;
using MicroLite.Mapping;

namespace MicroLite.Core
{
    /// <summary>
    /// The default implementation of <see cref="IReadOnlySession" />.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ConnectionScope: {ConnectionScope}")]
    internal class ReadOnlySession : SessionBase, IReadOnlySession, IIncludeSession, IAdvancedReadOnlySession
    {
        private readonly Queue<Include> _includes = new Queue<Include>();
        private readonly Queue<SqlQuery> _queries = new Queue<SqlQuery>();

        internal ReadOnlySession(ConnectionScope connectionScope, ISqlDialect sqlDialect, IDbDriver sqlDriver)
            : base(connectionScope, sqlDriver)
            => SqlDialect = sqlDialect;

        public IAdvancedReadOnlySession Advanced => this;

        public IIncludeSession Include => this;

        protected ISqlDialect SqlDialect { get; }

        IIncludeMany<T> IIncludeSession.All<T>()
            => Include.Many<T>(new SelectSqlBuilder(SqlDialect.SqlCharacters).From(typeof(T)).ToSqlQuery());

        public Task ExecutePendingQueriesAsync()
            => ExecutePendingQueriesAsync(CancellationToken.None);

        public async Task ExecutePendingQueriesAsync(CancellationToken cancellationToken)
        {
            if (Log.IsDebug)
            {
                Log.Debug(LogMessages.Session_ExecutingQueries, _queries.Count.ToString(CultureInfo.InvariantCulture));
            }

            try
            {
                if (DbDriver.SupportsBatchedQueries && _queries.Count > 1)
                {
                    await ExecuteQueriesCombinedAsync(cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    await ExecuteQueriesIndividuallyAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            catch (OperationCanceledException)
            {
                // Don't re-wrap Operation Canceled exceptions
                throw;
            }
            catch (MicroLiteException)
            {
                // Don't re-wrap MicroLite exceptions
                throw;
            }
            catch (Exception e)
            {
                throw new MicroLiteException(e.Message, e);
            }
            finally
            {
                _includes.Clear();
                _queries.Clear();
            }
        }

        public Task<IList<T>> FetchAsync<T>(SqlQuery sqlQuery)
            => FetchAsync<T>(sqlQuery, CancellationToken.None);

        public Task<IList<T>> FetchAsync<T>(SqlQuery sqlQuery, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (sqlQuery is null)
            {
                throw new ArgumentNullException(nameof(sqlQuery));
            }

            return FetchAsyncInternal<T>(sqlQuery, cancellationToken);
        }

        IIncludeMany<T> IIncludeSession.Many<T>(SqlQuery sqlQuery)
        {
            if (sqlQuery is null)
            {
                throw new ArgumentNullException(nameof(sqlQuery));
            }

            var include = new IncludeMany<T>();

            _includes.Enqueue(include);
            _queries.Enqueue(sqlQuery);

            return include;
        }

        public Task<PagedResult<T>> PagedAsync<T>(SqlQuery sqlQuery, PagingOptions pagingOptions)
            => PagedAsync<T>(sqlQuery, pagingOptions, CancellationToken.None);

        public Task<PagedResult<T>> PagedAsync<T>(SqlQuery sqlQuery, PagingOptions pagingOptions, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (sqlQuery is null)
            {
                throw new ArgumentNullException(nameof(sqlQuery));
            }

            if (pagingOptions == PagingOptions.None)
            {
                throw new MicroLiteException(ExceptionMessages.Session_PagingOptionsMustNotBeNone);
            }

            return PagedAsyncInternal<T>(sqlQuery, pagingOptions, cancellationToken);
        }

        IInclude<T> IIncludeSession.Scalar<T>(SqlQuery sqlQuery)
        {
            if (sqlQuery is null)
            {
                throw new ArgumentNullException(nameof(sqlQuery));
            }

            var include = new IncludeScalar<T>();

            _includes.Enqueue(include);
            _queries.Enqueue(sqlQuery);

            return include;
        }

        IInclude<T> IIncludeSession.Single<T>(object identifier)
        {
            if (identifier is null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            IObjectInfo objectInfo = ObjectInfo.For(typeof(T));

            SqlQuery sqlQuery = SqlDialect.BuildSelectSqlQuery(objectInfo, identifier);

            return Include.Single<T>(sqlQuery);
        }

        IInclude<T> IIncludeSession.Single<T>(SqlQuery sqlQuery)
        {
            if (sqlQuery is null)
            {
                throw new ArgumentNullException(nameof(sqlQuery));
            }

            var include = new IncludeSingle<T>();

            _includes.Enqueue(include);
            _queries.Enqueue(sqlQuery);

            return include;
        }

        public Task<T> SingleAsync<T>(object identifier)
            where T : class, new()
            => SingleAsync<T>(identifier, CancellationToken.None);

        public Task<T> SingleAsync<T>(object identifier, CancellationToken cancellationToken)
            where T : class, new()
        {
            ThrowIfDisposed();

            if (identifier is null)
            {
                throw new ArgumentNullException(nameof(identifier));
            }

            return SingleAsyncInternal<T>(identifier, cancellationToken);
        }

        public Task<T> SingleAsync<T>(SqlQuery sqlQuery)
            => SingleAsync<T>(sqlQuery, CancellationToken.None);

        public Task<T> SingleAsync<T>(SqlQuery sqlQuery, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            if (sqlQuery is null)
            {
                throw new ArgumentNullException(nameof(sqlQuery));
            }

            return SingleAsyncInternal<T>(sqlQuery, cancellationToken);
        }

        private async Task ExecuteQueriesCombinedAsync(CancellationToken cancellationToken)
        {
            SqlQuery combinedSqlQuery = _queries.Count == 2
                ? DbDriver.Combine(_queries.Dequeue(), _queries.Dequeue())
                : DbDriver.Combine(_queries);

            ConfigureCommand(combinedSqlQuery);

            try
            {
                var command = (DbCommand)Command;

                using (DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                {
                    do
                    {
                        Include include = _includes.Dequeue();
                        await include.BuildValueAsync(reader, cancellationToken).ConfigureAwait(false);
                    }
                    while (reader.NextResult());
                }
            }
            finally
            {
                CommandCompleted();
            }
        }

        private async Task ExecuteQueriesIndividuallyAsync(CancellationToken cancellationToken)
        {
            do
            {
                SqlQuery sqlQuery = _queries.Dequeue();

                ConfigureCommand(sqlQuery);

                try
                {
                    var command = (DbCommand)Command;

                    using (DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                    {
                        Include include = _includes.Dequeue();
                        await include.BuildValueAsync(reader, cancellationToken).ConfigureAwait(false);
                    }
                }
                finally
                {
                    CommandCompleted();
                }
            }
            while (_queries.Count > 0);
        }

        private async Task<IList<T>> FetchAsyncInternal<T>(SqlQuery sqlQuery, CancellationToken cancellationToken)
        {
            var include = new IncludeMany<T>();

            _includes.Enqueue(include);
            _queries.Enqueue(sqlQuery);

            await ExecutePendingQueriesAsync(cancellationToken).ConfigureAwait(false);

            return include.Values;
        }

        private async Task<PagedResult<T>> PagedAsyncInternal<T>(SqlQuery sqlQuery, PagingOptions pagingOptions, CancellationToken cancellationToken)
        {
            var includeCount = new IncludeScalar<int>();
            _includes.Enqueue(includeCount);

            SqlQuery countSqlQuery = SqlDialect.CountQuery(sqlQuery);
            _queries.Enqueue(countSqlQuery);

            var includeMany = new IncludeMany<T>();
            _includes.Enqueue(includeMany);

            SqlQuery pagedSqlQuery = SqlDialect.PageQuery(sqlQuery, pagingOptions);
            _queries.Enqueue(pagedSqlQuery);

            await ExecutePendingQueriesAsync(cancellationToken).ConfigureAwait(false);

            int page = (pagingOptions.Offset / pagingOptions.Count) + 1;

            return new PagedResult<T>(page, includeMany.Values, pagingOptions.Count, includeCount.Value);
        }

        private async Task<T> SingleAsyncInternal<T>(object identifier, CancellationToken cancellationToken)
            where T : class, new()
        {
            IInclude<T> include = Include.Single<T>(identifier);

            await ExecutePendingQueriesAsync(cancellationToken).ConfigureAwait(false);

            return include.Value;
        }

        private async Task<T> SingleAsyncInternal<T>(SqlQuery sqlQuery, CancellationToken cancellationToken)
        {
            var include = new IncludeSingle<T>();

            _includes.Enqueue(include);
            _queries.Enqueue(sqlQuery);

            await ExecutePendingQueriesAsync(cancellationToken).ConfigureAwait(false);

            return include.Value;
        }
    }
}
