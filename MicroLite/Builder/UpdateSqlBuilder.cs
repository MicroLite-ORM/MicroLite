// -----------------------------------------------------------------------
// <copyright file="UpdateSqlBuilder.cs" company="Project Contributors">
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
using MicroLite.Builder.Syntax.Write;
using MicroLite.Characters;
using MicroLite.FrameworkExtensions;
using MicroLite.Mapping;

namespace MicroLite.Builder
{
    [System.Diagnostics.DebuggerDisplay("{InnerSql}")]
    internal sealed class UpdateSqlBuilder : WriteSqlBuilderBase, IUpdate, ISetOrWhere
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="UpdateSqlBuilder"/> class with the starting command text 'UPDATE '.
        /// </summary>
        /// <param name="sqlCharacters">The SQL characters.</param>
        internal UpdateSqlBuilder(SqlCharacters sqlCharacters)
            : base(sqlCharacters) => InnerSql.Append("UPDATE ");

        public ISetOrWhere SetColumnValue(string columnName, object columnValue)
        {
            if (Arguments.Count > 0)
            {
                InnerSql.Append(',');
            }

            InnerSql.Append(SqlCharacters.EscapeSql(columnName))
                .Append(" = ")
                .Append(SqlCharacters.GetParameterName(Arguments.Count));

            Arguments.Add(new SqlArgument(columnValue));

            return this;
        }

        public ISetOrWhere Table(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ArgumentException(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("tableName"));
            }

            AppendTableName(tableName);
            InnerSql.Append(" SET ");

            return this;
        }

        public ISetOrWhere Table(Type forType)
        {
            IObjectInfo objectInfo = ObjectInfo.For(forType);

            return Table(objectInfo);
        }

        internal ISetOrWhere Table(IObjectInfo objectInfo)
        {
            AppendTableName(objectInfo);
            InnerSql.Append(" SET ");

            return this;
        }
    }
}
