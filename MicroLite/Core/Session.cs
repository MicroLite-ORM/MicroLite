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
    using System.Linq;
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
        private readonly IObjectBuilder objectBuilder;
        private readonly Queue<SqlQuery> queries = new Queue<SqlQuery>();
        private readonly ISqlDialect sqlDialect;
        private bool disposed;
        private IEnumerable<IListener> listeners;

        internal Session(
            IConnectionManager connectionManager,
            IObjectBuilder objectBuilder,
            ISqlDialect sqlDialect)
        {
            this.connectionManager = connectionManager;
            this.objectBuilder = objectBuilder;
            this.sqlDialect = sqlDialect;

            log.TryLogDebug(Messages.Session_Created, this.id);
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

        private IEnumerable<IListener> Listeners
        {
            get
            {
                return this.listeners ?? (this.listeners = Listener.Listeners.ToArray());
            }
        }

        IInclude<T> IIncludeSession.Single<T>(object identifier)
        {
            this.ThrowIfDisposed();

            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }

            var sqlQuery = this.sqlDialect.SelectQuery(typeof(T), identifier);

            return this.Include.Single<T>(sqlQuery);
        }

        IInclude<T> IIncludeSession.Single<T>(SqlQuery sqlQuery)
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            var include = new IncludeSingle<T>();

            this.includes.Enqueue(include);
            this.queries.Enqueue(sqlQuery);

            return include;
        }

        T ISession.Single<T>(object identifier)
        {
            var include = this.Include.Single<T>(identifier);

            this.ExecuteAllQueries();

            return include.Value;
        }

        T ISession.Single<T>(SqlQuery sqlQuery)
        {
            var include = this.Include.Single<T>(sqlQuery);

            this.ExecuteAllQueries();

            return include.Value;
        }

        public ITransaction BeginTransaction()
        {
            this.ThrowIfDisposed();

            return this.connectionManager.BeginTransaction();
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            this.ThrowIfDisposed();

            return this.connectionManager.BeginTransaction(isolationLevel);
        }

        public bool Delete(object instance)
        {
            this.ThrowIfDisposed();

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            this.Listeners.Each(l => l.BeforeDelete(instance));

            var sqlQuery = this.sqlDialect.DeleteQuery(instance);

            this.Listeners.Each(l => l.BeforeDelete(instance, sqlQuery));

            return this.Execute(sqlQuery) == 1;
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

            var sqlQuery = this.sqlDialect.DeleteQuery(type, identifier);

            return this.Execute(sqlQuery) == 1;
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                this.connectionManager.Dispose();

                log.TryLogDebug(Messages.Session_Disposed, this.id);
                this.disposed = true;
            }
        }

        public int Execute(SqlQuery sqlQuery)
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            try
            {
                using (var command = this.connectionManager.BuildCommand(sqlQuery))
                {
                    this.OpenConnectionIfClosed(command);

                    log.TryLogInfo(sqlQuery.CommandText);
                    var result = command.ExecuteNonQuery();

                    this.CloseConnectionIfNotInTransaction(command);

                    return result;
                }
            }
            catch (Exception e)
            {
                log.TryLogError(e.Message, e);
                throw new MicroLiteException(e.Message, e);
            }
        }

        public T ExecuteScalar<T>(SqlQuery sqlQuery)
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            try
            {
                using (var command = this.connectionManager.BuildCommand(sqlQuery))
                {
                    this.OpenConnectionIfClosed(command);

                    log.TryLogInfo(sqlQuery.CommandText);
                    var result = (T)command.ExecuteScalar();

                    this.CloseConnectionIfNotInTransaction(command);

                    return result;
                }
            }
            catch (Exception e)
            {
                log.TryLogError(e.Message, e);
                throw new MicroLiteException(e.Message, e);
            }
        }

        public IList<T> Fetch<T>(SqlQuery sqlQuery) where T : class, new()
        {
            var include = this.Include.Many<T>(sqlQuery);

            this.ExecuteAllQueries();

            return include.Values;
        }

        public void Insert(object instance)
        {
            this.ThrowIfDisposed();

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            this.Listeners.Each(l => l.BeforeInsert(instance));

            var sqlQuery = this.sqlDialect.InsertQuery(instance);

            this.Listeners.Each(l => l.BeforeInsert(instance, sqlQuery));

            var identifier = this.ExecuteScalar<object>(sqlQuery);

            this.Listeners.Each(l => l.AfterInsert(instance, identifier));
        }

        public IIncludeMany<T> Many<T>(SqlQuery sqlQuery) where T : class, new()
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            var include = new IncludeMany<T>();

            this.includes.Enqueue(include);
            this.queries.Enqueue(sqlQuery);

            return include;
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

            var pagedSqlQuery = this.sqlDialect.Page(sqlQuery, page, resultsPerPage);

            var include = this.Include.Many<T>(pagedSqlQuery);

            this.ExecuteAllQueries();

            return new PagedResult<T>(page, include.Values, resultsPerPage);
        }

#if !NET_3_5

        public IList<dynamic> Projection(SqlQuery sqlQuery)
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            try
            {
                using (var command = this.connectionManager.BuildCommand(sqlQuery))
                {
                    this.OpenConnectionIfClosed(command);

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

                    this.CloseConnectionIfNotInTransaction(command);

                    return results;
                }
            }
            catch (Exception e)
            {
                log.TryLogError(e.Message, e);
                throw new MicroLiteException(e.Message, e);
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

            var include = new IncludeScalar<T>();

            this.includes.Enqueue(include);
            this.queries.Enqueue(sqlQuery);

            return include;
        }

        public void Update(object instance)
        {
            this.ThrowIfDisposed();

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            this.Listeners.Each(l => l.BeforeUpdate(instance));

            var sqlQuery = this.sqlDialect.UpdateQuery(instance);

            this.Listeners.Each(l => l.BeforeUpdate(instance, sqlQuery));

            this.Execute(sqlQuery);
        }

        private void CloseConnectionIfNotInTransaction(IDbCommand command)
        {
            if (command.Transaction == null)
            {
                log.TryLogDebug(Messages.Session_ClosingConnection, this.id);
                command.Connection.Close();
            }
        }

        private void ExecuteAllQueries()
        {
            try
            {
                var sqlQuery = SqlUtil.Combine(this.queries);

                using (var command = this.connectionManager.BuildCommand(sqlQuery))
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
                log.TryLogError(e.Message, e);
                throw new MicroLiteException(e.Message, e);
            }
            finally
            {
                this.includes.Clear();
                this.queries.Clear();
            }
        }

        private void OpenConnectionIfClosed(IDbCommand command)
        {
            if (command.Connection.State == ConnectionState.Closed)
            {
                log.TryLogDebug(Messages.Session_OpeningConnection, this.id);
                command.Connection.Open();
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