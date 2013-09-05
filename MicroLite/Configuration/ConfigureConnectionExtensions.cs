// -----------------------------------------------------------------------
// <copyright file="ConfigureConnectionExtensions.cs" company="MicroLite">
// Copyright 2012 - 2013 Trevor Pilley
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

    /// <summary>
    /// Extensions for the <see cref="IConfigureConnection"/> interface.
    /// </summary>
    public static class ConfigureConnectionExtensions
    {
        /// <summary>
        /// Specifies the name of the connection string in the app config and that the MsSqlDialect should be used.
        /// </summary>
        /// <param name="configureConnection">The interface to configure connections.</param>
        /// <param name="connectionName">The name of the connection string in the app config.</param>
        /// <returns>The next step in the fluent configuration.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionName is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the connection is not found in the app config.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ForMs", Justification = "For MS, not Forms.")]
        public static ICreateSessionFactory ForMsSqlConnection(this IConfigureConnection configureConnection, string connectionName)
        {
            if (configureConnection == null)
            {
                throw new ArgumentNullException("configureConnection");
            }

            return configureConnection.ForConnection(connectionName, "MicroLite.Dialect.MsSqlDialect");
        }

        /// <summary>
        /// Specifies the name of the connection string in the app config and that the MySqlDialect should be used.
        /// </summary>
        /// <param name="configureConnection">The interface to configure connections.</param>
        /// <param name="connectionName">The name of the connection string in the app config.</param>
        /// <returns>The next step in the fluent configuration.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionName is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the connection is not found in the app config.</exception>
        public static ICreateSessionFactory ForMySqlConnection(this IConfigureConnection configureConnection, string connectionName)
        {
            if (configureConnection == null)
            {
                throw new ArgumentNullException("configureConnection");
            }

            return configureConnection.ForConnection(connectionName, "MicroLite.Dialect.MySqlDialect");
        }

        /// <summary>
        /// Specifies the name of the connection string in the app config and that the PostgreSqlDialect should be used.
        /// </summary>
        /// <param name="configureConnection">The interface to configure connections.</param>
        /// <param name="connectionName">The name of the connection string in the app config.</param>
        /// <returns>The next step in the fluent configuration.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionName is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the connection is not found in the app config.</exception>
        public static ICreateSessionFactory ForPostgreSqlConnection(this IConfigureConnection configureConnection, string connectionName)
        {
            if (configureConnection == null)
            {
                throw new ArgumentNullException("configureConnection");
            }

            return configureConnection.ForConnection(connectionName, "MicroLite.Dialect.PostgreSqlDialect");
        }

        /// <summary>
        /// Specifies the name of the connection string in the app config and that the SQLiteDialect should be used.
        /// </summary>
        /// <param name="configureConnection">The interface to configure connections.</param>
        /// <param name="connectionName">The name of the connection string in the app config.</param>
        /// <returns>The next step in the fluent configuration.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionName is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the connection is not found in the app config.</exception>
        public static ICreateSessionFactory ForSQLiteConnection(this IConfigureConnection configureConnection, string connectionName)
        {
            if (configureConnection == null)
            {
                throw new ArgumentNullException("configureConnection");
            }

            return configureConnection.ForConnection(connectionName, "MicroLite.Dialect.SQLiteDialect");
        }
    }
}