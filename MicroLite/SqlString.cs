// -----------------------------------------------------------------------
// <copyright file="SqlString.cs" company="Project Contributors">
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

namespace MicroLite
{
    /// <summary>
    /// A class which contains the clauses of a SQL command.
    /// </summary>
    public sealed class SqlString
    {
        private SqlString()
        {
        }

        /// <summary>
        /// Gets value of the FROM clause of the command text or an empty string if the command text does not contain a FROM clause
        /// or <see cref="Clauses"/>.From was not specified.
        /// </summary>
        /// <remarks>This is the value only without the FROM keyword.</remarks>
        public string From { get; private set; } = string.Empty;

        /// <summary>
        /// Gets value of the GROUP BY clause of the command text or an empty string if the command text does not contain a GROUP BY clause
        /// or <see cref="Clauses"/>.GroupBy was not specified.
        /// </summary>
        /// <remarks>This is the value only without the GROUP BY keyword.</remarks>
        public string GroupBy { get; private set; } = string.Empty;

        /// <summary>
        /// Gets value of the ORDER BY clause of the command text or an empty string if the command text does not contain a ORDER BY clause
        /// or <see cref="Clauses"/>.OrderBy was not specified.
        /// </summary>
        /// <remarks>This is the value only without the ORDER BY keyword.</remarks>
        public string OrderBy { get; private set; } = string.Empty;

        /// <summary>
        /// Gets value of the SELECT clause of the command text or an empty string if the command text does not contain a SELECT clause
        /// or <see cref="Clauses"/>.Select was not specified.
        /// </summary>
        /// <remarks>This is the value only without the SELECT keyword.</remarks>
        public string Select { get; private set; } = string.Empty;

        /// <summary>
        /// Gets value of the WHERE clause of the command text or an empty string if the command text does not contain a WHERE clause
        /// or <see cref="Clauses"/>.Where was not specified.
        /// </summary>
        /// <remarks>This is the value only without the WHERE keyword.</remarks>
        public string Where { get; private set; } = string.Empty;

        /// <summary>
        /// Parses the specified command text into a SqlString instance populating the specified <see cref="Clauses"/> if
        /// present in the command text.
        /// </summary>
        /// <param name="commandText">The SQL command text.</param>
        /// <param name="clauses">The clauses to include in the SqlString.</param>
        /// <returns>An SqlString instance populating the specified <see cref="Clauses"/> if present in the command text.</returns>
        public static SqlString Parse(string commandText, Clauses clauses)
        {
            if (string.IsNullOrEmpty(commandText))
            {
                return new SqlString();
            }

            if (commandText.Contains(Environment.NewLine))
            {
                commandText = commandText.Replace(Environment.NewLine, " ");
            }

            var segmentPositions = SegmentPositions.GetSegmentPositions(commandText);

            var sqlString = new SqlString();

            sqlString.AppendSelect(commandText, clauses, segmentPositions)
                .AppendFrom(commandText, clauses, segmentPositions)
                .AppendWhere(commandText, clauses, segmentPositions)
                .AppendGroupBy(commandText, clauses, segmentPositions)
                .AppendOrderBy(commandText, clauses, segmentPositions);

            return sqlString;
        }

        private SqlString AppendFrom(string commandText, Clauses clauses, SegmentPositions segmentPositions)
        {
            if (segmentPositions.FromIndex > -1
                && (clauses & Clauses.From) == Clauses.From)
            {
                // Remove the FROM keyword and the subsequent space
                int startIndex = segmentPositions.FromIndex + 5;
                int length = commandText.Length - startIndex;

                if (segmentPositions.WhereIndex > -1)
                {
                    // Remove the space before the WHERE keyword
                    length = segmentPositions.WhereIndex - startIndex;
                }
                else if (segmentPositions.GroupByIndex > -1)
                {
                    // Remove the space before the GROUP BY keyword
                    length = segmentPositions.GroupByIndex - startIndex;
                }
                else if (segmentPositions.OrderByIndex > -1)
                {
                    // Remove the space before the ORDER BY keyword
                    length = segmentPositions.OrderByIndex - startIndex;
                }

                string segment = commandText.Substring(startIndex, length).Trim();

                From = segment;
            }

            return this;
        }

