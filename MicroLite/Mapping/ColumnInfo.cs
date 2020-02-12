// -----------------------------------------------------------------------
// <copyright file="ColumnInfo.cs" company="Project Contributors">
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
using System.Data;
using System.Reflection;

namespace MicroLite.Mapping
{
    /// <summary>
    /// A class which contains information about a database table column and the property it is mapped to.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Column: {ColumnName}, Identifier: {IsIdentifier}, Insert: {AllowInsert}, Update: {AllowUpdate}")]
    public sealed class ColumnInfo
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ColumnInfo"/> class.
        /// </summary>
        /// <param name="columnName">The name of the column in the database table.</param>
        /// <param name="dbType">The <see cref="DbType"/> of the column in the database table.</param>
        /// <param name="propertyInfo">The property info for the property the column maps to.</param>
        /// <param name="isIdentifier">A value indicating whether column is the table identifier column (primary key).</param>
        /// <param name="allowInsert">true if the column can be inserted, otherwise false.</param>
        /// <param name="allowUpdate">true if the column can be updated, otherwise false.</param>
        /// <param name="sequenceName">The name of the sequence which generates the identifier value or null if sequences are not used.</param>
        /// <exception cref="ArgumentNullException">Thrown if columnName or propertyInfo are null.</exception>
        public ColumnInfo(
            string columnName,
            DbType dbType,
            PropertyInfo propertyInfo,
            bool isIdentifier,
            bool allowInsert,
            bool allowUpdate,
            string sequenceName)
        {
            this.ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
            this.DbType = dbType;
            this.PropertyInfo = propertyInfo ?? throw new ArgumentNullException(nameof(propertyInfo));
            this.IsIdentifier = isIdentifier;
            this.AllowInsert = allowInsert;
            this.AllowUpdate = allowUpdate;
            this.SequenceName = sequenceName;
        }

        /// <summary>
        /// Gets a value indicating whether the column value is allowed to be inserted.
        /// </summary>
        public bool AllowInsert { get; }

        /// <summary>
        /// Gets a value indicating whether the column value is allowed to be updated.
        /// </summary>
        public bool AllowUpdate { get; }

        /// <summary>
        /// Gets the name of the column in the database table.
        /// </summary>
        public string ColumnName { get; }

        /// <summary>
        /// Gets the <see cref="DbType"/> of the column in the database table.
        /// </summary>
        public DbType DbType { get; }

        /// <summary>
        /// Gets a value indicating whether column is the table identifier column (primary key).
        /// </summary>
        public bool IsIdentifier { get; }

        /// <summary>
        /// Gets the property info for the property the column maps to.
        /// </summary>
        public PropertyInfo PropertyInfo { get; }

        /// <summary>
        /// Gets the name of the sequence which generates the identifier value.
        /// </summary>
        public string SequenceName { get; }
    }
}
