// -----------------------------------------------------------------------
// <copyright file="MsSql2005Dialect.cs" company="Project Contributors">
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
using System.Globalization;
using System.Text;
using MicroLite.Characters;
using MicroLite.Mapping;

namespace MicroLite.Dialect
{
    /// <summary>
    /// The implementation of <see cref="ISqlDialect"/> for MsSql Server 2005 or later.
    /// </summary>
    internal class MsSql2005Dialect : SqlDialect
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MsSql2005Dialect"/> class.
        /// </summary>
        internal MsSql2005Dialect()
            : base(MsSqlCharacters.Instance)
        {
        }

        public override bool SupportsSelectInsertedIdentifier => true;

        public override SqlQuery BuildSelectInsertIdSqlQuery(IObjectInfo objectInfo) => new SqlQuery("SELECT SCOPE_IDENTITY()");

        public override SqlQuery PageQuery(SqlQuery sqlQuery, PagingOptions pagingOptions)
        {
            if (sqlQuery is null)
            {
                throw new ArgumentNullException(nameof(sqlQuery));
            }

            var arguments = new SqlArgument[sqlQuery.Arguments.Count + 2];
            Array.Copy(sqlQuery.ArgumentsArray, 0, arguments, 0, sqlQuery.Arguments.Count);
            arguments[arguments.Length - 2] = new SqlArgument(pagingOptions.Offset + 1, DbType.Int32);
            arguments[arguments.Length - 1] = new SqlArgument(pagingOptions.Offset + pagingOptions.Count, DbType.Int32);

            var sqlString = SqlString.Parse(sqlQuery.CommandText, Clauses.Select | Clauses.From | Clauses.Where | Clauses.OrderBy);

            var whereClause = !string.IsNullOrEmpty(sqlString.Where) ? " WHERE " + sqlString.Where : string.Empty;
            var orderByClause = !string.IsNullOrEmpty(sqlString.OrderBy) ? sqlString.OrderBy : "(SELECT NULL)";

            var stringBuilder = new StringBuilder(sqlQuery.CommandText.Length * 2)
                .AppendFormat(CultureInfo.InvariantCulture, "SELECT * FROM (SELECT {0},ROW_NUMBER() OVER(ORDER BY {1}) AS MicroLiteRowNumber FROM {2}{3}) AS [MicroLitePagedResults]", sqlString.Select, orderByClause, sqlString.From, whereClause)
                .AppendFormat(CultureInfo.InvariantCulture, " WHERE (MicroLiteRowNumber >= {0} AND MicroLiteRowNumber <= {1})", this.SqlCharacters.GetParameterName(arguments.Length - 2), this.SqlCharacters.GetParameterName(arguments.Length - 1));

            return new SqlQuery(stringBuilder.ToString(), arguments);
        }
    }
}
