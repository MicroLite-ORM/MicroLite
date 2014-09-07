// -----------------------------------------------------------------------
// <copyright file="MsSqlDialect.cs" company="MicroLite">
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
namespace MicroLite.Dialect
{
    using System;
    using System.Globalization;
    using System.Text;
    using MicroLite.Mapping;

    /// <summary>
    /// The implementation of <see cref="ISqlDialect"/> for MsSql server.
    /// </summary>
    internal sealed class MsSqlDialect : SqlDialect
    {
        private static readonly SqlQuery selectIdentityQuery = new SqlQuery("SELECT SCOPE_IDENTITY()");

        /// <summary>
        /// Initialises a new instance of the <see cref="MsSqlDialect"/> class.
        /// </summary>
        internal MsSqlDialect()
            : base(MsSqlCharacters.Instance)
        {
        }

        public override bool SupportsSelectInsertedIdentifier
        {
            get
            {
                return true;
            }
        }

        public override SqlQuery BuildSelectInsertIdSqlQuery(IObjectInfo objectInfo)
        {
            return selectIdentityQuery;
        }

        public override SqlQuery PageQuery(SqlQuery sqlQuery, PagingOptions pagingOptions)
        {
            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            var arguments = new object[sqlQuery.Arguments.Count + 2];
            Array.Copy(sqlQuery.ArgumentsArray, 0, arguments, 0, sqlQuery.Arguments.Count);
            arguments[arguments.Length - 2] = pagingOptions.Offset + 1;
            arguments[arguments.Length - 1] = pagingOptions.Offset + pagingOptions.Count;

            var sqlString = SqlString.Parse(sqlQuery.CommandText, Clauses.Select | Clauses.From | Clauses.Where | Clauses.OrderBy);

            var qualifiedTableName = sqlString.From;
            var position = qualifiedTableName.LastIndexOf('.') + 1;
            var tableName = position > 0 ? qualifiedTableName.Substring(position, qualifiedTableName.Length - position) : qualifiedTableName;

            var whereClause = !string.IsNullOrEmpty(sqlString.Where) ? " WHERE " + sqlString.Where : string.Empty;
            var orderByClause = !string.IsNullOrEmpty(sqlString.OrderBy) ? sqlString.OrderBy : "(SELECT NULL)";

            var stringBuilder = new StringBuilder(sqlQuery.CommandText.Length * 2)
                .Append("SELECT ")
                .Append(sqlString.Select)
                .Append(" FROM")
                .AppendFormat(CultureInfo.InvariantCulture, " (SELECT {0},ROW_NUMBER() OVER(ORDER BY {1}) AS RowNumber FROM {2}{3}) AS {4}", sqlString.Select, orderByClause, qualifiedTableName, whereClause, tableName)
                .AppendFormat(CultureInfo.InvariantCulture, " WHERE (RowNumber >= {0} AND RowNumber <= {1})", this.SqlCharacters.GetParameterName(arguments.Length - 2), this.SqlCharacters.GetParameterName(arguments.Length - 1));

            return new SqlQuery(stringBuilder.ToString(), arguments);
        }
    }
}