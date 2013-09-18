// -----------------------------------------------------------------------
// <copyright file="SessionFactoryOptions.cs" company="MicroLite">
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
namespace MicroLite.Core
{
    using System;
    using System.Data.Common;

    /// <summary>
    /// The class used to hold the options for configuring a session factory.
    /// </summary>
    internal sealed class SessionFactoryOptions
    {
        /// <summary>
        /// Gets or sets the connection name.
        /// </summary>
        internal string ConnectionName
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        internal string ConnectionString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the provider factory.
        /// </summary>
        internal DbProviderFactory ProviderFactory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the type of SqlDialect for the connection.
        /// </summary>
        internal Type SqlDialectType
        {
            get;
            set;
        }
    }
}