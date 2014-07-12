// -----------------------------------------------------------------------
// <copyright file="DeleteSqlBuilder.cs" company="MicroLite">
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
namespace MicroLite.Builder
{
    using System;
    using MicroLite.Mapping;

    [System.Diagnostics.DebuggerDisplay("{InnerSql}")]
    internal sealed class DeleteSqlBuilder : SqlBuilderBase, IDeleteFrom, IWhereEquals
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="DeleteSqlBuilder"/> class with the starting command text 'DELETE FROM '.
        /// </summary>
        /// <param name="sqlCharacters">The SQL characters.</param>
        internal DeleteSqlBuilder(SqlCharacters sqlCharacters)
            : base(sqlCharacters)
        {
            this.InnerSql.Append("DELETE FROM ");
        }

        public IWhereEquals From(string table)
        {
            this.AppendTableName(table);

            return this;
        }

        public IWhereEquals From(Type forType)
        {
            var objectInfo = ObjectInfo.For(forType);

            return this.From(objectInfo);
        }

        public IToSqlQuery WhereEquals(string column, object comparisonValue)
        {
            this.InnerSql.Append(" WHERE ")
                .Append(this.SqlCharacters.EscapeSql(column))
                .Append(" = ")
                .Append(this.SqlCharacters.GetParameterName(this.Arguments.Count));

            this.Arguments.Add(comparisonValue);

            return this;
        }

        internal IWhereEquals From(IObjectInfo objectInfo)
        {
            this.AppendTableName(objectInfo);

            return this;
        }
    }
}