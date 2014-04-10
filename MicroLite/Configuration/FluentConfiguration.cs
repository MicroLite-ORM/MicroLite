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
    using MicroLite.Driver;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Logging;

    /// <summary>
    /// The class used to configure the MicroLite ORM framework using the fluent API.
    /// </summary>
    internal sealed class FluentConfiguration : IConfigureConnection, ICreateSessionFactory
    {
        private static readonly object locker = new object();
        private readonly ILog log = LogManager.GetCurrentClassLog();
        private readonly Action<ISessionFactory> sessionFactoryCreated;
        private string chosenConnectionName;
        private IDbDriver chosenDbDriver;
        private ISqlDialect chosenSqlDialect;

        internal FluentConfiguration(Action<ISessionFactory> sessionFactoryCreated)
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
                        this.sessionFactoryCreated(sessionFactory);
                    }

                    SqlCharacters.Current = this.chosenSqlDialect.SqlCharacters;

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

            if (sqlDialect == null)
            {
                throw new ArgumentNullException("sqlDialect");
            }

            if (dbDriver == null)
            {
                throw new ArgumentNullException("dbDriver");
            }

            var configSection = ConfigurationManager.ConnectionStrings[connectionName];

            if (configSection == null)
            {
                throw new ConfigurationException(ExceptionMessages.FluentConfiguration_ConnectionNotFound.FormatWith(connectionName));
            }

            this.chosenConnectionName = configSection.Name;
            this.chosenSqlDialect = sqlDialect;
            this.chosenDbDriver = dbDriver;
            this.chosenDbDriver.ConnectionString = configSection.ConnectionString;
            this.chosenDbDriver.DbProviderFactory = DbProviderFactories.GetFactory(configSection.ProviderName);

            return this;
        }
    }
}