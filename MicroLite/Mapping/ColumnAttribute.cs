// -----------------------------------------------------------------------
// <copyright file="ColumnAttribute.cs" company="MicroLite">
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

    /// <summary>
    /// An attribute which can be applied to a property to specify the column name that the property maps to.
    /// </summary>
    /// <example>
    /// <code>
    /// // Option 1 - Column and property name match.
    /// [Column("FirstName")]
    /// public string FirstName
    /// {
    ///     get;
    ///     set;
    /// }
    /// </code>
    /// <code>
    /// // Option 2 - Column and property name differ.
    /// [Column("FName")]
    /// public string FirstName
    /// {
    ///     get;
    ///     set;
    /// }
    /// </code>
    /// <code>
    /// // Additionally, it is possible to restrict insert or updates to a column.
    /// [Column("Created", allowInsert: true, allowUpdate: false)]
    /// public DateTime Created
    /// {
    ///     get;
    ///     set;
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ColumnAttribute : Attribute
    {
        private readonly bool allowInsert;
        private readonly bool allowUpdate;
        private readonly string name;

        /// <summary>
        /// Initialises a new instance of the <see cref="ColumnAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the column in the database table that the property maps to.</param>
        public ColumnAttribute(string name)
            : this(name, allowInsert: true, allowUpdate: true)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="ColumnAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the column in the database table that the property maps to.</param>
        /// <param name="allowInsert">true if the column value can be inserted, otherwise false.</param>
        /// <param name="allowUpdate">true if the column value can be updated, otherwise false.</param>
        public ColumnAttribute(string name, bool allowInsert, bool allowUpdate)
        {
            this.name = name;
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
        /// Gets the name of the column in the database table that the property maps to
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }
    }
}