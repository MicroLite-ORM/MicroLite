// -----------------------------------------------------------------------
// <copyright file="PostgreSqlDbDriver.cs" company="MicroLite">
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
    /// The implementation of <see cref="IDbDriver"/> for PostgreSql server.
    /// </summary>
    internal sealed class PostgreSqlDbDriver : DbDriver
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="PostgreSqlDbDriver" /> class.
        /// </summary>
        internal PostgreSqlDbDriver()
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