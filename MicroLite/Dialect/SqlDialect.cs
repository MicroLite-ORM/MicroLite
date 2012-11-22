// -----------------------------------------------------------------------
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
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using MicroLite.Mapping;

    /// <summary>
    /// The base class for implementations of <see cref="ISqlDialect" />.
    /// </summary>
    internal abstract class SqlDialect : ISqlDialect
    {
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
        /// Gets the database generated identifier strategies.
        /// </summary>
        protected virtual IdentifierStrategy[] DatabaseGeneratedStrategies
        {
            get
            {
                return new IdentifierStrategy[0];
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
        /// Gets a value indicating whether SQL parameters include the position (parameter number).
        /// </summary>
        protected virtual bool SqlParameterIncludesPosition
        {
            get
            {
                return false;
            }
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
                    var identifierValue = objectInfo.GetPropertyValueForColumn(instance, objectInfo.TableInfo.IdentifierColumn);

                    return this.CreateQuery(StatementType.Delete, forType, identifierValue);

                case StatementType.Insert:
                    var insertValues = new List<object>();

                    var insertSqlBuilder = this.CreateSql(statementType, objectInfo);
                    insertSqlBuilder.Append(" VALUES (");

                    foreach (var column in objectInfo.TableInfo.Columns)
                    {
                        if (this.DatabaseGeneratedStrategies.Contains(objectInfo.TableInfo.IdentifierStrategy)
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

                    if (this.DatabaseGeneratedStrategies.Contains(objectInfo.TableInfo.IdentifierStrategy))
                    {
                        insertSqlBuilder.Append(this.SelectSeparator);
                        insertSqlBuilder.Append(this.SelectIdentityString);
                    }

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

                    updateValues.Add(objectInfo.GetPropertyValueForColumn(instance, objectInfo.TableInfo.IdentifierColumn));

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
                        " WHERE {0} = {1}",
                        this.EscapeSql(objectInfo.TableInfo.IdentifierColumn),
                        this.FormatParameter(0));

                    return new SqlQuery(sqlBuilder.ToString(), new[] { identifier });

                default:
                    throw new NotSupportedException(Messages.SqlDialect_StatementTypeNotSupported);
            }
        }

        public abstract SqlQuery PageQuery(SqlQuery sqlQuery, long page, long resultsPerPage);

        protected string EscapeSql(string sql)
        {
            return this.OpenQuote + sql + this.CloseQuote;
        }

        protected string FormatParameter(int parameterPosition)
        {
            if (this.SqlParameterIncludesPosition)
            {
                return this.SqlParameter + ('p' + parameterPosition.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                return this.SqlParameter.ToString();
            }
        }

        protected string ResolveTableName(ObjectInfo objectInfo)
        {
            var schema = !string.IsNullOrEmpty(objectInfo.TableInfo.Schema)
                ? objectInfo.TableInfo.Schema
                : this.DefaultTableSchema;

            var tableNameBuilder = new StringBuilder();

            if (!string.IsNullOrEmpty(schema))
            {
                tableNameBuilder.Append(this.OpenQuote);
                tableNameBuilder.Append(schema);
                tableNameBuilder.Append(this.CloseQuote);
                tableNameBuilder.Append('.');
            }

            tableNameBuilder.Append(this.OpenQuote);
            tableNameBuilder.Append(objectInfo.TableInfo.Name);
            tableNameBuilder.Append(this.CloseQuote);

            return tableNameBuilder.ToString();
        }

        private StringBuilder CreateSql(StatementType statementType, ObjectInfo objectInfo)
        {
            var sqlBuilder = new StringBuilder();

            switch (statementType)
            {
                case StatementType.Delete:
                    sqlBuilder.Append("DELETE FROM " + this.ResolveTableName(objectInfo));

                    break;

                case StatementType.Insert:
                    sqlBuilder.Append("INSERT INTO " + this.ResolveTableName(objectInfo) + " (");

                    foreach (var column in objectInfo.TableInfo.Columns)
                    {
                        if (this.DatabaseGeneratedStrategies.Contains(objectInfo.TableInfo.IdentifierStrategy)
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
                    sqlBuilder.Append(" FROM " + this.ResolveTableName(objectInfo));

                    break;

                case StatementType.Update:
                    sqlBuilder.Append("UPDATE " + this.ResolveTableName(objectInfo) + " SET");

                    break;
            }

            return sqlBuilder;
        }
    }
}