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
    using System;
    using System.Collections.Generic;
    using System.Text;

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
        /// Gets a value indicating whether SQL parameters are named.
        /// </summary>
        protected override bool SupportsNamedParameters
        {
            get
            {
                return true;
            }
        }

        public override SqlQuery PageQuery(SqlQuery sqlQuery, PagingOptions pagingOptions)
        {
            List<object> arguments = new List<object>(sqlQuery.Arguments.Count + 2);
            arguments.AddRange(sqlQuery.Arguments);
            arguments.Add(pagingOptions.Offset);
            arguments.Add(pagingOptions.Count);

            var sqlBuilder = new StringBuilder(sqlQuery.CommandText);
            sqlBuilder.Replace(Environment.NewLine, string.Empty);
            sqlBuilder.Append(" LIMIT ");
            sqlBuilder.Append(this.FormatParameter(arguments.Count - 2));
            sqlBuilder.Append(',');
            sqlBuilder.Append(this.FormatParameter(arguments.Count - 1));

            return new SqlQuery(sqlBuilder.ToString(), arguments.ToArray());
        }
    }
}