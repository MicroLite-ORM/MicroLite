// -----------------------------------------------------------------------
// <copyright file="PostgreSqlDialect.cs" company="MicroLite">
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
namespace MicroLite.Dialect
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// The implementation of <see cref="ISqlDialect"/> for Postgre server.
    /// </summary>
    internal sealed class PostgreSqlDialect : SqlDialect
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="PostgreSqlDialect"/> class.
        /// </summary>
        /// <remarks>Constructor needs to be public so that it can be instantiated by SqlDialectFactory.</remarks>
        public PostgreSqlDialect()
            : base(SqlCharacters.PostgreSql)
        {
        }

        /// <summary>
        /// Gets the select identity string.
        /// </summary>
        protected override string SelectIdentityString
        {
            get
            {
                return "SELECT lastval()";
            }
        }

        public override SqlQuery PageQuery(SqlQuery sqlQuery, PagingOptions pagingOptions)
        {
            List<object> arguments = new List<object>(sqlQuery.Arguments.Count + 2);
            arguments.AddRange(sqlQuery.Arguments);
            arguments.Add(pagingOptions.Count);
            arguments.Add(pagingOptions.Offset);

            var sqlBuilder = new StringBuilder(sqlQuery.CommandText);
            sqlBuilder.Replace(Environment.NewLine, string.Empty);
            sqlBuilder.Append(" LIMIT ");
            sqlBuilder.Append(this.SqlCharacters.GetParameterName(arguments.Count - 2));
            sqlBuilder.Append(" OFFSET ");
            sqlBuilder.Append(this.SqlCharacters.GetParameterName(arguments.Count - 1));

            return new SqlQuery(sqlBuilder.ToString(), arguments.ToArray());
        }
    }
}