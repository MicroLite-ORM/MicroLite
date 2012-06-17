// -----------------------------------------------------------------------
// <copyright file="TableInfo.cs" company="MicroLite">
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
namespace MicroLite
{
    using System.Collections.Generic;
    using MicroLite.Mapping;

    [System.Diagnostics.DebuggerDisplay("{Schema}.{Name}")]
    internal sealed class TableInfo
    {
        private readonly ICollection<string> columns = new List<string>();

        internal ICollection<string> Columns
        {
            get
            {
                return this.columns;
            }
        }

        internal string IdentifierColumn
        {
            get;
            set;
        }

        internal IdentifierStrategy IdentifierStrategy
        {
            get;
            set;
        }

        internal string Name
        {
            get;
            set;
        }

        internal string Schema
        {
            get;
            set;
        }
    }
}