// -----------------------------------------------------------------------
// <copyright file="IWhereExists.cs" company="MicroLite">
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
    /// <summary>
    /// The interface which specifies the where in method in the fluent sql builder syntax.
    /// </summary>
    public interface IWhereExists
    {
        /// <summary>
        /// Uses the specified SqlQuery as a sub query to filter the results.
        /// </summary>
        /// <param name="subQuery">The sub query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if subQuery is null.</exception>
        /// <example>
        /// This method allows us to specify that the results are filtered with the results being in the specified sub query.
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
        ///     .Where()
        ///     .InExists(customerQuery)
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT {Columns} FROM Invoices WHERE EXISTS (SELECT CustomerId FROM Customers WHERE Age > @p0)
        /// </example>
        IAndOrOrderBy Exists(SqlQuery subQuery);

        /// <summary>
        /// Uses the specified SqlQuery as a sub query to filter the results.
        /// </summary>
        /// <param name="subQuery">The sub query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if subQuery is null.</exception>
        /// <example>
        /// This method allows us to specify that the results are filtered with the results not being in the specified sub query.
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
        ///     .Where()
        ///     .InExists(customerQuery)
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT {Columns} FROM Invoices WHERE EXISTS (SELECT CustomerId FROM Customers WHERE Age > @p0)
        /// </example>
        IAndOrOrderBy NotExists(SqlQuery subQuery);
    }
}