// -----------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="MicroLite">
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
    using MicroLite.Mapping;

    /// <summary>
    /// Extensions for the MicroLite configuration.
    /// </summary>
    public static class ConfigurationExtensions
    {
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