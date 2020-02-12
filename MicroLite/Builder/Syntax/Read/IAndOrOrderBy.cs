// -----------------------------------------------------------------------
// <copyright file="IAndOrOrderBy.cs" company="Project Contributors">
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
    /// The interface which specifies the and/or methods to extend the where clause in the fluent sql builder syntax.
    /// </summary>
    public interface IAndOrOrderBy : IHideObjectMethods, IGroupBy, IOrderBy, IToSqlQuery
    {
        /// <summary>
        /// Adds a column as an AND to the where clause of the query.
        /// </summary>
        /// <param name="column">The column name to use in the where clause.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if column is null or empty.</exception>
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
        IWhereSingleColumn AndWhere(string column);

        /// <summary>
        /// Adds a predicate as an AND to the where clause of the query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The args.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if predicate is null or empty.</exception>
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
        IAndOrOrderBy AndWhere(string predicate, params object[] args);

        /// <summary>
        /// Adds a column as an OR to the where clause of the query.
        /// </summary>
        /// <param name="column">The column name to use in the where clause.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if column is null or empty.</exception>
        /// <example>
        /// This method allows us to specify a column to be used with the BETWEEN or IN keywords which is added to the query as an OR.
        /// <code>
        /// var query = SqlBuilder
        ///     .Select("*")
        ///     .From(typeof(Customer))
        ///     .Where("LastName = @p0", "Smith")
        ///     .OrWhere("DateRegistered")
        ///     ...
        /// </code>
        /// </example>
        IWhereSingleColumn OrWhere(string column);

        /// <summary>
        /// Adds a predicate as an OR to the where clause of the query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The args.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if predicate is null or empty.</exception>
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
        IAndOrOrderBy OrWhere(string predicate, params object[] args);
    }
}