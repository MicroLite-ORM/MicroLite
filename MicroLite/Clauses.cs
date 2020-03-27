// -----------------------------------------------------------------------
// <copyright file="Clauses.cs" company="Project Contributors">
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
using System;

namespace MicroLite
{
    /// <summary>
    /// An enumeration which represents the various clauses in a SQL Command.
    /// </summary>
    [Flags]
    public enum Clauses
    {
        /// <summary>
        /// The select clause.
        /// </summary>
        Select = 1,

        /// <summary>
        /// The from clause.
        /// </summary>
        From = 2,

        /// <summary>
        /// The where clause.
        /// </summary>
        Where = 4,

        /// <summary>
        /// The order by clause.
        /// </summary>
        OrderBy = 16,

        /// <summary>
        /// The group by clause.
        /// </summary>
        GroupBy = 8,
    }
}
