// -----------------------------------------------------------------------
// <copyright file="IWhereSingleColumn.cs" company="MicroLite">
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
namespace MicroLite.Builder.Syntax.Read
{
    /// <summary>
    /// The interface which specifies the where in method in the fluent sql builder syntax.
    /// </summary>
    public interface IWhereSingleColumn : IHideObjectMethods
    {
        /// <summary>
        /// Uses the specified arguments to filter the column.
        /// </summary>
        /// <param name="lower">The inclusive lower value.</param>
        /// <param name="upper">The inclusive upper value.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if lower or upper is null.</exception>
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
        /// Will generate SELECT {Columns} FROM Customers WHERE (DateRegistered BETWEEN @p0 AND @p1)
        /// </example>
        IAndOrOrderBy Between(object lower, object upper);

        /// <summary>
        /// Uses the specified arguments to filter the column.
        /// </summary>
        /// <param name="args">The arguments to filter the column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if args is null.</exception>
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
        /// Will generate SELECT {Columns} FROM Customers WHERE (Column1 IN (@p0, @p1, @p2))
        /// </example>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "In", Justification = "The method is to specify an In list.")]
        IAndOrOrderBy In(params object[] args);

        /// <summary>
        /// Uses the specified SQL queries as sub queries to filter the column.
        /// </summary>
        /// <param name="subQueries">The sub queries.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if subQueries is null.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "In", Justification = "The method is to specify an In list.")]
        IAndOrOrderBy In(params SqlQuery[] subQueries);

        /// <summary>
        /// Uses the specified SqlQuery as a sub query to filter the column.
        /// </summary>
        /// <param name="subQuery">The sub query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if subQuery is null.</exception>
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "In", Justification = "The method is to specify an In list.")]
        IAndOrOrderBy In(SqlQuery subQuery);

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
        /// Will generate SELECT {Columns} FROM Customers WHERE (DateRegistered = @p0)
        /// </example>
        IAndOrOrderBy IsEqualTo(object comparisonValue);

        /// <summary>
        /// Uses the specified sub query to filter the column.
        /// </summary>
        /// <param name="subQuery">The sub query to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// This method allows us to specify that a column is filtered with the results being equal to the result of the specified sub query.
        /// <code>
        /// var subQuery = new SqlQuery("SELECT TOP 1 CustomerId FROM Invoices ORDER BY Total DESC");
        ///
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("CustomerId")
        ///     .IsEqualTo(subQuery)
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT {Columns} FROM Customers WHERE (CustomerId = (SELECT TOP 1 CustomerId FROM Invoices ORDER BY Total DESC))
        /// </example>
        IAndOrOrderBy IsEqualTo(SqlQuery subQuery);

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
        /// Will generate SELECT {Columns} FROM Customers WHERE (DateRegistered > @p0)
        /// </example>
        IAndOrOrderBy IsGreaterThan(object comparisonValue);

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
        /// Will generate SELECT {Columns} FROM Customers WHERE (DateRegistered >= @p0)
        /// </example>
        IAndOrOrderBy IsGreaterThanOrEqualTo(object comparisonValue);

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
        /// Will generate SELECT {Columns} FROM Customers WHERE (DateRegistered <!--<--> @p0)
        /// </example>
        IAndOrOrderBy IsLessThan(object comparisonValue);

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
        /// Will generate SELECT {Columns} FROM Customers WHERE (DateRegistered &lt;= @p0)
        /// </example>
        IAndOrOrderBy IsLessThanOrEqualTo(object comparisonValue);

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
        ///     .Where("LastName")
        ///     .IsLike("Smi%")
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT {Columns} FROM Customers WHERE (LastName LIKE @p0)
        /// </example>
        IAndOrOrderBy IsLike(object comparisonValue);

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
        /// Will generate SELECT {Columns} FROM Customers WHERE (DateRegistered &lt;&gt; @p0)
        /// </example>
        IAndOrOrderBy IsNotEqualTo(object comparisonValue);

        /// <summary>
        /// Uses the specified sub query to filter the column.
        /// </summary>
        /// <param name="subQuery">The sub query to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// This method allows us to specify that a column is filtered with the results not being equal to the result of the specified sub query.
        /// <code>
        /// var subQuery = new SqlQuery("SELECT TOP 1 CustomerId FROM Invoices ORDER BY Total DESC");
        ///
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("CustomerId")
        ///     .IsNotEqualTo(subQuery)
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT {Columns} FROM Customers WHERE (CustomerId &lt;&gt; (SELECT TOP 1 CustomerId FROM Invoices ORDER BY Total DESC))
        /// </example>
        IAndOrOrderBy IsNotEqualTo(SqlQuery subQuery);

        /// <summary>
        /// Uses the specified argument to filter the column.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// This method allows us to specify that a column is filtered with the results not being like the specified comparisonValue.
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("LastName")
        ///     .IsNotLike("Smi%")
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT {Columns} FROM Customers WHERE (LastName NOT LIKE @p0)
        /// </example>
        IAndOrOrderBy IsNotLike(object comparisonValue);

        /// <summary>
        /// Specifies that the specified column contains a value which is not null.
        /// </summary>
        /// <returns>
        /// The next step in the fluent sql builder.
        /// </returns>
        IAndOrOrderBy IsNotNull();

        /// <summary>
        /// Specifies that the specified column contains a value which is null.
        /// </summary>
        /// <returns>
        /// The next step in the fluent sql builder.
        /// </returns>
        IAndOrOrderBy IsNull();

        /// <summary>
        /// Uses the specified arguments to filter the column.
        /// </summary>
        /// <param name="lower">The inclusive lower value.</param>
        /// <param name="upper">The inclusive upper value.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if lower or upper is null.</exception>
        /// <example>
        /// This method allows us to specify that a column is filtered with the results not being between the 2 specified values.
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("DateRegistered")
        ///     .NotBetween(new DateTime(2000, 1, 1), new DateTime(2009, 12, 31))
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT {Columns} FROM Customers WHERE (DateRegistered NOT BETWEEN @p0 AND @p1)
        /// </example>
        IAndOrOrderBy NotBetween(object lower, object upper);

        /// <summary>
        /// Uses the specified arguments to filter the column.
        /// </summary>
        /// <param name="args">The arguments to filter the column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if args is null.</exception>
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
        IAndOrOrderBy NotIn(params object[] args);

        /// <summary>
        /// Uses the specified SQL queries as sub queries to filter the column.
        /// </summary>
        /// <param name="subQueries">The sub queries.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if subQueries is null.</exception>
        IAndOrOrderBy NotIn(params SqlQuery[] subQueries);

        /// <summary>
        /// Uses the specified SqlQuery as a sub query to filter the column.
        /// </summary>
        /// <param name="subQuery">The sub query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if subQuery is null.</exception>
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
        IAndOrOrderBy NotIn(SqlQuery subQuery);
    }
}