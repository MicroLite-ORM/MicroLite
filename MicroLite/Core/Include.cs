// -----------------------------------------------------------------------
// <copyright file="Include.cs" company="MicroLite">
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
    using System.Data;

    /// <summary>
    /// The base class for include implementations.
    /// </summary>
    internal abstract class Include
    {
        /// <summary>
        /// Gets or sets a value indicating whether this include has a value.
        /// </summary>
        public bool HasValue
        {
            get;
            protected set;
        }

        /// <summary>
        /// Builds the included value from the results in the data reader using the supplied object builder.
        /// </summary>
        /// <param name="reader">The <see cref="IDataReader"/> containing the results.</param>
        /// <param name="objectBuilder">The object builder to use to build the included value.</param>
        internal abstract void BuildValue(IDataReader reader, IObjectBuilder objectBuilder);
    }
}