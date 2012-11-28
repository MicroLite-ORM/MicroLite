// -----------------------------------------------------------------------
// <copyright file="ConnectionManager.cs" company="MicroLite">
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
namespace MicroLite.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using MicroLite.FrameworkExtensions;

    /// <summary>
    /// The default implementation of <see cref="IConnectionManager"/>.
    /// </summary>
    internal sealed class ConnectionManager : IConnectionManager
    {
        private IDbConnection connection;
        private ITransaction currentTransaction;

        internal ConnectionManager(IDbConnection connection)
        {
            this.connection = connection;
        }

        public ITransaction CurrentTransaction
        {
            get
            {
                return this.currentTransaction;
            }
        }

        public ITransaction BeginTransaction()
        {
            if (this.currentTransaction == null || !this.currentTransaction.IsActive)
            {
                this.currentTransaction = Transaction.Begin(this.connection);
            }

            return this.currentTransaction;
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            if (this.currentTransaction == null || !this.currentTransaction.IsActive)
            {
                this.currentTransaction = Transaction.Begin(this.connection, isolationLevel);
            }

            return this.currentTransaction;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The purpose of this method is to build a command and return it.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "SqlQuery.CommandText is the parameterised query.")]
        public IDbCommand BuildCommand(SqlQuery sqlQuery)
        {
            var parameterNames = SqlUtil.GetParameterNames(sqlQuery.CommandText);

            if (parameterNames.Count != sqlQuery.Arguments.Count)
            {
                throw new MicroLiteException(Messages.ConnectionManager_ArgumentsCountMismatch.FormatWith(parameterNames.Count.ToString(CultureInfo.InvariantCulture), sqlQuery.Arguments.Count.ToString(CultureInfo.InvariantCulture)));
            }

            var command = this.connection.CreateCommand();
            command.CommandText = GetCommandText(sqlQuery.CommandText);
            command.CommandTimeout = sqlQuery.Timeout;
            command.CommandType = GetCommandType(sqlQuery.CommandText);
            AddParameters(command, sqlQuery, parameterNames);

            if (this.currentTransaction != null)
            {
                this.currentTransaction.Enlist(command);
            }

            return command;
        }

        public void Dispose()
        {
            if (this.connection != null)
            {
                this.connection.Close();
                this.connection.Dispose();
                this.connection = null;
            }

            if (this.currentTransaction != null)
            {
                this.currentTransaction.Dispose();
                this.currentTransaction = null;
            }
        }

        private static void AddParameters(IDbCommand command, SqlQuery sqlQuery, IList<string> parameterNames)
        {
            for (int i = 0; i < parameterNames.Count; i++)
            {
                var parameterName = parameterNames[i];

                var parameter = command.CreateParameter();
                parameter.Direction = ParameterDirection.Input;
                parameter.ParameterName = parameterName;
                parameter.Value = sqlQuery.Arguments[i] ?? DBNull.Value;

                command.Parameters.Add(parameter);
            }
        }

        private static string GetCommandText(string commandText)
        {
            if (commandText.StartsWith("EXEC", StringComparison.OrdinalIgnoreCase) && !commandText.Contains(";"))
            {
                var firstParameterPosition = SqlUtil.GetFirstParameterPosition(commandText);

                if (firstParameterPosition > 4)
                {
                    return commandText.Substring(4, firstParameterPosition - 4).Trim();
                }
                else
                {
                    return commandText.Substring(4, commandText.Length - 4).Trim();
                }
            }
            else
            {
                return commandText;
            }
        }

        private static CommandType GetCommandType(string commandText)
        {
            if (commandText.StartsWith("EXEC", StringComparison.OrdinalIgnoreCase) && !commandText.Contains(";"))
            {
                return CommandType.StoredProcedure;
            }

            return CommandType.Text;
        }
    }
}