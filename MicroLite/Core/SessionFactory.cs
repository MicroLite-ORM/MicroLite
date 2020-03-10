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
        private static readonly ILog s_log = LogManager.GetCurrentClassLog();
        private readonly SessionListeners _sessionListeners;
        private readonly ISqlDialect _sqlDialect;

        internal SessionFactory(string connectionName, IDbDriver dbDriver, ISqlDialect sqlDialect)
        {
            ConnectionName = connectionName;
            DbDriver = dbDriver;
            _sqlDialect = sqlDialect;

            _sessionListeners = new SessionListeners(Listener.DeleteListeners, Listener.InsertListener, Listener.UpdateListeners);
        }

        public string ConnectionName { get; }

        public IDbDriver DbDriver { get; }

        public IReadOnlySession OpenReadOnlySession() => OpenReadOnlySession(ConnectionScope.PerTransaction);

        public IReadOnlySession OpenReadOnlySession(ConnectionScope connectionScope)
        {
            if (s_log.IsDebug)
            {
                s_log.Debug(LogMessages.SessionFactory_CreatingAsyncReadOnlySession, ConnectionName);
            }

            SqlCharacters.Current = _sqlDialect.SqlCharacters;

            return new ReadOnlySession(connectionScope, _sqlDialect, DbDriver);
        }

        public ISession OpenSession() => OpenSession(ConnectionScope.PerTransaction);

        public ISession OpenSession(ConnectionScope connectionScope)
        {
            if (s_log.IsDebug)
            {
                s_log.Debug(LogMessages.SessionFactory_CreatingAsyncSession, ConnectionName);
            }

            SqlCharacters.Current = _sqlDialect.SqlCharacters;

            return new Session(connectionScope, _sqlDialect, DbDriver, _sessionListeners);
        }
    }
}
