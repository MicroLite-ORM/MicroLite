// -----------------------------------------------------------------------
// <copyright file="SqlBuilder.cs" company="MicroLite">
// Copyright 2012 - 2014 Project Contributors
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
    using System.Collections.Generic;
    using System.Text;
    using MicroLite.Mapping;

    /// <summary>
    /// A helper class for building an <see cref="SqlQuery" />.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{innerSql}")]
    public abstract class SqlBuilder : IToSqlQuery
    {
        private readonly List<object> arguments = new List<object>();
        private readonly StringBuilder innerSql = new StringBuilder(capacity: 128);
        private readonly SqlCharacters sqlCharacters;

        /// <summary>
        /// Initialises a new instance of the <see cref="SqlBuilder"/> class.
        /// </summary>
        /// <param name="sqlCharacters">The SQL characters.</param>
        protected SqlBuilder(SqlCharacters sqlCharacters)
        {
            this.sqlCharacters = sqlCharacters;
        }

        /// <summary>
        /// Gets or sets the SQL characters.
        /// </summary>
        /// <remarks>If no specific SqlCharacters are specified, SqlCharacters.Empty will be used.</remarks>
        public static SqlCharacters DefaultSqlCharacters
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
        /// Gets the SQL characters.
        /// </summary>
        protected SqlCharacters SqlCharacters
        {
            get
            {
                return this.sqlCharacters;
            }
        }

        /// <summary>
        /// Creates a new delete query builder.
        /// </summary>
        /// <returns>The next step in the fluent sql builder.</returns>
        public static IDeleteFrom Delete()
        {
            var sqlCharacters = SqlBuilder.DefaultSqlCharacters ?? SqlCharacters.Empty;

            return new DeleteSqlBuilder(sqlCharacters);
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
            var sqlCharacters = SqlBuilder.DefaultSqlCharacters ?? SqlCharacters.Empty;

            return new InsertSqlBuilder(sqlCharacters);
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
            var sqlCharacters = SqlBuilder.DefaultSqlCharacters ?? SqlCharacters.Empty;

            return new SelectSqlBuilder(sqlCharacters, null);
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
            var sqlCharacters = SqlBuilder.DefaultSqlCharacters ?? SqlCharacters.Empty;

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
            var sqlCharacters = SqlBuilder.DefaultSqlCharacters ?? SqlCharacters.Empty;

            return new SelectSqlBuilder(sqlCharacters, columns);
        }

        /// <summary>
        /// Creates a new update query builder.
        /// </summary>
        /// <returns>The next step in the fluent sql builder.</returns>
        public static IUpdate Update()
        {
            var sqlCharacters = SqlBuilder.DefaultSqlCharacters ?? SqlCharacters.Empty;

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

        /// <summary>
        /// Appends the table name to the inner sql.
        /// </summary>
        /// <param name="objectInfo">The object information.</param>
        protected void AppendTableName(IObjectInfo objectInfo)
        {
            if (!string.IsNullOrEmpty(objectInfo.TableInfo.Schema))
            {
                this.InnerSql.Append(this.sqlCharacters.LeftDelimiter)
                    .Append(objectInfo.TableInfo.Schema)
                    .Append(this.sqlCharacters.RightDelimiter)
                    .Append('.');
            }

            this.AppendTableName(objectInfo.TableInfo.Name);
        }

        /// <summary>
        /// Appends the table name to the inner sql.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        protected void AppendTableName(string tableName)
        {
            if (this.sqlCharacters.IsEscaped(tableName))
            {
                this.innerSql.Append(tableName);
            }
            else
            {
                this.InnerSql.Append(this.sqlCharacters.LeftDelimiter)
                    .Append(tableName)
                    .Append(this.sqlCharacters.RightDelimiter);
            }
        }
    }
}