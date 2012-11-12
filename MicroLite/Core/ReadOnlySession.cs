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
    /// The default implementation of <see cref="IReadOnlySession"/>.
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

        public ITransaction BeginTransaction()
        {
            this.ThrowIfDisposed();

            using (new SessionLoggingContext(this.Id))
            {
                return this.ConnectionManager.BeginTransaction();
            }
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

        public PagedResult<T> Paged<T>(SqlQuery sqlQuery, long page, long resultsPerPage) where T : class, new()
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            if (page < 1)
            {
                throw new ArgumentOutOfRangeException("page", Messages.Session_PagesStartAtOne);
            }

            if (resultsPerPage < 1)
            {
                throw new ArgumentOutOfRangeException("resultsPerPage", Messages.Session_MustHaveAtLeast1Result);
            }

            using (new SessionLoggingContext(this.Id))
            {
                var countSqlQuery = this.SqlDialect.CountQuery(sqlQuery);
                var pagedSqlQuery = this.SqlDialect.PageQuery(sqlQuery, page, resultsPerPage);

                var includeCount = this.Include.Scalar<long>(countSqlQuery);
                var includeMany = this.Include.Many<T>(pagedSqlQuery);

                this.ExecuteAllQueries();

                return new PagedResult<T>(page, includeMany.Values, resultsPerPage, includeCount.Value);
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
                    using (var command = this.ConnectionManager.BuildCommand(sqlQuery))
                    {
                        this.OpenConnectionIfClosed(command);

                        Log.TryLogInfo(sqlQuery.CommandText);

                        var results = new List<dynamic>();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var expando = this.objectBuilder.BuildDynamic(reader);

                                results.Add(expando);
                            }
                        }

                        this.CloseConnectionIfNotInTransaction(command);

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

        protected void CloseConnectionIfNotInTransaction(IDbCommand command)
        {
            if (command.Transaction == null)
            {
                Log.TryLogDebug(Messages.Session_ClosingConnection);
                command.Connection.Close();
            }
        }

        protected void OpenConnectionIfClosed(IDbCommand command)
        {
            if (command.Connection.State == ConnectionState.Closed)
            {
                Log.TryLogDebug(Messages.Session_OpeningConnection);
                command.Connection.Open();
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
                var sqlQuery = this.queries.Count == 1 ? this.queries.Peek() : SqlUtil.Combine(this.queries);

                using (var command = this.ConnectionManager.BuildCommand(sqlQuery))
                {
                    try
                    {
                        this.OpenConnectionIfClosed(command);

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
                        this.CloseConnectionIfNotInTransaction(command);
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