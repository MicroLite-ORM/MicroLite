// -----------------------------------------------------------------------
// <copyright file="SqlUtility.cs" company="Project Contributors">
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
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace MicroLite
{
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
        public static int GetFirstParameterPosition(string commandText) => GetParameterPosition(commandText, 0, parameterIdentifiers);

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
            if (commandText is null)
            {
                throw new ArgumentNullException(nameof(commandText));
            }

            var startIndex = GetParameterPosition(commandText, 0, namedParameterIdentifiers);

            if (startIndex == -1)
            {
                return emptyParameterNames;
            }

            var parameterNames = new List<string>();

            while (startIndex > -1 && startIndex < commandText.Length)
            {
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

                startIndex = GetParameterPosition(commandText, startIndex + 1, namedParameterIdentifiers);
            }

            return parameterNames;
        }

        /// <summary>
        /// Re-numbers the parameters in the SQL based upon the total number of arguments.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <param name="totalArgumentCount">The total number of arguments.</param>
        /// <returns>The re-numbered SQL.</returns>
        public static string RenumberParameters(string sql, int totalArgumentCount)
        {
            var parameterNames = GetParameterNames(sql);

            if (parameterNames.Count == 0)
            {
                return sql;
            }

            var argsAdded = 0;
            var charIndex = 0;
            var parameterPrefix = parameterNames[0].Substring(0, parameterNames[0].IndexOfAny(digits));
            var predicateReWriter = new StringBuilder(sql, capacity: sql.Length + parameterNames.Count);

            foreach (var parameterName in parameterNames.OrderByDescending(n => n, ParameterNameComparer.Instance))
            {
                charIndex = sql.Length;
                var newParameterName = parameterPrefix + (totalArgumentCount - ++argsAdded).ToString(CultureInfo.InvariantCulture);

                while ((charIndex = sql.LastIndexOf(parameterName, charIndex, StringComparison.Ordinal)) > -1)
                {
                    // If there is another digit following the parameter, continue (e.g. @p10 will match when looking for @p1
                    var indexFollowingParameter = charIndex + parameterName.Length;
                    if (indexFollowingParameter < sql.Length && char.IsDigit(sql[indexFollowingParameter]))
                    {
                        continue;
                    }

                    // If the parameters are out of sort-order in the SQL statement,
                    // we end up altering the position of the lower parameter name
                    // e.g. replacing @p3 with @p10 when @p3 is before @p2 in the string means that @p2 exists 1 postion later than before
                    while (predicateReWriter[charIndex] != newParameterName[0])
                    {
                        charIndex++;
                    }

                    predicateReWriter.Replace(parameterName, newParameterName, charIndex, parameterName.Length);
                }
            }

            return predicateReWriter.ToString();
        }

        private static int GetParameterPosition(string commandText, int position, char[] matchParameterIdentifiers)
        {
            if (commandText is null)
            {
                throw new ArgumentNullException(nameof(commandText));
            }

            if (position > commandText.Length)
            {
                throw new ArgumentOutOfRangeException(nameof(position));
            }

            var parameterPosition = commandText.IndexOfAny(matchParameterIdentifiers, position);

            while (parameterPosition > -1)
            {
                // Ignore identifiers followed by whitespace or linebreak.
                if (parameterPosition < commandText.Length && char.IsControl(commandText[parameterPosition + 1]))
                {
                    parameterPosition = commandText.IndexOfAny(matchParameterIdentifiers, parameterPosition + 1);
                    continue;
                }

                // Ignore @@ as we would have for SELECT @@Identity.
                if ((parameterPosition < commandText.Length && matchParameterIdentifiers.Contains(commandText[parameterPosition + 1]))
                    || (parameterPosition > 0 && matchParameterIdentifiers.Contains(commandText[parameterPosition - 1])))
                {
                    parameterPosition = commandText.IndexOfAny(matchParameterIdentifiers, parameterPosition + 1);
                    continue;
                }

                // Ignore identifiers inside a quoted string.
                if (commandText.IndexOfAny(quoteCharacters, position, parameterPosition - position) > -1
                    && commandText.IndexOfAny(quoteCharacters, parameterPosition) > -1)
                {
                    int quoteCount = 0;

                    for (int i = parameterPosition; i >= 0; i--)
                    {
                        if (commandText[i] == '\'')
                        {
                            quoteCount++;
                        }
                    }

                    if (quoteCount % 2 != 0)
                    {
                        parameterPosition = commandText.IndexOfAny(matchParameterIdentifiers, parameterPosition + 1);
                        continue;
                    }
                }

                break;
            }

            return parameterPosition;
        }
    }
}
