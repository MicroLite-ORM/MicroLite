// -----------------------------------------------------------------------
// <copyright file="SqlBuilder.cs" company="MicroLite">
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
namespace MicroLite.Query
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// A helper class for building an <see cref="SqlQuery" />.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{innerSql}")]
    public abstract class SqlBuilder : IToSqlQuery
    {
        private readonly List<object> arguments = new List<object>();
        private readonly StringBuilder innerSql = new StringBuilder(capacity: 120);

        /// <summary>
        /// Gets or sets the SQL characters.
        /// </summary>
        /// <remarks>If no specific SqlCharacters are specified, SqlCharacters.Empty will be used.</remarks>
        public static SqlCharacters SqlCharacters
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the arguments currently added to the sql builder.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Allowed in this instance, we want to make use of AddRange.")]
        protected List<object> Arguments
        {
            get
            {
                return this.arguments;
            }
        }

        /// <summary>
        /// Gets the inner sql the sql builder.
        /// </summary>
        protected StringBuilder InnerSql
        {
            get
            {
                return this.innerSql;
            }
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
            return new StoredProcedureSqlBuilder(procedure);
        }

        /// <summary>
        /// Creates a new insert query builder.
        /// </summary>
        /// <returns>The next step in the fluent sql builder.</returns>
        public static IInto Insert()
        {
            var sqlCharacters = SqlBuilder.SqlCharacters ?? SqlCharacters.Empty;

            return new InsertSqlBuilder(sqlCharacters);
        }

        /// <summary>
        /// Creates a new query which selects the specified columns.
        /// </summary>
        /// <param name="columns">The columns to be included in the query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// Option 1, don't enter any column names, this is generally used if you want to just call a function such as Count.
        /// <code>
        /// var query = SqlBuilder.Select()...
        /// </code>
        /// </example>
        /// <example>
        /// Option 2, enter specific column names.
        /// <code>
        /// var query = SqlBuilder.Select("Name", "DoB")...
        /// </code>
        /// </example>
        /// <example>
        /// Option 3, enter * followed by a table name
        /// <code>
        /// var query = SqlBuilder.Select("*").From("Customers")...
        ///
        /// // SELECT * FROM Customers
        /// // will be generated
        /// </code>
        /// </example>
        /// <example>
        /// Option 4, enter * followed by a type in From, all mapped columns will be specified in the SQL.
        /// <code>
        /// var query = SqlBuilder.Select("*").From(typeof(Customer))...
        ///
        /// // SELECT CustomerId, Name, DoB FROM Customers
        /// // will be generated
        /// </code>
        /// </example>
        public static IFunctionOrFrom Select(params string[] columns)
        {
            var sqlCharacters = SqlBuilder.SqlCharacters ?? SqlCharacters.Empty;

            return new SelectSqlBuilder(sqlCharacters, columns);
        }

        /// <summary>
        /// Creates a new update query builder.
        /// </summary>
        /// <returns>The next step in the fluent sql builder.</returns>
        public static IUpdate Update()
        {
            var sqlCharacters = SqlBuilder.SqlCharacters ?? SqlCharacters.Empty;

            return new UpdateSqlBuilder(sqlCharacters);
        }

        /// <summary>
        /// Creates a <see cref="SqlQuery"/> from the values specified.
        /// </summary>
        /// <returns>The created <see cref="SqlQuery"/>.</returns>
        /// <remarks>This method is called to return an SqlQuery once query has been defined.</remarks>
        public virtual SqlQuery ToSqlQuery()
        {
            return new SqlQuery(this.innerSql.ToString(), this.arguments.ToArray());
        }
    }
}