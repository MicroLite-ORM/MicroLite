// -----------------------------------------------------------------------
// <copyright file="MySqlConfigurationExtensions.cs" company="MicroLite">
// Copyright 2012 - 2016 Project Contributors
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
    using Dialect;
    using Driver;

    /// <summary>
    /// Extension methods for IConfigureExtensions.
    /// </summary>
    public static class MySqlConfigurationExtensions
    {
        /// <summary>
        /// Configures a MySql connection using the connection string with the specified name
        /// in the connection strings section of the app/web config.
        /// </summary>
        /// <param name="configureConnection">The interface to configure a connection.</param>
        /// <param name="connectionName">The name of the connection string in the app/web config.</param>
        /// <returns>The next step in the fluent configuration.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if any argument is null.</exception>
        /// <exception cref="ConfigurationException">Thrown if the connection is not found in the app config.</exception>
        public static ICreateSessionFactory ForMySqlConnection(this IConfigureConnection configureConnection, string connectionName)
        {
            if (configureConnection == null)
            {
                throw new ArgumentNullException(nameof(configureConnection));
            }

            return configureConnection.ForConnection(connectionName, new MySqlDialect(), new MySqlDbDriver());
        }

        /// <summary>
        /// Configures a MySql connection using the specified connection name,
        /// connection string string and provider name.
        /// </summary>
        /// <param name="configureConnection">The interface to configure a connection.</param>
        /// <param name="connectionName">The name for the connection.</param>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="providerName">The name of the provider.</param>
        /// <returns>The next step in the fluent configuration.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if any argument is null.</exception>
        public static ICreateSessionFactory ForMySqlConnection(this IConfigureConnection configureConnection, string connectionName, string connectionString, string providerName)
        {
            if (configureConnection == null)
            {
                throw new ArgumentNullException(nameof(configureConnection));
            }

            return configureConnection.ForConnection(connectionName, connectionString, providerName, new MySqlDialect(), new MySqlDbDriver());
        }
    }
}