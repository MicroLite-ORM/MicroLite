// -----------------------------------------------------------------------
// <copyright file="Include.cs" company="Project Contributors">
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
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace MicroLite.Core
{
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
        /// <param name="reader">The <see cref="DbDataReader"/> containing the results.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        internal abstract Task BuildValueAsync(DbDataReader reader, CancellationToken cancellationToken);
    }
}
