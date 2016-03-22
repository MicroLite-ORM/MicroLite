// -----------------------------------------------------------------------
// <copyright file="SqlDialect.cs" company="MicroLite">
// Copyright 2012 - 2016 Project Contributors
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
    using MicroLite.Builder;
    using MicroLite.Characters;
    using MicroLite.Logging;
    using MicroLite.Mapping;

    /// <summary>
    /// The base class for implementations of <see cref="ISqlDialect" />.
    /// </summary>
    public abstract class SqlDialect : ISqlDialect
    {
        private static readonly ILog log = LogManager.GetCurrentClassLog();
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
        /// Gets the SQL characters used by the SQL dialect.
        /// </summary>
        public SqlCharacters SqlCharacters
        {
            get
            {
                return this.sqlCharacters;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the SQL Dialect supports selecting the identifier value of an inserted column.
        /// </summary>
        public virtual bool SupportsSelectInsertedIdentifier
        {
            get
            {
                return false;
            }
        }

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
            if (objectInfo == null)
            {
                throw new ArgumentNullException("objectInfo");
            }

            if (log.IsDebug)
            {
                log.Debug(LogMessages.SqlDialect_CreatingSqlQuery, "DELETE");
            }

            string deleteCommand;

            if (!this.deleteCommandCache.TryGetValue(objectInfo.ForType, out deleteCommand))
            {
                deleteCommand = this.BuildDeleteCommandText(objectInfo);

                var newDeleteCommandCache = new Dictionary<Type, string>(this.deleteCommandCache);
                newDeleteCommandCache[objectInfo.ForType] = deleteCommand;

                this.deleteCommandCache = newDeleteCommandCache;
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
            if (objectInfo == null)
            {
                throw new ArgumentNullException("objectInfo");
            }

            if (log.IsDebug)
            {
                log.Debug(LogMessages.SqlDialect_CreatingSqlQuery, "INSERT");
            }

            string insertCommand;

            if (!this.insertCommandCache.TryGetValue(objectInfo.ForType, out insertCommand))
            {
                insertCommand = this.BuildInsertCommandText(objectInfo);

                var newInsertCommandCache = new Dictionary<Type, string>(this.insertCommandCache);
                newInsertCommandCache[objectInfo.ForType] = insertCommand;

                this.insertCommandCache = newInsertCommandCache;
            }

            var insertValues = objectInfo.GetInsertValues(instance);

            return new SqlQuery(insertCommand, insertValues);
        }

        /// <summary>
        /// Builds an SqlQuery to select the identity of an inserted object if the database supports Identity or AutoIncrement.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        /// <returns>
        /// The created <see cref="SqlQuery" />.
        /// </returns>
        public virtual SqlQuery BuildSelectInsertIdSqlQuery(IObjectInfo objectInfo)
        {
            return new SqlQuery(string.Empty);
        }

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
            if (objectInfo == null)
            {
                throw new ArgumentNullException("objectInfo");
            }

            if (log.IsDebug)
            {
                log.Debug(LogMessages.SqlDialect_CreatingSqlQuery, "SELECT");
            }

            string selectCommand;

            if (!this.selectCommandCache.TryGetValue(objectInfo.ForType, out selectCommand))
            {
                selectCommand = this.BuildSelectCommandText(objectInfo);

                var newSelectCommandCache = new Dictionary<Type, string>(this.selectCommandCache);
                newSelectCommandCache[objectInfo.ForType] = selectCommand;

                this.selectCommandCache = newSelectCommandCache;
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
            if (objectInfo == null)
            {
                throw new ArgumentNullException("objectInfo");
            }

            if (log.IsDebug)
            {
                log.Debug(LogMessages.SqlDialect_CreatingSqlQuery, "UPDATE");
            }

            string updateCommand;

            if (!this.updateCommandCache.TryGetValue(objectInfo.ForType, out updateCommand))
            {
                updateCommand = this.BuildUpdateCommandText(objectInfo);

                var newUpdateCommandCache = new Dictionary<Type, string>(this.updateCommandCache);
                newUpdateCommandCache[objectInfo.ForType] = updateCommand;

                this.updateCommandCache = newUpdateCommandCache;
            }

            var updateValues = objectInfo.GetUpdateValues(instance);

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
            if (objectDelta == null)
            {
                throw new ArgumentNullException("objectDelta");
            }

            if (log.IsDebug)
            {
                log.Debug(LogMessages.SqlDialect_CreatingSqlQuery, "UPDATE");
            }

            var objectInfo = ObjectInfo.For(objectDelta.ForType);

            var builder = new UpdateSqlBuilder(this.SqlCharacters)
                .Table(objectInfo);

            foreach (var change in objectDelta.Changes)
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
            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

            if (log.IsDebug)
            {
                log.Debug(LogMessages.SqlDialect_CreatingSqlQuery, "COUNT");
            }

            var sqlString = SqlString.Parse(sqlQuery.CommandText, Clauses.From | Clauses.Where);

            var qualifiedTableName = sqlString.From;
            var whereValue = sqlString.Where;
            var whereClause = !string.IsNullOrEmpty(whereValue) ? " WHERE " + whereValue : string.Empty;
            var argumentsArray = !string.IsNullOrEmpty(whereValue) ? sqlQuery.ArgumentsArray : null;

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
            if (objectInfo == null)
            {
                throw new ArgumentNullException("objectInfo");
            }

            var deleteSqlQuery = new DeleteSqlBuilder(this.SqlCharacters)
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
            if (objectInfo == null)
            {
                throw new ArgumentNullException("objectInfo");
            }

            var counter = 0;
            var insertColumns = new string[objectInfo.TableInfo.InsertColumnCount];

            for (int i = 0; i < objectInfo.TableInfo.Columns.Count; i++)
            {
                var columnInfo = objectInfo.TableInfo.Columns[i];

                if (columnInfo.AllowInsert)
                {
                    insertColumns[counter++] = columnInfo.ColumnName;
                }
            }

            var insertSqlQuery = new InsertSqlBuilder(this.SqlCharacters)
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
            if (objectInfo == null)
            {
                throw new ArgumentNullException("objectInfo");
            }

            var selectSqlQuery = new SelectSqlBuilder(this.SqlCharacters)
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
            if (objectInfo == null)
            {
                throw new ArgumentNullException("objectInfo");
            }

            var builder = new UpdateSqlBuilder(this.SqlCharacters)
                       .Table(objectInfo);

            for (int i = 0; i < objectInfo.TableInfo.Columns.Count; i++)
            {
                var columnInfo = objectInfo.TableInfo.Columns[i];

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