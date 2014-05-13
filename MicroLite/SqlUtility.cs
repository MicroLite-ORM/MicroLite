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

    /// <summary>
    /// A utility class containing useful methods for manipulating SQL.
    /// </summary>
    public static class SqlUtility
    {
        private static readonly char[] digits = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
        private static readonly string[] emptyParameterNames = new string[0];
        private static readonly char[] parameterIdentifiers = new[] { '@', ':', '?' };

        /// <summary>
        /// Gets the position of the first parameter in the specified SQL command text.
        /// </summary>
        /// <param name="commandText">The SQL command text.</param>
        /// <returns>The position of the first parameter in the command text or -1 if no parameters are found.</returns>
        public static int GetFirstParameterPosition(string commandText)
        {
            if (commandText == null)
            {
                throw new ArgumentNullException("commandText");
            }

            var firstParameterPosition = commandText.IndexOfAny(parameterIdentifiers);

            return firstParameterPosition;
        }

        /// <summary>
        /// Gets the parameter names from the specified SQL command text or an empty list if the command text does not contain any named parameters.
        /// </summary>
        /// <param name="commandText">The SQL command text.</param>
        /// <returns>A list containing the parameter names in the SQL command text.</returns>
        /// <remarks>Will return an empty list if the command text has no parameter names or the parameter names are all '?' (if the dialect does not support named parameters).</remarks>
        public static IList<string> GetParameterNames(string commandText)
        {
            if (commandText == null)
            {
                throw new ArgumentNullException("commandText");
            }

            if (GetFirstParameterPosition(commandText) == -1)
            {
                return emptyParameterNames;
            }

            var parameterNames = new List<string>();

            var startIndex = 0;
            var startsWithParameter = false;

            for (int i = 0; i < commandText.Length; i++)
            {
                var character = commandText[i];

                if (character == '@' || character == ':')
                {
                    // Ignore @@ as we would have for SELECT @@Identity
                    if ((i < commandText.Length && commandText[i + 1] == '@')
                        || (i > 0 && commandText[i - 1] == '@'))
                    {
                        continue;
                    }

                    startIndex = i;
                    startsWithParameter = i == 0;
                }
                else if ((startsWithParameter || startIndex > 0) && !char.IsLetterOrDigit(character) && character != '_')
                {
                    var length = i - startIndex;

                    var parameter = commandText.Substring(startIndex, length);

                    if (!parameterNames.Contains(parameter))
                    {
                        parameterNames.Add(parameter);
                    }

                    startIndex = 0;
                }
                else if ((startsWithParameter || startIndex > 0) && i == commandText.Length - 1)
                {
                    var length = commandText.Length - startIndex;

                    var parameter = commandText.Substring(startIndex, length);

                    if (!parameterNames.Contains(parameter))
                    {
                        parameterNames.Add(parameter);
                    }
                }
            }

            return parameterNames;
        }

        /// <summary>
        /// Reads the ORDER BY clause from the specified SQL command text excluding the ORDER BY keyword.
        /// </summary>
        /// <param name="commandText">The SQL command text.</param>
        /// <returns>The ORDER BY clause without the ORDER BY keyword, or an empty string if there is no ORDER BY clause.</returns>
        [Obsolete("This method has been supseceeded by the new SqlString class and its Parse method, please use that instead. This method will be removed in MicroLite 6.0", error: false)]
        public static string ReadOrderByClause(string commandText)
        {
            var sqlString = SqlString.Parse(commandText, Clauses.OrderBy);

            return sqlString.OrderBy;
        }

        /// <summary>
        /// Reads the SELECT clause from the specified SQL command text excluding the SELECT keyword.
        /// </summary>
        /// <param name="commandText">The SQL command text.</param>
        /// <returns>The SELECT clause without the SELECT keyword, or an empty string if there is no SELECT clause.</returns>
        [Obsolete("This method has been supseceeded by the new SqlString class and its Parse method, please use that instead. This method will be removed in MicroLite 6.0", error: false)]
        public static string ReadSelectClause(string commandText)
        {
            var sqlString = SqlString.Parse(commandText, Clauses.Select);

            return sqlString.Select;
        }

        /// <summary>
        /// Reads the name of the table the sql query is targeting.
        /// </summary>
        /// <param name="commandText">The SQL command text.</param>
        /// <returns>The name of the table the sql query is targeting.</returns>
        [Obsolete("This method has been supseceeded by the new SqlString class and its Parse method, please use that instead. This method will be removed in MicroLite 6.0", error: false)]
        public static string ReadTableName(string commandText)
        {
            var sqlString = SqlString.Parse(commandText, Clauses.From);

            return sqlString.From;
        }

        /// <summary>
        /// Reads the WHERE clause from the specified SQL command text excluding the WHERE keyword.
        /// </summary>
        /// <param name="commandText">The SQL command text.</param>
        /// <returns>The WHERE clause without the WHERE keyword, or an empty string if there is no WHERE clause.</returns>
        [Obsolete("This method has been supseceeded by the new SqlString class and its Parse method, please use that instead. This method will be removed in MicroLite 6.0", error: false)]
        public static string ReadWhereClause(string commandText)
        {
            var sqlString = SqlString.Parse(commandText, Clauses.Where);

            return sqlString.Where;
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
            var parameterPrefix = parameterNames[0].Substring(0, parameterNames[0].IndexOfAny(digits));

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