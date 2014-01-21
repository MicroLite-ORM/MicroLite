// -----------------------------------------------------------------------
// <copyright file="FluentConfiguration.cs" company="MicroLite">
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
namespace MicroLite.Configuration
{
    using System;
    using System.Configuration;
    using System.Data.Common;
    using System.Linq;
    using MicroLite.Core;
    using MicroLite.Dialect;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Logging;

    /// <summary>
    /// The class used to configure the MicroLite ORM framework using the fluent API.
    /// </summary>
    internal sealed class FluentConfiguration : IConfigureConnection, ICreateSessionFactory
    {
        private static readonly object locker = new object();
        private static readonly IObjectBuilder objectBuilder = new ObjectBuilder();
        private readonly ILog log = LogManager.GetCurrentClassLog();
        private readonly SessionFactoryOptions options = new SessionFactoryOptions();

        /// <summary>
        /// Creates the session factory for the configured connection.
        /// </summary>
        /// <returns>
        /// The session factory for the specified connection.
        /// </returns>
        public ISessionFactory CreateSessionFactory()
        {
            lock (locker)
            {
                var sessionFactory =
                    Configure.SessionFactories.SingleOrDefault(s => s.ConnectionName == this.options.ConnectionName);

                if (sessionFactory == null)
                {
                    if (this.log.IsDebug)
                    {
                        this.log.Debug(Messages.FluentConfiguration_CreatingSessionFactory, this.options.ConnectionName);
                    }

                    sessionFactory = new SessionFactory(objectBuilder, this.options);
                    SqlCharacters.Current = this.options.SqlDialect.SqlCharacters;

                    Configure.SessionFactories.Add(sessionFactory);
                }

                return sessionFactory;
            }
        }

        /// <summary>
        /// Specifies the name of the connection string in the app config and the sql dialect to be used.
        /// </summary>
        /// <param name="connectionName">The name of the connection string in the app config.</param>
        /// <param name="sqlDialect">The sql dialect to use for the connection.</param>
        /// <param name="providerFactory">The provider factory to use for the connection.</param>
        /// <returns>The next step in the fluent configuration.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if any argument is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the connection is not found in the app config.</exception>
        public ICreateSessionFactory ForConnection(string connectionName, ISqlDialect sqlDialect, DbProviderFactory providerFactory)
        {
            if (connectionName == null)
            {
                throw new ArgumentNullException("connectionName");
            }

            if (sqlDialect == null)
            {
                throw new ArgumentNullException("sqlDialect");
            }

            if (providerFactory == null)
            {
                throw new ArgumentNullException("providerFactory");
            }

            var configSection = ConfigurationManager.ConnectionStrings[connectionName];

            if (configSection == null)
            {
                this.log.Fatal(Messages.FluentConfiguration_ConnectionNotFound, connectionName);
                throw new ConfigurationException(Messages.FluentConfiguration_ConnectionNotFound.FormatWith(connectionName));
            }

            this.options.ConnectionName = configSection.Name;
            this.options.ConnectionString = configSection.ConnectionString;
            this.options.ProviderFactory = providerFactory;
            this.options.SqlDialect = sqlDialect;

            return this;
        }
    }
}