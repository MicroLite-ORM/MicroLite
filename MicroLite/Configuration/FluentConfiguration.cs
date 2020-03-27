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
        private static readonly object s_locker = new object();
        private readonly ILog _log = LogManager.GetCurrentClassLog();
        private readonly Func<ISessionFactory, ISessionFactory> _sessionFactoryCreated;
        private string _chosenConnectionName;
        private IDbDriver _chosenDbDriver;
        private ISqlDialect _chosenSqlDialect;

        internal FluentConfiguration(Func<ISessionFactory, ISessionFactory> sessionFactoryCreated)
            => _sessionFactoryCreated = sessionFactoryCreated;

        public ISessionFactory CreateSessionFactory()
        {
            lock (s_locker)
            {
                ISessionFactory sessionFactory =
                    Configure.SessionFactories.SingleOrDefault(s => s.ConnectionName == _chosenConnectionName);

                if (sessionFactory is null)
                {
                    if (_log.IsDebug)
                    {
                        _log.Debug(
                            LogMessages.FluentConfiguration_CreatingSessionFactory,
                            _chosenConnectionName,
                            _chosenDbDriver.GetType().Name,
                            _chosenSqlDialect.GetType().Name);
                    }

                    sessionFactory = new SessionFactory(_chosenConnectionName, _chosenDbDriver, _chosenSqlDialect);

                    if (_sessionFactoryCreated != null)
                    {
                        sessionFactory = _sessionFactoryCreated(sessionFactory);
                    }

                    Configure.SessionFactories.Add(sessionFactory);
                }

                return sessionFactory;
            }
        }

        public ICreateSessionFactory ForConnection(string connectionName, ISqlDialect sqlDialect, IDbDriver dbDriver)
        {
            if (connectionName is null)
            {
                throw new ArgumentNullException(nameof(connectionName));
            }

            ConnectionStringSettings configSection = ConfigurationManager.ConnectionStrings[connectionName];

            if (configSection is null)
            {
                throw new ConfigurationException(ExceptionMessages.FluentConfiguration_ConnectionNotFound.FormatWith(connectionName));
            }

            return ForConnection(configSection.Name, configSection.ConnectionString, configSection.ProviderName, sqlDialect, dbDriver);
        }

        public ICreateSessionFactory ForConnection(string connectionName, string connectionString, string providerName, ISqlDialect sqlDialect, IDbDriver dbDriver)
        {
            if (connectionName is null)
            {
                throw new ArgumentNullException(nameof(connectionName));
            }

            if (connectionString is null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (providerName is null)
            {
                throw new ArgumentNullException(nameof(providerName));
            }

            if (sqlDialect is null)
            {
                throw new ArgumentNullException(nameof(sqlDialect));
            }

            if (dbDriver is null)
            {
                throw new ArgumentNullException(nameof(dbDriver));
            }

            _chosenConnectionName = connectionName;
            _chosenDbDriver = dbDriver;
            _chosenDbDriver.ConnectionString = connectionString;
            _chosenDbDriver.DbProviderFactory = DbProviderFactories.GetFactory(providerName);
            _chosenSqlDialect = sqlDialect;

            return this;
        }
    }
}
