// -----------------------------------------------------------------------
// <copyright file="DbDriver.cs" company="MicroLite">
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
    using System.Data.Common;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using MicroLite.Characters;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Logging;

    /// <summary>
    /// The base class for implementations of <see cref="IDbDriver" />.
    /// </summary>
    public abstract class DbDriver : IDbDriver
    {
        private static readonly ILog log = LogManager.GetCurrentClassLog();
        private readonly SqlCharacters sqlCharacters;
        private bool handleStringsAsUnicode = true;

        /// <summary>
        /// Initialises a new instance of the <see cref="DbDriver" /> class.
        /// </summary>
        /// <param name="sqlCharacters">The SQL characters.</param>
        protected DbDriver(SqlCharacters sqlCharacters)
        {
            this.sqlCharacters = sqlCharacters;
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        public string ConnectionString
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the database provider factory.
        /// </summary>
        public DbProviderFactory DbProviderFactory
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether to handle strings as unicode (defaults to true).
        /// </summary>
        /// <remarks>
        /// Indicates whether strings should be handled as unicode (true by default)
        /// e.g. for MS SQL - if true, strings will be treated as NVARCHAR; if false, strings will be treated as VARCHAR.
        /// </remarks>
        public bool HandleStringsAsUnicode
        {
            get
            {
                return this.handleStringsAsUnicode;
            }

            set
            {
                this.handleStringsAsUnicode = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this DbDriver supports batched queries.
        /// </summary>
        /// <remarks>Returns false unless overridden.</remarks>
        public virtual bool SupportsBatchedQueries
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the SQL characters used by the DbDriver.
        /// </summary>
        protected SqlCharacters SqlCharacters
        {
            get
            {
                return this.sqlCharacters;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this DbDriver supports command timeout.
        /// </summary>
        /// <remarks>Returns true unless overridden.</remarks>
        protected virtual bool SupportsCommandTimeout
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this DbDriver supports stored procedures.
        /// </summary>
        protected bool SupportsStoredProcedures
        {
            get
            {
                return !string.IsNullOrEmpty(this.sqlCharacters.StoredProcedureInvocationCommand);
            }
        }

        /// <summary>
        /// Builds an IDbCommand command using the values in the specified SqlQuery.
        /// </summary>
        /// <param name="sqlQuery">The SQL query containing the values for the command.</param>
        /// <returns>An IDbCommand with the CommandText, CommandType, Timeout and Parameters set.</returns>
        /// <exception cref="MicroLiteException">Thrown if the number of arguments does not match the number of parameter names.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "SqlQuery.CommandText is the parameterised query.")]
        public IDbCommand BuildCommand(SqlQuery sqlQuery)
        {
            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            if (log.IsDebug)
            {
                log.Debug(LogMessages.DbDialect_BuildingCommand);
            }

            var parameterNames = SqlUtility.GetParameterNames(sqlQuery.CommandText);

            if (parameterNames.Count == 0 && sqlQuery.Arguments.Count > 0)
            {
                parameterNames = Enumerable.Range(0, sqlQuery.Arguments.Count)
                    .Select(c => "Parameter" + c.ToString(CultureInfo.InvariantCulture))
                    .ToArray();
            }

            if (parameterNames.Count != sqlQuery.Arguments.Count)
            {
                throw new MicroLiteException(ExceptionMessages.DbDriver_ArgumentsCountMismatch.FormatWith(parameterNames.Count.ToString(CultureInfo.InvariantCulture), sqlQuery.Arguments.Count.ToString(CultureInfo.InvariantCulture)));
            }

            var command = this.DbProviderFactory.CreateCommand();
            command.CommandText = this.GetCommandText(sqlQuery.CommandText);
            command.CommandType = this.GetCommandType(sqlQuery.CommandText);

            for (int i = 0; i < parameterNames.Count; i++)
            {
                var parameter = command.CreateParameter();
                var parameterName = parameterNames[i];
                var sqlArgument = sqlQuery.Arguments[i];

                this.BuildParameter(parameter, parameterName, sqlArgument);

                command.Parameters.Add(parameter);
            }

            if (this.SupportsCommandTimeout)
            {
                command.CommandTimeout = sqlQuery.Timeout;
            }

            return command;
        }

        /// <summary>
        /// Combines the specified SQL queries into a single SqlQuery.
        /// </summary>
        /// <param name="sqlQueries">The SQL queries to be combined.</param>
        /// <returns>
        /// An <see cref="SqlQuery" /> containing the combined command text and arguments.
        /// </returns>
        public SqlQuery Combine(IEnumerable<SqlQuery> sqlQueries)
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
                    sqlBuilder.Append(sqlQuery.CommandText).AppendLine(this.sqlCharacters.StatementSeparator);
                }
                else
                {
                    var commandText = this.IsStoredProcedureCall(sqlQuery.CommandText)
                        ? sqlQuery.CommandText
                        : SqlUtility.RenumberParameters(sqlQuery.CommandText, argumentsCount);

                    sqlBuilder.Append(commandText).AppendLine(this.sqlCharacters.StatementSeparator);
                }
            }

            var combinedQuery = new SqlQuery(sqlBuilder.ToString(0, sqlBuilder.Length - 3), sqlQueries.SelectMany(s => s.Arguments).ToArray());
            combinedQuery.Timeout = sqlQueries.Max(s => s.Timeout);

            return combinedQuery;
        }

        /// <summary>
        /// Combines the specified SQL queries into a single SqlQuery.
        /// </summary>
        /// <param name="sqlQuery1">The first SQL query to be combined.</param>
        /// <param name="sqlQuery2">The second SQL query to be combined.</param>
        /// <returns>
        /// An <see cref="SqlQuery" /> containing the combined command text and arguments.
        /// </returns>
        public SqlQuery Combine(SqlQuery sqlQuery1, SqlQuery sqlQuery2)
        {
            if (sqlQuery1 == null)
            {
                throw new ArgumentNullException("sqlQuery1");
            }

            if (sqlQuery2 == null)
            {
                throw new ArgumentNullException("sqlQuery2");
            }

            int argumentsCount = sqlQuery1.Arguments.Count + sqlQuery2.Arguments.Count;

            var arguments = new SqlArgument[argumentsCount];

            Array.Copy(sqlQuery1.ArgumentsArray, 0, arguments, 0, sqlQuery1.Arguments.Count);

            if (sqlQuery2.Arguments.Count > 0)
            {
                Array.Copy(sqlQuery2.ArgumentsArray, 0, arguments, sqlQuery1.Arguments.Count, sqlQuery2.Arguments.Count);
            }

            var query2CommandText = this.IsStoredProcedureCall(sqlQuery2.CommandText)
                ? sqlQuery2.CommandText
                : SqlUtility.RenumberParameters(sqlQuery2.CommandText, argumentsCount);

            var commandText = sqlQuery1.CommandText + this.sqlCharacters.StatementSeparator + Environment.NewLine + query2CommandText;

            var combinedQuery = new SqlQuery(commandText, arguments);
            combinedQuery.Timeout = Math.Max(sqlQuery1.Timeout, sqlQuery2.Timeout);

            return combinedQuery;
        }

        /// <summary>
        /// Gets an IDbConnection for the database.
        /// </summary>
        /// <param name="connectionScope">The connection scope of the connection.</param>
        /// <returns>The IDbConnection for the database.</returns>
        public IDbConnection GetConnection(ConnectionScope connectionScope)
        {
            var connection = this.DbProviderFactory.CreateConnection();
            connection.ConnectionString = this.ConnectionString;

            if (connectionScope == ConnectionScope.PerSession)
            {
                if (log.IsDebug)
                {
                    log.Debug(LogMessages.OpeningConnection);
                }

                connection.Open();
            }

            return connection;
        }

        /// <summary>
        /// Builds the the IDbDataParameter using the specified name and value.
        /// </summary>
        /// <param name="parameter">The parameter to build.</param>
        /// <param name="parameterName">The name for the parameter.</param>
        /// <param name="sqlArgument">The <see cref="SqlArgument"/> for the parameter.</param>
        protected virtual void BuildParameter(IDbDataParameter parameter, string parameterName, SqlArgument sqlArgument)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            if (!this.HandleStringsAsUnicode && sqlArgument.DbType == DbType.String)
            {
                parameter.DbType = DbType.AnsiString;
            }
            else
            {
                parameter.DbType = sqlArgument.DbType;
            }

            parameter.Direction = ParameterDirection.Input;
            parameter.ParameterName = parameterName;
            parameter.Value = sqlArgument.Value ?? DBNull.Value;
        }

        /// <summary>
        /// Gets the command text.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The actual command text.</returns>
        protected virtual string GetCommandText(string commandText)
        {
            if (commandText == null)
            {
                throw new ArgumentNullException("commandText");
            }

            if (this.IsStoredProcedureCall(commandText))
            {
                var invocationCommandLength = this.sqlCharacters.StoredProcedureInvocationCommand.Length;
                var firstParameterPosition = SqlUtility.GetFirstParameterPosition(commandText);

                if (firstParameterPosition > invocationCommandLength)
                {
                    return commandText.Substring(invocationCommandLength, firstParameterPosition - invocationCommandLength).Trim();
                }
                else
                {
                    return commandText.Substring(invocationCommandLength, commandText.Length - invocationCommandLength).Trim();
                }
            }

            return commandText;
        }

        /// <summary>
        /// Gets the type of the command.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The CommandType for the specified command text.</returns>
        protected virtual CommandType GetCommandType(string commandText)
        {
            if (commandText == null)
            {
                throw new ArgumentNullException("commandText");
            }

            if (this.IsStoredProcedureCall(commandText))
            {
                return CommandType.StoredProcedure;
            }

            return CommandType.Text;
        }

        /// <summary>
        /// Determines whether the command text is a stored procedure call.
        /// </summary>
        /// <param name="commandText">The command text to inspect.</param>
        /// <returns>true if the command text is a stored procedure call, otherwise false.</returns>
        protected virtual bool IsStoredProcedureCall(string commandText)
        {
            if (commandText == null)
            {
                throw new ArgumentNullException("commandText");
            }

            return this.SupportsStoredProcedures
                && commandText.StartsWith(this.sqlCharacters.StoredProcedureInvocationCommand, StringComparison.OrdinalIgnoreCase)
                && !commandText.Contains(this.sqlCharacters.StatementSeparator);
        }
    }
}