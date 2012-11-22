// -----------------------------------------------------------------------
// <copyright file="SQLiteDialect.cs" company="MicroLite">
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
    /// The implementation of <see cref="ISqlDialect"/> for SQLite.
    /// </summary>
    internal sealed class SQLiteDialect : SqlDialect
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="SQLiteDialect"/> class.
        /// </summary>
        /// <remarks>Constructor needs to be public so that it can be instantiated by SqlDialectFactory.</remarks>
        public SQLiteDialect()
        {
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
        /// Gets the select identity string.
        /// </summary>
        protected override string SelectIdentityString
        {
            get
            {
                return "SELECT last_insert_rowid()";
            }
        }

        /// <summary>
        /// Gets the SQL parameter.
        /// </summary>
        protected override char SqlParameter
        {
            get
            {
                return '@';
            }
        }

        /// <summary>
        /// Gets a value indicating whether SQL parameters include the position (parameter number).
        /// </summary>
        protected override bool SqlParameterIncludesPosition
        {
            get
            {
                return true;
            }
        }
    }
}