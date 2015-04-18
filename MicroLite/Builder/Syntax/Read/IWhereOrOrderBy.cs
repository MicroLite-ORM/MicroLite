// -----------------------------------------------------------------------
// <copyright file="IWhereOrOrderBy.cs" company="MicroLite">
// Copyright 2012 - 2015 Project Contributors
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
    /// The interface which specifies the where method in the fluent sql builder syntax.
    /// </summary>
    public interface IWhereOrOrderBy : IHideObjectMethods, IGroupBy, IOrderBy, IToSqlQuery
    {
        /// <summary>
        /// Specifies the where clause for the query.
        /// </summary>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// This method allows us to specify a sub query for the EXISTS keyword.
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where()
        ///     ...
        /// </code>
        /// </example>
        IWhereExists Where();

        /// <summary>
        /// Specifies the where clause for the query.
        /// </summary>
        /// <param name="column">The column name to use in the where clause.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if column is null or empty.</exception>
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
        IWhereSingleColumn Where(string column);

        /// <summary>
        /// Specifies the where clause for the query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The args.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if predicate is null or empty.</exception>
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
        IAndOrOrderBy Where(string predicate, params object[] args);
    }
}