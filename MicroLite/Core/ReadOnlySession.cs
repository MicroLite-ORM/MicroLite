// -----------------------------------------------------------------------
// <copyright file="ReadOnlySession.cs" company="MicroLite">
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
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using MicroLite.Builder;
    using MicroLite.Dialect;
    using MicroLite.Driver;
    using MicroLite.Logging;
    using MicroLite.Mapping;

    /// <summary>
    /// The default implementation of <see cref="IReadOnlySession" />.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ConnectionScope: {ConnectionScope}")]
    internal class ReadOnlySession : SessionBase, IReadOnlySession, IIncludeSession, IAdvancedReadOnlySession
    {
        private readonly Queue<Include> includes = new Queue<Include>();
        private readonly Queue<SqlQuery> queries = new Queue<SqlQuery>();
        private readonly ISqlDialect sqlDialect;

        internal ReadOnlySession(
            ConnectionScope connectionScope,
            ISqlDialect sqlDialect,
            IDbDriver sqlDriver)
            : base(connectionScope, sqlDriver)
        {
            this.sqlDialect = sqlDialect;
        }

        public IAdvancedReadOnlySession Advanced
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

        public void ExecutePendingQueries()
        {
            if (Log.IsDebug)
            {
                Log.Debug(LogMessages.Session_ExecutingQueries, this.queries.Count.ToString(CultureInfo.InvariantCulture));
            }

            try
            {
                if (this.DbDriver.SupportsBatchedQueries && this.queries.Count > 1)
                {
                    this.ExecuteQueriesCombined();
                }
                else
                {
                    this.ExecuteQueriesIndividually();
                }
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

        public IList<T> Fetch<T>(SqlQuery sqlQuery)
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            var include = new IncludeMany<T>();

            this.includes.Enqueue(include);
            this.queries.Enqueue(sqlQuery);

            this.ExecutePendingQueries();

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

        public PagedResult<T> Paged<T>(SqlQuery sqlQuery, PagingOptions pagingOptions)
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

            this.ExecutePendingQueries();

            var page = (pagingOptions.Offset / pagingOptions.Count) + 1;

            return new PagedResult<T>(page, includeMany.Values, pagingOptions.Count, includeCount.Value);
        }

        public T Single<T>(object identifier)
            where T : class, new()
        {
            this.ThrowIfDisposed();

            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }

            var include = this.Include.Single<T>(identifier);

            this.ExecutePendingQueries();

            return include.Value;
        }

        public T Single<T>(SqlQuery sqlQuery)
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            var include = new IncludeSingle<T>();

            this.includes.Enqueue(include);
            this.queries.Enqueue(sqlQuery);

            this.ExecutePendingQueries();

            return include.Value;
        }

        private void ExecuteQueriesCombined()
        {
            var combinedSqlQuery = this.queries.Count == 2
                ? this.DbDriver.Combine(this.queries.Dequeue(), this.queries.Dequeue())
                : this.DbDriver.Combine(this.queries);

            this.ConfigureCommand(combinedSqlQuery);

            try
            {
                using (var reader = this.Command.ExecuteReader())
                {
                    do
                    {
                        var include = this.includes.Dequeue();
                        include.BuildValue(reader);
                    }
                    while (reader.NextResult());
                }
            }
            finally
            {
                this.CommandCompleted();
            }
        }

        private void ExecuteQueriesIndividually()
        {
            do
            {
                var sqlQuery = this.queries.Dequeue();

                this.ConfigureCommand(sqlQuery);

                try
                {
                    using (var reader = this.Command.ExecuteReader())
                    {
                        var include = this.includes.Dequeue();
                        include.BuildValue(reader);
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
}