// -----------------------------------------------------------------------
// <copyright file="SqlBuilder.cs" company="MicroLite">
// Copyright 2012 - 2013 Trevor Pilley
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
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using MicroLite.Dialect;
    using MicroLite.Mapping;

    /// <summary>
    /// A helper class for building an <see cref="SqlQuery" />.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{innerSql}")]
    public sealed class SqlBuilder : IFrom, IFunctionOrFrom, IWhereOrOrderBy, IAndOrOrderBy, IGroupBy, IOrderBy, IToSqlQuery, IWithParameter, IWhereSingleColumn, IHavingOrOrderBy
    {
        private readonly List<object> arguments = new List<object>();
        private readonly StringBuilder innerSql;
        private readonly SqlCharacters sqlCharacters;
        private bool addedOrder = false;
        private bool addedWhere = false;
        private string operand;
        private string whereColumnName;

        private SqlBuilder(StringBuilder startingSql, SqlCharacters sqlCharacters)
        {
            this.sqlCharacters = sqlCharacters;
            this.innerSql = startingSql;
        }

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
            var sqlCharacters = SqlBuilder.SqlCharacters ?? SqlCharacters.Empty;

            var startingSql = new StringBuilder(capacity: 120);
            startingSql.Append("EXEC ");
            startingSql.Append(procedure);

            return new SqlBuilder(startingSql, sqlCharacters);
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

            var startingSql = new StringBuilder(capacity: 120);
            startingSql.Append("SELECT");

            if (columns != null && columns.Length > 0)
            {
                startingSql.Append(" ");

                for (int i = 0; i < columns.Length; i++)
                {
                    if (columns[i] == "*")
                    {
                        startingSql.Append("*");
                    }
                    else
                    {
                        if (i > 0)
                        {
                            startingSql.Append(", ");
                        }

                        startingSql.Append(sqlCharacters.EscapeSql(columns[i]));
                    }
                }
            }

            return new SqlBuilder(startingSql, sqlCharacters);
        }

        /// <summary>
        /// Adds a column as an AND to the where clause of the query.
        /// </summary>
        /// <param name="columnName">The column name to use in the where clause.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// This method allows us to specify a column to be used with the BETWEEN or IN keywords which is added to the query as an AND.
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("LastName = @p0", "Smith")
        ///     .AndWhere("DateRegistered")
        ///     ...
        /// </code>
        /// </example>
        public IWhereSingleColumn AndWhere(string columnName)
        {
            this.operand = " AND";
            this.whereColumnName = columnName;

            return this;
        }

        /// <summary>
        /// Adds a predicate as an AND to the where clause of the query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The args.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// Adds the an additional predicate to the query as an AND.
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("FirstName = @p0", "John")
        ///     .AndWhere("LastName = @p0", "Smith") // Each time, the parameter number relates to the individual method call.
        ///     .ToSqlQuery();
        /// </code>
        /// Would generate SELECT {Columns} FROM Customers WHERE (FirstName = @p0) AND (LastName = @p1)
        /// @p0 would be John
        /// @p1 would be Smith
        /// </example>
        /// <example>
        /// Additionally, we could construct the query as follows:
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("FirstName = @p0 AND LastName = @p1", "John", "Smith")
        ///     .ToSqlQuery();
        /// </code>
        /// Would generate SELECT {Columns} FROM Customers WHERE (FirstName = @p0 AND LastName = @p1)
        /// @p0 would be John
        /// @p1 would be Smith
        /// </example>
        public IAndOrOrderBy AndWhere(string predicate, params object[] args)
        {
            this.AppendPredicate(" AND ({0})", predicate, args);

            return this;
        }

        /// <summary>
        /// Selects the average value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// A simple query to find the average order total for a customer. By default, the result will be aliased as the column name.
        /// <code>
        /// var sqlQuery = SqlBuilder
        ///     .Select()
        ///     .Average("Total")
        ///     .From(typeof(Invoice))
        ///     .Where("CustomerId = @p0", 1022)
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT AVG(Total) AS Total FROM Invoices WHERE (CustomerId = @p0)
        /// </example>
        public IFunctionOrFrom Average(string columnName)
        {
            return this.Average(columnName, columnName);
        }

        /// <summary>
        /// Selects the average value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <param name="columnAlias">The alias in the result set for the calculated column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// A simple query to find the average order total for a customer. We can specify a custom column alias if required.
        /// <code>
        /// var sqlQuery = SqlBuilder
        ///     .Select()
        ///     .Average("Total", columnAlias: "AverageTotal")
        ///     .From(typeof(Invoice))
        ///     .Where("CustomerId = @p0", 1022)
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT AVG(Total) AS AverageTotal FROM Invoices WHERE (CustomerId = @p0)
        /// </example>
        public IFunctionOrFrom Average(string columnName, string columnAlias)
        {
            if (this.innerSql.Length > 6)
            {
                this.innerSql.Append(",");
            }

            this.innerSql.AppendFormat(" AVG({0}) AS {1}", this.sqlCharacters.EscapeSql(columnName), columnAlias);

            return this;
        }

        /// <summary>
        /// Uses the specified arguments to filter the column.
        /// </summary>
        /// <param name="lower">The inclusive lower value.</param>
        /// <param name="upper">The inclusive upper value.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// This method allows us to specify that a column is filtered with the results being between the 2 specified values.
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("DateRegistered")
        ///     .Between(new DateTime(2000, 1, 1), new DateTime(2009, 12, 31))
        ///     .ToSqlQuery();
        /// </code>
        /// For MsSqlCharacters, will generate SELECT {Columns} FROM Customers WHERE (DateRegistered BETWEEN @p0 AND @p1)
        /// </example>
        public IAndOrOrderBy Between(object lower, object upper)
        {
            if (!string.IsNullOrEmpty(this.operand))
            {
                this.innerSql.Append(this.operand);
            }

            this.AppendPredicate(" (" + this.whereColumnName + " BETWEEN {0})", this.sqlCharacters.GetParameterName(0) + " AND " + this.sqlCharacters.GetParameterName(1), new[] { lower, upper });

            return this;
        }

        /// <summary>
        /// Selects the number of records which match the specified filter.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// A simple query to find the number of customers. By default, the result will be aliased as the column name.
        /// <code>
        /// var sqlQuery = SqlBuilder
        ///     .Select()
        ///     .Count("CustomerId")
        ///     .From(typeof(Customer))
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT COUNT(CustomerId) AS CustomerId FROM Customers
        /// </example>
        public IFunctionOrFrom Count(string columnName)
        {
            return this.Count(columnName, columnName);
        }

        /// <summary>
        /// Selects the number of records which match the specified filter.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <param name="columnAlias">The alias in the result set for the calculated column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// A simple query to find the number of customers. We can specify a custom column alias if required.
        /// <code>
        /// var sqlQuery = SqlBuilder
        ///     .Select()
        ///     .Count("CustomerId", columnAlias: "CustomerCount")
        ///     .From(typeof(Customer))
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT COUNT(CustomerId) AS CustomerCount FROM Customers
        /// </example>
        public IFunctionOrFrom Count(string columnName, string columnAlias)
        {
            if (this.innerSql.Length > 6)
            {
                this.innerSql.Append(",");
            }

            this.innerSql.AppendFormat(" COUNT({0}) AS {1}", this.sqlCharacters.EscapeSql(columnName), columnAlias);

            return this;
        }

        /// <summary>
        /// Specifies the table to perform the query against.
        /// </summary>
        /// <param name="table">The name of the table.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// <code>
        /// var query = SqlBuilder.Select("Col1", "Col2").From("Customers")... // Add remainder of query
        /// </code>
        /// </example>
        public IWhereOrOrderBy From(string table)
        {
            this.innerSql.Append(" FROM ");
            this.innerSql.Append(this.sqlCharacters.EscapeSql(table));

            return this;
        }

        /// <summary>
        /// Specifies the type to perform the query against.
        /// </summary>
        /// <param name="forType">The type of object the query relates to.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// If the select criteria is * then all mapped columns will be used in the select list instead, otherwise the specified columns will be used.
        /// <code>
        /// var query = SqlBuilder.Select("Col1", "Col2").From(typeof(Customer))... // Add remainder of query
        /// </code>
        /// </example>
        public IWhereOrOrderBy From(Type forType)
        {
            var objectInfo = ObjectInfo.For(forType);

            IFrom select = this;

            if (this.innerSql.Length > 7 && this.innerSql[7].CompareTo('*') == 0)
            {
                select = Select(objectInfo.TableInfo.Columns.Select(c => c.ColumnName).ToArray());
            }

            return !string.IsNullOrEmpty(objectInfo.TableInfo.Schema)
                ? select.From(objectInfo.TableInfo.Schema + "." + objectInfo.TableInfo.Name)
                : select.From(objectInfo.TableInfo.Name);
        }

        /// <summary>
        /// Groups the results of the query by the specified columns.
        /// </summary>
        /// <param name="columns">The columns to group by.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// <code>
        /// var sqlQuery = SqlBuilder
        ///     .Select("CustomerId")
        ///     .Max("Total")
        ///     .From(typeof(Invoice))
        ///     .GroupBy("CustomerId")
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT CustomerId, MAX(Total) AS Total FROM Invoices GROUP BY CustomerId
        /// </example>
        public IHavingOrOrderBy GroupBy(params string[] columns)
        {
            this.innerSql.Append(" GROUP BY ");

            for (int i = 0; i < columns.Length; i++)
            {
                if (i > 0)
                {
                    this.innerSql.Append(", ");
                }

                this.innerSql.Append(this.sqlCharacters.EscapeSql(columns[i]));
            }

            return this;
        }

        /// <summary>
        /// Specifies the having clause for the query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="value">The argument value.</param>
        /// <returns>
        /// The next step in the fluent sql builder.
        /// </returns>
        /// <example>
        /// <code>
        /// var sqlQuery = SqlBuilder
        ///     .Select("CustomerId")
        ///     .Max("Total")
        ///     .From(typeof(Invoice))
        ///     .GroupBy("CustomerId")
        ///     .Having("MAX(Total) > @p0", 10000M)
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT CustomerId, MAX(Total) AS Total FROM Invoices GROUP BY CustomerId HAVING MAX(Total) > @p0
        /// </example>
        public IOrderBy Having(string predicate, object value)
        {
            this.AppendPredicate(" HAVING {0}", predicate, value);

            return this;
        }

        /// <summary>
        /// Uses the specified arguments to filter the column.
        /// </summary>
        /// <param name="args">The arguments to filter the column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// This method allows us to specify that a column is filtered with the results being in the specified values.
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("Column1")
        ///     .In("X", "Y", "Z")
        ///     .ToSqlQuery();
        /// </code>
        /// For MsSqlCharacters, will generate SELECT {Columns} FROM Customers WHERE (Column1 IN (@p0, @p1, @p2))
        /// </example>
        public IAndOrOrderBy In(params object[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (!string.IsNullOrEmpty(this.operand))
            {
                this.innerSql.Append(this.operand);
            }

#if NET_3_5
            var predicate = string.Join(", ", Enumerable.Range(0, args.Length).Select(i => this.sqlCharacters.GetParameterName(i)).ToArray());
#else
            var predicate = string.Join(", ", Enumerable.Range(0, args.Length).Select(i => this.sqlCharacters.GetParameterName(i)));
#endif
            this.AppendPredicate(" (" + this.whereColumnName + " IN ({0}))", predicate, args);

            return this;
        }

        /// <summary>
        /// Uses the specified SqlQuery as a sub query to filter the column.
        /// </summary>
        /// <param name="subQuery">The sub query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// This method allows us to specify that a column is filtered with the results being in the specified values.
        /// <code>
        /// var customerQuery = SqlBuilder
        ///     .Select("CustomerId")
        ///     .From(typeof(Customer))
        ///     .Where("Age > @p0", 40)
        ///     .ToSqlQuery();
        ///
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Invoice))
        ///     .Where("CustomerId")
        ///     .In(customerQuery)
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT {Columns} FROM Invoices WHERE (CustomerId IN (SELECT CustomerId FROM Customers WHERE Age > @p0))
        /// </example>
        public IAndOrOrderBy In(SqlQuery subQuery)
        {
            if (subQuery == null)
            {
                throw new ArgumentNullException("subQuery");
            }

            if (!string.IsNullOrEmpty(this.operand))
            {
                this.innerSql.Append(this.operand);
            }

            this.AppendPredicate(" (" + this.whereColumnName + " IN ({0}))", subQuery.CommandText, subQuery.Arguments.ToArray());

            return this;
        }

        /// <summary>
        /// Uses the specified argument to filter the column.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// This method allows us to specify that a column is filtered with the results being equal to the specified comparisonValue.
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("DateRegistered")
        ///     .IsEqualTo(new DateTime(2000, 1, 1))
        ///     .ToSqlQuery();
        /// </code>
        /// For MsSqlCharacters, will generate SELECT {Columns} FROM Customers WHERE (DateRegistered = @p0)
        /// </example>
        public IAndOrOrderBy IsEqualTo(object comparisonValue)
        {
            if (!string.IsNullOrEmpty(this.operand))
            {
                this.innerSql.Append(this.operand);
            }

            this.AppendPredicate(" (" + this.whereColumnName + " = {0})", this.sqlCharacters.GetParameterName(0), comparisonValue);

            return this;
        }

        /// <summary>
        /// Uses the specified argument to filter the column.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// This method allows us to specify that a column is filtered with the results being greater than the specified comparisonValue.
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("DateRegistered")
        ///     .IsGreaterThan(new DateTime(2000, 1, 1))
        ///     .ToSqlQuery();
        /// </code>
        /// For MsSqlCharacters, will generate SELECT {Columns} FROM Customers WHERE (DateRegistered > @p0)
        /// </example>
        public IAndOrOrderBy IsGreaterThan(object comparisonValue)
        {
            if (!string.IsNullOrEmpty(this.operand))
            {
                this.innerSql.Append(this.operand);
            }

            this.AppendPredicate(" (" + this.whereColumnName + " > {0})", this.sqlCharacters.GetParameterName(0), comparisonValue);

            return this;
        }

        /// <summary>
        /// Uses the specified argument to filter the column.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// This method allows us to specify that a column is filtered with the results being greater than or equal to the specified comparisonValue.
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("DateRegistered")
        ///     .IsGreaterThanOrEqualTo(new DateTime(2000, 1, 1))
        ///     .ToSqlQuery();
        /// </code>
        /// For MsSqlCharacters, will generate SELECT {Columns} FROM Customers WHERE (DateRegistered >= @p0)
        /// </example>
        public IAndOrOrderBy IsGreaterThanOrEqualTo(object comparisonValue)
        {
            if (!string.IsNullOrEmpty(this.operand))
            {
                this.innerSql.Append(this.operand);
            }

            this.AppendPredicate(" (" + this.whereColumnName + " >= {0})", this.sqlCharacters.GetParameterName(0), comparisonValue);

            return this;
        }

        /// <summary>
        /// Uses the specified argument to filter the column.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// This method allows us to specify that a column is filtered with the results being less than the specified comparisonValue.
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("DateRegistered")
        ///     .IsLessThan(new DateTime(2000, 1, 1))
        ///     .ToSqlQuery();
        /// </code>
        /// For MsSqlCharacters, will generate SELECT {Columns} FROM Customers WHERE (DateRegistered <!--<--> @p0)
        /// </example>
        public IAndOrOrderBy IsLessThan(object comparisonValue)
        {
            if (!string.IsNullOrEmpty(this.operand))
            {
                this.innerSql.Append(this.operand);
            }

            this.AppendPredicate(" (" + this.whereColumnName + " < {0})", this.sqlCharacters.GetParameterName(0), comparisonValue);

            return this;
        }

        /// <summary>
        /// Uses the specified argument to filter the column.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// This method allows us to specify that a column is filtered with the results being less than or equal to the specified comparisonValue.
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("DateRegistered")
        ///     .IsLessThanOrEqualTo(new DateTime(2000, 1, 1))
        ///     .ToSqlQuery();
        /// </code>
        /// For MsSqlCharacters, will generate SELECT {Columns} FROM Customers WHERE (DateRegistered <!--<-->= @p0)
        /// </example>
        public IAndOrOrderBy IsLessThanOrEqualTo(object comparisonValue)
        {
            if (!string.IsNullOrEmpty(this.operand))
            {
                this.innerSql.Append(this.operand);
            }

            this.AppendPredicate(" (" + this.whereColumnName + " <= {0})", this.sqlCharacters.GetParameterName(0), comparisonValue);

            return this;
        }

        /// <summary>
        /// Uses the specified argument to filter the column.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// This method allows us to specify that a column is filtered with the results being like the specified comparisonValue.
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("DateRegistered")
        ///     .IsLike(new DateTime(2000, 1, 1))
        ///     .ToSqlQuery();
        /// </code>
        /// For MsSqlCharacters, will generate SELECT {Columns} FROM Customers WHERE (DateRegistered LIKE @p0)
        /// </example>
        public IAndOrOrderBy IsLike(object comparisonValue)
        {
            if (!string.IsNullOrEmpty(this.operand))
            {
                this.innerSql.Append(this.operand);
            }

            this.AppendPredicate(" (" + this.whereColumnName + " LIKE {0})", this.sqlCharacters.GetParameterName(0), comparisonValue);

            return this;
        }

        /// <summary>
        /// Uses the specified argument to filter the column.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// This method allows us to specify that a column is filtered with the results not being equal to the specified comparisonValue.
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("DateRegistered")
        ///     .IsNotEqualTo(new DateTime(2000, 1, 1))
        ///     .ToSqlQuery();
        /// </code>
        /// For MsSqlCharacters, will generate SELECT {Columns} FROM Customers WHERE (DateRegistered <!--<>--> @p0)
        /// </example>
        public IAndOrOrderBy IsNotEqualTo(object comparisonValue)
        {
            if (!string.IsNullOrEmpty(this.operand))
            {
                this.innerSql.Append(this.operand);
            }

            this.AppendPredicate(" (" + this.whereColumnName + " <> {0})", this.sqlCharacters.GetParameterName(0), comparisonValue);

            return this;
        }

        /// <summary>
        /// Specifies that the specified column contains a value which is not null.
        /// </summary>
        /// <returns>
        /// The next step in the fluent sql builder.
        /// </returns>
        public IAndOrOrderBy IsNotNull()
        {
            if (!string.IsNullOrEmpty(this.operand))
            {
                this.innerSql.Append(this.operand);
            }

            this.innerSql.AppendFormat(" ({0} IS NOT NULL)", this.whereColumnName);

            return this;
        }

        /// <summary>
        /// Specifies that the specified column contains a value which is null.
        /// </summary>
        /// <returns>
        /// The next step in the fluent sql builder.
        /// </returns>
        public IAndOrOrderBy IsNull()
        {
            if (!string.IsNullOrEmpty(this.operand))
            {
                this.innerSql.Append(this.operand);
            }

            this.innerSql.AppendFormat(" ({0} IS NULL)", this.whereColumnName);

            return this;
        }

        /// <summary>
        /// Selects the maximum value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// A simple query to find the max order total for a customer. By default, the result will be aliased as the column name.
        /// <code>
        /// var sqlQuery = SqlBuilder
        ///     .Select()
        ///     .Max("Total")
        ///     .From(typeof(Invoice))
        ///     .Where("CustomerId = @p0", 1022)
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT MAX(Total) AS Total FROM Invoices WHERE (CustomerId = @p0)
        /// </example>
        public IFunctionOrFrom Max(string columnName)
        {
            return this.Max(columnName, columnName);
        }

        /// <summary>
        /// Selects the maximum value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <param name="columnAlias">The alias in the result set for the calculated column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// A simple query to find the max order total for a customer. We can specify a custom column alias if required.
        /// <code>
        /// var sqlQuery = SqlBuilder
        ///     .Select()
        ///     .Max("Total", columnAlias: "MaxTotal")
        ///     .From(typeof(Invoice))
        ///     .Where("CustomerId = @p0", 1022)
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT MAX(Total) AS MaxTotal FROM Invoices WHERE (CustomerId = @p0)
        /// </example>
        public IFunctionOrFrom Max(string columnName, string columnAlias)
        {
            if (this.innerSql.Length > 6)
            {
                this.innerSql.Append(",");
            }

            this.innerSql.AppendFormat(" MAX({0}) AS {1}", this.sqlCharacters.EscapeSql(columnName), columnAlias);

            return this;
        }

        /// <summary>
        /// Selects the minimum value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// A simple query to find the min order total for a customer. By default, the result will be aliased as the column name.
        /// <code>
        /// var sqlQuery = SqlBuilder
        ///     .Select()
        ///     .Min("Total")
        ///     .From(typeof(Invoice))
        ///     .Where("CustomerId = @p0", 1022)
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT MIN(Total) AS Total FROM Invoices WHERE (CustomerId = @p0)
        /// </example>
        public IFunctionOrFrom Min(string columnName)
        {
            return this.Min(columnName, columnName);
        }

        /// <summary>
        /// Selects the minimum value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <param name="columnAlias">The alias in the result set for the calculated column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// A simple query to find the min order total for a customer. We can specify a custom column alias if required.
        /// <code>
        /// var sqlQuery = SqlBuilder
        ///     .Select()
        ///     .Min("Total", columnAlias: "MinTotal")
        ///     .From(typeof(Invoice))
        ///     .Where("CustomerId = @p0", 1022)
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT MIN(Total) AS MinTotal FROM Invoices WHERE (CustomerId = @p0)
        /// </example>
        public IFunctionOrFrom Min(string columnName, string columnAlias)
        {
            if (this.innerSql.Length > 6)
            {
                this.innerSql.Append(",");
            }

            this.innerSql.AppendFormat(" MIN({0}) AS {1}", this.sqlCharacters.EscapeSql(columnName), columnAlias);

            return this;
        }

        /// <summary>
        /// Uses the specified arguments to filter the column.
        /// </summary>
        /// <param name="args">The arguments to filter the column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// This method allows us to specify that a column is filtered with the results being in the specified values.
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("Column1")
        ///     .NotIn("X", "Y", "Z")
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT {Columns} FROM Customers WHERE (Column1 NOT IN (@p0, @p1, @p2))
        /// </example>
        public IAndOrOrderBy NotIn(params object[] args)
        {
            if (args == null)
            {
                throw new ArgumentNullException("args");
            }

            if (!string.IsNullOrEmpty(this.operand))
            {
                this.innerSql.Append(this.operand);
            }

#if NET_3_5
            var predicate = string.Join(", ", Enumerable.Range(0, args.Length).Select(i => "@p" + i.ToString(CultureInfo.InvariantCulture)).ToArray());
#else
            var predicate = string.Join(", ", Enumerable.Range(0, args.Length).Select(i => this.sqlCharacters.GetParameterName(i)));
#endif
            this.AppendPredicate(" (" + this.whereColumnName + " NOT IN ({0}))", predicate, args);

            return this;
        }

        /// <summary>
        /// Uses the specified SqlQuery as a sub query to filter the column.
        /// </summary>
        /// <param name="subQuery">The sub query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// This method allows us to specify that a column is filtered with the results being in the specified values.
        /// <code>
        /// var customerQuery = SqlBuilder
        ///     .Select("CustomerId")
        ///     .From(typeof(Customer))
        ///     .Where("Age > @p0", 40)
        ///     .ToSqlQuery();
        ///
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Invoice))
        ///     .Where("CustomerId")
        ///     .NotIn(customerQuery)
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT {Columns} FROM Invoices WHERE (CustomerId NOT IN (SELECT CustomerId FROM Customers WHERE Age > @p0))
        /// </example>
        public IAndOrOrderBy NotIn(SqlQuery subQuery)
        {
            if (subQuery == null)
            {
                throw new ArgumentNullException("subQuery");
            }

            if (!string.IsNullOrEmpty(this.operand))
            {
                this.innerSql.Append(this.operand);
            }

            this.AppendPredicate(" (" + this.whereColumnName + " NOT IN ({0}))", subQuery.CommandText, subQuery.Arguments.ToArray());

            return this;
        }

        /// <summary>
        /// Orders the results of the query by the specified columns in ascending order.
        /// </summary>
        /// <param name="columns">The columns to order by.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .OrderByAscending("CustomerId")
        ///     .ToSqlQuery();
        /// </code>
        /// Would generate SELECT [Columns] FROM Customers ORDER BY CustomerId ASC
        /// </example>
        public IOrderBy OrderByAscending(params string[] columns)
        {
            this.AddOrder(columns, " ASC");

            return this;
        }

        /// <summary>
        /// Orders the results of the query by the specified columns in descending order.
        /// </summary>
        /// <param name="columns">The columns to order by.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .OrderByDescending("CustomerId")
        ///     .ToSqlQuery();
        /// </code>
        /// Would generate SELECT [Columns] FROM Customers ORDER BY CustomerId DESC
        /// </example>
        public IOrderBy OrderByDescending(params string[] columns)
        {
            this.AddOrder(columns, " DESC");

            return this;
        }

        /// <summary>
        /// Adds a column as an OR to the where clause of the query.
        /// </summary>
        /// <param name="columnName">The column name to use in the where clause.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IWhereSingleColumn OrWhere(string columnName)
        {
            this.operand = " OR";
            this.whereColumnName = columnName;

            return this;
        }

        /// <summary>
        /// Adds a predicate as an OR to the where clause of the query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The args.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// Adds the an additional predicate to the query as an OR.
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("LastName = @p0", "Smith")
        ///     .OrWhere("LastName = @p0", "Smithson") // Each time, the parameter number relates to the individual method call.
        ///     .ToSqlQuery();
        /// </code>
        /// Would generate SELECT [Columns] FROM Customers WHERE (LastName = @p0) OR (LastName = @p1)
        /// @p0 would be Smith
        /// @p1 would be Smithson
        /// </example>
        /// <example>
        /// Additionally, we could construct the query as follows:
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("LastName = @p0 OR LastName = @p1", "Smith", "Smithson")
        ///     .ToSqlQuery();
        /// </code>
        /// Would generate SELECT [Columns] FROM Customers WHERE (LastName = @p0 OR LastName = @p1)
        /// @p0 would be Smith
        /// @p1 would be Smithson
        /// </example>
        public IAndOrOrderBy OrWhere(string predicate, params object[] args)
        {
            this.AppendPredicate(" OR ({0})", predicate, args);

            return this;
        }

        /// <summary>
        /// Selects the sum of the values in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// A simple query to find the total order total for a customer. By default, the result will be aliased as the column name.
        /// <code>
        /// var sqlQuery = SqlBuilder
        ///     .Select()
        ///     .Sum("Total")
        ///     .From(typeof(Invoice))
        ///     .Where("CustomerId = @p0", 1022)
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT SUM(Total) AS Total FROM Invoices WHERE (CustomerId = @p0)
        /// </example>
        public IFunctionOrFrom Sum(string columnName)
        {
            return this.Sum(columnName, columnName);
        }

        /// <summary>
        /// Selects the sum of the values in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <param name="columnAlias">The alias in the result set for the calculated column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// A simple query to find the total order total for a customer. We can specify a custom column alias if required.
        /// <code>
        /// var sqlQuery = SqlBuilder
        ///     .Select()
        ///     .Sum("Total", columnAlias: "SumTotal")
        ///     .From(typeof(Invoice))
        ///     .Where("CustomerId = @p0", 1022)
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT SUM(Total) AS SumTotal FROM Invoices WHERE (CustomerId = @p0)
        /// </example>
        public IFunctionOrFrom Sum(string columnName, string columnAlias)
        {
            if (this.innerSql.Length > 6)
            {
                this.innerSql.Append(",");
            }

            this.innerSql.AppendFormat(" SUM({0}) AS {1}", this.sqlCharacters.EscapeSql(columnName), columnAlias);

            return this;
        }

        /// <summary>
        /// Creates a <see cref="SqlQuery"/> from the values specified.
        /// </summary>
        /// <returns>The created <see cref="SqlQuery"/>.</returns>
        /// <remarks>This method is called to return an SqlQuery once query has been defined.</remarks>
        public SqlQuery ToSqlQuery()
        {
            return new SqlQuery(this.innerSql.ToString(), this.arguments.ToArray());
        }

        /// <summary>
        /// Specifies the where clause for the query.
        /// </summary>
        /// <param name="columnName">The column name to use in the where clause.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// This method allows us to specify a column to be used with the BETWEEN or IN keywords.
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("DateRegistered")
        ///     ...
        /// </code>
        /// </example>
        public IWhereSingleColumn Where(string columnName)
        {
            this.whereColumnName = this.sqlCharacters.EscapeSql(columnName);

            if (!this.addedWhere)
            {
                this.innerSql.Append(" WHERE");
                this.addedWhere = true;
            }

            return this;
        }

        /// <summary>
        /// Specifies the where clause for the query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The args.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// Adds the first predicate to the query.
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("LastName = @p0", "Smith")
        ///     .ToSqlQuery();
        /// </code>
        /// Would generate SELECT [Columns] FROM Customers WHERE (LastName = @p0)
        /// </example>
        /// <example>
        /// You can refer to the same parameter multiple times
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("LastName = @p0 OR @p0 IS NULL", lastName)
        ///     .ToSqlQuery();
        /// </code>
        /// Would generate SELECT [Columns] FROM Customers WHERE (LastName = @p0 OR @p0 IS NULL)
        /// </example>
        public IAndOrOrderBy Where(string predicate, params object[] args)
        {
            this.AppendPredicate(" WHERE ({0})", predicate, args);
            this.addedWhere = true;

            return this;
        }

        /// <summary>
        /// Specifies that the stored procedure should be executed the specified parameter and argument.
        /// </summary>
        /// <param name="parameter">The parameter to be added.</param>
        /// <param name="arg">The argument value for the parameter.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// Add each parameter separately, specifying the parameter name and value.
        /// <code>
        /// var sqlQuery = SqlBuilder
        ///     .Execute("GetCustomerInvoices")
        ///     .WithParameter("@CustomerId", 7633245)
        ///     .WithParameter("@StartDate", DateTime.Today.AddMonths(-3))
        ///     .WithParameter("@EndDate", DateTime.Today)
        ///     .ToSqlQuery();
        /// </code>
        /// </example>
        public IWithParameter WithParameter(string parameter, object arg)
        {
            if (this.arguments.Count > 0)
            {
                this.innerSql.Append(",");
            }

            this.arguments.Add(arg);
            this.innerSql.Append(" ");
            this.innerSql.Append(parameter);

            return this;
        }

        private void AddOrder(string[] columns, string direction)
        {
            this.innerSql.Append(!this.addedOrder ? " ORDER BY " : ", ");

            for (int i = 0; i < columns.Length; i++)
            {
                if (i > 0)
                {
                    this.innerSql.Append(", ");
                }

                this.innerSql.Append(this.sqlCharacters.EscapeSql(columns[i]));
            }

            this.innerSql.Append(direction);
            this.addedOrder = true;
        }

        private void AppendPredicate(string appendFormat, string predicate, params object[] args)
        {
            this.arguments.AddRange(args);

            var renumberedPredicate = SqlUtility.RenumberParameters(predicate, this.arguments.Count);

            this.innerSql.AppendFormat(appendFormat, renumberedPredicate);
        }
    }
}