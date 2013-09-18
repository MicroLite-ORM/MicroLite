// -----------------------------------------------------------------------
// <copyright file="Configure.cs" company="MicroLite">
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
        /// <remarks>Extensions should be configured before configuring any connections.</remarks>
        /// <example>
        /// Extensions can be added in any order although it is advised to add the logging extension first if you are using one
        /// so that other extensions can write to the log.
        /// <code>
        /// Configure
        ///     .Extensions()
        ///     .WithLog4Net() // To use log4net, install the MicroLite.Extensions.Log4Net package (there is also an NLog package).
        ///     .WithMvc(); // To use the MVC extensions, install the MicroLite.Extensions.Mvc package.
        /// </code>
        /// </example>
        public static IConfigureExtensions Extensions()
        {
            return new ConfigureExtensions();
        }

        /// <summary>
        /// Begins the configuration process using the fluent API.
        /// </summary>
        /// <returns>The next step in the fluent configuration.</returns>
        /// <example>
        /// Option 1: Use the default SqlDialect which is MicroLite.Dialect.MsSqlDialect.
        /// <code>
        /// var sessionFactory = Configure
        ///     .Fluently()
        ///     .ForConnection("TestDB")
        ///     .CreateSessionFactory();
        /// </code>
        /// </example>
        /// <example>
        /// Option 2: Use an alternative SqlDialect which is supported by MicroLite (such as SQLite).
        /// <code>
        /// var sessionFactory = Configure
        ///     .Fluently()
        ///     .ForConnection("TestDB", "MicroLite.Dialect.SQLiteDialect")
        ///     .CreateSessionFactory();
        /// </code>
        /// </example>
        public static IConfigureConnection Fluently()
        {
            return new FluentConfiguration();
        }
    }
}