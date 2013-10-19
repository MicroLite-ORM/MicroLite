// -----------------------------------------------------------------------
// <copyright file="SessionFactory.cs" company="MicroLite">
// Copyright 2012 - 2013 Project Contributors
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
    using System.Data;
    using System.Linq;
    using MicroLite.Dialect;
    using MicroLite.Listeners;
    using MicroLite.Logging;

    /// <summary>
    /// The default implementation of <see cref="ISessionFactory"/>.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("SessionFactory for {ConnectionName} using {sessionFactoryOptions.SqlDialectType.Name}")]
    internal sealed class SessionFactory : ISessionFactory
    {
        private static readonly ILog log = LogManager.GetCurrentClassLog();
        private readonly object locker = new object();
        private readonly IObjectBuilder objectBuilder;
        private readonly SessionFactoryOptions sessionFactoryOptions;
        private readonly ISqlDialect sqlDialect;

        internal SessionFactory(IObjectBuilder objectBuilder, SessionFactoryOptions sessionFactoryOptions)
        {
            this.objectBuilder = objectBuilder;
            this.sessionFactoryOptions = sessionFactoryOptions;
            this.sqlDialect = (ISqlDialect)Activator.CreateInstance(sessionFactoryOptions.SqlDialectType);
        }

        public string ConnectionName
        {
            get
            {
                return this.sessionFactoryOptions.ConnectionName;
            }
        }

        public ISqlDialect SqlDialect
        {
            get
            {
                return this.sqlDialect;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This method is provided to create and return an ISession for the caller to use, it should not dispose of it, that is the responsibility of the caller.")]
        public IReadOnlySession OpenReadOnlySession()
        {
            var connection = this.GetNewConnectionWithConnectionString();

            log.TryLogDebug(Messages.SessionFactory_CreatingReadOnlySession, this.ConnectionName, this.sessionFactoryOptions.SqlDialectType.Name);
            return new ReadOnlySession(
                this,
                new ConnectionManager(connection),
                this.objectBuilder);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This method is provided to create and return an ISession for the caller to use, it should not dispose of it, that is the responsibility of the caller.")]
        public ISession OpenSession()
        {
            var connection = this.GetNewConnectionWithConnectionString();

            log.TryLogDebug(Messages.SessionFactory_CreatingSession, this.ConnectionName, this.sessionFactoryOptions.SqlDialectType.Name);
            return new Session(
                this,
                new ConnectionManager(connection),
                this.objectBuilder,
                Listener.Listeners.ToArray());
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