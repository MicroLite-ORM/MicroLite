// -----------------------------------------------------------------------
// <copyright file="MsSqlDialect.cs" company="MicroLite">
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
namespace MicroLite.Dialect
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The implementation of <see cref="ISqlDialect"/> for MsSql server.
    /// </summary>
    internal sealed class MsSqlDialect : SqlDialect
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MsSqlDialect"/> class.
        /// </summary>
        /// <remarks>Constructor needs to be public so that it can be instantiated by SqlDialectFactory.</remarks>
        public MsSqlDialect()
            : base(SqlCharacters.MsSql)
        {
        }

        /// <summary>
        /// Gets the select identity string.
        /// </summary>
        protected override string SelectIdentityString
        {
            get
            {
                return "SELECT SCOPE_IDENTITY()";
            }
        }

        public override SqlQuery Combine(IEnumerable<SqlQuery> sqlQueries)
        {
            if (sqlQueries == null)
            {
                throw new ArgumentNullException("sqlQueries");
            }

            int argumentsCount = 0;
            var stringBuilder = new StringBuilder(sqlQueries.Sum(s => s.CommandText.Length));

            foreach (var sqlQuery in sqlQueries)
            {
                argumentsCount += sqlQuery.Arguments.Count;

                var commandText = sqlQuery.CommandText.StartsWith("EXEC", StringComparison.OrdinalIgnoreCase)
                    ? sqlQuery.CommandText
                    : SqlUtility.RenumberParameters(sqlQuery.CommandText, argumentsCount);

                stringBuilder.AppendLine(commandText + this.SqlCharacters.StatementSeparator);
            }

            var combinedQuery = new SqlQuery(stringBuilder.ToString(0, stringBuilder.Length - 3), sqlQueries.SelectMany(s => s.Arguments).ToArray());
            combinedQuery.Timeout = sqlQueries.Max(s => s.Timeout);

            return combinedQuery;
        }

        public override SqlQuery PageQuery(SqlQuery sqlQuery, PagingOptions pagingOptions)
        {
            int fromRowNumber = pagingOptions.Offset + 1;
            int toRowNumber = pagingOptions.Offset + pagingOptions.Count;

            var arguments = new object[sqlQuery.Arguments.Count + 2];
            sqlQuery.Arguments.CopyTo(arguments, 0);
            arguments[arguments.Length - 2] = fromRowNumber;
            arguments[arguments.Length - 1] = toRowNumber;

            var selectStatement = SqlUtility.ReadSelectClause(sqlQuery.CommandText);
            var qualifiedTableName = SqlUtility.ReadTableName(sqlQuery.CommandText);
            var position = qualifiedTableName.LastIndexOf('.') + 1;
            var tableName = position > 0 ? qualifiedTableName.Substring(position, qualifiedTableName.Length - position) : qualifiedTableName;

            var whereValue = SqlUtility.ReadWhereClause(sqlQuery.CommandText);
            var whereClause = !string.IsNullOrEmpty(whereValue) ? " WHERE " + whereValue : string.Empty;

            var orderByValue = SqlUtility.ReadOrderByClause(sqlQuery.CommandText);
            var orderByClause = "ORDER BY " + (!string.IsNullOrEmpty(orderByValue) ? orderByValue : "(SELECT NULL)");

            var stringBuilder = new StringBuilder(sqlQuery.CommandText.Length * 2)
                .Append("SELECT ")
                .Append(selectStatement)
                .Append(" FROM")
                .AppendFormat(CultureInfo.InvariantCulture, " (SELECT {0}, ROW_NUMBER() OVER({1}) AS RowNumber FROM {2}{3}) AS {4}", selectStatement, orderByClause, qualifiedTableName, whereClause, tableName)
                .AppendFormat(CultureInfo.InvariantCulture, " WHERE (RowNumber >= {0} AND RowNumber <= {1})", this.SqlCharacters.GetParameterName(arguments.Length - 2), this.SqlCharacters.GetParameterName(arguments.Length - 1));

            return new SqlQuery(stringBuilder.ToString(), arguments);
        }

        protected override string GetCommandText(string commandText)
        {
            if (commandText.StartsWith("EXEC", StringComparison.OrdinalIgnoreCase)
                && !commandText.Contains(this.SqlCharacters.StatementSeparator))
            {
                var firstParameterPosition = SqlUtility.GetFirstParameterPosition(commandText);

                if (firstParameterPosition > 4)
                {
                    return commandText.Substring(4, firstParameterPosition - 4).Trim();
                }
                else
                {
                    return commandText.Substring(4, commandText.Length - 4).Trim();
                }
            }

            return base.GetCommandText(commandText);
        }

        protected override CommandType GetCommandType(string commandText)
        {
            if (commandText.StartsWith("EXEC", StringComparison.OrdinalIgnoreCase)
                && !commandText.Contains(this.SqlCharacters.StatementSeparator))
            {
                return CommandType.StoredProcedure;
            }

            return base.GetCommandType(commandText);
        }
    }
}