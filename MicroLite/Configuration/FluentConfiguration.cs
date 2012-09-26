// -----------------------------------------------------------------------
// <copyright file="FluentConfiguration.cs" company="MicroLite">
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
namespace MicroLite.Configuration
{
    using System;
    using System.Configuration;
    using System.Data.Common;
    using System.Linq;
    using MicroLite.Core;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Logging;

    /// <summary>
    /// The class used to configure the MicroLite ORM framework using the fluent API.
    /// </summary>
    internal sealed class FluentConfiguration : IConfigureConnection, ICreateSessionFactory
    {
        private readonly ILog log = LogManager.GetLog("MicroLite.Configuration");
        private readonly SessionFactoryOptions options = new SessionFactoryOptions();

        /// <summary>
        /// Creates the session factory for the configured connection.
        /// </summary>
        /// <returns>
        /// The session factory for the specified connection.
        /// </returns>
        public ISessionFactory CreateSessionFactory()
        {
            var sessionFactory =
                Configure.SessionFactories.SingleOrDefault(s => s.ConnectionName == this.options.ConnectionName);

            if (sessionFactory == null)
            {
                this.log.TryLogInfo(Messages.FluentConfiguration_CreatingSessionFactory, this.options.ConnectionName);
                sessionFactory = new SessionFactory(this.options);

                Configure.SessionFactories.Add(sessionFactory);
            }

            return sessionFactory;
        }

        /// <summary>
        /// Specifies the named connection string in the app config to be used.
        /// </summary>
        /// <param name="connectionName">The name of the connection string in the app config.</param>
        /// <returns>
        /// The next step in the fluent configuration.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if connectionName is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the connection is not found in the app config.</exception>
        /// <exception cref="NotSupportedException">Thrown if the provider name is not supported.</exception>
        public ICreateSessionFactory ForConnection(string connectionName)
        {
            return this.ForConnection(connectionName, "MicroLite.Dialect.MsSqlDialect");
        }

        private ICreateSessionFactory ForConnection(string connectionName, string sqlDialect)
        {
            if (connectionName == null)
            {
                throw new ArgumentNullException("connectionName");
            }

            this.log.TryLogDebug(Messages.FluentConfiguration_ReadingConnection, connectionName);
            var configSection = ConfigurationManager.ConnectionStrings[connectionName];

            if (configSection == null)
            {
                this.log.TryLogFatal(Messages.FluentConfiguration_ConnectionNotFound, connectionName);
                throw new MicroLiteException(Messages.FluentConfiguration_ConnectionNotFound.FormatWith(connectionName));
            }

            try
            {
                this.options.ConnectionName = configSection.Name;
                this.options.ConnectionString = configSection.ConnectionString;
                this.options.ProviderFactory = DbProviderFactories.GetFactory(configSection.ProviderName);
                this.options.SqlDialect = sqlDialect;

                return this;
            }
            catch (Exception e)
            {
                this.log.TryLogError(e.Message, e);
                throw new MicroLiteException(e.Message, e);
            }
        }
    }
}