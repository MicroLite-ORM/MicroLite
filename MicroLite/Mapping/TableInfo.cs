// -----------------------------------------------------------------------
// <copyright file="TableInfo.cs" company="MicroLite">
// Copyright 2012 - 2013 Project Contributors
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
    using MicroLite.FrameworkExtensions;
    using MicroLite.Logging;

    /// <summary>
    /// A class which contains information about a database table .
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{Schema}.{Name}")]
    public sealed class TableInfo
    {
        private static readonly ILog log = LogManager.GetCurrentClassLog();

        private readonly IList<ColumnInfo> columns;
        private readonly string identifierColumn;
        private readonly string identifierProperty;
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
        /// <exception cref="ArgumentNullException">Thrown if columns or name are null.</exception>
        /// <exception cref="MicroLiteException">Thrown if no identifier column is specified.</exception>
        public TableInfo(
            IList<ColumnInfo> columns,
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

            this.identifierStrategy = identifierStrategy;
            this.name = name;
            this.schema = schema;

            this.ValidateColumns(columns);

            this.columns = new System.Collections.ObjectModel.ReadOnlyCollection<ColumnInfo>(columns);

            var identifierColumnInfo = columns.Single(c => c.IsIdentifier);

            this.identifierColumn = identifierColumnInfo.ColumnName;
            this.identifierProperty = identifierColumnInfo.PropertyInfo.Name;
        }

        /// <summary>
        /// Gets the columns that are mapped for the table.
        /// </summary>
        public IList<ColumnInfo> Columns
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
        /// Gets the name of the property that is the object identifier property mapped to the table identifier column.
        /// </summary>
        public string IdentifierProperty
        {
            get
            {
                return this.identifierProperty;
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

        private void ValidateColumns(IEnumerable<ColumnInfo> mappedColumns)
        {
            var duplicatedColumn = mappedColumns
                .GroupBy(c => c.ColumnName)
                .Select(x => new
                {
                    Key = x.Key,
                    Count = x.Count()
                })
                .FirstOrDefault(x => x.Count > 1);

            if (duplicatedColumn != null)
            {
                log.TryLogFatal(Messages.TableInfo_ColumnMappedMultipleTimes, duplicatedColumn.Key);
                throw new MicroLiteException(Messages.TableInfo_ColumnMappedMultipleTimes.FormatWith(duplicatedColumn.Key));
            }

            if (!mappedColumns.Any(c => c.IsIdentifier))
            {
                log.TryLogFatal(Messages.TableInfo_NoIdentifierColumn, this.schema, this.name);
                throw new MicroLiteException(Messages.TableInfo_NoIdentifierColumn.FormatWith(this.schema, this.name));
            }
        }
    }
}