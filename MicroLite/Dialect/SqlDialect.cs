// -----------------------------------------------------------------------
// <copyright file="SqlDialect.cs" company="Project Contributors">
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
using MicroLite.Builder;
using MicroLite.Builder.Syntax.Write;
using MicroLite.Characters;
using MicroLite.Logging;
using MicroLite.Mapping;

namespace MicroLite.Dialect
{
    /// <summary>
    /// The base class for implementations of <see cref="ISqlDialect" />.
    /// </summary>
    public abstract class SqlDialect : ISqlDialect
    {
        private static readonly ILog s_log = LogManager.GetCurrentClassLog();
        private Dictionary<Type, string> _deleteCommandCache = new Dictionary<Type, string>();
        private Dictionary<Type, string> _insertCommandCache = new Dictionary<Type, string>();
        private Dictionary<Type, string> _selectCommandCache = new Dictionary<Type, string>();
        private Dictionary<Type, string> _updateCommandCache = new Dictionary<Type, string>();

        /// <summary>
        /// Initialises a new instance of the <see cref="SqlDialect"/> class.
        /// </summary>
        /// <param name="sqlCharacters">The SQL characters.</param>
        protected SqlDialect(SqlCharacters sqlCharacters)
            => SqlCharacters = sqlCharacters ?? throw new ArgumentNullException(nameof(sqlCharacters));

        /// <summary>
        /// Gets the SQL characters used by the SQL dialect.
        /// </summary>
        public SqlCharacters SqlCharacters { get; }

        /// <summary>
        /// Gets a value indicating whether the SQL Dialect supports selecting the identifier value of an inserted column.
        /// </summary>
        public virtual bool SupportsSelectInsertedIdentifier => false;

        /// <summary>
        /// Builds an SqlQuery to delete the database record with the specified identifier for the type specified by the IObjectInfo.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        /// <param name="identifier">The identifier of the instance to delete.</param>
        /// <returns>
        /// The created <see cref="SqlQuery" />.
        /// </returns>
        public SqlQuery BuildDeleteSqlQuery(IObjectInfo objectInfo, object identifier)
        {
            if (objectInfo is null)
            {
                throw new ArgumentNullException(nameof(objectInfo));
            }

            if (s_log.IsDebug)
            {
                s_log.Debug(LogMessages.SqlDialect_CreatingSqlQuery, "DELETE");
            }

            if (!_deleteCommandCache.TryGetValue(objectInfo.ForType, out string deleteCommand))
            {
                deleteCommand = BuildDeleteCommandText(objectInfo);

                var newDeleteCommandCache = new Dictionary<Type, string>(_deleteCommandCache)
                {
                    [objectInfo.ForType] = deleteCommand,
                };

                _deleteCommandCache = newDeleteCommandCache;
            }

            return new SqlQuery(deleteCommand, new SqlArgument(identifier, objectInfo.TableInfo.IdentifierColumn.DbType));
        }

        /// <summary>
        /// Builds an SqlQuery to insert a database record for the specified instance with the current property values of the instance.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        /// <param name="instance">The instance to insert.</param>
        /// <returns>
        /// The created <see cref="SqlQuery" />.
        /// </returns>
        public SqlQuery BuildInsertSqlQuery(IObjectInfo objectInfo, object instance)
        {
            if (objectInfo is null)
            {
                throw new ArgumentNullException(nameof(objectInfo));
            }

            if (s_log.IsDebug)
            {
                s_log.Debug(LogMessages.SqlDialect_CreatingSqlQuery, "INSERT");
            }

            if (!_insertCommandCache.TryGetValue(objectInfo.ForType, out string insertCommand))
            {
                insertCommand = BuildInsertCommandText(objectInfo);

                var newInsertCommandCache = new Dictionary<Type, string>(_insertCommandCache)
                {
                    [objectInfo.ForType] = insertCommand,
                };

                _insertCommandCache = newInsertCommandCache;
            }

            SqlArgument[] insertValues = objectInfo.GetInsertValues(instance);

            return new SqlQuery(insertCommand, insertValues);
        }

