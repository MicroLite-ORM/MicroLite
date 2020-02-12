// -----------------------------------------------------------------------
// <copyright file="IFunctionOrFrom.cs" company="Project Contributors">
// Copyright Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Builder.Syntax.Read
{
    /// <summary>
    /// The interface which specifies the from method or function in the fluent sql builder syntax.
    /// </summary>
    public interface IFunctionOrFrom : IHideObjectMethods, ISelectFrom
    {
        /// <summary>
        /// Selects the average value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if columnName is null or empty.</exception>
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
        /// Will generate SELECT AVG(Total) AS Total FROM Invoices WHERE (CustomerId = @p0).
        /// </example>
        IFunctionOrFrom Average(string columnName);

        /// <summary>
        /// Selects the average value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <param name="columnAlias">The alias in the result set for the calculated column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if columnName or columnAlias is null or empty.</exception>
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
        /// Will generate SELECT AVG(Total) AS AverageTotal FROM Invoices WHERE (CustomerId = @p0).
        /// </example>
        IFunctionOrFrom Average(string columnName, string columnAlias);

        /// <summary>
        /// Selects the number of records which match the specified filter.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if columnName is null or empty.</exception>
        /// <example>
        /// A simple query to find the number of customers. By default, the result will be aliased as the column name.
        /// <code>
        /// var sqlQuery = SqlBuilder
        ///     .Select()
        ///     .Count("CustomerId")
        ///     .From(typeof(Customer))
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT COUNT(CustomerId) AS CustomerId FROM Customers.
        /// </example>
        IFunctionOrFrom Count(string columnName);

        /// <summary>
        /// Selects the number of records which match the specified filter.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <param name="columnAlias">The alias in the result set for the calculated column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if columnName or columnAlias is null or empty.</exception>
        /// <example>
        /// A simple query to find the number of customers. We can specify a custom column alias if required.
        /// <code>
        /// var sqlQuery = SqlBuilder
        ///     .Select()
        ///     .Count("CustomerId", columnAlias: "CustomerCount")
        ///     .From(typeof(Customer))
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT COUNT(CustomerId) AS CustomerCount FROM Customers.
        /// </example>
        IFunctionOrFrom Count(string columnName, string columnAlias);

        /// <summary>
        /// Selects the distinct values in the specified column.
        /// </summary>
        /// <param name="column">The column to be included in the query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if columnName is null or empty.</exception>
        /// <example>
        /// A simple query to find the distinct order totals for a customer.
        /// <code>
        /// var sqlQuery = SqlBuilder
        ///     .Select()
        ///     .Distinct("Total")
        ///     .From(typeof(Invoice))
        ///     .Where("CustomerId = @p0", 1022)
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT DISTINCT Total FROM Invoices WHERE (CustomerId = @p0).
        /// </example>
        IFunctionOrFrom Distinct(string column);

        /// <summary>
        /// Selects the distinct values in the specified columns.
        /// </summary>
        /// <param name="columns">The columns to be included in the query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if columns is null.</exception>
        /// <example>
        /// A simple query to find the distinct order totals and dates for a customer.
        /// <code>
        /// var sqlQuery = SqlBuilder
        ///     .Select()
        ///     .Distinct("Total", "Date")
        ///     .From(typeof(Invoice))
        ///     .Where("CustomerId = @p0", 1022)
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT DISTINCT Total, Date FROM Invoices WHERE (CustomerId = @p0).
        /// </example>
        IFunctionOrFrom Distinct(params string[] columns);

        /// <summary>
        /// Selects the maximum value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if columnName is null or empty.</exception>
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
        /// Will generate SELECT MAX(Total) AS Total FROM Invoices WHERE (CustomerId = @p0).
        /// </example>
        IFunctionOrFrom Max(string columnName);

        /// <summary>
        /// Selects the maximum value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <param name="columnAlias">The alias in the result set for the calculated column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if columnName or columnAlias is null or empty.</exception>
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
        /// Will generate SELECT MAX(Total) AS MaxTotal FROM Invoices WHERE (CustomerId = @p0).
        /// </example>
        IFunctionOrFrom Max(string columnName, string columnAlias);

        /// <summary>
        /// Selects the minimum value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if columnName is null or empty.</exception>
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
        /// Will generate SELECT MIN(Total) AS Total FROM Invoices WHERE (CustomerId = @p0).
        /// </example>
        IFunctionOrFrom Min(string columnName);

        /// <summary>
        /// Selects the minimum value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <param name="columnAlias">The alias in the result set for the calculated column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if columnName or columnAlias is null or empty.</exception>
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
        /// Will generate SELECT MIN(Total) AS MinTotal FROM Invoices WHERE (CustomerId = @p0).
        /// </example>
        IFunctionOrFrom Min(string columnName, string columnAlias);

        /// <summary>
        /// Selects the sum of the values in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if columnName is null or empty.</exception>
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
        /// Will generate SELECT SUM(Total) AS Total FROM Invoices WHERE (CustomerId = @p0).
        /// </example>
        IFunctionOrFrom Sum(string columnName);

        /// <summary>
        /// Selects the sum of the values in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <param name="columnAlias">The alias in the result set for the calculated column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if columnName or columnAlias is null or empty.</exception>
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
        /// Will generate SELECT SUM(Total) AS SumTotal FROM Invoices WHERE (CustomerId = @p0).
        /// </example>
        IFunctionOrFrom Sum(string columnName, string columnAlias);
    }
}
