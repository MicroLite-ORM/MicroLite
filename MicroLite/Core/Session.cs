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
    using MicroLite.FrameworkExtensions;
    using MicroLite.Listeners;
    using MicroLite.Logging;
    using MicroLite.Mapping;

    /// <summary>
    /// The default implementation of <see cref="ISession"/>.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Session {id}")]
    internal sealed class Session : ISession, IAdvancedSession
    {
        private static readonly ILog log = LogManager.GetLog("MicroLite.Session");
        private readonly IConnectionManager connectionManager;
        private readonly string id = Guid.NewGuid().ToString();
        private readonly IObjectBuilder objectBuilder;
        private readonly ISqlQueryBuilder queryBuilder;
        private bool disposed;
        private IEnumerable<IListener> listeners;

        internal Session(
            IConnectionManager connectionManager,
            IObjectBuilder objectBuilder,
            ISqlQueryBuilder queryBuilder)
        {
            this.connectionManager = connectionManager;
            this.objectBuilder = objectBuilder;
            this.queryBuilder = queryBuilder;

            log.TryLogDebug(Messages.Session_Created, this.id);
        }

        public IAdvancedSession Advanced
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

            var sqlQuery = this.queryBuilder.DeleteQuery(instance);

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

            var sqlQuery = this.queryBuilder.DeleteQuery(type, identifier);

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
                using (var command = this.connectionManager.Build(sqlQuery))
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
                using (var command = this.connectionManager.Build(sqlQuery))
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
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            return this.Query<T>(sqlQuery).ToList();
        }

        public void Insert(object instance)
        {
            this.ThrowIfDisposed();

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            this.Listeners.Each(l => l.BeforeInsert(instance));

            var sqlQuery = this.queryBuilder.InsertQuery(instance);

            this.Listeners.Each(l => l.BeforeInsert(instance, sqlQuery));

            var identifier = this.ExecuteScalar<object>(sqlQuery);

            this.Listeners.Each(l => l.AfterInsert(instance, identifier));
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

            var pagedSqlQuery = this.queryBuilder.Page(sqlQuery, page, resultsPerPage);

            var results = this.Query<T>(pagedSqlQuery).ToList();

            return new PagedResult<T>(page, results, resultsPerPage);
        }

        public T Single<T>(object identifier) where T : class, new()
        {
            this.ThrowIfDisposed();

            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }

            var sqlQuery = this.queryBuilder.SelectQuery(typeof(T), identifier);

            return this.Query<T>(sqlQuery).SingleOrDefault();
        }

        public T Single<T>(SqlQuery sqlQuery) where T : class, new()
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            return this.Query<T>(sqlQuery).SingleOrDefault();
        }

        public void Update(object instance)
        {
            this.ThrowIfDisposed();

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            this.Listeners.Each(l => l.BeforeUpdate(instance));

            var sqlQuery = this.queryBuilder.UpdateQuery(instance);

            this.Listeners.Each(l => l.BeforeUpdate(instance, sqlQuery));

            this.Execute(sqlQuery);
        }

        internal IEnumerable<T> Query<T>(SqlQuery sqlQuery) where T : class, new()
        {
            this.ThrowIfDisposed();

            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            using (var command = this.connectionManager.Build(sqlQuery))
            {
                IDataReader reader = null;
                bool hasRow = false;

                try
                {
                    this.OpenConnectionIfClosed(command);

                    log.TryLogInfo(command.CommandText);
                    reader = command.ExecuteReader();
                    hasRow = reader.Read();
                }
                catch (Exception e)
                {
                    if (reader != null)
                    {
                        reader.Close();
                    }

                    log.TryLogError(e.Message, e);
                    throw new MicroLiteException(e.Message, e);
                }

                var objectInfo = ObjectInfo.For(typeof(T));

                using (reader)
                {
                    while (hasRow)
                    {
                        yield return this.objectBuilder.BuildNewInstance<T>(objectInfo, reader);

                        try
                        {
                            hasRow = reader.Read();
                        }
                        catch (Exception e)
                        {
                            log.TryLogError(e.Message, e);
                            throw new MicroLiteException(e.Message, e);
                        }
                    }
                }

                try
                {
                    this.CloseConnectionIfNotInTransaction(command);
                }
                catch (Exception e)
                {
                    log.TryLogError(e.Message, e);
                    throw new MicroLiteException(e.Message, e);
                }
            }
        }

        private void CloseConnectionIfNotInTransaction(IDbCommand command)
        {
            if (command.Transaction == null)
            {
                log.TryLogDebug(Messages.Session_ClosingConnection, this.id);
                command.Connection.Close();
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