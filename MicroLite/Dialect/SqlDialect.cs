// -----------------------------------------------------------------------
// <copyright file="SqlDialect.cs" company="MicroLite">
// Copyright 2012 - 2013 Trevor Pilley
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
    using System.Text.RegularExpressions;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;

    /// <summary>
    /// The base class for implementations of <see cref="ISqlDialect" />.
    /// </summary>
    public abstract class SqlDialect : ISqlDialect
    {
        private static readonly Regex orderByRegex = new Regex("(?<=ORDER BY)(.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        private static readonly Regex selectRegex = new Regex("SELECT(.+)(?=FROM)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        private static readonly Regex tableNameRegex = new Regex("(?<=FROM)(.+)(?=WHERE)|(?<=FROM)(.+)(?=ORDER BY)|(?<=FROM)(.+)(?=WHERE)?|(?<=FROM)(.+)(?=ORDER BY)?", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        private static readonly Regex whereRegex = new Regex("(?<=WHERE)(.+)(?=ORDER BY)|(?<=WHERE)(.+)(?=ORDER BY)?", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);

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
        /// Gets the close quote character.
        /// </summary>
        protected virtual char CloseQuote
        {
            get
            {
                return '"';
            }
        }

        /// <summary>
        /// Gets the default table schema.
        /// </summary>
        protected virtual string DefaultTableSchema
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the open quote character.
        /// </summary>
        protected virtual char OpenQuote
        {
            get
            {
                return '"';
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
        /// Gets the select separator.
        /// </summary>
        protected virtual char SelectSeparator
        {
            get
            {
                return ';';
            }
        }

        /// <summary>
        /// Gets the SQL parameter.
        /// </summary>
        protected virtual char SqlParameter
        {
            get
            {
                return '?';
            }
        }

        /// <summary>
        /// Gets a value indicating whether SQL parameters are named.
        /// </summary>
        protected virtual bool SupportsNamedParameters
        {
            get
            {
                return false;
            }
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
            var parameterNames = this.SupportsNamedParameters
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

                sqlBuilder.AppendLine(commandText + this.SelectSeparator);
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
            var qualifiedTableName = this.ReadTableName(sqlQuery.CommandText);
            var whereValue = this.ReadWhereClause(sqlQuery.CommandText);
            var whereClause = !string.IsNullOrEmpty(whereValue) ? " WHERE " + whereValue : string.Empty;

            return new SqlQuery("SELECT COUNT(*) FROM " + qualifiedTableName + whereClause, sqlQuery.Arguments.ToArray());
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
            var forType = instance.GetType();
            var objectInfo = ObjectInfo.For(forType);

            switch (statementType)
            {
                case StatementType.Delete:
                    var identifierValue = objectInfo.GetIdentifierValue(instance);

                    return this.CreateQuery(StatementType.Delete, forType, identifierValue);

                case StatementType.Insert:
                    var insertValues = new List<object>(objectInfo.TableInfo.Columns.Count);

                    var insertSqlBuilder = this.CreateSql(statementType, objectInfo);
                    insertSqlBuilder.Append(" VALUES (");

                    foreach (var column in objectInfo.TableInfo.Columns)
                    {
                        if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.DbGenerated
                            && column.ColumnName.Equals(objectInfo.TableInfo.IdentifierColumn))
                        {
                            continue;
                        }

                        if (column.AllowInsert)
                        {
                            insertSqlBuilder.Append(this.FormatParameter(insertValues.Count) + ", ");

                            var value = objectInfo.GetPropertyValueForColumn(instance, column.ColumnName);

                            insertValues.Add(value);
                        }
                    }

                    insertSqlBuilder.Remove(insertSqlBuilder.Length - 2, 2);
                    insertSqlBuilder.Append(")");

                    if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.DbGenerated)
                    {
                        insertSqlBuilder.Append(this.SelectSeparator);
                        insertSqlBuilder.Append(this.SelectIdentityString);
                    }

                    return new SqlQuery(insertSqlBuilder.ToString(), insertValues.ToArray());

                case StatementType.Update:
                    var updateValues = new List<object>(objectInfo.TableInfo.Columns.Count);

                    var updateSqlBuilder = this.CreateSql(StatementType.Update, objectInfo);

                    foreach (var column in objectInfo.TableInfo.Columns)
                    {
                        if (column.AllowUpdate
                            && !column.ColumnName.Equals(objectInfo.TableInfo.IdentifierColumn))
                        {
                            updateSqlBuilder.AppendFormat(
                                        " {0} = {1},",
                                        this.EscapeSql(column.ColumnName),
                                        this.FormatParameter(updateValues.Count));

                            var value = objectInfo.GetPropertyValueForColumn(instance, column.ColumnName);

                            updateValues.Add(value);
                        }
                    }

                    updateSqlBuilder.Remove(updateSqlBuilder.Length - 1, 1);

                    updateSqlBuilder.AppendFormat(
                        " WHERE {0} = {1}",
                        this.EscapeSql(objectInfo.TableInfo.IdentifierColumn),
                        this.FormatParameter(updateValues.Count));

                    updateValues.Add(objectInfo.GetIdentifierValue(instance));

                    return new SqlQuery(updateSqlBuilder.ToString(), updateValues.ToArray());

                default:
                    throw new NotSupportedException(Messages.SqlDialect_StatementTypeNotSupported);
            }
        }

        /// <summary>
        /// Creates an SqlQuery with the specified statement type for the specified type and identifier.
        /// </summary>
        /// <param name="statementType">Type of the statement.</param>
        /// <param name="forType">The type of object to create the query for.</param>
        /// <param name="identifier">The identifier of the instance to create the query for.</param>
        /// <returns>
        /// The created <see cref="SqlQuery" />.
        /// </returns>
        /// <exception cref="System.NotSupportedException">Thrown if the StatementType is not supported.</exception>
        public virtual SqlQuery CreateQuery(StatementType statementType, Type forType, object identifier)
        {
            switch (statementType)
            {
                case StatementType.Delete:
                case StatementType.Select:
                    var objectInfo = ObjectInfo.For(forType);

                    var sqlBuilder = this.CreateSql(statementType, objectInfo);
                    sqlBuilder.AppendFormat(
                        " WHERE {0} = {1}",
                        this.EscapeSql(objectInfo.TableInfo.IdentifierColumn),
                        this.FormatParameter(0));

                    return new SqlQuery(sqlBuilder.ToString(), identifier);

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
        /// Appends the name of the table.
        /// </summary>
        /// <param name="objectInfo">The object info.</param>
        /// <param name="sqlBuilder">The SQL builder.</param>
        protected void AppendTableName(IObjectInfo objectInfo, StringBuilder sqlBuilder)
        {
            var schema = !string.IsNullOrEmpty(objectInfo.TableInfo.Schema)
                ? objectInfo.TableInfo.Schema
                : this.DefaultTableSchema;

            if (!string.IsNullOrEmpty(schema))
            {
                sqlBuilder.Append(this.OpenQuote);
                sqlBuilder.Append(schema);
                sqlBuilder.Append(this.CloseQuote);
                sqlBuilder.Append('.');
            }

            sqlBuilder.Append(this.OpenQuote);
            sqlBuilder.Append(objectInfo.TableInfo.Name);
            sqlBuilder.Append(this.CloseQuote);
        }

        /// <summary>
        /// Escapes the SQL.
        /// </summary>
        /// <param name="sql">The SQL.</param>
        /// <returns>The escaped SQL.</returns>
        protected string EscapeSql(string sql)
        {
            return this.OpenQuote + sql + this.CloseQuote;
        }

        /// <summary>
        /// Formats the parameter.
        /// </summary>
        /// <param name="parameterPosition">The parameter position.</param>
        /// <returns>The formatted parameter.</returns>
        protected string FormatParameter(int parameterPosition)
        {
            if (this.SupportsNamedParameters)
            {
                return this.SqlParameter + ('p' + parameterPosition.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                return this.SqlParameter.ToString();
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

        /// <summary>
        /// Reads the order by clause from the specified command text excluding the ORDER BY keyword.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The columns in the order by list.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Intentionally left as an instance method so that it's easily discoverable from subclasses.")]
        protected string ReadOrderBy(string commandText)
        {
            return orderByRegex.Match(commandText).Groups[0].Value.Replace(Environment.NewLine, string.Empty).Trim();
        }

        /// <summary>
        /// Reads the select clause from the specified command text including the SELECT keyword.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The columns in the select list.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Intentionally left as an instance method so that it's easily discoverable from subclasses.")]
        protected string ReadSelectList(string commandText)
        {
            return selectRegex.Match(commandText).Groups[0].Value.Replace(Environment.NewLine, string.Empty).Trim();
        }

        /// <summary>
        /// Reads the name of the table the sql query is targeting.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The name of the table the sql query is targeting.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Intentionally left as an instance method so that it's easily discoverable from subclasses.")]
        protected string ReadTableName(string commandText)
        {
            return tableNameRegex.Match(commandText).Groups[0].Value.Replace(Environment.NewLine, string.Empty).Trim();
        }

        /// <summary>
        /// Reads the where clause from the specified command text excluding the WHERE keyword.
        /// </summary>
        /// <param name="commandText">The command text.</param>
        /// <returns>The where clause without the WHERE keyword.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Intentionally left as an instance method so that it's easily discoverable from subclasses.")]
        protected string ReadWhereClause(string commandText)
        {
            return whereRegex.Match(commandText).Groups[0].Value.Replace(Environment.NewLine, string.Empty).Trim();
        }

        private StringBuilder CreateSql(StatementType statementType, IObjectInfo objectInfo)
        {
            var sqlBuilder = new StringBuilder(capacity: 120);

            switch (statementType)
            {
                case StatementType.Delete:
                    sqlBuilder.Append("DELETE FROM ");
                    this.AppendTableName(objectInfo, sqlBuilder);

                    break;

                case StatementType.Insert:
                    sqlBuilder.Append("INSERT INTO ");
                    this.AppendTableName(objectInfo, sqlBuilder);
                    sqlBuilder.Append(" (");

                    foreach (var column in objectInfo.TableInfo.Columns)
                    {
                        if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.DbGenerated
                            && column.ColumnName.Equals(objectInfo.TableInfo.IdentifierColumn))
                        {
                            continue;
                        }

                        if (column.AllowInsert)
                        {
                            sqlBuilder.Append(this.EscapeSql(column.ColumnName) + ", ");
                        }
                    }

                    sqlBuilder.Remove(sqlBuilder.Length - 2, 2);
                    sqlBuilder.Append(")");

                    break;

                case StatementType.Select:
                    sqlBuilder.Append("SELECT ");

#if NET_3_5
                    sqlBuilder.Append(string.Join(", ", objectInfo.TableInfo.Columns.Select(x => this.EscapeSql(x.ColumnName)).ToArray()));
#else
                    sqlBuilder.Append(string.Join(", ", objectInfo.TableInfo.Columns.Select(x => this.EscapeSql(x.ColumnName))));
#endif
                    sqlBuilder.Append(" FROM ");
                    this.AppendTableName(objectInfo, sqlBuilder);

                    break;

                case StatementType.Update:
                    sqlBuilder.Append("UPDATE ");
                    this.AppendTableName(objectInfo, sqlBuilder);
                    sqlBuilder.Append(" SET");

                    break;
            }

            return sqlBuilder;
        }
    }
}