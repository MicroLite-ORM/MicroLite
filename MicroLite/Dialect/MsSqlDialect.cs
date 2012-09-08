// -----------------------------------------------------------------------
// <copyright file="MsSqlDialect.cs" company="MicroLite">
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
    using System.Text.RegularExpressions;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;

    /// <summary>
    /// The implementation of <see cref="ISqlDialect"/> for MsSql server.
    /// </summary>
    internal sealed class MsSqlDialect : SqlDialect
    {
        private static readonly Regex orderByRegex = new Regex("(?<=ORDER BY)(.+)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        private static readonly Regex selectRegex = new Regex("SELECT(.+)(?=FROM)", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        private static readonly Regex tableNameRegex = new Regex("(?<=FROM)(.+)(?=WHERE)|(?<=FROM)(.+)(?=ORDER BY)|(?<=FROM)(.+)(?=WHERE)?|(?<=FROM)(.+)(?=ORDER BY)?", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        private static readonly Regex whereRegex = new Regex("(?<=WHERE)(.+)(?=ORDER BY)|(?<=WHERE)(.+)(?=ORDER BY)?", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline);
        private readonly string defaultTableSchema = "dbo";

        /// <summary>
        /// Initialises a new instance of the <see cref="MsSqlDialect"/> class.
        /// </summary>
        /// <remarks>Constructor needs to be public so that it can be instantiated by SqlDialectFactory.</remarks>
        public MsSqlDialect()
        {
        }

        public override SqlQuery CountQuery(SqlQuery sqlQuery)
        {
            var qualifiedTableName = tableNameRegex.Match(sqlQuery.CommandText).Groups[0].Value.Replace(Environment.NewLine, string.Empty).Trim();
            var whereMatchValue = whereRegex.Match(sqlQuery.CommandText).Groups[0].Value.Replace(Environment.NewLine, string.Empty).Trim();
            var whereClause = !string.IsNullOrEmpty(whereMatchValue) ? " WHERE " + whereMatchValue : string.Empty;

            var sqlBuilder = new StringBuilder("SELECT COUNT(*) FROM ");
            sqlBuilder.Append(qualifiedTableName);
            sqlBuilder.Append(whereClause);

            return new SqlQuery(sqlBuilder.ToString(), sqlQuery.Arguments.ToArray());
        }

        public override SqlQuery DeleteQuery(Type type, object identifier)
        {
            var objectInfo = ObjectInfo.For(type);

            var sqlBuilder = this.CreateSql(StatementType.Delete, objectInfo);
            sqlBuilder.AppendFormat(
                " WHERE [{0}].[{1}] = {2}",
                objectInfo.TableInfo.Name,
                objectInfo.TableInfo.IdentifierColumn,
                this.FormatParameter(0));

            return new SqlQuery(sqlBuilder.ToString(), new[] { identifier });
        }

        public override SqlQuery InsertQuery(object instance)
        {
            var objectInfo = ObjectInfo.For(instance.GetType());

            var values = new List<object>();

            var sqlBuilder = this.CreateSql(StatementType.Insert, objectInfo);
            sqlBuilder.Append(" VALUES (");

            foreach (var column in objectInfo.TableInfo.Columns)
            {
                if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.Identity
                    && column.ColumnName.Equals(objectInfo.TableInfo.IdentifierColumn))
                {
                    continue;
                }

                if (column.AllowInsert)
                {
                    sqlBuilder.Append(this.FormatParameter(values.Count) + ", ");

                    var propertyInfo = objectInfo.GetPropertyInfoForColumn(column.ColumnName);

                    var value = propertyInfo.GetValue(instance);

                    values.Add(value);
                }
            }

            sqlBuilder.Remove(sqlBuilder.Length - 2, 2);
            sqlBuilder.Append(")");

            return new SqlQuery(sqlBuilder.ToString(), values.ToArray());
        }

        public override SqlQuery Page(SqlQuery sqlQuery, long page, long resultsPerPage)
        {
            long fromRowNumber = ((page - 1) * resultsPerPage) + 1;
            long toRowNumber = (fromRowNumber - 1) + resultsPerPage;

            List<object> parameters = new List<object>();
            parameters.AddRange(sqlQuery.Arguments);
            parameters.Add(fromRowNumber);
            parameters.Add(toRowNumber);

            var selectStatement = selectRegex.Match(sqlQuery.CommandText).Groups[0].Value.Replace(Environment.NewLine, string.Empty).Trim();
            var qualifiedTableName = tableNameRegex.Match(sqlQuery.CommandText).Groups[0].Value.Replace(Environment.NewLine, string.Empty).Trim();
            var position = qualifiedTableName.LastIndexOf(".", StringComparison.OrdinalIgnoreCase) + 1;
            var tableName = position > 0 ? qualifiedTableName.Substring(position, qualifiedTableName.Length - position) : qualifiedTableName;

            var whereMatchValue = whereRegex.Match(sqlQuery.CommandText).Groups[0].Value.Replace(Environment.NewLine, string.Empty).Trim();
            var whereClause = !string.IsNullOrEmpty(whereMatchValue) ? " WHERE " + whereMatchValue : string.Empty;

            var orderByMatchValue = orderByRegex.Match(sqlQuery.CommandText).Groups[0].Value.Replace(Environment.NewLine, string.Empty).Trim();
            var orderByClause = "ORDER BY " + (!string.IsNullOrEmpty(orderByMatchValue) ? orderByMatchValue : "(SELECT NULL)");

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append(selectStatement);
            sqlBuilder.Append(" FROM");
            sqlBuilder.AppendFormat(CultureInfo.InvariantCulture, " ({0}, ROW_NUMBER() OVER({1}) AS RowNumber FROM {2}{3}) AS {4}", selectStatement, orderByClause, qualifiedTableName, whereClause, tableName);
            sqlBuilder.AppendFormat(CultureInfo.InvariantCulture, " WHERE (RowNumber >= {0} AND RowNumber <= {1})", this.FormatParameter(parameters.Count - 2), this.FormatParameter(parameters.Count - 1));

            return new SqlQuery(sqlBuilder.ToString(), parameters.ToArray());
        }

        public override SqlQuery SelectQuery(Type forType, object identifier)
        {
            var objectInfo = ObjectInfo.For(forType);

            var sqlBuilder = this.CreateSql(StatementType.Select, objectInfo);

            sqlBuilder.AppendFormat(
                " WHERE [{0}].[{1}] = {2}",
                objectInfo.TableInfo.Name,
                objectInfo.TableInfo.IdentifierColumn,
                this.FormatParameter(0));

            return new SqlQuery(sqlBuilder.ToString(), new[] { identifier });
        }

        public override SqlQuery UpdateQuery(object instance)
        {
            var objectInfo = ObjectInfo.For(instance.GetType());

            var values = new List<object>();

            var sqlBuilder = this.CreateSql(StatementType.Update, objectInfo);

            foreach (var column in objectInfo.TableInfo.Columns)
            {
                if (column.AllowUpdate
                    && !column.ColumnName.Equals(objectInfo.TableInfo.IdentifierColumn))
                {
                    sqlBuilder.AppendFormat(
                                " [{0}].[{1}] = {2},",
                                objectInfo.TableInfo.Name,
                                column.ColumnName,
                                this.FormatParameter(values.Count));

                    var propertyInfo = objectInfo.GetPropertyInfoForColumn(column.ColumnName);

                    var value = propertyInfo.GetValue(instance);

                    values.Add(value);
                }
            }

            sqlBuilder.Remove(sqlBuilder.Length - 1, 1);

            sqlBuilder.AppendFormat(
                " WHERE [{0}].[{1}] = {2}",
                objectInfo.TableInfo.Name,
                objectInfo.TableInfo.IdentifierColumn,
                this.FormatParameter(values.Count));

            var identifierPropertyInfo =
                objectInfo.GetPropertyInfoForColumn(objectInfo.TableInfo.IdentifierColumn);

            var identifierValue = identifierPropertyInfo.GetValue(instance);

            values.Add(identifierValue);

            return new SqlQuery(sqlBuilder.ToString(), values.ToArray());
        }

        private StringBuilder CreateSql(StatementType statementType, ObjectInfo objectInfo)
        {
            var sqlBuilder = new StringBuilder();

            switch (statementType)
            {
                case StatementType.Delete:
                    sqlBuilder.AppendFormat(
                        "DELETE FROM [{0}].[{1}]",
                        !string.IsNullOrEmpty(objectInfo.TableInfo.Schema) ? objectInfo.TableInfo.Schema : this.defaultTableSchema,
                        objectInfo.TableInfo.Name);

                    break;

                case StatementType.Insert:
                    sqlBuilder.AppendFormat(
                        "INSERT INTO [{0}].[{1}] (",
                        !string.IsNullOrEmpty(objectInfo.TableInfo.Schema) ? objectInfo.TableInfo.Schema : this.defaultTableSchema,
                        objectInfo.TableInfo.Name);

                    foreach (var column in objectInfo.TableInfo.Columns)
                    {
                        if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.Identity
                            && column.ColumnName.Equals(objectInfo.TableInfo.IdentifierColumn))
                        {
                            continue;
                        }

                        if (column.AllowInsert)
                        {
                            sqlBuilder.AppendFormat("[{0}].[{1}], ", objectInfo.TableInfo.Name, column.ColumnName);
                        }
                    }

                    sqlBuilder.Remove(sqlBuilder.Length - 2, 2);
                    sqlBuilder.Append(")");

                    break;

                case StatementType.Select:
                    sqlBuilder.Append("SELECT");

                    foreach (var column in objectInfo.TableInfo.Columns)
                    {
                        sqlBuilder.AppendFormat(" [{0}].[{1}],", objectInfo.TableInfo.Name, column.ColumnName);
                    }

                    sqlBuilder.Remove(sqlBuilder.Length - 1, 1);

                    sqlBuilder.AppendFormat(
                        " FROM [{0}].[{1}]",
                        !string.IsNullOrEmpty(objectInfo.TableInfo.Schema) ? objectInfo.TableInfo.Schema : this.defaultTableSchema,
                        objectInfo.TableInfo.Name);

                    break;

                case StatementType.Update:
                    sqlBuilder.AppendFormat(
                        "UPDATE [{0}].[{1}] SET",
                        !string.IsNullOrEmpty(objectInfo.TableInfo.Schema) ? objectInfo.TableInfo.Schema : this.defaultTableSchema,
                        objectInfo.TableInfo.Name);

                    break;
            }

            return sqlBuilder;
        }

        private string FormatParameter(int parameterPosition)
        {
            return "@p" + parameterPosition.ToString(CultureInfo.InvariantCulture);
        }
    }
}