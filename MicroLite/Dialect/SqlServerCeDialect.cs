// -----------------------------------------------------------------------
// <copyright file="SqlServerCeDialect.cs" company="MicroLite">
// Copyright 2012 - 2016 Project Contributors
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
    using System.Data;
    using System.Text;
    using MicroLite.Characters;
    using MicroLite.Mapping;

    /// <summary>
    /// The implementation of <see cref="ISqlDialect"/> for SQL Server Compact Edition.
    /// </summary>
    internal sealed class SqlServerCeDialect : SqlDialect
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="SqlServerCeDialect"/> class.
        /// </summary>
        internal SqlServerCeDialect()
            : base(SqlServerCeCharacters.Instance)
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
            return new SqlQuery("SELECT @@IDENTITY");
        }

        public override SqlQuery PageQuery(SqlQuery sqlQuery, PagingOptions pagingOptions)
        {
            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            var arguments = new SqlArgument[sqlQuery.Arguments.Count + 2];
            Array.Copy(sqlQuery.ArgumentsArray, 0, arguments, 0, sqlQuery.Arguments.Count);
            arguments[arguments.Length - 2] = new SqlArgument(pagingOptions.Offset, DbType.Int32);
            arguments[arguments.Length - 1] = new SqlArgument(pagingOptions.Count, DbType.Int32);

            var sqlString = SqlString.Parse(sqlQuery.CommandText, Clauses.OrderBy);

            var commandText = string.IsNullOrEmpty(sqlString.OrderBy)
                ? sqlQuery.CommandText + " ORDER BY GETDATE()"
                : sqlQuery.CommandText;

            var stringBuilder = new StringBuilder(commandText)
                .Replace(Environment.NewLine, string.Empty)
                .Append(" OFFSET ")
                .Append(this.SqlCharacters.GetParameterName(arguments.Length - 2))
                .Append(" ROWS FETCH NEXT ")
                .Append(this.SqlCharacters.GetParameterName(arguments.Length - 1))
                .Append(" ROWS ONLY");

            return new SqlQuery(stringBuilder.ToString(), arguments);
        }
    }
}