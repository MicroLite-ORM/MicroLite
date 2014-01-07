// -----------------------------------------------------------------------
// <copyright file="SqlUtility.cs" company="MicroLite">
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
        private static readonly char[] parameterIdentifiers = new[] { '@', ':', '?' };
        private static readonly Regex parameterRegex = new Regex(@"((@|:)[\w]+)", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.Multiline);

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
            if (string.IsNullOrEmpty(commandText))
            {
                return string.Empty;
            }

            var segmentPositions = SegmentPositions.GetSegmentPositions(commandText);

            if (segmentPositions.OrderByIndex == -1)
            {
                return string.Empty;
            }

            var startIndex = segmentPositions.OrderByIndex + 8; // Remove the ORDER BY keyword
            var length = commandText.Length - startIndex;

            var segment = commandText.Substring(startIndex, length).Trim();

            if (segment.Contains(Environment.NewLine))
            {
                segment = segment.Replace(Environment.NewLine, Strings.Space);
            }

            return segment;
        }

        /// <summary>
        /// Reads the SELECT clause from the specified command text excluding the SELECT keyword.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The SELECT clause without the SELECT keyword.</returns>
        public static string ReadSelectClause(string commandText)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                return string.Empty;
            }

            var segmentPositions = SegmentPositions.GetSegmentPositions(commandText);

            if (segmentPositions.FromIndex < 6)
            {
                return string.Empty;
            }

            var startIndex = 6; // Remove the SELECT keyword
            var length = segmentPositions.FromIndex - startIndex;

            var segment = commandText.Substring(startIndex, length).Trim();

            if (segment.Contains(Environment.NewLine))
            {
                segment = segment.Replace(Environment.NewLine, Strings.Space);
            }

            return segment;
        }

        /// <summary>
        /// Reads the name of the table the sql query is targeting.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The name of the table the sql query is targeting.</returns>
        public static string ReadTableName(string commandText)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                return string.Empty;
            }

            var segmentPositions = SegmentPositions.GetSegmentPositions(commandText);

            if (segmentPositions.FromIndex == -1)
            {
                return string.Empty;
            }

            var startIndex = segmentPositions.FromIndex + 4; // Start after the FROM keyword.
            var length = commandText.Length - startIndex;

            if (segmentPositions.WhereIndex != -1)
            {
                length = segmentPositions.WhereIndex - startIndex;
            }
            else if (segmentPositions.OrderByIndex != -1)
            {
                length = segmentPositions.OrderByIndex - startIndex;
            }

            var segment = commandText.Substring(startIndex, length).Trim();

            if (segment.Contains(Environment.NewLine))
            {
                segment = segment.Replace(Environment.NewLine, Strings.Space);
            }

            return segment;
        }

        /// <summary>
        /// Reads the WHERE clause from the specified command text excluding the WHERE keyword.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The WHERE clause without the WHERE keyword.</returns>
        public static string ReadWhereClause(string commandText)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                return string.Empty;
            }

            var segmentPositions = SegmentPositions.GetSegmentPositions(commandText);

            if (segmentPositions.WhereIndex == -1)
            {
                return string.Empty;
            }

            var startIndex = segmentPositions.WhereIndex + 5; // Start after the WHERE keyword.
            var length = segmentPositions.OrderByIndex != -1
                ? segmentPositions.OrderByIndex - startIndex
                : commandText.Length - startIndex;

            var segment = commandText.Substring(startIndex, length).Trim();

            if (segment.Contains(Environment.NewLine))
            {
                segment = segment.Replace(Environment.NewLine, Strings.Space);
            }

            return segment;
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

        private struct SegmentPositions
        {
            public int FromIndex
            {
                get;
                set;
            }

            public int OrderByIndex
            {
                get;
                set;
            }

            public int WhereIndex
            {
                get;
                set;
            }

            internal static SegmentPositions GetSegmentPositions(string commandText)
            {
                var segmentPositions = new SegmentPositions
                {
                    FromIndex = commandText.IndexOf("FROM", StringComparison.OrdinalIgnoreCase),
                    OrderByIndex = commandText.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase),
                    WhereIndex = commandText.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase)
                };

                return segmentPositions;
            }
        }
    }
}