// -----------------------------------------------------------------------
// <copyright file="IdentifierAttribute.cs" company="MicroLite">
// Copyright 2012 - 2014 Project Contributors
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
    /// <example>
    /// If the IdentifierStrategy Sequence is used, a sequence name must also be specified:
    /// <code>
    /// [Column("CustomerId")]
    /// [Identifier(IdentifierStrategy.Sequence, "CustomerIdSequence")]
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
        private readonly string sequenceName;

        /// <summary>
        /// Initialises a new instance of the <see cref="IdentifierAttribute"/> class.
        /// </summary>
        /// <param name="identifierStrategy">The identifier strategy used to manage the identifier's value.</param>
        public IdentifierAttribute(IdentifierStrategy identifierStrategy)
            : this(identifierStrategy, null)
        {
        }

        /// <summary>
        /// Initialises a new instance of the <see cref="IdentifierAttribute"/> class.
        /// </summary>
        /// <param name="identifierStrategy">The identifier strategy used to manage the identifier's value.</param>
        /// <param name="sequenceName">The name of the sequence which generates the identifier value.</param>
        public IdentifierAttribute(IdentifierStrategy identifierStrategy, string sequenceName)
        {
            this.identifierStrategy = identifierStrategy;
            this.sequenceName = sequenceName;
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

        /// <summary>
        /// Gets the name of the sequence which generates the identifier value.
        /// </summary>
        public string SequenceName
        {
            get
            {
                return this.sequenceName;
            }
        }
    }
}