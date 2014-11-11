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
    using System.Data;
    using System.Text;
    using MicroLite.Characters;
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

        public override SqlQuery PageQuery(SqlQuery sqlQuery, PagingOptions pagingOptions)
        {
            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            var arguments = new SqlArgument[sqlQuery.Arguments.Count + 2];
            Array.Copy(sqlQuery.ArgumentsArray, 0, arguments, 0, sqlQuery.Arguments.Count);
            arguments[arguments.Length - 2] = new SqlArgument(pagingOptions.Offset + 1, DbType.Int32);
            arguments[arguments.Length - 1] = new SqlArgument(pagingOptions.Offset + pagingOptions.Count, DbType.Int32);

            var stringBuilder = new StringBuilder(sqlQuery.CommandText)
                .Replace(Environment.NewLine, string.Empty)
                .Append(" ROWS ")
                .Append(this.SqlCharacters.GetParameterName(arguments.Length - 2))
                .Append(" TO ")
                .Append(this.SqlCharacters.GetParameterName(arguments.Length - 1));

            return new SqlQuery(stringBuilder.ToString(), arguments);
        }

        protected override string BuildInsertCommandText(IObjectInfo objectInfo)
        {
            if (objectInfo == null)
            {
                throw new ArgumentNullException("objectInfo");
            }

            var commandText = base.BuildInsertCommandText(objectInfo);

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.Sequence)
            {
                var firstParenthesisIndex = commandText.IndexOf('(') + 1;

                commandText = commandText.Insert(
                    firstParenthesisIndex,
                    this.SqlCharacters.EscapeSql(objectInfo.TableInfo.IdentifierColumn.ColumnName) + ",");

                var secondParenthesisIndex = commandText.IndexOf('(', firstParenthesisIndex) + 1;

                commandText = commandText.Insert(
                    secondParenthesisIndex,
                    "GEN_ID(" + objectInfo.TableInfo.IdentifierColumn.SequenceName + ", 1),");
            }

            if (objectInfo.TableInfo.IdentifierStrategy != IdentifierStrategy.Assigned)
            {
                commandText += " RETURNING " + objectInfo.TableInfo.IdentifierColumn.ColumnName;
            }

            return commandText;
        }
    }
}