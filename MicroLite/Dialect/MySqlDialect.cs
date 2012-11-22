// -----------------------------------------------------------------------
// <copyright file="MySqlDialect.cs" company="MicroLite">
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
namespace MicroLite.Dialect
{
    using MicroLite.Mapping;

    /// <summary>
    /// The implementation of <see cref="ISqlDialect"/> for MySql server.
    /// </summary>
    internal sealed class MySqlDialect : SqlDialect
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MySqlDialect"/> class.
        /// </summary>
        /// <remarks>Constructor needs to be public so that it can be instantiated by SqlDialectFactory.</remarks>
        public MySqlDialect()
        {
        }

        /// <summary>
        /// Gets the close quote character.
        /// </summary>
        protected override char CloseQuote
        {
            get
            {
                return '`';
            }
        }

        /// <summary>
        /// Gets the database generated identifier strategies.
        /// </summary>
        protected override IdentifierStrategy[] DatabaseGeneratedStrategies
        {
            get
            {
                return new[] { IdentifierStrategy.AutoIncrement };
            }
        }

        /// <summary>
        /// Gets the open quote character.
        /// </summary>
        protected override char OpenQuote
        {
            get
            {
                return '`';
            }
        }

        /// <summary>
        /// Gets the select identity string.
        /// </summary>
        protected override string SelectIdentityString
        {
            get
            {
                return "SELECT LAST_INSERT_ID()";
            }
        }
    }
}