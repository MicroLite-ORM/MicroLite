// -----------------------------------------------------------------------
// <copyright file="SqlServerCeDbDriver.cs" company="MicroLite">
// Copyright 2012 - 2015 Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Driver
{
    using MicroLite.Characters;

    /// <summary>
    /// The implementation of <see cref="IDbDriver"/> for SQL Server Compact Edition.
    /// </summary>
    internal sealed class SqlServerCeDbDriver : DbDriver
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="SqlServerCeDbDriver" /> class.
        /// </summary>
        internal SqlServerCeDbDriver()
            : base(SqlServerCeCharacters.Instance)
        {
        }

        /// <summary>
        /// Gets a value indicating whether this DbDriver supports command timeout.
        /// </summary>
        /// <remarks>
        /// Returns true unless overridden.
        /// </remarks>
        /// <remarks>SQL Server Compact Edition doesn't support command timeout.</remarks>
        protected override bool SupportsCommandTimeout
        {
            get
            {
                return false;
            }
        }
    }
}