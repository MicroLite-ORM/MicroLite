// -----------------------------------------------------------------------
// <copyright file="FirebirdSqlDialect.cs" company="MicroLite">
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
    /// The implementation of <see cref="ISqlDialect"/> for Firebird.
    /// </summary>
    internal sealed class FirebirdSqlDialect : SqlDialect
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="FirebirdSqlDialect"/> class.
        /// </summary>
        internal FirebirdSqlDialect()
            : base(FirebirdSqlCharacters.Instance)
        {
        }

        public override bool SupportsIdentity
        {
            get
            {
                return true;
            }
        }

        public override SqlQuery BuildSelectIdentitySqlQuery(IObjectInfo objectInfo)
        {
            if (objectInfo == null)
            {
                throw new ArgumentNullException("objectInfo");
            }

            return new SqlQuery("RETURNING " + objectInfo.TableInfo.IdentifierColumn.ColumnName);
        }

        public override SqlQuery PageQuery(SqlQuery sqlQuery, PagingOptions pagingOptions)
        {
            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            var arguments = new object[sqlQuery.Arguments.Count + 2];
            Array.Copy(sqlQuery.GetArgumentArray(), 0, arguments, 0, sqlQuery.Arguments.Count);
            arguments[arguments.Length - 2] = pagingOptions.Offset + 1;
            arguments[arguments.Length - 1] = pagingOptions.Offset + pagingOptions.Count;

            var stringBuilder = new StringBuilder(sqlQuery.CommandText)
                .Replace(Environment.NewLine, string.Empty)
                .Append(" ROWS ")
                .Append(this.SqlCharacters.GetParameterName(arguments.Length - 2))
                .Append(" TO ")
                .Append(this.SqlCharacters.GetParameterName(arguments.Length - 1));

            return new SqlQuery(stringBuilder.ToString(), arguments);
        }
    }
}