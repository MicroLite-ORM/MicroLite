// -----------------------------------------------------------------------
// <copyright file="Options.cs" company="MicroLite">
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
    using System.Data.Common;

    /// <summary>
    /// The class used to hold all the configurable options during configuration.
    /// </summary>
    internal sealed class Options
    {
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
    }
}