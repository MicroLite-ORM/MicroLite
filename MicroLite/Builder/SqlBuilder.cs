// -----------------------------------------------------------------------
// <copyright file="SqlBuilder.cs" company="MicroLite">
// Copyright 2012 - 2016 Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Builder
{
    using MicroLite.Builder.Syntax;
    using MicroLite.Builder.Syntax.Read;
    using MicroLite.Builder.Syntax.Write;
    using MicroLite.Characters;

    /// <summary>
    /// A helper class for building an <see cref="SqlQuery" />.
    /// </summary>
    public static class SqlBuilder
    {
        /// <summary>
        /// Creates a new delete query builder.
        /// </summary>
        /// <returns>The next step in the fluent sql builder.</returns>
        public static IDeleteFrom Delete()
        {
            return new DeleteSqlBuilder(SqlCharacters.Current);
        }

        /// <summary>
        /// Species the name of the procedure to be executed.
        /// </summary>
        /// <param name="procedure">The name of the stored procedure.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <remarks>If the stored procedure has no parameters, call .ToSqlQuery() otherwise add the parameters (see the WithParameter method).</remarks>
        /// <example>
        /// <code>
        /// var query = SqlBuilder.Execute("CustomersOver50").ToSqlQuery();
        /// </code>
        /// </example>
        public static IWithParameter Execute(string procedure)
        {
            return new StoredProcedureSqlBuilder(SqlCharacters.Current, procedure);
        }

        /// <summary>
        /// Creates a new insert query builder.
        /// </summary>
        /// <returns>The next step in the fluent sql builder.</returns>
        public static IInsertIntoTable Insert()
        {
            return new InsertSqlBuilder(SqlCharacters.Current);
        }

        /// <summary>
        /// Creates a new select query with no specified columns.
        /// </summary>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// Don't enter any column names, this is generally used if you want to just call a function such as Count.
        /// <code>
        /// var query = SqlBuilder.Select()...
        /// </code>
        /// </example>
        public static IFunctionOrFrom Select()
        {
            return new SelectSqlBuilder(SqlCharacters.Current, (string)null);
        }

        /// <summary>
        /// Creates a new select query with a single specified column or '*'.
        /// </summary>
        /// <param name="column">The column (or wildcard *) to be included in the query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// Option 1, enter * followed by a table name
        /// <code>
        /// var query = SqlBuilder.Select("*").From("Customers")...
        ///
        /// // SELECT * FROM Customers
        /// // will be generated
        /// </code>
        /// </example>
        /// <example>
        /// Option 2, enter * followed by a type in From, all mapped columns will be specified in the SQL.
        /// <code>
        /// var query = SqlBuilder.Select("*").From(typeof(Customer))...
        ///
        /// // SELECT CustomerId, Name, DoB FROM Customers
        /// // will be generated
        /// </code>
        /// </example>
        public static IFunctionOrFrom Select(string column)
        {
            var sqlCharacters = SqlCharacters.Current;

            if (column == sqlCharacters.SelectWildcard)
            {
                return new SelectSqlBuilder(sqlCharacters);
            }

            return new SelectSqlBuilder(sqlCharacters, column);
        }

        /// <summary>
        /// Creates a new query which selects the specified columns.
        /// </summary>
        /// <param name="columns">The columns to be included in the query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// Enter specific column names.
        /// <code>
        /// var query = SqlBuilder.Select("Name", "DoB")...
        /// </code>
        /// </example>
        public static IFunctionOrFrom Select(params string[] columns)
        {
            return new SelectSqlBuilder(SqlCharacters.Current, columns);
        }

        /// <summary>
        /// Creates a new update query builder.
        /// </summary>
        /// <returns>The next step in the fluent sql builder.</returns>
        public static IUpdate Update()
        {
            return new UpdateSqlBuilder(SqlCharacters.Current);
        }
    }
}