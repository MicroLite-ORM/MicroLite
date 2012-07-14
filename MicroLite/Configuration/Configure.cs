// -----------------------------------------------------------------------
// <copyright file="Configure.cs" company="MicroLite">
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
    using System.Collections.Generic;

    /// <summary>
    /// The class used to configure the MicroLite ORM framework.
    /// </summary>
    public static class Configure
    {
        private static readonly IList<ISessionFactory> sessionFactories = new List<ISessionFactory>();

        /// <summary>
        /// Gets the session factories created by the configuration.
        /// </summary>
        public static ICollection<ISessionFactory> SessionFactories
        {
            get
            {
                return sessionFactories;
            }
        }

        /// <summary>
        /// Begins the process of specifying the extensions which should be used by MicroLite.
        /// </summary>
        /// <returns>The interface which provides the extension points.</returns>
        public static IConfigureExtensions Extensions()
        {
            return new ConfigureExtensions();
        }

        /// <summary>
        /// Begins the configuration process using the fluent API.
        /// </summary>
        /// <returns>The next step in the fluent configuration.</returns>
        public static IConfigureConnection Fluently()
        {
            return new FluentConfiguration();
        }
    }
}