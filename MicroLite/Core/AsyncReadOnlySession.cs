// -----------------------------------------------------------------------
// <copyright file="AsyncReadOnlySession.cs" company="MicroLite">
// Copyright 2012 - 2016 Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Core
{
#if NET_4_5

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

    /// <summary>
    /// The default implementation of <see cref="IAsyncReadOnlySession" />.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ConnectionScope: {ConnectionScope}")]
    internal class AsyncReadOnlySession : SessionBase, IAsyncReadOnlySession, IIncludeSession, IAdvancedAsyncReadOnlySession
    {
        private readonly Queue<Include> includes = new Queue<Include>();
        private readonly Queue<SqlQuery> queries = new Queue<SqlQuery>();
        private readonly ISqlDialect sqlDialect;

        internal AsyncReadOnlySession(
            ConnectionScope connectionScope,
            ISqlDialect sqlDialect,
            IDbDriver sqlDriver)
            : base(connectionScope, sqlDriver)
        {
            this.sqlDialect = sqlDialect;
        }

        public IAdvancedAsyncReadOnlySession Advanced
        {
            get
            {
                return this;
            }
        }

        public IIncludeSession Include
        {
            get
            {
                return this;
            }
        }

        protected ISqlDialect SqlDialect
        {
            get
            {
                return this.sqlDialect;
            }
        }

        public Task ExecutePendingQueriesAsync()
        {
            return this.ExecutePendingQueriesAsync(CancellationToken.None);
        }

        public async Task ExecutePendingQueriesAsync(CancellationToken cancellationToken)
        {
            if (Log.IsDebug)
            {
                Log.Debug(LogMessages.Session_ExecutingQueries, this.queries.Count.ToString(CultureInfo.InvariantCulture));
            }

            try
            {
                if (this.DbDriver.SupportsBatchedQueries && this.queries.Count > 1)
                {
                    await this.ExecuteQueriesCombinedAsync(cancellationToken).ConfigureAwait(false);
                }
                else
                {
                    await this.ExecuteQueriesIndividuallyAsync(cancellationToken).ConfigureAwait(false);
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
                this.includes.Clear();
                this.queries.Clear();
            }
        }

        public Task<IList<T>> FetchAsync<T>(SqlQuery sqlQuery)
        {
            return this.FetchAsync<T>(sqlQuery, CancellationToken.None);
        }

        public async Task<IList<T>> FetchAsync<T>(SqlQuery sqlQuery, CancellationToken cancellationToken)
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            var include = new IncludeMany<T>();

            this.includes.Enqueue(include);
            this.queries.Enqueue(sqlQuery);

            await this.ExecutePendingQueriesAsync(cancellationToken).ConfigureAwait(false);

            return include.Values;
        }

        IIncludeMany<T> IIncludeSession.All<T>()
        {
            var sqlQuery = new SelectSqlBuilder(this.SqlDialect.SqlCharacters)
                .From(typeof(T))
                .ToSqlQuery();

            var include = this.Include.Many<T>(sqlQuery);

            return include;
        }

        IIncludeMany<T> IIncludeSession.Many<T>(SqlQuery sqlQuery)
        {
            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            var include = new IncludeMany<T>();

            this.includes.Enqueue(include);
            this.queries.Enqueue(sqlQuery);

            return include;
        }

        IInclude<T> IIncludeSession.Scalar<T>(SqlQuery sqlQuery)
        {
            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            var include = new IncludeScalar<T>();

            this.includes.Enqueue(include);
            this.queries.Enqueue(sqlQuery);

            return include;
        }

        IInclude<T> IIncludeSession.Single<T>(object identifier)
        {
            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }

            var objectInfo = ObjectInfo.For(typeof(T));

            var sqlQuery = this.SqlDialect.BuildSelectSqlQuery(objectInfo, identifier);

            var include = this.Include.Single<T>(sqlQuery);

            return include;
        }

        IInclude<T> IIncludeSession.Single<T>(SqlQuery sqlQuery)
        {
            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            var include = new IncludeSingle<T>();

            this.includes.Enqueue(include);
            this.queries.Enqueue(sqlQuery);

            return include;
        }

        public Task<PagedResult<T>> PagedAsync<T>(SqlQuery sqlQuery, PagingOptions pagingOptions)
        {
            return this.PagedAsync<T>(sqlQuery, pagingOptions, CancellationToken.None);
        }

        public async Task<PagedResult<T>> PagedAsync<T>(SqlQuery sqlQuery, PagingOptions pagingOptions, CancellationToken cancellationToken)
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            if (pagingOptions == PagingOptions.None)
            {
                throw new MicroLiteException(ExceptionMessages.Session_PagingOptionsMustNotBeNone);
            }

            var includeCount = new IncludeScalar<int>();
            this.includes.Enqueue(includeCount);

            var countSqlQuery = this.SqlDialect.CountQuery(sqlQuery);
            this.queries.Enqueue(countSqlQuery);

            var includeMany = new IncludeMany<T>();
            this.includes.Enqueue(includeMany);

            var pagedSqlQuery = this.SqlDialect.PageQuery(sqlQuery, pagingOptions);
            this.queries.Enqueue(pagedSqlQuery);

            await this.ExecutePendingQueriesAsync(cancellationToken).ConfigureAwait(false);

            var page = (pagingOptions.Offset / pagingOptions.Count) + 1;

            return new PagedResult<T>(page, includeMany.Values, pagingOptions.Count, includeCount.Value);
        }

        public Task<T> SingleAsync<T>(object identifier)
            where T : class, new()
        {
            return this.SingleAsync<T>(identifier, CancellationToken.None);
        }

        public async Task<T> SingleAsync<T>(object identifier, CancellationToken cancellationToken)
            where T : class, new()
        {
            this.ThrowIfDisposed();

            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }

            var include = this.Include.Single<T>(identifier);

            await this.ExecutePendingQueriesAsync(cancellationToken).ConfigureAwait(false);

            return include.Value;
        }

        public Task<T> SingleAsync<T>(SqlQuery sqlQuery)
        {
            return this.SingleAsync<T>(sqlQuery, CancellationToken.None);
        }

        public async Task<T> SingleAsync<T>(SqlQuery sqlQuery, CancellationToken cancellationToken)
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            var include = new IncludeSingle<T>();

            this.includes.Enqueue(include);
            this.queries.Enqueue(sqlQuery);

            await this.ExecutePendingQueriesAsync(cancellationToken).ConfigureAwait(false);

            return include.Value;
        }

        private async Task ExecuteQueriesCombinedAsync(CancellationToken cancellationToken)
        {
            var combinedSqlQuery = this.queries.Count == 2
                ? this.DbDriver.Combine(this.queries.Dequeue(), this.queries.Dequeue())
                : this.DbDriver.Combine(this.queries);

            this.ConfigureCommand(combinedSqlQuery);

            try
            {
                var command = (DbCommand)this.Command;

                using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                {
                    do
                    {
                        var include = this.includes.Dequeue();
                        await include.BuildValueAsync(reader, cancellationToken).ConfigureAwait(false);
                    }
                    while (reader.NextResult());
                }
            }
            finally
            {
                this.CommandCompleted();
            }
        }

        private async Task ExecuteQueriesIndividuallyAsync(CancellationToken cancellationToken)
        {
            do
            {
                var sqlQuery = this.queries.Dequeue();

                this.ConfigureCommand(sqlQuery);

                try
                {
                    var command = (DbCommand)this.Command;

                    using (var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false))
                    {
                        var include = this.includes.Dequeue();
                        await include.BuildValueAsync(reader, cancellationToken).ConfigureAwait(false);
                    }
                }
                finally
                {
                    this.CommandCompleted();
                }
            }
            while (this.queries.Count > 0);
        }
    }

#endif
}