// -----------------------------------------------------------------------
// <copyright file="MySqlDialect.cs" company="MicroLite">
// Copyright 2012 - 2013 Project Contributors
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
    using System.Text;

    /// <summary>
    /// The implementation of <see cref="ISqlDialect"/> for MySql server.
    /// </summary>
    internal sealed class MySqlDialect : SqlDialect
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="MySqlDialect"/> class.
        /// </summary>
        /// <remarks>Constructor needs to be public so that it can be instantiated by SqlDialectFactory.</remarks>
        public MySqlDialect()
            : base(SqlCharacters.MySql)
        {
        }

        /// <summary>
        /// Gets the select identity string.
        /// </summary>
        protected override string SelectIdentityString
        {
            get
            {
                return "SELECT LAST_INSERT_ID()";
            }
        }

        public override SqlQuery PageQuery(SqlQuery sqlQuery, PagingOptions pagingOptions)
        {
            var arguments = new object[sqlQuery.Arguments.Count + 2];
            sqlQuery.Arguments.CopyTo(arguments, 0);
            arguments[arguments.Length - 2] = pagingOptions.Offset;
            arguments[arguments.Length - 1] = pagingOptions.Count;

            var stringBuilder = new StringBuilder(sqlQuery.CommandText)
                .Replace(Environment.NewLine, string.Empty)
                .Append(" LIMIT ")
                .Append(this.SqlCharacters.GetParameterName(arguments.Length - 2))
                .Append(',')
                .Append(this.SqlCharacters.GetParameterName(arguments.Length - 1));

            return new SqlQuery(stringBuilder.ToString(), arguments);
        }
    }
}