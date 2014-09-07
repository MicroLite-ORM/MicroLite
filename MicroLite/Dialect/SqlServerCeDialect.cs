// -----------------------------------------------------------------------
// <copyright file="SqlServerCeDialect.cs" company="MicroLite">
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
    using System.Text;
    using MicroLite.Mapping;

    /// <summary>
    /// The implementation of <see cref="ISqlDialect"/> for SQL Server Compact Edition.
    /// </summary>
    internal sealed class SqlServerCeDialect : SqlDialect
    {
        private static readonly SqlQuery selectIdentityQuery = new SqlQuery("SELECT @@IDENTITY");

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
            return selectIdentityQuery;
        }

        public override SqlQuery PageQuery(SqlQuery sqlQuery, PagingOptions pagingOptions)
        {
            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            var sqlString = SqlString.Parse(sqlQuery.CommandText, Clauses.OrderBy);

            if (string.IsNullOrEmpty(sqlString.OrderBy))
            {
                throw new MicroLiteException(ExceptionMessages.SqlServerCeDialect_PagedRequiresOrderBy);
            }

            var arguments = new object[sqlQuery.Arguments.Count + 2];
            Array.Copy(sqlQuery.ArgumentsArray, 0, arguments, 0, sqlQuery.Arguments.Count);
            arguments[arguments.Length - 2] = pagingOptions.Offset;
            arguments[arguments.Length - 1] = pagingOptions.Count;

            var stringBuilder = new StringBuilder(sqlQuery.CommandText)
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