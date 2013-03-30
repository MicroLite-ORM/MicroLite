﻿// -----------------------------------------------------------------------
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
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;

    /// <summary>
    /// A utility class containing useful methods for manipulating Sql.
    /// </summary>
    internal static class SqlUtil
    {
        private static readonly Regex parameterRegex = new Regex(@"((@|:)[\w]+)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        private static char[] parameterIdentifiers = new[] { '@', ':', '?' };

        /// <summary>
        /// Gets the position of the first parameter in the specified command text.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The position of the first parameter in the command text or -1 if no parameters are found.</returns>
        internal static int GetFirstParameterPosition(string commandText)
        {
            var firstParameterPosition = commandText.IndexOfAny(parameterIdentifiers, 0);

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
            var parameterNames = GetParameterNames(sql);

            if (parameterNames.Count == 0)
            {
                return sql;
            }

            var argsAdded = 0;
            var parameterPrefix = parameterNames.First().Substring(0, 2);

            var predicateReWriter = new StringBuilder(sql);

            foreach (var parameterName in parameterNames.OrderByDescending(n => n))
            {
                var newParameterName = parameterPrefix + (totalArgumentCount - ++argsAdded).ToString(CultureInfo.InvariantCulture);

                predicateReWriter.Replace(parameterName, newParameterName);
            }

            return predicateReWriter.ToString();
        }
    }
}