// -----------------------------------------------------------------------
// <copyright file="SqlDialect.cs" company="MicroLite">
// Copyright 2012 - 2013 Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Dialect
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;
    using MicroLite.Query;

    /// <summary>
    /// The base class for implementations of <see cref="ISqlDialect" />.
    /// </summary>
    public abstract class SqlDialect : ISqlDialect
    {
        private readonly SqlCharacters sqlCharacters;

        /// <summary>
        /// Initialises a new instance of the <see cref="SqlDialect"/> class.
        /// </summary>
        /// <param name="sqlCharacters">The SQL characters.</param>
        protected SqlDialect(SqlCharacters sqlCharacters)
        {
            this.sqlCharacters = sqlCharacters;
        }

        /// <summary>
        /// Gets the SQL characters for the SQL dialect.
        /// </summary>
        public SqlCharacters SqlCharacters
        {
            get
            {
                return this.sqlCharacters;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this SqlDialect supports batched queries.
        /// </summary>
        public virtual bool SupportsBatchedQueries
        {
            get
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the select identity string.
        /// </summary>
        protected abstract string SelectIdentityString
        {
            get;
        }

        /// <summary>
        /// Builds the command using the values in the specified SqlQuery.
        /// </summary>
        /// <param name="command">The command to build.</param>
        /// <param name="sqlQuery">The SQL query containing the values for the command.</param>
        /// <exception cref="MicroLiteException">Thrown if the number of arguments does not match the number of parameter names.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "SqlQuery.CommandText is the parameterised query.")]
        public virtual void BuildCommand(IDbCommand command, SqlQuery sqlQuery)
        {
            var parameterNames = this.sqlCharacters.SupportsNamedParameters
                ? SqlUtility.GetParameterNames(sqlQuery.CommandText)
                : Enumerable.Range(0, sqlQuery.Arguments.Count).Select(c => "Parameter" + c.ToString(CultureInfo.InvariantCulture)).ToArray();

            if (parameterNames.Count != sqlQuery.Arguments.Count)
            {
                throw new MicroLiteException(Messages.SqlDialect_ArgumentsCountMismatch.FormatWith(parameterNames.Count.ToString(CultureInfo.InvariantCulture), sqlQuery.Arguments.Count.ToString(CultureInfo.InvariantCulture)));
            }

            command.CommandText = this.GetCommandText(sqlQuery.CommandText);
            command.CommandTimeout = sqlQuery.Timeout;
            command.CommandType = this.GetCommandType(sqlQuery.CommandText);
            this.AddParameters(command, sqlQuery, parameterNames);
        }

        /// <summary>
        /// Combines the specified SQL queries into a single SqlQuery.
        /// </summary>
        /// <param name="sqlQueries">The SQL queries to be combined.</param>
        /// <returns>
        /// The combined <see cref="SqlQuery" />.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown if sqlQueries is null.</exception>
        public virtual SqlQuery Combine(IEnumerable<SqlQuery> sqlQueries)
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

                var commandText = SqlUtility.RenumberParameters(sqlQuery.CommandText, argumentsCount);

                sqlBuilder.AppendLine(commandText + this.sqlCharacters.StatementSeparator);
            }

            var combinedQuery = new SqlQuery(sqlBuilder.ToString(0, sqlBuilder.Length - 3), sqlQueries.SelectMany(s => s.Arguments).ToArray());
            combinedQuery.Timeout = sqlQueries.Max(s => s.Timeout);

            return combinedQuery;
        }

        /// <summary>
        /// Creates an SqlQuery to count the number of records which would be returned by the specified SqlQuery.
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <returns>
        /// An <see cref="SqlQuery" /> to count the number of records which would be returned by the specified SqlQuery.
        /// </returns>
        public virtual SqlQuery CountQuery(SqlQuery sqlQuery)
        {
            var qualifiedTableName = SqlUtility.ReadTableName(sqlQuery.CommandText);
            var whereValue = SqlUtility.ReadWhereClause(sqlQuery.CommandText);
            var whereClause = !string.IsNullOrEmpty(whereValue) ? " WHERE " + whereValue : string.Empty;

            return new SqlQuery("SELECT COUNT(*) FROM " + qualifiedTableName + whereClause, sqlQuery.GetArgumentArray());
        }

        /// <summary>
        /// Creates an SqlQuery with the specified statement type for the specified instance.
        /// </summary>
        /// <param name="statementType">Type of the statement.</param>
        /// <param name="instance">The instance to generate the SqlQuery for.</param>
        /// <returns>
        /// The created <see cref="SqlQuery" />.
        /// </returns>
        /// <exception cref="System.NotSupportedException">Thrown if the StatementType is not supported.</exception>
        public virtual SqlQuery CreateQuery(StatementType statementType, object instance)
        {
            var objectInfo = ObjectInfo.For(instance.GetType());

            switch (statementType)
            {
                case StatementType.Insert:
                    var insertSqlBuilder = new InsertSqlBuilder(this.SqlCharacters);
                    insertSqlBuilder.Into(objectInfo);

                    for (int i = 0; i < objectInfo.TableInfo.Columns.Count; i++)
                    {
                        var columnInfo = objectInfo.TableInfo.Columns[i];

                        if (columnInfo.AllowInsert)
                        {
                            var value = objectInfo.GetPropertyValueForColumn(instance, columnInfo.ColumnName);

                            insertSqlBuilder.Value(columnInfo.ColumnName, value);
                        }
                    }

                    var insertSqlQuery = objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.DbGenerated
                        ? insertSqlBuilder.ToSqlQuery(this.sqlCharacters.StatementSeparator + this.SelectIdentityString)
                        : insertSqlBuilder.ToSqlQuery();

                    return insertSqlQuery;

                case StatementType.Update:
                    var updateSqlBuilder = new UpdateSqlBuilder(this.SqlCharacters);
                    updateSqlBuilder.Table(objectInfo);

                    for (int i = 0; i < objectInfo.TableInfo.Columns.Count; i++)
                    {
                        var columnInfo = objectInfo.TableInfo.Columns[i];

                        if (columnInfo.AllowUpdate)
                        {
                            var value = objectInfo.GetPropertyValueForColumn(instance, columnInfo.ColumnName);

                            updateSqlBuilder.SetColumnValue(columnInfo.ColumnName, value);
                        }
                    }

                    updateSqlBuilder.Where(
                        this.sqlCharacters.EscapeSql(objectInfo.TableInfo.IdentifierColumn),
                        objectInfo.GetIdentifierValue(instance));

                    var updateSqlQuery = updateSqlBuilder.ToSqlQuery();

                    return updateSqlQuery;

                default:
                    throw new NotSupportedException(Messages.SqlDialect_StatementTypeNotSupported);
            }
        }

        /// <summary>
        /// Creates an SqlQuery to page the records which would be returned by the specified SqlQuery based upon the paging options.
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <param name="pagingOptions">The paging options.</param>
        /// <returns>
        /// A <see cref="SqlQuery" /> to return the paged results of the specified query.
        /// </returns>
        public abstract SqlQuery PageQuery(SqlQuery sqlQuery, PagingOptions pagingOptions);

        /// <summary>
        /// Adds the parameters.
        /// </summary>
        /// <param name="command">The command.</param>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <param name="parameterNames">The parameter names.</param>
        protected virtual void AddParameters(IDbCommand command, SqlQuery sqlQuery, IList<string> parameterNames)
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

        /// <summary>
        /// Gets the command text.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The actual command text.</returns>
        protected virtual string GetCommandText(string commandText)
        {
            return commandText;
        }

        /// <summary>
        /// Gets the type of the command.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The CommandType for the specified command text.</returns>
        protected virtual CommandType GetCommandType(string commandText)
        {
            return CommandType.Text;
        }
    }
}