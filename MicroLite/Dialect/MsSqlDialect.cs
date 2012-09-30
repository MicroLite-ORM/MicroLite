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
    using System.Globalization;
    using System.Text;

    /// <summary>
    /// The implementation of <see cref="ISqlDialect"/> for MsSql server.
    /// </summary>
    internal sealed class MsSqlDialect : SqlDialect
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MsSqlDialect"/> class.
        /// </summary>
        /// <remarks>Constructor needs to be public so that it can be instantiated by SqlDialectFactory.</remarks>
        public MsSqlDialect()
            : base("dbo")
        {
        }

        public override SqlQuery PageQuery(SqlQuery sqlQuery, long page, long resultsPerPage)
        {
            long fromRowNumber = ((page - 1) * resultsPerPage) + 1;
            long toRowNumber = (fromRowNumber - 1) + resultsPerPage;

            List<object> parameters = new List<object>();
            parameters.AddRange(sqlQuery.Arguments);
            parameters.Add(fromRowNumber);
            parameters.Add(toRowNumber);

            var selectStatement = SqlUtil.ReadSelectList(sqlQuery.CommandText);
            var qualifiedTableName = SqlUtil.ReadTableName(sqlQuery.CommandText);
            var position = qualifiedTableName.LastIndexOf(".", StringComparison.OrdinalIgnoreCase) + 1;
            var tableName = position > 0 ? qualifiedTableName.Substring(position, qualifiedTableName.Length - position) : qualifiedTableName;

            var whereValue = SqlUtil.ReadWhereClause(sqlQuery.CommandText);
            var whereClause = !string.IsNullOrEmpty(whereValue) ? " WHERE " + whereValue : string.Empty;

            var orderByValue = SqlUtil.ReadOrderBy(sqlQuery.CommandText);
            var orderByClause = "ORDER BY " + (!string.IsNullOrEmpty(orderByValue) ? orderByValue : "(SELECT NULL)");

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append(selectStatement);
            sqlBuilder.Append(" FROM");
            sqlBuilder.AppendFormat(CultureInfo.InvariantCulture, " ({0}, ROW_NUMBER() OVER({1}) AS RowNumber FROM {2}{3}) AS {4}", selectStatement, orderByClause, qualifiedTableName, whereClause, tableName);
            sqlBuilder.AppendFormat(CultureInfo.InvariantCulture, " WHERE (RowNumber >= {0} AND RowNumber <= {1})", this.FormatParameter(parameters.Count - 2), this.FormatParameter(parameters.Count - 1));

            return new SqlQuery(sqlBuilder.ToString(), parameters.ToArray());
        }

        ////public override SqlQuery UpdateQuery(object instance)
        ////{
        ////    var objectInfo = ObjectInfo.For(instance.GetType());

        ////    var values = new List<object>();

        ////    var sqlBuilder = this.CreateSql(StatementType.Update, objectInfo);

        ////    foreach (var column in objectInfo.TableInfo.Columns)
        ////    {
        ////        if (column.AllowUpdate
        ////            && !column.ColumnName.Equals(objectInfo.TableInfo.IdentifierColumn))
        ////        {
        ////            sqlBuilder.AppendFormat(
        ////                        " [{0}].[{1}] = {2},",
        ////                        objectInfo.TableInfo.Name,
        ////                        column.ColumnName,
        ////                        this.FormatParameter(values.Count));

        ////            var propertyInfo = objectInfo.GetPropertyInfoForColumn(column.ColumnName);

        ////            var value = propertyInfo.GetValue(instance);

        ////            values.Add(value);
        ////        }
        ////    }

        ////    sqlBuilder.Remove(sqlBuilder.Length - 1, 1);

        ////    sqlBuilder.AppendFormat(
        ////        " WHERE [{0}].[{1}] = {2}",
        ////        objectInfo.TableInfo.Name,
        ////        objectInfo.TableInfo.IdentifierColumn,
        ////        this.FormatParameter(values.Count));

        ////    var identifierPropertyInfo =
        ////        objectInfo.GetPropertyInfoForColumn(objectInfo.TableInfo.IdentifierColumn);

        ////    var identifierValue = identifierPropertyInfo.GetValue(instance);

        ////    values.Add(identifierValue);

        ////    return new SqlQuery(sqlBuilder.ToString(), values.ToArray());
        ////}

        protected override string EscapeSql(string sql)
        {
            return "[" + sql + "]";
        }

        protected override string FormatParameter(int parameterPosition)
        {
            return "@p" + parameterPosition.ToString(CultureInfo.InvariantCulture);
        }
    }
}