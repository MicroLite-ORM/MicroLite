// -----------------------------------------------------------------------
// <copyright file="SQLiteDialect.cs" company="MicroLite">
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
    using MicroLite.Mapping;

    /// <summary>
    /// The implementation of <see cref="ISqlDialect"/> for SQLite.
    /// </summary>
    internal sealed class SQLiteDialect : SqlDialect
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="SQLiteDialect"/> class.
        /// </summary>
        /// <remarks>Constructor needs to be public so that it can be instantiated by SqlDialectFactory.</remarks>
        public SQLiteDialect()
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
            var sqlBuilder = new StringBuilder(commandText);
            sqlBuilder.Replace(Environment.NewLine, string.Empty);
            sqlBuilder.Append(SqlUtil.ReNumberParameters(" LIMIT @p0,@p1", argumentCount));

            return sqlBuilder.ToString();
        }

        protected override string ResolveTableName(ObjectInfo objectInfo)
        {
            return "[" + objectInfo.TableInfo.Name + "]";
        }
    }
}