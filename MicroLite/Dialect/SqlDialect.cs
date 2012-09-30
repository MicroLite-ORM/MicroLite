﻿// -----------------------------------------------------------------------
// <copyright file="SqlDialect.cs" company="MicroLite">
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
namespace MicroLite.Dialect
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Text;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;

    /// <summary>
    /// The base class for implementations of <see cref="ISqlDialect"/>.
    /// </summary>
    internal abstract class SqlDialect : ISqlDialect
    {
        private readonly string defaultTableSchema;

        protected SqlDialect(string defaultTableSchema)
        {
            this.defaultTableSchema = defaultTableSchema;
        }

        public virtual SqlQuery CountQuery(SqlQuery sqlQuery)
        {
            var qualifiedTableName = SqlUtil.ReadTableName(sqlQuery.CommandText);
            var whereValue = SqlUtil.ReadWhereClause(sqlQuery.CommandText);
            var whereClause = !string.IsNullOrEmpty(whereValue) ? " WHERE " + whereValue : string.Empty;

            return new SqlQuery("SELECT COUNT(*) FROM " + qualifiedTableName + whereClause, sqlQuery.Arguments.ToArray());
        }

        public virtual SqlQuery CreateQuery(StatementType statementType, object instance)
        {
            var forType = instance.GetType();
            var objectInfo = ObjectInfo.For(forType);

            switch (statementType)
            {
                case StatementType.Delete:
                    var identifierValue = objectInfo.GetPropertyInfoForColumn(objectInfo.TableInfo.IdentifierColumn).GetValue(instance);

                    return this.CreateQuery(StatementType.Delete, forType, identifierValue);

                case StatementType.Insert:
                    var insertValues = new List<object>();

                    var insertSqlBuilder = this.CreateSql(statementType, objectInfo);
                    insertSqlBuilder.Append(" VALUES (");

                    foreach (var column in objectInfo.TableInfo.Columns)
                    {
                        if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.Identity
                            && column.ColumnName.Equals(objectInfo.TableInfo.IdentifierColumn))
                        {
                            continue;
                        }

                        if (column.AllowInsert)
                        {
                            insertSqlBuilder.Append(this.FormatParameter(insertValues.Count) + ", ");

                            var propertyInfo = objectInfo.GetPropertyInfoForColumn(column.ColumnName);

                            var value = propertyInfo.GetValue(instance);

                            insertValues.Add(value);
                        }
                    }

                    insertSqlBuilder.Remove(insertSqlBuilder.Length - 2, 2);
                    insertSqlBuilder.Append(")");

                    return new SqlQuery(insertSqlBuilder.ToString(), insertValues.ToArray());

                case StatementType.Update:
                    var updateValues = new List<object>();

                    var updateSqlBuilder = this.CreateSql(StatementType.Update, objectInfo);

                    foreach (var column in objectInfo.TableInfo.Columns)
                    {
                        if (column.AllowUpdate
                            && !column.ColumnName.Equals(objectInfo.TableInfo.IdentifierColumn))
                        {
                            updateSqlBuilder.AppendFormat(
                                        " {0}.{1} = {2},",
                                        this.EscapeSql(objectInfo.TableInfo.Name),
                                        this.EscapeSql(column.ColumnName),
                                        this.FormatParameter(updateValues.Count));

                            var propertyInfo = objectInfo.GetPropertyInfoForColumn(column.ColumnName);

                            var value = propertyInfo.GetValue(instance);

                            updateValues.Add(value);
                        }
                    }

                    updateSqlBuilder.Remove(updateSqlBuilder.Length - 1, 1);

                    updateSqlBuilder.AppendFormat(
                        " WHERE {0}.{1} = {2}",
                        this.EscapeSql(objectInfo.TableInfo.Name),
                        this.EscapeSql(objectInfo.TableInfo.IdentifierColumn),
                        this.FormatParameter(updateValues.Count));

                    updateValues.Add(objectInfo.GetPropertyInfoForColumn(objectInfo.TableInfo.IdentifierColumn).GetValue(instance));

                    return new SqlQuery(updateSqlBuilder.ToString(), updateValues.ToArray());

                default:
                    throw new NotSupportedException(Messages.SqlDialect_StatementTypeNotSupported);
            }
        }

        public virtual SqlQuery CreateQuery(StatementType statementType, Type forType, object identifier)
        {
            switch (statementType)
            {
                case StatementType.Delete:
                case StatementType.Select:
                    var objectInfo = ObjectInfo.For(forType);

                    var sqlBuilder = this.CreateSql(statementType, objectInfo);
                    sqlBuilder.AppendFormat(
                        " WHERE {0}.{1} = {2}",
                        this.EscapeSql(objectInfo.TableInfo.Name),
                        this.EscapeSql(objectInfo.TableInfo.IdentifierColumn),
                        this.FormatParameter(0));

                    return new SqlQuery(sqlBuilder.ToString(), new[] { identifier });

                default:
                    throw new NotSupportedException(Messages.SqlDialect_StatementTypeNotSupported);
            }
        }

        public abstract SqlQuery PageQuery(SqlQuery sqlQuery, long page, long resultsPerPage);

        protected abstract string EscapeSql(string sql);

        protected abstract string FormatParameter(int parameterPosition);

        private StringBuilder CreateSql(StatementType statementType, ObjectInfo objectInfo)
        {
            var sqlBuilder = new StringBuilder();

            switch (statementType)
            {
                case StatementType.Delete:
                    sqlBuilder.AppendFormat(
                        "DELETE FROM {0}.{1}",
                        this.EscapeSql(!string.IsNullOrEmpty(objectInfo.TableInfo.Schema) ? objectInfo.TableInfo.Schema : this.defaultTableSchema),
                        this.EscapeSql(objectInfo.TableInfo.Name));

                    break;

                case StatementType.Insert:
                    sqlBuilder.AppendFormat(
                        "INSERT INTO {0}.{1} (",
                        this.EscapeSql(!string.IsNullOrEmpty(objectInfo.TableInfo.Schema) ? objectInfo.TableInfo.Schema : this.defaultTableSchema),
                        this.EscapeSql(objectInfo.TableInfo.Name));

                    foreach (var column in objectInfo.TableInfo.Columns)
                    {
                        if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.Identity
                            && column.ColumnName.Equals(objectInfo.TableInfo.IdentifierColumn))
                        {
                            continue;
                        }

                        if (column.AllowInsert)
                        {
                            sqlBuilder.AppendFormat("{0}.{1}, ", this.EscapeSql(objectInfo.TableInfo.Name), this.EscapeSql(column.ColumnName));
                        }
                    }

                    sqlBuilder.Remove(sqlBuilder.Length - 2, 2);
                    sqlBuilder.Append(")");

                    break;

                case StatementType.Select:
                    sqlBuilder.Append("SELECT");

                    foreach (var column in objectInfo.TableInfo.Columns)
                    {
                        sqlBuilder.AppendFormat(" {0}.{1},", this.EscapeSql(objectInfo.TableInfo.Name), this.EscapeSql(column.ColumnName));
                    }

                    sqlBuilder.Remove(sqlBuilder.Length - 1, 1);

                    sqlBuilder.AppendFormat(
                        " FROM {0}.{1}",
                        this.EscapeSql(!string.IsNullOrEmpty(objectInfo.TableInfo.Schema) ? objectInfo.TableInfo.Schema : this.defaultTableSchema),
                        this.EscapeSql(objectInfo.TableInfo.Name));

                    break;

                case StatementType.Update:
                    sqlBuilder.AppendFormat(
                        "UPDATE {0}.{1} SET",
                        this.EscapeSql(!string.IsNullOrEmpty(objectInfo.TableInfo.Schema) ? objectInfo.TableInfo.Schema : this.defaultTableSchema),
                        this.EscapeSql(objectInfo.TableInfo.Name));

                    break;
            }

            return sqlBuilder;
        }
    }
}