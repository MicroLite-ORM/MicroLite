// -----------------------------------------------------------------------
// <copyright file="ConfigurationExtensions.cs" company="MicroLite">
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
    /// <summary>
    /// Extensions for the MicroLite configuration.
    /// </summary>
    public static class ConfigurationExtensions
    {
        /// <summary>
        /// Configures the MicroLite ORM Framework to use strict attribute based mapping.
        /// </summary>
        /// <param name="configureExtensions">The interface to configure extensions.</param>
        /// <returns>The configure extensions.</returns>
        public static IConfigureExtensions WithStrictAttributeMapping(this IConfigureExtensions configureExtensions)
        {
            configureExtensions.SetMappingConvention(new MicroLite.Mapping.StrictAttributeMappingConvention());

            return configureExtensions;
        }
    }
}