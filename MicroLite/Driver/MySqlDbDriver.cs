// -----------------------------------------------------------------------
// <copyright file="MySqlDbDriver.cs" company="MicroLite">
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
    /// The implementation of <see cref="IDbDriver"/> for MySql server.
    /// </summary>
    internal sealed class MySqlDbDriver : DbDriver
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MySqlDbDriver" /> class.
        /// </summary>
        internal MySqlDbDriver()
            : base(MySqlCharacters.Instance)
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