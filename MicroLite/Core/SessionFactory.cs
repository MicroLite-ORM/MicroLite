// -----------------------------------------------------------------------
// <copyright file="SessionFactory.cs" company="MicroLite">
// Copyright 2012 - 2014 Project Contributors
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
    using MicroLite.Characters;
    using MicroLite.Dialect;
    using MicroLite.Driver;
    using MicroLite.Listeners;
    using MicroLite.Logging;

    /// <summary>
    /// The default implementation of <see cref="ISessionFactory"/>.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("SessionFactory for {ConnectionName}")]
    internal sealed class SessionFactory : ISessionFactory
    {
        private static readonly ILog log = LogManager.GetCurrentClassLog();
        private readonly string connectionName;
        private readonly IDbDriver dbDriver;
        private readonly ISqlDialect sqlDialect;

        internal SessionFactory(string connectionName, IDbDriver dbDriver, ISqlDialect sqlDialect)
        {
            this.connectionName = connectionName;
            this.dbDriver = dbDriver;
            this.sqlDialect = sqlDialect;
        }

        public string ConnectionName
        {
            get
            {
                return this.connectionName;
            }
        }

        public IDbDriver DbDriver
        {
            get
            {
                return this.dbDriver;
            }
        }

#if NET_4_5

        public IAsyncReadOnlySession OpenAsyncReadOnlySession()
        {
            return this.OpenAsyncReadOnlySession(ConnectionScope.PerTransaction);
        }

        public IAsyncReadOnlySession OpenAsyncReadOnlySession(ConnectionScope connectionScope)
        {
            if (log.IsDebug)
            {
                log.Debug(LogMessages.SessionFactory_CreatingReadOnlySession, this.connectionName);
            }

            SqlCharacters.Current = this.sqlDialect.SqlCharacters;

            return new AsyncReadOnlySession(connectionScope, this.sqlDialect, this.dbDriver);
        }

        public IAsyncSession OpenAsyncSession()
        {
            return this.OpenAsyncSession(ConnectionScope.PerTransaction);
        }

        public IAsyncSession OpenAsyncSession(ConnectionScope connectionScope)
        {
            if (log.IsDebug)
            {
                log.Debug(LogMessages.SessionFactory_CreatingSession, this.connectionName);
            }

            SqlCharacters.Current = this.sqlDialect.SqlCharacters;

            return new AsyncSession(connectionScope, this.sqlDialect, this.dbDriver, Listener.Listeners);
        }

#endif

        public IReadOnlySession OpenReadOnlySession()
        {
            return this.OpenReadOnlySession(ConnectionScope.PerTransaction);
        }

        public IReadOnlySession OpenReadOnlySession(ConnectionScope connectionScope)
        {
            if (log.IsDebug)
            {
                log.Debug(LogMessages.SessionFactory_CreatingReadOnlySession, this.connectionName);
            }

            SqlCharacters.Current = this.sqlDialect.SqlCharacters;

            return new ReadOnlySession(connectionScope, this.sqlDialect, this.dbDriver);
        }

        public ISession OpenSession()
        {
            return this.OpenSession(ConnectionScope.PerTransaction);
        }

        public ISession OpenSession(ConnectionScope connectionScope)
        {
            if (log.IsDebug)
            {
                log.Debug(LogMessages.SessionFactory_CreatingSession, this.connectionName);
            }

            SqlCharacters.Current = this.sqlDialect.SqlCharacters;

            return new Session(connectionScope, this.sqlDialect, this.dbDriver, Listener.Listeners);
        }
    }
}