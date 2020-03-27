// -----------------------------------------------------------------------
// <copyright file="TableAttribute.cs" company="Project Contributors">
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

namespace MicroLite.Mapping.Attributes
{
    /// <summary>
    /// An attribute which can be applied to a class to specify the table name and database schema the table
    /// exists within.
    /// </summary>
    /// <example>
    /// <code>
    /// // Option 1 - Specify schema and table name.
    /// [Table("dbo", "Customers")]
    /// public class Customer
    /// {
    ///    ...
    /// }
    /// </code>
    /// <code>
    /// // Option 2 - Specify table name only.
    /// [Table("Customers")]
    /// public class Customer
    /// {
    ///    ...
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class TableAttribute : Attribute
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="TableAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the table.</param>
        public TableAttribute(string name)
            : this(null, name)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="TableAttribute"/> class.
        /// </summary>
        /// <param name="schema">The database schema the table exists within (e.g. 'dbo'); otherwise null.</param>
        /// <param name="name">The name of the table.</param>
        public TableAttribute(string schema, string name)
        {
            Name = name;
            Schema = schema;
        }

        /// <summary>
        /// Gets the name of the table.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the database schema the table exists within (e.g. 'dbo'); otherwise null.
        /// </summary>
        public string Schema { get; }
    }
}
