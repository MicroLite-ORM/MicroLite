// -----------------------------------------------------------------------
// <copyright file="SqlBuilder.cs" company="MicroLite">
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
namespace MicroLite.Query
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using MicroLite.Mapping;

    /// <summary>
    /// A helper class for building an <see cref="SqlQuery" />.
    /// </summary>
    public sealed class SqlBuilder : IFrom, IFunctionOrFrom, IWhereOrOrderBy, IAndOrOrderBy, IGroupBy, IOrderBy, IToSqlQuery, IWithParameter, IWhereIn
    {
        private readonly List<object> arguments = new List<object>();
        private readonly StringBuilder innerSql = new StringBuilder();
        private bool addedOrder = false;
        private bool addedWhere = false;
        private string operand;
        private string whereColumnName;

        private SqlBuilder(string startingSql)
        {
            this.innerSql.Append(startingSql);
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
            return new SqlBuilder("EXEC " + procedure);
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
            if (columns == null || columns.Length == 0)
            {
                return new SqlBuilder("SELECT");
            }

            return new SqlBuilder("SELECT " + string.Join(", ", columns));
        }

        /// <summary>
        /// Adds a column as an AND to the where clause of the query.
        /// </summary>
        /// <param name="columnName">The column name to use in the where clause.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IWhereIn AndWhere(string columnName)
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
        /// Adds the an additional predicate to the query as an OR.
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("FirstName = @p0", "John")
        ///     .AndWhere("LastName = @p0", "Smith") // Each time, the parameter number relates to the individual method call.
        ///     .ToSqlQuery();
        /// </code>
        /// Would generate SELECT [Columns] FROM Customers WHERE (FirstName = @p0) AND (LastName = @p1)
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
        /// Would generate SELECT [Columns] FROM Customers WHERE (FirstName = @p0 AND LastName = @p1)
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
        /// Will generate SELECT AVG(Total) AS Total FROM Sales.Invoices WHERE (CustomerId = @p0)
        /// </example>
        public IFrom Average(string columnName)
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
        /// Will generate SELECT AVG(Total) AS AverageTotal FROM Sales.Invoices WHERE (CustomerId = @p0)
        /// </example>
        public IFrom Average(string columnName, string columnAlias)
        {
            if (this.innerSql.Length > 6)
            {
                this.innerSql.Append(",");
            }

            this.innerSql.Append(" AVG(" + columnName + ") AS " + columnAlias);

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
        /// Will generate SELECT COUNT(CustomerId) AS CustomerId FROM Sales.Customers
        /// </example>
        public IFrom Count(string columnName)
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
        /// Will generate SELECT COUNT(CustomerId) AS CustomerCount FROM Sales.Customers
        /// </example>
        public IFrom Count(string columnName, string columnAlias)
        {
            if (this.innerSql.Length > 6)
            {
                this.innerSql.Append(",");
            }

            this.innerSql.Append(" COUNT(" + columnName + ") AS " + columnAlias);

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
            this.innerSql.Append(" FROM " + table);

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

            return select.From(objectInfo.TableInfo.Schema + "." + objectInfo.TableInfo.Name);
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
        /// Will generate SELECT CustomerId, MAX(Total) AS Total FROM Sales.Invoices GROUP BY CustomerId
        /// </example>
        public IOrderBy GroupBy(params string[] columns)
        {
            this.innerSql.Append(" GROUP BY " + string.Join(", ", columns));

            return this;
        }

        /// <summary>
        /// Uses the specified arguments to filter the column.
        /// </summary>
        /// <param name="args">The arguments to filter the column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IAndOrOrderBy In(params object[] args)
        {
            if (!this.addedWhere)
            {
                this.innerSql.Append(" WHERE");
                this.addedWhere = true;
            }

            if (!string.IsNullOrEmpty(this.operand))
            {
                this.innerSql.Append(this.operand);
            }

            var predicate = string.Join(", ", Enumerable.Range(0, args.Length).Select(i => "@p" + i.ToString(CultureInfo.InvariantCulture)));

            this.AppendPredicate(" (" + this.whereColumnName + " IN ({0}))", predicate, args);

            return this;
        }

        /// <summary>
        /// Uses the specified SqlQuery as a sub query to filter the column.
        /// </summary>
        /// <param name="subQuery">The sub query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        public IAndOrOrderBy In(SqlQuery subQuery)
        {
            if (!this.addedWhere)
            {
                this.innerSql.Append(" WHERE");
                this.addedWhere = true;
            }

            if (!string.IsNullOrEmpty(this.operand))
            {
                this.innerSql.Append(this.operand);
            }

            this.AppendPredicate(" (" + this.whereColumnName + " IN ({0}))", subQuery.CommandText, subQuery.Arguments.ToArray());

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
        /// Will generate SELECT MAX(Total) AS Total FROM Sales.Invoices WHERE (CustomerId = @p0)
        /// </example>
        public IFrom Max(string columnName)
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
        /// Will generate SELECT MAX(Total) AS MaxTotal FROM Sales.Invoices WHERE (CustomerId = @p0)
        /// </example>
        public IFrom Max(string columnName, string columnAlias)
        {
            if (this.innerSql.Length > 6)
            {
                this.innerSql.Append(",");
            }

            this.innerSql.Append(" MAX(" + columnName + ") AS " + columnAlias);

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
        /// Will generate SELECT MIN(Total) AS Total FROM Sales.Invoices WHERE (CustomerId = @p0)
        /// </example>
        public IFrom Min(string columnName)
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
        /// Will generate SELECT MIN(Total) AS MinTotal FROM Sales.Invoices WHERE (CustomerId = @p0)
        /// </example>
        public IFrom Min(string columnName, string columnAlias)
        {
            if (this.innerSql.Length > 6)
            {
                this.innerSql.Append(",");
            }

            this.innerSql.Append(" MIN(" + columnName + ") AS " + columnAlias);

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
        public IWhereIn OrWhere(string columnName)
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
        ///     .OrWhere("LastName = @p0", "Smythe") // Each time, the parameter number relates to the individual method call.
        ///     .ToSqlQuery();
        /// </code>
        /// Would generate SELECT [Columns] FROM Customers WHERE (LastName = @p0) OR (LastName = @p1)
        /// @p0 would be Smith
        /// @p1 would be Smythe
        /// </example>
        /// <example>
        /// Additionally, we could construct the query as follows:
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("LastName = @p0 OR LastName = @p1", "Smith", "Smythe")
        ///     .ToSqlQuery();
        /// </code>
        /// Would generate SELECT [Columns] FROM Customers WHERE (LastName = @p0 OR LastName = @p1)
        /// @p0 would be Smith
        /// @p1 would be Smythe
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
        /// Will generate SELECT SUM(Total) AS Total FROM Sales.Invoices WHERE (CustomerId = @p0)
        /// </example>
        public IFrom Sum(string columnName)
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
        /// Will generate SELECT SUM(Total) AS SumTotal FROM Sales.Invoices WHERE (CustomerId = @p0)
        /// </example>
        public IFrom Sum(string columnName, string columnAlias)
        {
            if (this.innerSql.Length > 6)
            {
                this.innerSql.Append(",");
            }

            this.innerSql.Append(" SUM(" + columnName + ") AS " + columnAlias);

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
        public IWhereIn Where(string columnName)
        {
            this.whereColumnName = columnName;

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
            this.innerSql.Append(" " + parameter);

            return this;
        }

        private void AddOrder(string[] columns, string direction)
        {
            if (!this.addedOrder)
            {
                this.innerSql.Append(" ORDER BY " + string.Join(", ", columns) + direction);
                this.addedOrder = true;
            }
            else
            {
                this.innerSql.Append(", " + string.Join(", ", columns) + direction);
            }
        }

        private void AppendPredicate(string appendFormat, string predicate, params object[] args)
        {
            this.arguments.AddRange(args);

            var renumberedPredicate = SqlUtil.ReNumberParameters(predicate, this.arguments.Count);

            this.innerSql.AppendFormat(appendFormat, renumberedPredicate);
        }
    }
}