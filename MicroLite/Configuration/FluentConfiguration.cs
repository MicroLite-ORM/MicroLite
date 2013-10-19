// -----------------------------------------------------------------------
// <copyright file="FluentConfiguration.cs" company="MicroLite">
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
                    this.log.TryLogDebug(Messages.FluentConfiguration_CreatingSessionFactory, this.options.ConnectionName);
                    sessionFactory = new SessionFactory(objectBuilder, this.options);
                    MicroLite.Query.SqlBuilder.SqlCharacters = sessionFactory.SqlDialect.SqlCharacters;

                    Configure.SessionFactories.Add(sessionFactory);
                }

                return sessionFactory;
            }
        }

        /// <summary>
        /// Specifies the name of the connection string in the app config and the sql dialect to be used.
        /// </summary>
        /// <param name="connectionName">The name of the connection string in the app config.</param>
        /// <param name="sqlDialect">The name of the sql dialect to use for the connection.</param>
        /// <returns>The next step in the fluent configuration.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionName is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the connection is not found in the app config.</exception>
        /// <exception cref="System.NotSupportedException">Thrown if the provider name or sql dialect is not supported.</exception>
        public ICreateSessionFactory ForConnection(string connectionName, string sqlDialect)
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

            var sqlDialectType = this.GetSqlDialectType(sqlDialect);

            try
            {
                this.options.ConnectionName = configSection.Name;
                this.options.ConnectionString = configSection.ConnectionString;
                this.options.ProviderFactory = DbProviderFactories.GetFactory(configSection.ProviderName);
                this.options.SqlDialectType = sqlDialectType;

                return this;
            }
            catch (Exception e)
            {
                this.log.TryLogFatal(e.Message, e);
                throw new MicroLiteException(e.Message, e);
            }
        }

        private Type GetSqlDialectType(string sqlDialect)
        {
            var sqlDialectType = Type.GetType(sqlDialect, throwOnError: false);

            if (sqlDialectType == null)
            {
                this.log.TryLogFatal(Messages.FluentConfiguration_DialectNotSupported.FormatWith(sqlDialect));
                throw new NotSupportedException(Messages.FluentConfiguration_DialectNotSupported.FormatWith(sqlDialect));
            }

            if (!typeof(ISqlDialect).IsAssignableFrom(sqlDialectType))
            {
                this.log.TryLogFatal(Messages.FluentConfiguration_DialectMustImplementISqlDialect.FormatWith(sqlDialect));
                throw new NotSupportedException(Messages.FluentConfiguration_DialectMustImplementISqlDialect.FormatWith(sqlDialect));
            }

            return sqlDialectType;
        }
    }
}