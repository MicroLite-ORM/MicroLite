// -----------------------------------------------------------------------
// <copyright file="SqlDialect.cs" company="MicroLite">
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
namespace MicroLite.Dialect
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using MicroLite.Builder;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;

    /// <summary>
    /// The base class for implementations of <see cref="ISqlDialect" />.
    /// </summary>
    public abstract class SqlDialect : ISqlDialect
    {
        private readonly SqlCharacters sqlCharacters;
        private Dictionary<Type, string> deleteCommandCache = new Dictionary<Type, string>();
        private Dictionary<Type, string> insertCommandCache = new Dictionary<Type, string>();
        private Dictionary<Type, string> selectCommandCache = new Dictionary<Type, string>();
        private Dictionary<Type, string> updateCommandCache = new Dictionary<Type, string>();

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

                sqlBuilder.Append(commandText).AppendLine(this.sqlCharacters.StatementSeparator);
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
        /// Creates an SqlQuery to perform an update based upon the values in the object delta.
        /// </summary>
        /// <param name="objectDelta">The object delta to create the query for.</param>
        /// <returns>
        /// The created <see cref="SqlQuery" />.
        /// </returns>
        public SqlQuery CreateQuery(ObjectDelta objectDelta)
        {
            var objectInfo = ObjectInfo.For(objectDelta.ForType);

            var builder = new UpdateSqlBuilder(this.SqlCharacters)
                .Table(objectInfo);

            foreach (var change in objectDelta.Changes)
            {
                builder.SetColumnValue(change.Key, change.Value);
            }

            var sqlQuery = builder
                .Where(objectInfo.TableInfo.IdentifierColumn, objectDelta.Identifier)
                .ToSqlQuery();

            return sqlQuery;
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
        public SqlQuery CreateQuery(StatementType statementType, object instance)
        {
            var objectInfo = ObjectInfo.For(instance.GetType());

            switch (statementType)
            {
                case StatementType.Delete:
                    return this.BuildDeleteSqlQuery(instance, objectInfo);

                case StatementType.Insert:
                    return this.BuildInsertSqlQuery(instance, objectInfo);

                case StatementType.Update:
                    return this.BuildUpdateSqlQuery(instance, objectInfo);

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
        public SqlQuery CreateQuery(StatementType statementType, Type forType, object identifier)
        {
            var objectInfo = ObjectInfo.For(forType);

            switch (statementType)
            {
                case StatementType.Delete:
                    return this.BuildDeleteSqlQuery(objectInfo, identifier);

                case StatementType.Select:
                    return this.BuildSelectSqlQuery(objectInfo, identifier);

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
        /// Builds the delete SQL query.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        /// <param name="identifier">The identifier of the object to delete.</param>
        /// <returns>
        /// The created <see cref="SqlQuery" />.
        /// </returns>
        protected virtual SqlQuery BuildDeleteSqlQuery(IObjectInfo objectInfo, object identifier)
        {
            string deleteCommand;

            if (!this.deleteCommandCache.TryGetValue(objectInfo.ForType, out deleteCommand))
            {
                var deleteSqlQuery = new DeleteSqlBuilder(this.SqlCharacters)
                    .From(objectInfo)
                    .WhereEquals(objectInfo.TableInfo.IdentifierColumn, identifier)
                    .ToSqlQuery();

                var newDeleteCommandCache = new Dictionary<Type, string>(this.deleteCommandCache);
                newDeleteCommandCache[objectInfo.ForType] = deleteSqlQuery.CommandText;

                this.deleteCommandCache = newDeleteCommandCache;

                return deleteSqlQuery;
            }

            return new SqlQuery(deleteCommand, identifier);
        }

        /// <summary>
        /// Builds the delete SQL query.
        /// </summary>
        /// <param name="instance">The instance to be deleted.</param>
        /// <param name="objectInfo">The object information.</param>
        /// <returns>
        /// The created <see cref="SqlQuery" />.
        /// </returns>
        protected virtual SqlQuery BuildDeleteSqlQuery(object instance, IObjectInfo objectInfo)
        {
            string deleteCommand;

            if (!this.deleteCommandCache.TryGetValue(objectInfo.ForType, out deleteCommand))
            {
                var deleteSqlQuery = new DeleteSqlBuilder(this.SqlCharacters)
                    .From(objectInfo)
                    .WhereEquals(objectInfo.TableInfo.IdentifierColumn, objectInfo.GetIdentifierValue(instance))
                    .ToSqlQuery();

                var newDeleteCommandCache = new Dictionary<Type, string>(this.deleteCommandCache);
                newDeleteCommandCache[objectInfo.ForType] = deleteSqlQuery.CommandText;

                this.deleteCommandCache = newDeleteCommandCache;

                return deleteSqlQuery;
            }

            return new SqlQuery(deleteCommand, objectInfo.GetIdentifierValue(instance));
        }

        /// <summary>
        /// Builds the insert SQL query.
        /// </summary>
        /// <param name="instance">The instance to be inserted.</param>
        /// <param name="objectInfo">The object information.</param>
        /// <returns>
        /// The created <see cref="SqlQuery" />.
        /// </returns>
        protected virtual SqlQuery BuildInsertSqlQuery(object instance, IObjectInfo objectInfo)
        {
            string insertCommand;

            if (!this.insertCommandCache.TryGetValue(objectInfo.ForType, out insertCommand))
            {
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

                var newInsertCommandCache = new Dictionary<Type, string>(this.insertCommandCache);
                newInsertCommandCache[objectInfo.ForType] = insertSqlQuery.CommandText;

                this.insertCommandCache = newInsertCommandCache;

                return insertSqlQuery;
            }

            var insertValues = new List<object>(objectInfo.TableInfo.Columns.Count);

            for (int i = 0; i < objectInfo.TableInfo.Columns.Count; i++)
            {
                var columnInfo = objectInfo.TableInfo.Columns[i];

                if (columnInfo.AllowInsert)
                {
                    var value = objectInfo.GetPropertyValueForColumn(instance, columnInfo.ColumnName);
                    insertValues.Add(value);
                }
            }

            return new SqlQuery(insertCommand, insertValues.ToArray());
        }

        /// <summary>
        /// Builds the select SQL query.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        /// <param name="identifier">The identifier.</param>
        /// <returns>
        /// The created <see cref="SqlQuery" />.
        /// </returns>
        protected virtual SqlQuery BuildSelectSqlQuery(IObjectInfo objectInfo, object identifier)
        {
            string selectCommand;

            if (!this.selectCommandCache.TryGetValue(objectInfo.ForType, out selectCommand))
            {
                var selectSqlQuery = new SelectSqlBuilder(this.SqlCharacters)
                    .From(objectInfo)
                    .Where(objectInfo.TableInfo.IdentifierColumn).IsEqualTo(identifier)
                    .ToSqlQuery();

                var newSelectCommandCache = new Dictionary<Type, string>(this.selectCommandCache);
                newSelectCommandCache[objectInfo.ForType] = selectSqlQuery.CommandText;

                this.selectCommandCache = newSelectCommandCache;

                return selectSqlQuery;
            }

            return new SqlQuery(selectCommand, identifier);
        }

        /// <summary>
        /// Builds the update SQL query.
        /// </summary>
        /// <param name="instance">The instance to be updated.</param>
        /// <param name="objectInfo">The object information.</param>
        /// <returns>
        /// The created <see cref="SqlQuery" />.
        /// </returns>
        protected virtual SqlQuery BuildUpdateSqlQuery(object instance, IObjectInfo objectInfo)
        {
            string updateCommand;

            if (!this.updateCommandCache.TryGetValue(objectInfo.ForType, out updateCommand))
            {
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

                updateSqlBuilder.Where(objectInfo.TableInfo.IdentifierColumn, objectInfo.GetIdentifierValue(instance));

                var updateSqlQuery = updateSqlBuilder.ToSqlQuery();

                var newUpdateCommandCache = new Dictionary<Type, string>(this.updateCommandCache);
                newUpdateCommandCache[objectInfo.ForType] = updateSqlQuery.CommandText;

                this.updateCommandCache = newUpdateCommandCache;

                return updateSqlQuery;
            }

            var updateValues = new List<object>(objectInfo.TableInfo.Columns.Count);

            for (int i = 0; i < objectInfo.TableInfo.Columns.Count; i++)
            {
                var columnInfo = objectInfo.TableInfo.Columns[i];

                if (columnInfo.AllowUpdate)
                {
                    var value = objectInfo.GetPropertyValueForColumn(instance, columnInfo.ColumnName);
                    updateValues.Add(value);
                }
            }

            updateValues.Add(objectInfo.GetIdentifierValue(instance));

            return new SqlQuery(updateCommand, updateValues.ToArray());
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