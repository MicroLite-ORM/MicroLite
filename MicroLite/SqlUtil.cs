// -----------------------------------------------------------------------
// <copyright file="SqlUtil.cs" company="MicroLite">
// Copyright 2012 Trevor Pilley
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A utility class containing useful methods for manipulating Sql.
    /// </summary>
    internal static class SqlUtil
    {
        private static readonly Regex parameterRegex = new Regex(@"(@[\w]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);

        /// <summary>
        /// Combines the specified SQL queries into a single SqlQuery.
        /// </summary>
        /// <param name="sqlQueries">The SQL queries to be combined.</param>
        /// <returns>The combined <see cref="SqlQuery" />.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if sqlQueries is null.</exception>
        internal static SqlQuery Combine(IEnumerable<SqlQuery> sqlQueries)
        {
            if (sqlQueries == null)
            {
                throw new ArgumentNullException("sqlQueries");
            }

            int argumentsCount = 0;
            var sqlBuilder = new StringBuilder();

            foreach (var sqlQuery in sqlQueries)
            {
                argumentsCount += sqlQuery.Arguments.Count;

                var sql = ReNumberParameters(sqlQuery.CommandText, argumentsCount);

                sqlBuilder.AppendLine(sql + ";");
            }

            var combinedQuery = new SqlQuery(sqlBuilder.ToString(0, sqlBuilder.Length - 3), sqlQueries.SelectMany(s => s.Arguments).ToArray());
            combinedQuery.Timeout = sqlQueries.Max(s => s.Timeout);

            return combinedQuery;
        }

        /// <summary>
        /// Gets the type of the command.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The <see cref="CommandType"/> for the specified command text</returns>
        internal static CommandType GetCommandType(string commandText)
        {
            if (commandText.StartsWith("EXEC", StringComparison.OrdinalIgnoreCase))
            {
                return CommandType.StoredProcedure;
            }

            return CommandType.Text;
        }

        /// <summary>
        /// Gets the position of the first parameter in the specified command text.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The position of the first parameter in the command text or -1 if no parameters are found.</returns>
        internal static int GetFirstParameterPosition(string commandText)
        {
            var firstParameterPosition = commandText.IndexOf('@', 0);

            return firstParameterPosition;
        }

        /// <summary>
        /// Gets the parameter names from the specified command text.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The parameter names in the command text.</returns>
        internal static IList<string> GetParameterNames(string commandText)
        {
            return new HashSet<string>(parameterRegex.Matches(commandText).Cast<Match>().Select(x => x.Value)).ToList();
        }

        /// <summary>
        /// Re-numbers the parameters in the SQL based upon the total number of arguments.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="totalArgumentCount">The total number of arguments.</param>
        /// <returns>The re-numbered SQL</returns>
        internal static string ReNumberParameters(string sql, int totalArgumentCount)
        {
            var argsAdded = 0;

            var predicateReWriter = new StringBuilder(sql);

            var parameterNames = new HashSet<string>(parameterRegex.Matches(sql).Cast<Match>().Select(x => x.Value));

            if (parameterNames.Count > 0)
            {
                var parameterPrefix = parameterNames.First().Substring(0, 2);

                foreach (var parameterName in parameterNames.OrderByDescending(n => n))
                {
                    var newParameterName = parameterPrefix + (totalArgumentCount - ++argsAdded).ToString(CultureInfo.InvariantCulture);

                    predicateReWriter.Replace(parameterName, newParameterName);
                }
            }

            return predicateReWriter.ToString();
        }
    }
}