// -----------------------------------------------------------------------
// <copyright file="IHavingOrOrderBy.cs" company="MicroLite">
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
    /// The interface which specifies the having method in the fluent sql builder syntax.
    /// </summary>
    public interface IHavingOrOrderBy : IHideObjectMethods, IOrderBy
    {
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
        IOrderBy Having(string predicate, object value);
    }
}