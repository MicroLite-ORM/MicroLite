// -----------------------------------------------------------------------
// <copyright file="ColumnInfo.cs" company="MicroLite">
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
    using System.Reflection;

    /// <summary>
    /// A class which contains information about a database table column and the property it is mapped to.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Column {ColumnName}, Identifier:{IsIdentifier}, Insert:{AllowInsert}, Update:{AllowUpdate}")]
    public sealed class ColumnInfo
    {
        private readonly bool allowInsert;
        private readonly bool allowUpdate;
        private readonly string columnName;
        private readonly bool isIdentifier;
        private readonly PropertyInfo propertyInfo;

        /// <summary>
        /// Initialises a new instance of the <see cref="ColumnInfo"/> class.
        /// </summary>
        /// <param name="columnName">The name of the column in the database table.</param>
        /// <param name="propertyInfo">The property info for the property the column maps to.</param>
        /// <param name="isIdentifier">A value indicating whether column is the table identifier column (primary key).</param>
        /// <param name="allowInsert">true if the column can be inserted, otherwise false.</param>
        /// <param name="allowUpdate">true if the column can be updated, otherwise false.</param>
        /// <exception cref="ArgumentNullException">Thrown if columnName or propertyInfo are null.</exception>
        public ColumnInfo(
            string columnName,
            PropertyInfo propertyInfo,
            bool isIdentifier,
            bool allowInsert,
            bool allowUpdate)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException("columnName");
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException("propertyInfo");
            }

            this.columnName = columnName;
            this.isIdentifier = isIdentifier;
            this.propertyInfo = propertyInfo;
            this.allowInsert = allowInsert;
            this.allowUpdate = allowUpdate;
        }

        /// <summary>
        /// Gets a value indicating whether the column value is allowed to be inserted.
        /// </summary>
        public bool AllowInsert
        {
            get
            {
                return this.allowInsert;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the column value is allowed to be updated.
        /// </summary>
        public bool AllowUpdate
        {
            get
            {
                return this.allowUpdate;
            }
        }

        /// <summary>
        /// Gets the name of the column in the database table.
        /// </summary>
        public string ColumnName
        {
            get
            {
                return this.columnName;
            }
        }

        /// <summary>
        /// Gets a value indicating whether column is the table identifier column (primary key).
        /// </summary>
        public bool IsIdentifier
        {
            get
            {
                return this.isIdentifier;
            }
        }

        /// <summary>
        /// Gets the property info for the property the column maps to.
        /// </summary>
        public PropertyInfo PropertyInfo
        {
            get
            {
                return this.propertyInfo;
            }
        }
    }
}