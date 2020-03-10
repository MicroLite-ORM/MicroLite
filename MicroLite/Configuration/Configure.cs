// -----------------------------------------------------------------------
// <copyright file="Configure.cs" company="Project Contributors">
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
using System.Collections.Generic;

namespace MicroLite.Configuration
{
    /// <summary>
    /// The class used to configure the MicroLite ORM framework.
    /// </summary>
    public static class Configure
    {
        private static readonly IList<ISessionFactory> s_sessionFactories = new List<ISessionFactory>();

        /// <summary>
        /// Gets or sets a function which will be called when a session factory is created.
        /// </summary>
        /// <remarks>
        /// This is null by default, but if set it will be called when a session factory has been created.
        /// - Input is the ISessionFactory which has been created before it is added to Configure.SessionFactories.
        /// - Output is added to Configure.SessionFactories.
        /// The purpose of the method is to allow the session factory to be wrapped for profiling.
        /// </remarks>
        public static Func<ISessionFactory, ISessionFactory> OnSessionFactoryCreated { get; set; }

        /// <summary>
        /// Gets the collection of session factories which have created by the configuration.
        /// </summary>
        public static ICollection<ISessionFactory> SessionFactories => s_sessionFactories;

        /// <summary>
        /// Begins the process of specifying the extensions which should be used by MicroLite ORM.
        /// </summary>
        /// <returns>The interface which provides the extension points.</returns>
        /// <remarks>Extensions should be configured before configuring any connections.</remarks>
        /// <example>
        /// Extensions can be added in any order although it is advised to add the logging extension first if you are using one
        /// so that other extensions can write to the log.
        /// <code>
        /// Configure
        ///     .Extensions()
        ///     .WithLog4Net() // To use log4net, install the MicroLite.Logging.Log4Net package (there is also an NLog package).
        ///     .WithMvc() // To use the MVC extensions, install the MicroLite.Extensions.Mvc package.
        ///     .WithWebApi(); // To use the WebApi extensions, install the MicroLite.Extensions.WebApi package.
        /// </code>
        /// </example>
        public static IConfigureExtensions Extensions() => new ConfigureExtensions();

        /// <summary>
        /// Begins the configuration process using the fluent API.
        /// </summary>
        /// <returns>The next step in the fluent configuration.</returns>
        /// <example>
        /// <code>
        /// var sessionFactory = Configure
        ///     .Fluently()
        ///     .ForMsSql2005Connection("TestDB") // or any other supported SQL connection.
        ///     .CreateSessionFactory();
        /// </code>
        /// </example>
        public static IConfigureConnection Fluently() => new FluentConfiguration(OnSessionFactoryCreated);
    }
}
