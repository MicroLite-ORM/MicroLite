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
    using MicroLite.Builder;
    using MicroLite.Mapping;

    /// <summary>
    /// The base class for implementations of <see cref="ISqlDialect" />.
    /// </summary>
    internal abstract class SqlDialect : ISqlDialect
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
        /// Gets a value indicating whether the SQL Dialect supports Identity or AutoIncrement columns.
        /// </summary>
        /// <remarks>Returns false unless overridden.</remarks>
        public virtual bool SupportsIdentity
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

            string deleteCommand;

            if (!this.deleteCommandCache.TryGetValue(objectInfo.ForType, out deleteCommand))
            {
                var deleteSqlQuery = new DeleteSqlBuilder(this.SqlCharacters)
                    .From(objectInfo)
                    .WhereEquals(objectInfo.TableInfo.IdentifierColumn.ColumnName, identifier)
                    .ToSqlQuery();

                var newDeleteCommandCache = new Dictionary<Type, string>(this.deleteCommandCache);
                newDeleteCommandCache[objectInfo.ForType] = deleteSqlQuery.CommandText;

                this.deleteCommandCache = newDeleteCommandCache;

                return deleteSqlQuery;
            }

            return new SqlQuery(deleteCommand, identifier);
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

            string insertCommand;

            if (!this.insertCommandCache.TryGetValue(objectInfo.ForType, out insertCommand))
            {
                var commandText = this.BuildInsertCommandText(objectInfo);

                var newInsertCommandCache = new Dictionary<Type, string>(this.insertCommandCache);
                newInsertCommandCache[objectInfo.ForType] = commandText;
                insertCommand = commandText;

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
        public virtual SqlQuery BuildSelectIdentitySqlQuery(IObjectInfo objectInfo)
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

            string selectCommand;

            if (!this.selectCommandCache.TryGetValue(objectInfo.ForType, out selectCommand))
            {
                var selectSqlQuery = new SelectSqlBuilder(this.SqlCharacters)
                    .From(objectInfo)
                    .Where(objectInfo.TableInfo.IdentifierColumn.ColumnName).IsEqualTo(identifier)
                    .ToSqlQuery();

                var newSelectCommandCache = new Dictionary<Type, string>(this.selectCommandCache);
                newSelectCommandCache[objectInfo.ForType] = selectSqlQuery.CommandText;

                this.selectCommandCache = newSelectCommandCache;

                return selectSqlQuery;
            }

            return new SqlQuery(selectCommand, identifier);
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

            string updateCommand;

            if (!this.updateCommandCache.TryGetValue(objectInfo.ForType, out updateCommand))
            {
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

                var updateSqlQuery = builder.WhereEquals(objectInfo.TableInfo.IdentifierColumn.ColumnName, objectInfo.GetIdentifierValue(instance))
                    .ToSqlQuery();

                var newUpdateCommandCache = new Dictionary<Type, string>(this.updateCommandCache);
                newUpdateCommandCache[objectInfo.ForType] = updateSqlQuery.CommandText;
                updateCommand = updateSqlQuery.CommandText;

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

            var objectInfo = ObjectInfo.For(objectDelta.ForType);

            var builder = new UpdateSqlBuilder(this.SqlCharacters)
                .Table(objectInfo);

            foreach (var change in objectDelta.Changes)
            {
                builder.SetColumnValue(change.Key, change.Value);
            }

            var sqlQuery = builder
                .WhereEquals(objectInfo.TableInfo.IdentifierColumn.ColumnName, objectDelta.Identifier)
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

            var sqlString = SqlString.Parse(sqlQuery.CommandText, Clauses.From | Clauses.Where);

            var qualifiedTableName = sqlString.From;
            var whereValue = sqlString.Where;
            var whereClause = !string.IsNullOrEmpty(whereValue) ? " WHERE " + whereValue : string.Empty;

            return new SqlQuery("SELECT COUNT(*) FROM " + qualifiedTableName + whereClause, sqlQuery.GetArgumentArray());
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
        /// Builds the command text to insert a database record for the specified <see cref="IObjectInfo"/>.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        /// <returns>
        /// The created command text.
        /// </returns>
        protected virtual string BuildInsertCommandText(IObjectInfo objectInfo)
        {
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
    }
}