// -----------------------------------------------------------------------
// <copyright file="Configure.cs" company="MicroLite">
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
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Common;
    using System.Linq;
    using MicroLite.Core;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Listeners;
    using MicroLite.Logging;

    /// <summary>
    /// The class used to configure the MicroLite ORM framework.
    /// </summary>
    public sealed class Configure : IConfigureConnection, ICreateSessionFactory, IHideObjectMethods
    {
        private static readonly ILog log = LogManager.GetLog("MicroLite.Configure");
        private static readonly IList<ISessionFactory> sessionFactories = new List<ISessionFactory>();
        private readonly SessionFactoryOptions options = new SessionFactoryOptions();

        /// <summary>
        /// Prevents a default instance of the <see cref="Configure"/> class from being created.
        /// </summary>
        private Configure()
        {
        }

        /// <summary>
        /// Gets the session factories created by the configuration.
        /// </summary>
        public static IEnumerable<ISessionFactory> SessionFactories
        {
            get
            {
                return Configure.sessionFactories;
            }
        }

        /// <summary>
        /// Enables extensions to be loaded.
        /// </summary>
        /// <returns>The interface which provides the extension points.</returns>
        public static IConfigureExtensions Extensions()
        {
            return new ConfigureExtensions();
        }

        /// <summary>
        /// Begins the configuration process using the fluent API.
        /// </summary>
        /// <returns>The next step in the fluent configuration.</returns>
        public static IConfigureConnection Fluently()
        {
            return new Configure();
        }

        /// <summary>
        /// Creates the session factory for the configured connection.
        /// </summary>
        /// <returns>
        /// The session factory for the specified connection.
        /// </returns>
        public ISessionFactory CreateSessionFactory()
        {
            var sessionFactory = SessionFactories.SingleOrDefault(s => s.ConnectionName == this.options.ConnectionName);

            if (sessionFactory == null)
            {
                log.TryLogInfo(Messages.Configure_CreatingSessionFactory, this.options.ConnectionName);
                sessionFactory = new SessionFactory(this.options);

                sessionFactories.Add(sessionFactory);
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
            if (connectionName == null)
            {
                throw new ArgumentNullException("connectionName");
            }

            log.TryLogDebug(Messages.Configure_ReadingConnection, connectionName);
            var configSection = ConfigurationManager.ConnectionStrings[connectionName];

            if (configSection == null)
            {
                var message = Messages.Configure_ConnectionNotFound.FormatWith(connectionName);
                log.TryLogFatal(message);
                throw new MicroLiteException(message);
            }

            if (configSection.ProviderName != "System.Data.SqlClient")
            {
                var message = Messages.Configure_ProviderNotSupported.FormatWith(configSection.ProviderName);
                log.TryLogFatal(message);
                throw new NotSupportedException(message);
            }

            try
            {
                this.options.ConnectionName = configSection.Name;
                this.options.ConnectionString = configSection.ConnectionString;
                this.options.ProviderFactory = DbProviderFactories.GetFactory(configSection.ProviderName);
                return this;
            }
            catch (Exception e)
            {
                throw new MicroLiteException(e.Message, e);
            }
        }
    }
}