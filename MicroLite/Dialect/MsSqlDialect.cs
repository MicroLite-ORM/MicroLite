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
    using System.Text;
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

        /// <summary>
        /// Gets the close quote character.
        /// </summary>
        protected override char CloseQuote
        {
            get
            {
                return ']';
            }
        }

        /// <summary>
        /// Gets the database generated identifier strategies.
        /// </summary>
        protected override IdentifierStrategy[] DatabaseGeneratedStrategies
        {
            get
            {
                return new[] { IdentifierStrategy.Identity };
            }
        }

        /// <summary>
        /// Gets the open quote character.
        /// </summary>
        protected override char OpenQuote
        {
            get
            {
                return '[';
            }
        }

        /// <summary>
        /// Gets the select identity string.
        /// </summary>
        protected override string SelectIdentityString
        {
            get
            {
                return "SELECT SCOPE_IDENTITY()";
            }
        }

        /// <summary>
        /// Gets the SQL parameter.
        /// </summary>
        protected override char SqlParameter
        {
            get
            {
                return '@';
            }
        }

        /// <summary>
        /// Gets a value indicating whether SQL parameters are named.
        /// </summary>
        protected override bool SupportsNamedParameters
        {
            get
            {
                return true;
            }
        }

        public override SqlQuery PageQuery(SqlQuery sqlQuery, PagingOptions pagingOptions)
        {
            long fromRowNumber = pagingOptions.Offset + 1;
            long toRowNumber = pagingOptions.Offset + pagingOptions.Count;

            List<object> arguments = new List<object>();
            arguments.AddRange(sqlQuery.Arguments);
            arguments.Add(fromRowNumber);
            arguments.Add(toRowNumber);

            var selectStatement = this.ReadSelectList(sqlQuery.CommandText);
            var qualifiedTableName = this.ReadTableName(sqlQuery.CommandText);
            var position = qualifiedTableName.LastIndexOf(".", StringComparison.OrdinalIgnoreCase) + 1;
            var tableName = position > 0 ? qualifiedTableName.Substring(position, qualifiedTableName.Length - position) : qualifiedTableName;

            var whereValue = this.ReadWhereClause(sqlQuery.CommandText);
            var whereClause = !string.IsNullOrEmpty(whereValue) ? " WHERE " + whereValue : string.Empty;

            var orderByValue = this.ReadOrderBy(sqlQuery.CommandText);
            var orderByClause = "ORDER BY " + (!string.IsNullOrEmpty(orderByValue) ? orderByValue : "(SELECT NULL)");

            var sqlBuilder = new StringBuilder();
            sqlBuilder.Append(selectStatement);
            sqlBuilder.Append(" FROM");
            sqlBuilder.AppendFormat(CultureInfo.InvariantCulture, " ({0}, ROW_NUMBER() OVER({1}) AS RowNumber FROM {2}{3}) AS {4}", selectStatement, orderByClause, qualifiedTableName, whereClause, tableName);
            sqlBuilder.AppendFormat(CultureInfo.InvariantCulture, " WHERE (RowNumber >= {0} AND RowNumber <= {1})", this.FormatParameter(arguments.Count - 2), this.FormatParameter(arguments.Count - 1));

            return new SqlQuery(sqlBuilder.ToString(), arguments.ToArray());
        }

        protected override string GetCommandText(string commandText)
        {
            if (commandText.StartsWith("EXEC", StringComparison.OrdinalIgnoreCase) && !commandText.Contains(this.SelectSeparator.ToString()))
            {
                var firstParameterPosition = SqlUtil.GetFirstParameterPosition(commandText);

                if (firstParameterPosition > 4)
                {
                    return commandText.Substring(4, firstParameterPosition - 4).Trim();
                }
                else
                {
                    return commandText.Substring(4, commandText.Length - 4).Trim();
                }
            }

            return base.GetCommandText(commandText);
        }

        protected override CommandType GetCommandType(string commandText)
        {
            if (commandText.StartsWith("EXEC", StringComparison.OrdinalIgnoreCase) && !commandText.Contains(this.SelectSeparator.ToString()))
            {
                return CommandType.StoredProcedure;
            }

            return base.GetCommandType(commandText);
        }
    }
}