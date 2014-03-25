// -----------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="MicroLite">
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
    using MicroLite.Dialect;
    using MicroLite.Driver;
    using MicroLite.Mapping;

    /// <summary>
    /// Extension methods for IConfigureExtensions.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Configures a MsSql connection using the connection string with the specified name
        /// in the connection strings section of the app/web config.
        /// </summary>
        /// <param name="configureConnection">The interface to configure a connection.</param>
        /// <param name="connectionName">The name of the connection string in the app/web config.</param>
        /// <returns>The next step in the fluent configuration.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if configureConnection or connectionName is null.</exception>
        /// <exception cref="ConfigurationException">Thrown if the connection is not found in the app config.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "ForMs", Justification = "For MS, not Forms.")]
        public static ICreateSessionFactory ForMsSqlConnection(this IConfigureConnection configureConnection, string connectionName)
        {
            if (configureConnection == null)
            {
                throw new ArgumentNullException("configureConnection");
            }

            return configureConnection.ForConnection(connectionName, new MsSqlDialect(), new MsDbDriver());
        }

        /// <summary>
        /// Configures the MicroLite ORM Framework to use attribute based mapping instead of the default convention based mapping.
        /// </summary>
        /// <param name="configureExtensions">The interface to configure extensions.</param>
        /// <returns>The configure extensions.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if configureExtensions is null.</exception>
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
        /// Configures the MicroLite ORM Framework to use the default convention based mapping.
        /// </summary>
        /// <param name="configureExtensions">The interface to configure extensions.</param>
        /// <param name="settings">The settings for the convention mapping.</param>
        /// <returns>The configure extensions.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if configureExtensions or settings is null.</exception>
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