        /// <summary>
        /// Builds an SqlQuery to select the identity of an inserted object if the database supports Identity or AutoIncrement.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        /// <returns>
        /// The created <see cref="SqlQuery" />.
        /// </returns>
        public virtual SqlQuery BuildSelectInsertIdSqlQuery(IObjectInfo objectInfo) => new SqlQuery(string.Empty);

        /// <summary>
        /// Builds an SqlQuery to select the database record with the specified identifier for the type specified by the IObjectInfo.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        /// <param name="identifier">The identifier of the instance to select.</param>
        /// <returns>
        /// The created <see cref="SqlQuery" />.
        /// </returns>
        public SqlQuery BuildSelectSqlQuery(IObjectInfo objectInfo, object identifier)
        {
            if (objectInfo is null)
            {
                throw new ArgumentNullException(nameof(objectInfo));
            }

            if (s_log.IsDebug)
            {
                s_log.Debug(LogMessages.SqlDialect_CreatingSqlQuery, "SELECT");
            }

            if (!_selectCommandCache.TryGetValue(objectInfo.ForType, out string selectCommand))
            {
                selectCommand = BuildSelectCommandText(objectInfo);

                var newSelectCommandCache = new Dictionary<Type, string>(_selectCommandCache)
                {
                    [objectInfo.ForType] = selectCommand,
                };

                _selectCommandCache = newSelectCommandCache;
            }

            return new SqlQuery(selectCommand, new SqlArgument(identifier, objectInfo.TableInfo.IdentifierColumn.DbType));
        }

        /// <summary>
        /// Builds an SqlQuery to update the database record for the specified instance with the current property values of the instance.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        /// <param name="instance">The instance to update.</param>
        /// <returns>
        /// The created <see cref="SqlQuery" />.
        /// </returns>
        public SqlQuery BuildUpdateSqlQuery(IObjectInfo objectInfo, object instance)
        {
            if (objectInfo is null)
            {
                throw new ArgumentNullException(nameof(objectInfo));
            }

            if (s_log.IsDebug)
            {
                s_log.Debug(LogMessages.SqlDialect_CreatingSqlQuery, "UPDATE");
            }

            if (!_updateCommandCache.TryGetValue(objectInfo.ForType, out string updateCommand))
            {
                updateCommand = BuildUpdateCommandText(objectInfo);

                var newUpdateCommandCache = new Dictionary<Type, string>(_updateCommandCache)
                {
                    [objectInfo.ForType] = updateCommand,
                };

                _updateCommandCache = newUpdateCommandCache;
            }

            SqlArgument[] updateValues = objectInfo.GetUpdateValues(instance);

            return new SqlQuery(updateCommand, updateValues);
        }

        /// <summary>
        /// Creates an SqlQuery to perform an update based upon the values in the object delta.
        /// </summary>
        /// <param name="objectDelta">The object delta to create the query for.</param>
        /// <returns>
        /// The created <see cref="SqlQuery" />.
        /// </returns>
        public SqlQuery BuildUpdateSqlQuery(ObjectDelta objectDelta)
        {
            if (objectDelta is null)
            {
                throw new ArgumentNullException(nameof(objectDelta));
            }

            if (s_log.IsDebug)
            {
                s_log.Debug(LogMessages.SqlDialect_CreatingSqlQuery, "UPDATE");
            }

            IObjectInfo objectInfo = ObjectInfo.For(objectDelta.ForType);

            ISetOrWhere builder = new UpdateSqlBuilder(SqlCharacters)
                .Table(objectInfo);

            foreach (KeyValuePair<string, object> change in objectDelta.Changes)
            {
                builder.SetColumnValue(change.Key, change.Value);
            }

            var sqlQuery = builder
                .Where(objectInfo.TableInfo.IdentifierColumn.ColumnName).IsEqualTo(objectDelta.Identifier)
                .ToSqlQuery();

            return sqlQuery;
        }

