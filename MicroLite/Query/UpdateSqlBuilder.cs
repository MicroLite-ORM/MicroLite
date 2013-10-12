// -----------------------------------------------------------------------
// <copyright file="UpdateSqlBuilder.cs" company="MicroLite">
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
    internal sealed class UpdateSqlBuilder : SqlBuilder, IUpdate, ISetOrWhere
    {
        private readonly SqlCharacters sqlCharacters;

        internal UpdateSqlBuilder(SqlCharacters sqlCharacters)
        {
            this.sqlCharacters = sqlCharacters;
            this.InnerSql.Append("UPDATE ");
        }

        public ISetOrWhere SetColumnValue(string columnName, object columnValue)
        {
            if (this.Arguments.Count > 0)
            {
                this.InnerSql.Append(", ");
            }
            else
            {
                this.InnerSql.Append(" ");
            }

            this.InnerSql.Append(this.sqlCharacters.EscapeSql(columnName));
            this.InnerSql.Append(" = ");

            this.InnerSql.Append(this.sqlCharacters.GetParameterName(this.Arguments.Count));
            this.Arguments.Add(columnValue);

            return this;
        }

        public ISetOrWhere Table(string tableName)
        {
            this.InnerSql.Append(this.sqlCharacters.EscapeSql(tableName));
            this.InnerSql.Append(" SET");

            return this;
        }

        public ISetOrWhere Table(Type forType)
        {
            var objectInfo = ObjectInfo.For(forType);

            if (!string.IsNullOrEmpty(objectInfo.TableInfo.Schema))
            {
                this.InnerSql.Append(this.sqlCharacters.LeftDelimiter);
                this.InnerSql.Append(objectInfo.TableInfo.Schema);
                this.InnerSql.Append(this.sqlCharacters.RightDelimiter);
                this.InnerSql.Append(".");
            }

            this.InnerSql.Append(this.sqlCharacters.LeftDelimiter);
            this.InnerSql.Append(objectInfo.TableInfo.Name);
            this.InnerSql.Append(this.sqlCharacters.RightDelimiter);
            this.InnerSql.Append(" SET");

            return this;
        }

        public IToSqlQuery Where(string columnName, object columnValue)
        {
            this.InnerSql.Append(" WHERE ");
            this.InnerSql.Append(this.sqlCharacters.EscapeSql(columnName));
            this.InnerSql.Append(" = ");

            this.InnerSql.Append(this.sqlCharacters.GetParameterName(this.Arguments.Count));
            this.Arguments.Add(columnValue);

            return this;
        }
    }
}