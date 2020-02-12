// -----------------------------------------------------------------------
// <copyright file="SessionFactory.cs" company="Project Contributors">
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
using MicroLite.Characters;
using MicroLite.Dialect;
using MicroLite.Driver;
using MicroLite.Listeners;
using MicroLite.Logging;

namespace MicroLite.Core
{
    /// <summary>
    /// The default implementation of <see cref="ISessionFactory"/>.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("SessionFactory for {ConnectionName}")]
    internal sealed class SessionFactory : ISessionFactory
    {
        private static readonly ILog log = LogManager.GetCurrentClassLog();
        private readonly SessionListeners sessionListeners;
        private readonly ISqlDialect sqlDialect;

        internal SessionFactory(string connectionName, IDbDriver dbDriver, ISqlDialect sqlDialect)
        {
            this.ConnectionName = connectionName;
            this.DbDriver = dbDriver;
            this.sqlDialect = sqlDialect;

            this.sessionListeners = new SessionListeners(Listener.DeleteListeners, Listener.InsertListener, Listener.UpdateListeners);
        }

        public string ConnectionName { get; }

        public IDbDriver DbDriver { get; }

        public IAsyncReadOnlySession OpenAsyncReadOnlySession() => this.OpenAsyncReadOnlySession(ConnectionScope.PerTransaction);

        public IAsyncReadOnlySession OpenAsyncReadOnlySession(ConnectionScope connectionScope)
        {
            if (log.IsDebug)
            {
                log.Debug(LogMessages.SessionFactory_CreatingAsyncReadOnlySession, this.ConnectionName);
            }

            SqlCharacters.Current = this.sqlDialect.SqlCharacters;

            return new AsyncReadOnlySession(connectionScope, this.sqlDialect, this.DbDriver);
        }

        public IAsyncSession OpenAsyncSession() => this.OpenAsyncSession(ConnectionScope.PerTransaction);

        public IAsyncSession OpenAsyncSession(ConnectionScope connectionScope)
        {
            if (log.IsDebug)
            {
                log.Debug(LogMessages.SessionFactory_CreatingAsyncSession, this.ConnectionName);
            }

            SqlCharacters.Current = this.sqlDialect.SqlCharacters;

            return new AsyncSession(connectionScope, this.sqlDialect, this.DbDriver, this.sessionListeners);
        }
    }
}