        /// <summary>
        /// Creates an SqlQuery to count the number of records which would be returned by the specified SqlQuery.
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <returns>
        /// An <see cref="SqlQuery" /> to count the number of records which would be returned by the specified SqlQuery.
        /// </returns>
        public SqlQuery CountQuery(SqlQuery sqlQuery)
        {
            if (sqlQuery is null)
            {
                throw new ArgumentNullException(nameof(sqlQuery));
            }

            if (s_log.IsDebug)
            {
                s_log.Debug(LogMessages.SqlDialect_CreatingSqlQuery, "COUNT");
            }

            var sqlString = SqlString.Parse(sqlQuery.CommandText, Clauses.From | Clauses.Where);

            string qualifiedTableName = sqlString.From;
            string whereValue = sqlString.Where;
            string whereClause = !string.IsNullOrEmpty(whereValue) ? " WHERE " + whereValue : string.Empty;
            SqlArgument[] argumentsArray = !string.IsNullOrEmpty(whereValue) ? sqlQuery.ArgumentsArray : null;

            return new SqlQuery("SELECT COUNT(*) FROM " + qualifiedTableName + whereClause, argumentsArray);
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
        /// Builds the command text to delete a database record for the specified <see cref="IObjectInfo"/>.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        /// <returns>
        /// The created command text.
        /// </returns>
        protected virtual string BuildDeleteCommandText(IObjectInfo objectInfo)
        {
            if (objectInfo is null)
            {
                throw new ArgumentNullException(nameof(objectInfo));
            }

            var deleteSqlQuery = new DeleteSqlBuilder(SqlCharacters)
                .From(objectInfo)
                .Where(objectInfo.TableInfo.IdentifierColumn.ColumnName).IsEqualTo(0)
                .ToSqlQuery();

            return deleteSqlQuery.CommandText;
        }

        /// <summary>
        /// Builds the command text to insert a database record for the specified <see cref="IObjectInfo"/>.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        /// <returns>
        /// The created command text.
        /// </returns>
        protected virtual string BuildInsertCommandText(IObjectInfo objectInfo)
        {
            if (objectInfo is null)
            {
                throw new ArgumentNullException(nameof(objectInfo));
            }

            int counter = 0;
            string[] insertColumns = new string[objectInfo.TableInfo.InsertColumnCount];

            for (int i = 0; i < objectInfo.TableInfo.Columns.Count; i++)
            {
                ColumnInfo columnInfo = objectInfo.TableInfo.Columns[i];

                if (columnInfo.AllowInsert)
                {
                    insertColumns[counter++] = columnInfo.ColumnName;
                }
            }

            var insertSqlQuery = new InsertSqlBuilder(SqlCharacters)
                .Into(objectInfo)
                .Columns(insertColumns)
                .Values(new object[objectInfo.TableInfo.InsertColumnCount])
                .ToSqlQuery();

            return insertSqlQuery.CommandText;
        }

        /// <summary>
        /// Builds the command text to select a database record for the specified <see cref="IObjectInfo"/>.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        /// <returns>
        /// The created command text.
        /// </returns>
        protected virtual string BuildSelectCommandText(IObjectInfo objectInfo)
        {
            if (objectInfo is null)
            {
                throw new ArgumentNullException(nameof(objectInfo));
            }

            var selectSqlQuery = new SelectSqlBuilder(SqlCharacters)
                .From(objectInfo)
                .Where(objectInfo.TableInfo.IdentifierColumn.ColumnName).IsEqualTo(0)
                .ToSqlQuery();

            return selectSqlQuery.CommandText;
        }

        /// <summary>
        /// Builds the command text to update a database record for the specified <see cref="IObjectInfo"/>.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        /// <returns>
        /// The created command text.
        /// </returns>
        protected virtual string BuildUpdateCommandText(IObjectInfo objectInfo)
        {
            if (objectInfo is null)
            {
                throw new ArgumentNullException(nameof(objectInfo));
            }

            ISetOrWhere builder = new UpdateSqlBuilder(SqlCharacters)
                       .Table(objectInfo);

            for (int i = 0; i < objectInfo.TableInfo.Columns.Count; i++)
            {
                ColumnInfo columnInfo = objectInfo.TableInfo.Columns[i];

                if (columnInfo.AllowUpdate)
                {
                    builder.SetColumnValue(columnInfo.ColumnName, null);
                }
            }

            var updateSqlQuery = builder
                .Where(objectInfo.TableInfo.IdentifierColumn.ColumnName).IsEqualTo(0)
                .ToSqlQuery();

            return updateSqlQuery.CommandText;
        }
    }
}
