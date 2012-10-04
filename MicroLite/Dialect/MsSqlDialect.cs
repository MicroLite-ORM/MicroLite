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
    using System.Globalization;
    using System.Text;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;

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
        {
        }

        protected override string EscapeSql(string sql)
        {
            return "[" + sql + "]";
        }

        protected override string FormatParameter(int parameterPosition)
        {
            return "@p" + parameterPosition.ToString(CultureInfo.InvariantCulture);
        }

        protected override string PageCommandText(string commandText, int argumentCount)
        {
            var selectStatement = SqlUtil.ReadSelectList(commandText);
            var qualifiedTableName = SqlUtil.ReadTableName(commandText);
            var position = qualifiedTableName.LastIndexOf(".", StringComparison.OrdinalIgnoreCase) + 1;
            var tableName = position > 0 ? qualifiedTableName.Substring(position, qualifiedTableName.Length - position) : qualifiedTableName;

            var whereValue = SqlUtil.ReadWhereClause(commandText);
            var whereClause = !string.IsNullOrEmpty(whereValue) ? " WHERE " + whereValue : string.Empty;

            var orderByValue = SqlUtil.ReadOrderBy(commandText);
            var orderByClause = "ORDER BY " + (!string.IsNullOrEmpty(orderByValue) ? orderByValue : "(SELECT NULL)");

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append(selectStatement);
            sqlBuilder.Append(" FROM");
            sqlBuilder.AppendFormat(CultureInfo.InvariantCulture, " ({0}, ROW_NUMBER() OVER({1}) AS RowNumber FROM {2}{3}) AS {4}", selectStatement, orderByClause, qualifiedTableName, whereClause, tableName);
            sqlBuilder.AppendFormat(CultureInfo.InvariantCulture, " WHERE (RowNumber >= {0} AND RowNumber <= {1})", this.FormatParameter(argumentCount - 2), this.FormatParameter(argumentCount - 1));

            return sqlBuilder.ToString();
        }

        protected override string ResolveTableName(ObjectInfo objectInfo)
        {
            return "[{0}].[{1}]".FormatWith(string.IsNullOrEmpty(objectInfo.TableInfo.Schema) ? "dbo" : objectInfo.TableInfo.Schema, objectInfo.TableInfo.Name);
        }
    }
}