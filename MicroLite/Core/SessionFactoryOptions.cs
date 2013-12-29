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
    /// The options for configuring a session factory.
    /// </summary>
    internal sealed class SessionFactoryOptions
    {
        /// <summary>
        /// The connection name.
        /// </summary>
        internal string ConnectionName
        {
            get;
            set;
        }

        /// <summary>
        /// The connection string.
        /// </summary>
        internal string ConnectionString
        {
            get;
            set;
        }

        /// <summary>
        /// The provider factory.
        /// </summary>
        internal DbProviderFactory ProviderFactory
        {
            get;
            set;
        }

        /// <summary>
        /// The type of SqlDialect for the connection.
        /// </summary>
        internal Type SqlDialectType
        {
            get;
            set;
        }
    }
}