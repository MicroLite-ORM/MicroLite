// -----------------------------------------------------------------------
// <copyright file="MsSqlDbDriver.cs" company="MicroLite">
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
namespace MicroLite.Driver
{
    /// <summary>
    /// The implementation of <see cref="IDbDriver"/> for MsSql server.
    /// </summary>
    internal sealed class MsSqlDbDriver : DbDriver
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MsSqlDbDriver" /> class.
        /// </summary>
        internal MsSqlDbDriver()
            : base(MicroLite.Dialect.MsSqlCharacters.Instance)
        {
        }

        public override bool SupportsBatchedQueries
        {
            get
            {
                return true;
            }
        }
    }
}