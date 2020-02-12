// -----------------------------------------------------------------------
// <copyright file="PostgreSqlDialect.cs" company="Project Contributors">
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
using System.Data;
using System.Text;
using MicroLite.Characters;
using MicroLite.Mapping;

namespace MicroLite.Dialect
{
    /// <summary>
    /// The implementation of <see cref="ISqlDialect"/> for Postgre server.
    /// </summary>
    internal sealed class PostgreSqlDialect : SqlDialect
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="PostgreSqlDialect"/> class.
        /// </summary>
        internal PostgreSqlDialect()
            : base(PostgreSqlCharacters.Instance)
        {
        }

        public override bool SupportsSelectInsertedIdentifier => true;

        public override SqlQuery BuildSelectInsertIdSqlQuery(IObjectInfo objectInfo) => new SqlQuery("SELECT lastval()");

        public override SqlQuery PageQuery(SqlQuery sqlQuery, PagingOptions pagingOptions)
        {
            if (sqlQuery is null)
            {
                throw new ArgumentNullException(nameof(sqlQuery));
            }

            var arguments = new SqlArgument[sqlQuery.Arguments.Count + 2];
            Array.Copy(sqlQuery.ArgumentsArray, 0, arguments, 0, sqlQuery.Arguments.Count);
            arguments[arguments.Length - 2] = new SqlArgument(pagingOptions.Count, DbType.Int32);
            arguments[arguments.Length - 1] = new SqlArgument(pagingOptions.Offset, DbType.Int32);

            var stringBuilder = new StringBuilder(sqlQuery.CommandText)
                .Replace(Environment.NewLine, string.Empty)
                .Append(" LIMIT ")
                .Append(this.SqlCharacters.GetParameterName(arguments.Length - 2))
                .Append(" OFFSET ")
                .Append(this.SqlCharacters.GetParameterName(arguments.Length - 1));

            return new SqlQuery(stringBuilder.ToString(), arguments);
        }
    }
}
