// -----------------------------------------------------------------------
// <copyright file="Session.cs" company="MicroLite">
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
    using MicroLite.FrameworkExtensions;
    using MicroLite.Listeners;
    using MicroLite.Logging;

    /// <summary>
    /// The default implementation of <see cref="ISession"/>.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Session {id}")]
    internal sealed class Session : ISession, IAdvancedSession, IIncludeSession
    {
        private static readonly ILog log = LogManager.GetLog("MicroLite.Session");
        private readonly IConnectionManager connectionManager;
        private readonly string id = Guid.NewGuid().ToString();
        private readonly Queue<Include> includes = new Queue<Include>();
        private readonly IEnumerable<IListener> listeners;
        private readonly IObjectBuilder objectBuilder;
        private readonly Queue<SqlQuery> queries = new Queue<SqlQuery>();
        private readonly ISqlDialect sqlDialect;
        private bool disposed;

        internal Session(
            IConnectionManager connectionManager,
            IObjectBuilder objectBuilder,
            ISqlDialect sqlDialect,
            IEnumerable<IListener> listeners)
        {
            this.connectionManager = connectionManager;
            this.objectBuilder = objectBuilder;
            this.sqlDialect = sqlDialect;
            this.listeners = listeners;

            using (new SessionLoggingContext(this.id))
            {
                log.TryLogDebug(Messages.Session_Created);
            }
        }

        public IAdvancedSession Advanced
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

        public ITransaction Transaction
        {
            get
            {
                return this.connectionManager.CurrentTransaction;
            }
        }

        IInclude<T> IIncludeSession.Single<T>(object identifier)
        {
            this.ThrowIfDisposed();

            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }

            using (new SessionLoggingContext(this.id))
            {
                var sqlQuery = this.sqlDialect.CreateQuery(StatementType.Select, typeof(T), identifier);

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

            using (new SessionLoggingContext(this.id))
            {
                var include = new IncludeSingle<T>();

                this.includes.Enqueue(include);
                this.queries.Enqueue(sqlQuery);

                return include;
            }
        }

        T ISession.Single<T>(object identifier)
        {
            using (new SessionLoggingContext(this.id))
            {
                var include = this.Include.Single<T>(identifier);

                this.ExecuteAllQueries();

                return include.Value;
            }
        }

        T ISession.Single<T>(SqlQuery sqlQuery)
        {
            using (new SessionLoggingContext(this.id))
            {
                var include = this.Include.Single<T>(sqlQuery);

                this.ExecuteAllQueries();

                return include.Value;
            }
        }

        public ITransaction BeginTransaction()
        {
            this.ThrowIfDisposed();

            using (new SessionLoggingContext(this.id))
            {
                return this.connectionManager.BeginTransaction();
            }
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            this.ThrowIfDisposed();

            using (new SessionLoggingContext(this.id))
            {
                return this.connectionManager.BeginTransaction(isolationLevel);
            }
        }

        public bool Delete(object instance)
        {
            this.ThrowIfDisposed();

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            using (new SessionLoggingContext(this.id))
            {
                this.listeners.Each(l => l.BeforeDelete(instance));

                var sqlQuery = this.sqlDialect.CreateQuery(StatementType.Delete, instance);

                this.listeners.Each(l => l.BeforeDelete(instance, sqlQuery));

                var rowsAffected = this.Execute(sqlQuery);

                this.listeners.Each(l => l.AfterDelete(instance, rowsAffected));

                return rowsAffected == 1;
            }
        }

        public bool Delete(Type type, object identifier)
        {
            this.ThrowIfDisposed();

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }

            using (new SessionLoggingContext(this.id))
            {
                var sqlQuery = this.sqlDialect.CreateQuery(StatementType.Delete, type, identifier);

                return this.Execute(sqlQuery) == 1;
            }
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                using (new SessionLoggingContext(this.id))
                {
                    this.connectionManager.Dispose();

                    log.TryLogDebug(Messages.Session_Disposed);
                    this.disposed = true;
                }
            }
        }

        public int Execute(SqlQuery sqlQuery)
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            using (new SessionLoggingContext(this.id))
            {
                try
                {
                    using (var command = this.connectionManager.BuildCommand(sqlQuery))
                    {
                        OpenConnectionIfClosed(command);

                        log.TryLogInfo(sqlQuery.CommandText);
                        var result = command.ExecuteNonQuery();

                        CloseConnectionIfNotInTransaction(command);

                        return result;
                    }
                }
                catch (Exception e)
                {
                    log.TryLogError(e.Message, e);
                    throw new MicroLiteException(e.Message, e);
                }
            }
        }

        public T ExecuteScalar<T>(SqlQuery sqlQuery)
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            using (new SessionLoggingContext(this.id))
            {
                try
                {
                    using (var command = this.connectionManager.BuildCommand(sqlQuery))
                    {
                        OpenConnectionIfClosed(command);

                        log.TryLogInfo(sqlQuery.CommandText);
                        var result = (T)command.ExecuteScalar();

                        CloseConnectionIfNotInTransaction(command);

                        return result;
                    }
                }
                catch (Exception e)
                {
                    log.TryLogError(e.Message, e);
                    throw new MicroLiteException(e.Message, e);
                }
            }
        }

        public IList<T> Fetch<T>(SqlQuery sqlQuery) where T : class, new()
        {
            using (new SessionLoggingContext(this.id))
            {
                var include = this.Include.Many<T>(sqlQuery);

                this.ExecuteAllQueries();

                return include.Values;
            }
        }

        public void Insert(object instance)
        {
            this.ThrowIfDisposed();

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            using (new SessionLoggingContext(this.id))
            {
                this.listeners.Each(l => l.BeforeInsert(instance));

                var sqlQuery = this.sqlDialect.CreateQuery(StatementType.Insert, instance);

                this.listeners.Each(l => l.BeforeInsert(instance, sqlQuery));

                var identifier = this.ExecuteScalar<object>(sqlQuery);

                this.listeners.Each(l => l.AfterInsert(instance, identifier));
            }
        }

        public IIncludeMany<T> Many<T>(SqlQuery sqlQuery) where T : class, new()
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            using (new SessionLoggingContext(this.id))
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

            using (new SessionLoggingContext(this.id))
            {
                var countSqlQuery = this.sqlDialect.CountQuery(sqlQuery);
                var pagedSqlQuery = this.sqlDialect.PageQuery(sqlQuery, page, resultsPerPage);

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

            using (new SessionLoggingContext(this.id))
            {
                try
                {
                    using (var command = this.connectionManager.BuildCommand(sqlQuery))
                    {
                        OpenConnectionIfClosed(command);

                        log.TryLogInfo(sqlQuery.CommandText);

                        var results = new List<dynamic>();

                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var expando = this.objectBuilder.BuildDynamic(reader);

                                results.Add(expando);
                            }
                        }

                        CloseConnectionIfNotInTransaction(command);

                        return results;
                    }
                }
                catch (Exception e)
                {
                    log.TryLogError(e.Message, e);
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

            using (new SessionLoggingContext(this.id))
            {
                var include = new IncludeScalar<T>();

                this.includes.Enqueue(include);
                this.queries.Enqueue(sqlQuery);

                return include;
            }
        }

        public void Update(object instance)
        {
            this.ThrowIfDisposed();

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            using (new SessionLoggingContext(this.id))
            {
                this.listeners.Each(l => l.BeforeUpdate(instance));

                var sqlQuery = this.sqlDialect.CreateQuery(StatementType.Update, instance);

                this.listeners.Each(l => l.BeforeUpdate(instance, sqlQuery));

                var rowsAffected = this.Execute(sqlQuery);

                this.listeners.Each(l => l.AfterUpdate(instance, rowsAffected));
            }
        }

        private static void CloseConnectionIfNotInTransaction(IDbCommand command)
        {
            if (command.Transaction == null)
            {
                log.TryLogDebug(Messages.Session_ClosingConnection);
                command.Connection.Close();
            }
        }

        private static void OpenConnectionIfClosed(IDbCommand command)
        {
            if (command.Connection.State == ConnectionState.Closed)
            {
                log.TryLogDebug(Messages.Session_OpeningConnection);
                command.Connection.Open();
            }
        }

        private void ExecuteAllQueries()
        {
            try
            {
                var sqlQuery = this.queries.Count == 1 ? this.queries.Peek() : SqlUtil.Combine(this.queries);

                using (var command = this.connectionManager.BuildCommand(sqlQuery))
                {
                    try
                    {
                        OpenConnectionIfClosed(command);

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
                        CloseConnectionIfNotInTransaction(command);
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
                log.TryLogError(e.Message, e);
                throw new MicroLiteException(e.Message, e);
            }
            finally
            {
                this.includes.Clear();
                this.queries.Clear();
            }
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }
    }
}