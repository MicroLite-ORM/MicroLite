// -----------------------------------------------------------------------
// <copyright file="InsertSqlBuilder.cs" company="Project Contributors">
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
using MicroLite.Builder.Syntax;
using MicroLite.Builder.Syntax.Write;
using MicroLite.Characters;
using MicroLite.Mapping;

namespace MicroLite.Builder
{
    [System.Diagnostics.DebuggerDisplay("{InnerSql}")]
    internal sealed class InsertSqlBuilder : SqlBuilderBase, IInsertIntoTable, IInsertColumn
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="InsertSqlBuilder"/> class with the starting command text 'INSERT INTO '.
        /// </summary>
        /// <param name="sqlCharacters">The SQL characters.</param>
        internal InsertSqlBuilder(SqlCharacters sqlCharacters)
            : base(sqlCharacters) => InnerSql.Append("INSERT INTO ");

        public IInsertValue Columns(params string[] columnNames)
        {
            InnerSql.Append(" (");

            if (columnNames != null)
            {
                for (int i = 0; i < columnNames.Length; i++)
                {
                    if (i > 0)
                    {
                        InnerSql.Append(',');
                    }

                    InnerSql.Append(SqlCharacters.EscapeSql(columnNames[i]));
                }
            }

            InnerSql.Append(')');

            return this;
        }

        public IInsertColumn Into(string table)
        {
            AppendTableName(table);

            return this;
        }

        public IInsertColumn Into(Type forType)
        {
            IObjectInfo objectInfo = ObjectInfo.For(forType);

            return Into(objectInfo);
        }

        public IToSqlQuery Values(params object[] columnValues)
        {
            InnerSql.Append(" VALUES (");

            if (columnValues != null)
            {
                for (int i = 0; i < columnValues.Length; i++)
                {
                    Arguments.Add(new SqlArgument(columnValues[i]));

                    InnerSql.Append(SqlCharacters.GetParameterName(i));

                    if (i < columnValues.Length - 1)
                    {
                        InnerSql.Append(',');
                    }
                }
            }

            InnerSql.Append(')');

            return this;
        }

        internal IInsertColumn Into(IObjectInfo objectInfo)
        {
            AppendTableName(objectInfo);

            return this;
        }
    }
}
