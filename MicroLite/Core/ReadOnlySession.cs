// -----------------------------------------------------------------------
// <copyright file="ReadOnlySession.cs" company="MicroLite">
// Copyright 2012 Trevor Pilley
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
    using System.Data;
    using MicroLite.Dialect;
    using MicroLite.Logging;

    /// <summary>
    /// The default implementation of <see cref="IReadOnlySession" />.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ReadOnlySession {Id}")]
    internal class ReadOnlySession : IReadOnlySession, IIncludeSession
    {
        protected static readonly ILog Log = LogManager.GetLog("MicroLite.Session");
        private readonly IConnectionManager connectionManager;
        private readonly string id = GuidGenerator.CreateComb().ToString();
        private readonly Queue<Include> includes = new Queue<Include>();
        private readonly IObjectBuilder objectBuilder;
        private readonly Queue<SqlQuery> queries = new Queue<SqlQuery>();
        private readonly ISqlDialect sqlDialect;
        private bool disposed;

        internal ReadOnlySession(
            IConnectionManager connectionManager,
            IObjectBuilder objectBuilder,
            ISqlDialect sqlDialect)
        {
            this.connectionManager = connectionManager;
            this.objectBuilder = objectBuilder;
            this.sqlDialect = sqlDialect;
        }

        public IIncludeSession Include
        {
            get
            {
                return this;
            }
        }

        public ITransaction Transaction
        {
            get
            {
                return this.ConnectionManager.CurrentTransaction;
            }
        }

        protected IConnectionManager ConnectionManager
        {
            get
            {
                return this.connectionManager;
            }
        }

        protected string Id
        {
            get
            {
                return this.id;
            }
        }

        protected ISqlDialect SqlDialect
        {
            get
            {
                return this.sqlDialect;
            }
        }

        public ITransaction BeginTransaction()
        {
            this.ThrowIfDisposed();

            return this.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            this.ThrowIfDisposed();

            using (new SessionLoggingContext(this.Id))
            {
                return this.ConnectionManager.BeginTransaction(isolationLevel);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public IList<T> Fetch<T>(SqlQuery sqlQuery) where T : class, new()
        {
            using (new SessionLoggingContext(this.Id))
            {
                var include = this.Include.Many<T>(sqlQuery);

                this.ExecuteAllQueries();

                return include.Values;
            }
        }

        IInclude<T> IIncludeSession.Single<T>(object identifier)
        {
            this.ThrowIfDisposed();

            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }

            using (new SessionLoggingContext(this.Id))
            {
                var sqlQuery = this.SqlDialect.CreateQuery(StatementType.Select, typeof(T), identifier);

                return this.Include.Single<T>(sqlQuery);
            }
        }

        IInclude<T> IIncludeSession.Single<T>(SqlQuery sqlQuery)
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            using (new SessionLoggingContext(this.Id))
            {
                var include = new IncludeSingle<T>();

                this.includes.Enqueue(include);
                this.queries.Enqueue(sqlQuery);

                return include;
            }
        }

        T IReadOnlySession.Single<T>(object identifier)
        {
            using (new SessionLoggingContext(this.Id))
            {
                var include = this.Include.Single<T>(identifier);

                this.ExecuteAllQueries();

                return include.Value;
            }
        }

        T IReadOnlySession.Single<T>(SqlQuery sqlQuery)
        {
            using (new SessionLoggingContext(this.Id))
            {
                var include = this.Include.Single<T>(sqlQuery);

                this.ExecuteAllQueries();

                return include.Value;
            }
        }

        public IIncludeMany<T> Many<T>(SqlQuery sqlQuery) where T : class, new()
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            using (new SessionLoggingContext(this.Id))
            {
                var include = new IncludeMany<T>();

                this.includes.Enqueue(include);
                this.queries.Enqueue(sqlQuery);

                return include;
            }
        }

        public PagedResult<T> Paged<T>(SqlQuery sqlQuery, int page, int resultsPerPage) where T : class, new()
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            return this.Paged<T>(sqlQuery, PagingOptions.ForPage(page, resultsPerPage));
        }

        public PagedResult<T> Paged<T>(SqlQuery sqlQuery, PagingOptions pagingOptions) where T : class, new()
        {
            if (pagingOptions == PagingOptions.None)
            {
                throw new MicroLiteException(Messages.Session_PagingOptionsMustNotBeNone);
            }

            using (new SessionLoggingContext(this.Id))
            {
                var countSqlQuery = this.SqlDialect.CountQuery(sqlQuery);
                var pagedSqlQuery = this.SqlDialect.PageQuery(sqlQuery, pagingOptions);

                var includeCount = this.Include.Scalar<int>(countSqlQuery);
                var includeMany = this.Include.Many<T>(pagedSqlQuery);

                this.ExecuteAllQueries();

                var page = (pagingOptions.Offset / pagingOptions.Count) + 1;

                return new PagedResult<T>(page, includeMany.Values, pagingOptions.Count, includeCount.Value);
            }
        }

#if !NET_3_5

        public IList<dynamic> Projection(SqlQuery sqlQuery)
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            using (new SessionLoggingContext(this.Id))
            {
                try
                {
                    using (var command = this.ConnectionManager.CreateCommand())
                    {
                        this.SqlDialect.BuildCommand(command, sqlQuery);

                        var results = new List<dynamic>();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var expando = this.objectBuilder.BuildDynamic(reader);

                                results.Add(expando);
                            }
                        }

                        this.connectionManager.CommandCompleted(command);

                        return results;
                    }
                }
                catch (Exception e)
                {
                    Log.TryLogError(e.Message, e);
                    throw new MicroLiteException(e.Message, e);
                }
            }
        }

#endif

        public IInclude<T> Scalar<T>(SqlQuery sqlQuery)
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            using (new SessionLoggingContext(this.Id))
            {
                var include = new IncludeScalar<T>();

                this.includes.Enqueue(include);
                this.queries.Enqueue(sqlQuery);

                return include;
            }
        }

        protected void Dispose(bool disposing)
        {
            if (disposing && !this.disposed)
            {
                using (new SessionLoggingContext(this.Id))
                {
                    this.ConnectionManager.Dispose();

                    Log.TryLogDebug(Messages.Session_Disposed);
                    this.disposed = true;
                }
            }
        }

        protected void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        private void ExecuteAllQueries()
        {
            try
            {
                var sqlQuery = this.queries.Count == 1 ? this.queries.Peek() : this.SqlDialect.Combine(this.queries);

                using (var command = this.ConnectionManager.CreateCommand())
                {
                    try
                    {
                        this.SqlDialect.BuildCommand(command, sqlQuery);

                        using (var reader = command.ExecuteReader())
                        {
                            do
                            {
                                var include = this.includes.Dequeue();
                                include.BuildValue(reader, this.objectBuilder);
                            }
                            while (reader.NextResult());
                        }
                    }
                    finally
                    {
                        this.connectionManager.CommandCompleted(command);
                    }
                }
            }
            catch (MicroLiteException)
            {
                // Don't re-wrap MicroLite exceptions
                throw;
            }
            catch (Exception e)
            {
                Log.TryLogError(e.Message, e);
                throw new MicroLiteException(e.Message, e);
            }
            finally
            {
                this.includes.Clear();
                this.queries.Clear();
            }
        }
    }
}