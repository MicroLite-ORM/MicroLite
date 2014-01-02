// -----------------------------------------------------------------------
// <copyright file="DeleteSqlBuilder.cs" company="MicroLite">
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
namespace MicroLite.Query
{
    using System;
    using MicroLite.Mapping;

    [System.Diagnostics.DebuggerDisplay("{InnerSql}")]
    internal sealed class DeleteSqlBuilder : SqlBuilder, IDeleteFrom, IWhereEquals
    {
        internal DeleteSqlBuilder(SqlCharacters sqlCharacters)
            : base(sqlCharacters)
        {
            this.InnerSql.Append("DELETE FROM ");
        }

        public IWhereEquals From(IObjectInfo objectInfo)
        {
            this.AppendTableName(objectInfo);

            return this;
        }

        public IWhereEquals From(string tableName)
        {
            this.AppendTableName(tableName);

            return this;
        }

        public IWhereEquals From(Type forType)
        {
            var objectInfo = ObjectInfo.For(forType);

            return this.From(objectInfo);
        }

        public IWhereEquals WhereEquals(string columnName, object comparisonValue)
        {
            this.InnerSql.Append(" WHERE ")
                .Append(this.SqlCharacters.EscapeSql(columnName))
                .Append(" = ")
                .Append(this.SqlCharacters.GetParameterName(this.Arguments.Count));

            this.Arguments.Add(comparisonValue);

            return this;
        }
    }
}