// -----------------------------------------------------------------------
// <copyright file="FluentConfiguration.cs" company="Project Contributors">
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
using System;
using System.Configuration;
using System.Data.Common;
using System.Linq;
using MicroLite.Core;
using MicroLite.Dialect;
using MicroLite.Driver;
using MicroLite.FrameworkExtensions;
using MicroLite.Logging;

namespace MicroLite.Configuration
{
    /// <summary>
    /// The class used to configure the MicroLite ORM framework using the fluent API.
    /// </summary>
    internal sealed class FluentConfiguration : IConfigureConnection, ICreateSessionFactory
    {
        private static readonly object locker = new object();
        private readonly ILog log = LogManager.GetCurrentClassLog();
        private readonly Func<ISessionFactory, ISessionFactory> sessionFactoryCreated;
        private string chosenConnectionName;
        private IDbDriver chosenDbDriver;
        private ISqlDialect chosenSqlDialect;

        internal FluentConfiguration(Func<ISessionFactory, ISessionFactory> sessionFactoryCreated)
        {
            this.sessionFactoryCreated = sessionFactoryCreated;
        }

        public ISessionFactory CreateSessionFactory()
        {
            lock (locker)
            {
                var sessionFactory =
                    Configure.SessionFactories.SingleOrDefault(s => s.ConnectionName == this.chosenConnectionName);

                if (sessionFactory == null)
                {
                    if (this.log.IsDebug)
                    {
                        this.log.Debug(
                            LogMessages.FluentConfiguration_CreatingSessionFactory,
                            this.chosenConnectionName,
                            this.chosenDbDriver.GetType().Name,
                            this.chosenSqlDialect.GetType().Name);
                    }

                    sessionFactory = new SessionFactory(this.chosenConnectionName, this.chosenDbDriver, this.chosenSqlDialect);

                    if (this.sessionFactoryCreated != null)
                    {
                        sessionFactory = this.sessionFactoryCreated(sessionFactory);
                    }

                    Configure.SessionFactories.Add(sessionFactory);
                }

                return sessionFactory;
            }
        }

        public ICreateSessionFactory ForConnection(string connectionName, ISqlDialect sqlDialect, IDbDriver dbDriver)
        {
            if (connectionName == null)
            {
                throw new ArgumentNullException("connectionName");
            }

            var configSection = ConfigurationManager.ConnectionStrings[connectionName];

            if (configSection == null)
            {
                throw new ConfigurationException(ExceptionMessages.FluentConfiguration_ConnectionNotFound.FormatWith(connectionName));
            }

            return this.ForConnection(configSection.Name, configSection.ConnectionString, configSection.ProviderName, sqlDialect, dbDriver);
        }

        public ICreateSessionFactory ForConnection(string connectionName, string connectionString, string providerName, ISqlDialect sqlDialect, IDbDriver dbDriver)
        {
            if (connectionName == null)
            {
                throw new ArgumentNullException("connectionName");
            }

            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }

            if (providerName == null)
            {
                throw new ArgumentNullException("providerName");
            }

            if (sqlDialect == null)
            {
                throw new ArgumentNullException("sqlDialect");
            }

            if (dbDriver == null)
            {
                throw new ArgumentNullException("dbDriver");
            }

            this.chosenConnectionName = connectionName;
            this.chosenDbDriver = dbDriver;
            this.chosenDbDriver.ConnectionString = connectionString;
            this.chosenDbDriver.DbProviderFactory = DbProviderFactories.GetFactory(providerName);
            this.chosenSqlDialect = sqlDialect;

            return this;
        }
    }
}
