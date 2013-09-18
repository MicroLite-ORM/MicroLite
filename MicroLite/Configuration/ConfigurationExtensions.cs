// -----------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="MicroLite">
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
    using MicroLite.Mapping;

    /// <summary>
    /// Extensions for the MicroLite configuration.
    /// </summary>
    public static class ConfigurationExtensions
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

        /// <summary>
        /// Configures the MicroLite ORM Framework to use the default attribute based mapping.
        /// </summary>
        /// <param name="configureExtensions">The interface to configure extensions.</param>
        /// <returns>The configure extensions.</returns>
        public static IConfigureExtensions WithAttributeBasedMapping(
            this IConfigureExtensions configureExtensions)
        {
            if (configureExtensions == null)
            {
                throw new ArgumentNullException("configureExtensions");
            }

            configureExtensions.SetMappingConvention(new AttributeMappingConvention());

            return configureExtensions;
        }

        /// <summary>
        /// Configures the MicroLite ORM Framework to use convention based mapping instead of the default
        /// attribute based mapping.
        /// </summary>
        /// <param name="configureExtensions">The interface to configure extensions.</param>
        /// <param name="settings">The settings for the convention mapping.</param>
        /// <returns>The configure extensions.</returns>
        public static IConfigureExtensions WithConventionBasedMapping(
            this IConfigureExtensions configureExtensions,
            ConventionMappingSettings settings)
        {
            if (configureExtensions == null)
            {
                throw new ArgumentNullException("configureExtensions");
            }

            if (settings == null)
            {
                throw new ArgumentNullException("settings");
            }

            configureExtensions.SetMappingConvention(new ConventionMappingConvention(settings));

            return configureExtensions;
        }
    }
}