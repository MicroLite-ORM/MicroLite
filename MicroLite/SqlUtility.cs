// -----------------------------------------------------------------------
// <copyright file="SqlUtility.cs" company="MicroLite">
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
namespace MicroLite
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A utility class containing useful methods for manipulating Sql.
    /// </summary>
    public static class SqlUtility
    {
        private static readonly string[] emptyStringArray = new string[0];
        private static readonly Regex orderByRegex = new Regex("(?<=ORDER BY)(.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        private static readonly char[] parameterIdentifiers = new[] { '@', ':', '?' };
        private static readonly Regex parameterRegex = new Regex(@"((@|:)[\w]+)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.Multiline);
        private static readonly Regex selectRegex = new Regex("(?<=SELECT)(.+?)(?=FROM)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        private static readonly Regex tableNameRegexGreedy = new Regex("(?<=FROM)(.+)(?=WHERE)|(?<=FROM)(.+)(?=ORDER BY)|(?<=FROM)(.+)(?=WHERE)?|(?<=FROM)(.+)(?=ORDER BY)?", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        private static readonly Regex tableNameRegexLazy = new Regex("(?<=FROM)(.+?)(?=WHERE)|(?<=FROM)(.+?)(?=ORDER BY)|(?<=FROM)(.+?)(?=WHERE)?|(?<=FROM)(.+?)(?=ORDER BY)?", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        private static readonly Regex whereRegex = new Regex("(?<=WHERE)(.+)(?=ORDER BY)|(?<=WHERE)(.+)(?=ORDER BY)?", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);

        /// <summary>
        /// Gets the position of the first parameter in the specified command text.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The position of the first parameter in the command text or -1 if no parameters are found.</returns>
        public static int GetFirstParameterPosition(string commandText)
        {
            if (commandText == null)
            {
                throw new ArgumentNullException("commandText");
            }

            var firstParameterPosition = commandText.IndexOfAny(parameterIdentifiers, 0);

            return firstParameterPosition;
        }

        /// <summary>
        /// Gets the parameter names from the specified command text.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The parameter names in the command text.</returns>
        public static IList<string> GetParameterNames(string commandText)
        {
            if (commandText == null)
            {
                throw new ArgumentNullException("commandText");
            }

            var match = parameterRegex.Match(commandText);

            if (!match.Success)
            {
                return emptyStringArray;
            }

            var list = new List<string>();

            do
            {
                if (!list.Contains(match.Value))
                {
                    list.Add(match.Value);
                }

                match = match.NextMatch();
            }
            while (match.Success);

            return list;
        }

        /// <summary>
        /// Reads the ORDER BY clause from the specified command text excluding the ORDER BY keyword.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The ORDER BY clause without the ORDER BY keyword.</returns>
        public static string ReadOrderByClause(string commandText)
        {
            return ExtractUsingRegex(commandText, orderByRegex);
        }

        /// <summary>
        /// Reads the SELECT clause from the specified command text excluding the SELECT keyword.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The SELECT clause without the SELECT keyword.</returns>
        public static string ReadSelectClause(string commandText)
        {
            return ExtractUsingRegex(commandText, selectRegex);
        }

        /// <summary>
        /// Reads the name of the table the sql query is targeting.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The name of the table the sql query is targeting.</returns>
        public static string ReadTableName(string commandText)
        {
            var tableName = ExtractUsingRegex(commandText, tableNameRegexLazy);

            if (string.IsNullOrEmpty(tableName))
            {
                tableName = ExtractUsingRegex(commandText, tableNameRegexGreedy);
            }

            return tableName;
        }

        /// <summary>
        /// Reads the WHERE clause from the specified command text excluding the WHERE keyword.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The WHERE clause without the WHERE keyword.</returns>
        public static string ReadWhereClause(string commandText)
        {
            return ExtractUsingRegex(commandText, whereRegex);
        }

        /// <summary>
        /// Re-numbers the parameters in the SQL based upon the total number of arguments.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="totalArgumentCount">The total number of arguments.</param>
        /// <returns>The re-numbered SQL</returns>
        public static string RenumberParameters(string sql, int totalArgumentCount)
        {
            var parameterNames = GetParameterNames(sql);

            if (parameterNames.Count == 0)
            {
                return sql;
            }

            var argsAdded = 0;
            var parameterPrefix = parameterNames[0].Substring(0, 2);

            var predicateReWriter = new StringBuilder(sql);

            foreach (var parameterName in parameterNames.OrderByDescending(n => n))
            {
                var newParameterName = parameterPrefix + (totalArgumentCount - ++argsAdded).ToString(CultureInfo.InvariantCulture);

                predicateReWriter.Replace(parameterName, newParameterName);
            }

            return predicateReWriter.ToString();
        }

        private static string ExtractUsingRegex(string commandText, Regex regex)
        {
            var match = regex.Match(commandText);

            if (!match.Success)
            {
                return string.Empty;
            }

            var sql = match.Value.Trim();

            if (sql.Contains(Environment.NewLine))
            {
                sql = sql.Replace(Environment.NewLine, Strings.Space);
            }

            return sql;
        }
    }
}