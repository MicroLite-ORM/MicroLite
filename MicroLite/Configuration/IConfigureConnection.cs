// -----------------------------------------------------------------------
// <copyright file="IConfigureConnection.cs" company="MicroLite">
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
    using MicroLite.Dialect;
    using MicroLite.Driver;

    /// <summary>
    /// The interface which specifies the options for configuring the connection in the fluent configuration
    /// of the MicroLite ORM framework.
    /// </summary>
    public interface IConfigureConnection : IHideObjectMethods
    {
        /// <summary>
        /// Specifies the name of the connection string and the implementations of ISqlDialect and DbProviderFactory to use
        /// for the connection.
        /// </summary>
        /// <param name="connectionName">The name of the connection string in the app config.</param>
        /// <param name="sqlDialect">The sql dialect to use for the connection.</param>
        /// <param name="dbDriver">The db driver to use for the connection.</param>
        /// <returns>The next step in the fluent configuration.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if any argument is null.</exception>
        /// <exception cref="ConfigurationException">Thrown if the connection is not found in the app config.</exception>
        /// <remarks>This method should not be called by user code, rather it is the extension point used by the custom configuration extension method for a supported database type.</remarks>
        ICreateSessionFactory ForConnection(string connectionName, ISqlDialect sqlDialect, IDbDriver dbDriver);
    }
}