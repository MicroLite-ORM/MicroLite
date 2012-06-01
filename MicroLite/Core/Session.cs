namespace MicroLite.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Listeners;
    using MicroLite.Logging;

    /// <summary>
    /// The default implementation of <see cref="ISession"/>.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Session {id}")]
    internal sealed class Session : ISession, IAdvancedSession
    {
        private static readonly ILog log = LogManager.GetLogInstance("MicroLite.Session");
        private readonly IConnectionManager connectionManager;
        private readonly Guid id = Guid.NewGuid();
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

            log.TryLogDebug(LogMessages.Session_Created, this.id.ToString());
        }

        public IAdvancedSession Advanced
        {
            get
            {
                return this;
            }
        }

        private IEnumerable<IListener> Listeners
        {
            get
            {
                return this.listeners ?? (this.listeners = ListenerManager.Create());
            }
        }

        public ITransaction BeginTransaction()
        {
            return this.connectionManager.BeginTransaction();
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            return this.connectionManager.BeginTransaction(isolationLevel);
        }

        public bool Delete(object instance)
        {
            this.ThrowIfDisposed();

            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            var sqlQuery = this.queryBuilder.DeleteQuery(instance);

            return this.Execute(sqlQuery) == 1;
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                this.connectionManager.Dispose();

                log.TryLogDebug(LogMessages.Session_Disposed, this.id.ToString());
                this.disposed = true;
            }
        }

        public int Execute(SqlQuery sqlQuery)
        {
            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            try
            {
                int result;

                using (var command = this.connectionManager.Build(sqlQuery))
                {
                    if (command.Connection.State == ConnectionState.Closed)
                    {
                        log.TryLogDebug(LogMessages.Session_OpeningConnection, this.id.ToString());
                        command.Connection.Open();
                    }

                    log.TryLogDebug(sqlQuery.CommandText);
                    result = command.ExecuteNonQuery();

                    if (command.Transaction == null)
                    {
                        log.TryLogDebug(LogMessages.Session_ClosingConnection, this.id.ToString());
                        command.Connection.Close();
                    }

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
            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            try
            {
                T result;

                using (var command = this.connectionManager.Build(sqlQuery))
                {
                    if (command.Connection.State == ConnectionState.Closed)
                    {
                        log.TryLogDebug(LogMessages.Session_OpeningConnection, this.id.ToString());
                        command.Connection.Open();
                    }

                    log.TryLogDebug(sqlQuery.CommandText);
                    result = (T)command.ExecuteScalar();

                    if (command.Transaction == null)
                    {
                        log.TryLogDebug(LogMessages.Session_ClosingConnection, this.id.ToString());
                        command.Connection.Close();
                    }

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

            this.Listeners.Each(l => l.BeforeInsert(instance.GetType(), sqlQuery));

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
                throw new ArgumentOutOfRangeException("page");
            }

            if (resultsPerPage < 1)
            {
                throw new ArgumentOutOfRangeException("resultsPerPage");
            }

            var query = this.queryBuilder.Page(sqlQuery, page, resultsPerPage);

            var results = this.Query<T>(query).ToList();

            return new PagedResult<T>(page, results);
        }

        public T Single<T>(ValueType identifier) where T : class, new()
        {
            this.ThrowIfDisposed();

            if (identifier == null)
            {
                throw new ArgumentNullException("identifier");
            }

            var sqlQuery = this.queryBuilder.SelectQuery(typeof(T), identifier);

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
                IDataReader reader;

                try
                {
                    if (command.Connection.State == ConnectionState.Closed)
                    {
                        log.TryLogDebug(LogMessages.Session_OpeningConnection, this.id.ToString());
                        command.Connection.Open();
                    }

                    log.TryLogDebug(command.CommandText);
                    reader = command.ExecuteReader();
                }
                catch (Exception e)
                {
                    log.TryLogError(e.Message, e);
                    throw new MicroLiteException(e.Message, e);
                }

                using (reader)
                {
                    while (reader.Read())
                    {
                        yield return this.objectBuilder.BuildNewInstance<T>(reader);
                    }
                }

                if (command.Transaction == null)
                {
                    log.TryLogDebug(LogMessages.Session_ClosingConnection, this.id.ToString());
                    command.Connection.Close();
                }
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