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
    using System.Data;
    using MicroLite.Dialect;
    using MicroLite.Listeners;
    using MicroLite.Logging;

    /// <summary>
    /// The default implementation of <see cref="ISessionFactory"/>.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("SessionFactory for {ConnectionName}")]
    internal sealed class SessionFactory : ISessionFactory
    {
        private static readonly ILog log = LogManager.GetCurrentClassLog();
        private readonly object locker = new object();
        private readonly SessionFactoryOptions sessionFactoryOptions;
        private readonly ISqlDialect sqlDialect;

        internal SessionFactory(SessionFactoryOptions sessionFactoryOptions)
        {
            this.sessionFactoryOptions = sessionFactoryOptions;
            this.sqlDialect = sessionFactoryOptions.SqlDialect;
        }

        public string ConnectionName
        {
            get
            {
                return this.sessionFactoryOptions.ConnectionName;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This method is provided to create and return an ISession for the caller to use, it should not dispose of it, that is the responsibility of the caller.")]
        public IReadOnlySession OpenReadOnlySession()
        {
            return this.OpenReadOnlySession(ConnectionScope.PerTransaction);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This method is provided to create and return an ISession for the caller to use, it should not dispose of it, that is the responsibility of the caller.")]
        public IReadOnlySession OpenReadOnlySession(ConnectionScope connectionScope)
        {
            var connection = this.GetNewConnectionWithConnectionString();

            if (log.IsDebug)
            {
                log.Debug(Messages.SessionFactory_CreatingReadOnlySession, this.ConnectionName, this.sqlDialect.GetType().Name);
            }

            return new ReadOnlySession(connectionScope, connection, this.sqlDialect);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This method is provided to create and return an ISession for the caller to use, it should not dispose of it, that is the responsibility of the caller.")]
        public ISession OpenSession()
        {
            return this.OpenSession(ConnectionScope.PerTransaction);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This method is provided to create and return an ISession for the caller to use, it should not dispose of it, that is the responsibility of the caller.")]
        public ISession OpenSession(ConnectionScope connectionScope)
        {
            var connection = this.GetNewConnectionWithConnectionString();

            if (log.IsDebug)
            {
                log.Debug(Messages.SessionFactory_CreatingSession, this.ConnectionName, this.sqlDialect.GetType().Name);
            }

            return new Session(connectionScope, connection, this.sqlDialect, Listener.Listeners);
        }

        private IDbConnection GetNewConnectionWithConnectionString()
        {
            IDbConnection connection;

            lock (this.locker)
            {
                connection = this.sessionFactoryOptions.ProviderFactory.CreateConnection();
            }

            connection.ConnectionString = this.sessionFactoryOptions.ConnectionString;

            return connection;
        }
    }
}