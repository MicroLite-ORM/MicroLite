// -----------------------------------------------------------------------
// <copyright file="Include.cs" company="MicroLite">
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
namespace MicroLite.Core
{
    using System.Data;
    using System.Data.Common;

    /// <summary>
    /// The base class for include implementations.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("HasValue: {HasValue}, Value: {Value}")]
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
        /// Builds the included value from the results in the data reader.
        /// </summary>
        /// <param name="reader">The <see cref="IDataReader"/> containing the results.</param>
        internal abstract void BuildValue(IDataReader reader);

#if NET_4_5

        /// <summary>
        /// Builds the included value from the results in the data reader.
        /// </summary>
        /// <param name="reader">The <see cref="DbDataReader"/> containing the results.</param>
        /// <returns>A task which can be awaited.</returns>
        internal abstract System.Threading.Tasks.Task BuildValueAsync(DbDataReader reader);

#endif
    }
}