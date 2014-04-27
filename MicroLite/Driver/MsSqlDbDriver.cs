// -----------------------------------------------------------------------
// <copyright file="MsSqlDbDriver.cs" company="MicroLite">
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
namespace MicroLite.Driver
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The implementation of <see cref="IDbDriver"/> for MsSql server.
    /// </summary>
    internal sealed class MsSqlDbDriver : DbDriver
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MsSqlDbDriver" /> class.
        /// </summary>
        internal MsSqlDbDriver()
        {
        }

        public override bool SupportsBatchedQueries
        {
            get
            {
                return true;
            }
        }

        public override SqlQuery Combine(IEnumerable<SqlQuery> sqlQueries)
        {
            if (sqlQueries == null)
            {
                throw new ArgumentNullException("sqlQueries");
            }

            int argumentsCount = 0;
            var sqlBuilder = new StringBuilder(sqlQueries.Sum(s => s.CommandText.Length));

            foreach (var sqlQuery in sqlQueries)
            {
                argumentsCount += sqlQuery.Arguments.Count;

                if (sqlBuilder.Length == 0)
                {
                    sqlBuilder.Append(sqlQuery.CommandText).AppendLine(this.StatementSeparator);
                }
                else
                {
                    var commandText = sqlQuery.CommandText.StartsWith("EXEC", StringComparison.OrdinalIgnoreCase)
                        ? sqlQuery.CommandText
                        : SqlUtility.RenumberParameters(sqlQuery.CommandText, argumentsCount);

                    sqlBuilder.Append(commandText).AppendLine(this.StatementSeparator);
                }
            }

            var combinedQuery = new SqlQuery(sqlBuilder.ToString(0, sqlBuilder.Length - 3), sqlQueries.SelectMany(s => s.Arguments).ToArray());
            combinedQuery.Timeout = sqlQueries.Max(s => s.Timeout);

            return combinedQuery;
        }

        protected override string GetCommandText(string commandText)
        {
            if (commandText.StartsWith("EXEC", StringComparison.OrdinalIgnoreCase)
                && !commandText.Contains(this.StatementSeparator))
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
                && !commandText.Contains(this.StatementSeparator))
            {
                return CommandType.StoredProcedure;
            }

            return base.GetCommandType(commandText);
        }
    }
}