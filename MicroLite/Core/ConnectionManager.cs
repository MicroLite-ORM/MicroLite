namespace MicroLite.Core
{
    using System;
    using System.Data;
    using System.Globalization;
    using System.Text.RegularExpressions;
    using MicroLite.Logging;

    /// <summary>
    /// The default implementation of <see cref="IConnectionManager"/>.
    /// </summary>
    internal sealed class ConnectionManager : IConnectionManager
    {
        private static readonly Regex parameterRegex = new Regex(@"@[\w]+", RegexOptions.Compiled);
        private IDbConnection connection;
        private Transaction currentTransaction;

        internal ConnectionManager(IDbConnection connection)
        {
            this.connection = connection;
        }

        public ITransaction BeginTransaction()
        {
            var dbTransaction = this.connection.BeginTransaction();

            this.currentTransaction = new Transaction(dbTransaction);

            return this.currentTransaction;
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            var dbTransaction = this.connection.BeginTransaction(isolationLevel);

            this.currentTransaction = new Transaction(dbTransaction);

            return this.currentTransaction;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The purpose of this method is to build a command and return it.")]
        public IDbCommand Build(SqlQuery sqlQuery)
        {
            var parameterNames = parameterRegex.Matches(sqlQuery.CommandText);

            if (parameterNames.Count != sqlQuery.Parameters.Count)
            {
                throw new MicroLiteException(Messages.ParameterCountMismatch.FormatWith(parameterNames.Count.ToString(CultureInfo.InvariantCulture), sqlQuery.Parameters.Count.ToString(CultureInfo.InvariantCulture)));
            }

            var command = this.connection.CreateCommand();
            SetCommandText(command, sqlQuery);
            SetCommandType(command, sqlQuery);

            if (this.currentTransaction != null)
            {
                this.currentTransaction.Enlist(command);
            }

            for (int i = 0; i < parameterNames.Count; i++)
            {
                var parameterName = parameterNames[i].Value;

                var parameter = command.CreateParameter();
                parameter.Direction = ParameterDirection.Input;
                parameter.ParameterName = parameterName;
                parameter.Value = sqlQuery.Parameters[i] ?? DBNull.Value;

                command.Parameters.Add(parameter);
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

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "SqlQuery.CommandText is the parameterised query.")]
        private static void SetCommandText(IDbCommand command, SqlQuery sqlQuery)
        {
            if (sqlQuery.CommandText.StartsWith("EXEC", StringComparison.OrdinalIgnoreCase))
            {
                var firstParameterPosition = sqlQuery.CommandText.IndexOf('@', 0);

                if (firstParameterPosition > 4)
                {
                    command.CommandText = sqlQuery.CommandText.Substring(4, firstParameterPosition - 4).Trim();
                }
                else
                {
                    command.CommandText = sqlQuery.CommandText.Substring(4, sqlQuery.CommandText.Length - 4).Trim();
                }
            }
            else
            {
                command.CommandText = sqlQuery.CommandText;
            }
        }

        private static void SetCommandType(IDbCommand command, SqlQuery sqlQuery)
        {
            command.CommandType = sqlQuery.CommandText.StartsWith("EXEC", StringComparison.OrdinalIgnoreCase)
                ? CommandType.StoredProcedure
                : CommandType.Text;
        }
    }
}