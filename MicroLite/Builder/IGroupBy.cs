// -----------------------------------------------------------------------
// <copyright file="IGroupBy.cs" company="MicroLite">
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
    /// The interface which specifies the group by method in the fluent sql builder syntax.
    /// </summary>
    public interface IGroupBy : IHideObjectMethods
    {
        /// <summary>
        /// Groups the results of the query by the specified column.
        /// </summary>
        /// <param name="column">The column to group by.</param>
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
        IHavingOrOrderBy GroupBy(string column);

        /// <summary>
        /// Groups the results of the query by the specified columns.
        /// </summary>
        /// <param name="columns">The columns to group by.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// <code>
        /// var sqlQuery = SqlBuilder
        ///     .Select("CustomerId, OrderDate")
        ///     .Max("Total")
        ///     .From(typeof(Invoice))
        ///     .GroupBy("CustomerId, OrderDate")
        ///     .ToSqlQuery();
        /// </code>
        /// Will generate SELECT CustomerId, OrderDate, MAX(Total) AS Total FROM Invoices GROUP BY CustomerId, OrderDate
        /// </example>
        IHavingOrOrderBy GroupBy(params string[] columns);
    }
}