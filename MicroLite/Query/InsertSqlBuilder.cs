// -----------------------------------------------------------------------
// <copyright file="InsertSqlBuilder.cs" company="MicroLite">
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
    internal sealed class InsertSqlBuilder : SqlBuilder, IInto, IInsertValue
    {
        private readonly SqlCharacters sqlCharacters;

        internal InsertSqlBuilder(SqlCharacters sqlCharacters)
        {
            this.sqlCharacters = sqlCharacters;
            this.InnerSql.Append("INSERT INTO ");
        }

        public IInsertValue Into(string tableName)
        {
            this.InnerSql.Append(this.sqlCharacters.EscapeSql(tableName));
            this.InnerSql.Append(" (");

            return this;
        }

        public IInsertValue Into(Type forType)
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
            this.InnerSql.Append(" (");

            return this;
        }

        public override SqlQuery ToSqlQuery()
        {
            this.InnerSql.Append(") VALUES (");

            for (int i = 0; i < this.Arguments.Count; i++)
            {
                this.InnerSql.Append(this.sqlCharacters.GetParameterName(i));

                if (i < this.Arguments.Count - 1)
                {
                    this.InnerSql.Append(", ");
                }
            }

            this.InnerSql.Append(")");

            return base.ToSqlQuery();
        }

        public IInsertValue Value(string columnName, object columnValue)
        {
            if (this.Arguments.Count > 0)
            {
                this.InnerSql.Append(", ");
            }

            this.InnerSql.Append(this.sqlCharacters.EscapeSql(columnName));
            this.Arguments.Add(columnValue);

            return this;
        }
    }
}