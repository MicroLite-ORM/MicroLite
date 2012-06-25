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
namespace MicroLite.Mapping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// A class which contains information about a database table .
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Schema}.{Name}")]
    public sealed class TableInfo
    {
        private readonly ICollection<ColumnInfo> columns;
        private readonly string identifierColumn;
        private readonly IdentifierStrategy identifierStrategy;
        private readonly string name;
        private readonly string schema;

        /// <summary>
        /// Initialises a new instance of the <see cref="TableInfo"/> class.
        /// </summary>
        /// <param name="columns">The columns that are mapped for the table.</param>
        /// <param name="identifierStrategy">The identifier strategy used by the table.</param>
        /// <param name="name">The name of the table.</param>
        /// <param name="schema">The name of the schema the table exists within.</param>
        public TableInfo(
            IEnumerable<ColumnInfo> columns,
            IdentifierStrategy identifierStrategy,
            string name,
            string schema)
        {
            if (columns == null)
            {
                throw new ArgumentNullException("columns");
            }

            if (name == null)
            {
                throw new ArgumentNullException("name");
            }

            if (schema == null)
            {
                throw new ArgumentNullException("schema");
            }

            var identifierColumn = columns.SingleOrDefault(c => c.IsIdentifier);

            if (identifierColumn == null)
            {
                throw new MicroLiteException(Messages.TableInfo_NoIdentifierColumn);
            }

            this.columns = new List<ColumnInfo>(columns);
            this.identifierColumn = identifierColumn.ColumnName;
            this.identifierStrategy = identifierStrategy;
            this.name = name;
            this.schema = schema;
        }

        /// <summary>
        /// Gets the columns that are mapped for the table.
        /// </summary>
        public IEnumerable<ColumnInfo> Columns
        {
            get
            {
                return this.columns;
            }
        }

        /// <summary>
        /// Gets the name of the column that is the table identifier column (primary key).
        /// </summary>
        public string IdentifierColumn
        {
            get
            {
                return this.identifierColumn;
            }
        }

        /// <summary>
        /// Gets the identifier strategy used by the table.
        /// </summary>
        public IdentifierStrategy IdentifierStrategy
        {
            get
            {
                return this.identifierStrategy;
            }
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }

        /// <summary>
        /// Gets the name of the schema the table exists within.
        /// </summary>
        public string Schema
        {
            get
            {
                return this.schema;
            }
        }
    }
}