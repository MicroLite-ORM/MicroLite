// -----------------------------------------------------------------------
// <copyright file="IdentifierAttribute.cs" company="MicroLite">
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
    /// An attribute which can be applied to a property to specify that it maps to the row identifier (primary key)
    /// in the table and also defines the <see cref="IdentifierStrategy"/> used to manage the identifier's value.
    /// </summary>
    /// <example>
    /// <code>
    /// [Column("CustomerId")]
    /// [Identifier(IdentifierStrategy.DbGenerated)]
    /// public int Id
    /// {
    ///     get;
    ///     set;
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class IdentifierAttribute : Attribute
    {
        private readonly IdentifierStrategy identifierStrategy;

        /// <summary>
        /// Initialises a new instance of the <see cref="IdentifierAttribute"/> class.
        /// </summary>
        /// <param name="identifierStrategy">The identifier strategy used to manage the identifier's value.</param>
        public IdentifierAttribute(IdentifierStrategy identifierStrategy)
        {
            this.identifierStrategy = identifierStrategy;
        }

        /// <summary>
        /// Gets the identifier strategy used to manage the identifier's value.
        /// </summary>
        public IdentifierStrategy IdentifierStrategy
        {
            get
            {
                return this.identifierStrategy;
            }
        }
    }
}