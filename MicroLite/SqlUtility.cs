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
        private static readonly char[] namedParameterIdentifiers = new[] { '@', ':' };
        private static readonly char[] parameterIdentifiers = new[] { '@', ':', '?' };
        private static readonly char[] quoteCharacters = new[] { '\'' };

        /// <summary>
        /// Gets the position of the first parameter in the specified SQL command text.
        /// </summary>
        /// <param name="commandText">The SQL command text.</param>
        /// <returns>
        /// The position of the first parameter in the command text or -1 if no parameters are found.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if commandText is null.</exception>
        public static int GetFirstParameterPosition(string commandText)
        {
            if (commandText == null)
            {
                throw new ArgumentNullException("commandText");
            }

            var firstParameterPosition = commandText.IndexOfAny(parameterIdentifiers);

            while (firstParameterPosition > -1)
            {
                // Ignore identifiers inside a quoted string
                if (commandText.IndexOfAny(quoteCharacters, 0, firstParameterPosition) > -1
                    && commandText.IndexOfAny(quoteCharacters, firstParameterPosition) > -1)
                {
                    firstParameterPosition = commandText.IndexOfAny(parameterIdentifiers, firstParameterPosition + 1);
                    continue;
                }

                break;
            }

            return firstParameterPosition;
        }

        /// <summary>
        /// Gets the parameter names from the specified SQL command text or an empty list if the command text does not
        /// contain any named parameters.
        /// </summary>
        /// <param name="commandText">The SQL command text.</param>
        /// <returns>
        /// A list containing the parameter names in the SQL command text.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if commandText is null.</exception>
        /// <remarks>
        /// Will return an empty list if the command text has no parameter names or the parameter names are all '?'
        /// (if the dialect does not support named parameters).
        /// </remarks>
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

            var startIndex = commandText.IndexOfAny(namedParameterIdentifiers);

            while (startIndex > -1)
            {
                // Ignore @@ as we would have for SELECT @@Identity
                if ((startIndex < commandText.Length && commandText[startIndex + 1] == '@')
                    || (startIndex > 0 && commandText[startIndex - 1] == '@'))
                {
                    startIndex = commandText.IndexOfAny(namedParameterIdentifiers, startIndex + 1);
                    continue;
                }

                var endIndex = commandText.Length;

                for (int i = startIndex + 1; i < commandText.Length; i++)
                {
                    var character = commandText[i];

                    if (!char.IsLetterOrDigit(character) && character != '_')
                    {
                        endIndex = i;
                        break;
                    }
                }

                var length = endIndex - startIndex;

                var parameter = commandText.Substring(startIndex, length);

                if (!parameterNames.Contains(parameter))
                {
                    parameterNames.Add(parameter);
                }

                startIndex = commandText.IndexOfAny(namedParameterIdentifiers, startIndex + 1);
            }

            return parameterNames;
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