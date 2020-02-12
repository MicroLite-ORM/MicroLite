// -----------------------------------------------------------------------
// <copyright file="FirebirdDbDriver.cs" company="Project Contributors">
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
namespace MicroLite.Driver
{
    using MicroLite.Characters;

    /// <summary>
    /// The implementation of <see cref="IDbDriver"/> for Firebird.
    /// </summary>
    internal sealed class FirebirdDbDriver : DbDriver
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="FirebirdDbDriver" /> class.
        /// </summary>
        internal FirebirdDbDriver()
            : base(FirebirdSqlCharacters.Instance)
        {
        }
    }
}