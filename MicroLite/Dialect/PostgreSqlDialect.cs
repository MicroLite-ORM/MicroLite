﻿// -----------------------------------------------------------------------
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
        /// The select identity string.
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
            var arguments = new object[sqlQuery.Arguments.Count + 2];
            sqlQuery.Arguments.CopyTo(arguments, 0);
            arguments[arguments.Length - 2] = pagingOptions.Count;
            arguments[arguments.Length - 1] = pagingOptions.Offset;

            var sqlBuilder = new StringBuilder(sqlQuery.CommandText);
            sqlBuilder.Replace(Environment.NewLine, string.Empty);
            sqlBuilder.Append(" LIMIT ");
            sqlBuilder.Append(this.SqlCharacters.GetParameterName(arguments.Length - 2));
            sqlBuilder.Append(" OFFSET ");
            sqlBuilder.Append(this.SqlCharacters.GetParameterName(arguments.Length - 1));

            return new SqlQuery(sqlBuilder.ToString(), arguments);
        }
    }
}