        private SqlString AppendGroupBy(string commandText, Clauses clauses, SegmentPositions segmentPositions)
        {
            if (segmentPositions.GroupByIndex > -1
                && (clauses & Clauses.GroupBy) == Clauses.GroupBy)
            {
                // Remove the GROUP BY keyword and the subsequent space
                int startIndex = segmentPositions.GroupByIndex + 10;
                int length = commandText.Length - startIndex;

                if (segmentPositions.OrderByIndex > -1)
                {
                    // Remove the space before the ORDER BY keyword
                    length = segmentPositions.OrderByIndex - startIndex;
                }

                string segment = commandText.Substring(startIndex, length).Trim();

                GroupBy = segment;
            }

            return this;
        }

        private void AppendOrderBy(string commandText, Clauses clauses, SegmentPositions segmentPositions)
        {
            if (segmentPositions.OrderByIndex > -1
                && (clauses & Clauses.OrderBy) == Clauses.OrderBy)
            {
                // Remove the ORDER BY keyword and the subsequent space
                int startIndex = segmentPositions.OrderByIndex + 10;
                int length = commandText.Length - startIndex;

                string segment = commandText.Substring(startIndex, length).Trim();

                OrderBy = segment;
            }
        }

        private SqlString AppendSelect(string commandText, Clauses clauses, SegmentPositions segmentPositions)
        {
            if (segmentPositions.FromIndex > -1
                && (clauses & Clauses.Select) == Clauses.Select)
            {
                // Remove the SELECT keyword and the subsequent space
                int startIndex = 7;

                // Remove the space before the FROM keyword
                int length = segmentPositions.FromIndex - startIndex;

                string segment = commandText.Substring(startIndex, length).Trim();

                Select = segment;
            }

            return this;
        }

        private SqlString AppendWhere(string commandText, Clauses clauses, SegmentPositions segmentPositions)
        {
            if (segmentPositions.WhereIndex > -1
                && (clauses & Clauses.Where) == Clauses.Where)
            {
                // Remove the WHERE keyword and the subsequent space
                int startIndex = segmentPositions.WhereIndex + 6;
                int length = commandText.Length - startIndex;

                if (segmentPositions.GroupByIndex > -1)
                {
                    // Remove the space before the GROUP BY keyword
                    length = segmentPositions.GroupByIndex - startIndex;
                }
                else if (segmentPositions.OrderByIndex > -1)
                {
                    // Remove the space before the ORDER BY keyword
                    length = segmentPositions.OrderByIndex - startIndex;
                }

                string segment = commandText.Substring(startIndex, length).Trim();

                Where = segment;
            }

            return this;
        }

        private struct SegmentPositions
        {
            private SegmentPositions(int fromIndex, int whereIndex, int groupByIndex, int orderByIndex)
            {
                FromIndex = fromIndex;
                WhereIndex = whereIndex;
                GroupByIndex = groupByIndex;
                OrderByIndex = orderByIndex;
            }

            internal int FromIndex { get; }

            internal int GroupByIndex { get; }

            internal int OrderByIndex { get; }

            internal int WhereIndex { get; }

            internal static SegmentPositions GetSegmentPositions(string commandText)
            {
                var segmentPositions = new SegmentPositions(
                    commandText.IndexOf(" FROM", 0, commandText.Length, StringComparison.OrdinalIgnoreCase),
                    commandText.IndexOf(" WHERE", 0, commandText.Length, StringComparison.OrdinalIgnoreCase),
                    commandText.IndexOf(" GROUP BY", 0, commandText.Length, StringComparison.OrdinalIgnoreCase),
                    commandText.IndexOf(" ORDER BY", 0, commandText.Length, StringComparison.OrdinalIgnoreCase));

                return segmentPositions;
            }
        }
    }
}
