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
        /// Creates an SqlQuery to count the number of records which would be returned by the specified SqlQuery.
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <returns>
        /// An <see cref="SqlQuery" /> to count the number of records which would be returned by the specified SqlQuery.
        /// </returns>
        public virtual SqlQuery CountQuery(SqlQuery sqlQuery)
        {
            if (sqlQuery == null)
            {
                throw new ArgumentNullException("sqlQuery");
            }

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
                .Where(objectInfo.TableInfo.IdentifierColumn.ColumnName, objectDelta.Identifier)
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
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            var objectInfo = ObjectInfo.For(instance.GetType());

            switch (statementType)
            {
                case StatementType.Delete:
                    return this.BuildDeleteSqlQuery(objectInfo, objectInfo.GetIdentifierValue(instance));

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
        /// Creates an SqlQuery to select the identity of an inserted object if the database supports Identity or AutoIncrement.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        /// <returns>
        /// The created <see cref="SqlQuery" />.
        /// </returns>
        public virtual SqlQuery CreateSelectIdentityQuery(IObjectInfo objectInfo)
        {
            return new SqlQuery(string.Empty);
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

                var insertSqlBuilder = new InsertSqlBuilder(this.SqlCharacters);
                insertSqlBuilder.Into(objectInfo);
                insertSqlBuilder.Columns(insertColumns);
                insertSqlBuilder.Values(new object[objectInfo.TableInfo.InsertColumnCount]);

                var insertSqlQuery = insertSqlBuilder.ToSqlQuery();

                var newInsertCommandCache = new Dictionary<Type, string>(this.insertCommandCache);
                newInsertCommandCache[objectInfo.ForType] = insertSqlQuery.CommandText;
                insertCommand = insertSqlQuery.CommandText;

                this.insertCommandCache = newInsertCommandCache;
            }

            var insertValues = objectInfo.GetInsertValues(instance);

            return new SqlQuery(insertCommand, insertValues);
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
                        updateSqlBuilder.SetColumnValue(columnInfo.ColumnName, null);
                    }
                }

                updateSqlBuilder.Where(objectInfo.TableInfo.IdentifierColumn.ColumnName, objectInfo.GetIdentifierValue(instance));

                var updateSqlQuery = updateSqlBuilder.ToSqlQuery();

                var newUpdateCommandCache = new Dictionary<Type, string>(this.updateCommandCache);
                newUpdateCommandCache[objectInfo.ForType] = updateSqlQuery.CommandText;
                updateCommand = updateSqlQuery.CommandText;

                this.updateCommandCache = newUpdateCommandCache;
            }

            var updateValues = objectInfo.GetUpdateValues(instance);

            return new SqlQuery(updateCommand, updateValues);
        }
    }
}