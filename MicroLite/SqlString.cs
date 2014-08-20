// -----------------------------------------------------------------------
// <copyright file="SqlString.cs" company="MicroLite">
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

    /// <summary>
    /// A class which contains the clauses of a SQL command.
    /// </summary>
    public sealed class SqlString
    {
        private SqlString()
        {
            this.From = string.Empty;
            this.GroupBy = string.Empty;
            this.OrderBy = string.Empty;
            this.Select = string.Empty;
            this.Where = string.Empty;
        }

        /// <summary>
        /// Gets value of the FROM clause of the command text or an empty string if the command text does not contain a FROM clause
        /// or <see cref="Clauses"/>.From was not specified.
        /// </summary>
        /// <remarks>This is the value only without the FROM keyword.</remarks>
        public string From
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets value of the GROUP BY clause of the command text or an empty string if the command text does not contain a GROUP BY clause
        /// or <see cref="Clauses"/>.GroupBy was not specified.
        /// </summary>
        /// <remarks>This is the value only without the GROUP BY keyword.</remarks>
        public string GroupBy
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets value of the ORDER BY clause of the command text or an empty string if the command text does not contain a ORDER BY clause
        /// or <see cref="Clauses"/>.OrderBy was not specified.
        /// </summary>
        /// <remarks>This is the value only without the ORDER BY keyword.</remarks>
        public string OrderBy
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets value of the SELECT clause of the command text or an empty string if the command text does not contain a SELECT clause
        /// or <see cref="Clauses"/>.Select was not specified.
        /// </summary>
        /// <remarks>This is the value only without the SELECT keyword.</remarks>
        public string Select
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets value of the WHERE clause of the command text or an empty string if the command text does not contain a WHERE clause
        /// or <see cref="Clauses"/>.Where was not specified.
        /// </summary>
        /// <remarks>This is the value only without the WHERE keyword.</remarks>
        public string Where
        {
            get;
            private set;
        }

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
                var startIndex = segmentPositions.FromIndex + 5;
                var length = commandText.Length - startIndex;

                if (segmentPositions.WhereIndex > -1)
                {
                    // Remove the space before the WHERE keyword
                    length = segmentPositions.WhereIndex - startIndex - 1;
                }
                else if (segmentPositions.GroupByIndex > -1)
                {
                    // Remove the space before the GROUP BY keyword
                    length = segmentPositions.GroupByIndex - startIndex - 1;
                }
                else if (segmentPositions.OrderByIndex > -1)
                {
                    // Remove the space before the ORDER BY keyword
                    length = segmentPositions.OrderByIndex - startIndex - 1;
                }

                var segment = commandText.Substring(startIndex, length).Trim();

                this.From = segment;
            }

            return this;
        }

        private SqlString AppendGroupBy(string commandText, Clauses clauses, SegmentPositions segmentPositions)
        {
            if (segmentPositions.GroupByIndex > -1
                && (clauses & Clauses.GroupBy) == Clauses.GroupBy)
            {
                // Remove the GROUP BY keyword and the subsequent space
                var startIndex = segmentPositions.GroupByIndex + 9;
                var length = commandText.Length - startIndex;

                if (segmentPositions.OrderByIndex > -1)
                {
                    // Remove the space before the ORDER BY keyword
                    length = segmentPositions.OrderByIndex - startIndex - 1;
                }

                var segment = commandText.Substring(startIndex, length).Trim();

                this.GroupBy = segment;
            }

            return this;
        }

        private SqlString AppendOrderBy(string commandText, Clauses clauses, SegmentPositions segmentPositions)
        {
            if (segmentPositions.OrderByIndex > -1
                && (clauses & Clauses.OrderBy) == Clauses.OrderBy)
            {
                // Remove the ORDER BY keyword and the subsequent space
                var startIndex = segmentPositions.OrderByIndex + 9;
                var length = commandText.Length - startIndex;

                var segment = commandText.Substring(startIndex, length).Trim();

                this.OrderBy = segment;
            }

            return this;
        }

        private SqlString AppendSelect(string commandText, Clauses clauses, SegmentPositions segmentPositions)
        {
            if (segmentPositions.FromIndex > -1
                && (clauses & Clauses.Select) == Clauses.Select)
            {
                // Remove the SELECT keyword and the subsequent space
                var startIndex = 7;

                // Remove the space before the FROM keyword
                var length = segmentPositions.FromIndex - startIndex - 1;

                var segment = commandText.Substring(startIndex, length).Trim();

                this.Select = segment;
            }

            return this;
        }

        private SqlString AppendWhere(string commandText, Clauses clauses, SegmentPositions segmentPositions)
        {
            if (segmentPositions.WhereIndex > -1
                && (clauses & Clauses.Where) == Clauses.Where)
            {
                // Remove the WHERE keyword and the subsequent space
                var startIndex = segmentPositions.WhereIndex + 5;
                var length = commandText.Length - startIndex;

                if (segmentPositions.GroupByIndex > -1)
                {
                    // Remove the space before the GROUP BY keyword
                    length = segmentPositions.GroupByIndex - startIndex - 1;
                }
                else if (segmentPositions.OrderByIndex > -1)
                {
                    // Remove the space before the ORDER BY keyword
                    length = segmentPositions.OrderByIndex - startIndex - 1;
                }

                var segment = commandText.Substring(startIndex, length).Trim();

                this.Where = segment;
            }

            return this;
        }

        private struct SegmentPositions
        {
            private readonly int fromIndex;
            private readonly int groupByIndex;
            private readonly int orderByIndex;
            private readonly int whereIndex;

            private SegmentPositions(int fromIndex, int whereIndex, int groupByIndex, int orderByIndex)
            {
                this.fromIndex = fromIndex;
                this.whereIndex = whereIndex;
                this.groupByIndex = groupByIndex;
                this.orderByIndex = orderByIndex;
            }

            internal int FromIndex
            {
                get
                {
                    return this.fromIndex;
                }
            }

            internal int GroupByIndex
            {
                get
                {
                    return this.groupByIndex;
                }
            }

            internal int OrderByIndex
            {
                get
                {
                    return this.orderByIndex;
                }
            }

            internal int WhereIndex
            {
                get
                {
                    return this.whereIndex;
                }
            }

            internal static SegmentPositions GetSegmentPositions(string commandText)
            {
                var segmentPositions = new SegmentPositions(
                    commandText.IndexOf("FROM", 0, commandText.Length, StringComparison.OrdinalIgnoreCase),
                    commandText.IndexOf("WHERE", 0, commandText.Length, StringComparison.OrdinalIgnoreCase),
                    commandText.IndexOf("GROUP BY", 0, commandText.Length, StringComparison.OrdinalIgnoreCase),
                    commandText.IndexOf("ORDER BY", 0, commandText.Length, StringComparison.OrdinalIgnoreCase));

                return segmentPositions;
            }
        }
    }
}