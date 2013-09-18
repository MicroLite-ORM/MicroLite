// -----------------------------------------------------------------------
// <copyright file="IAndOrOrderBy.cs" company="MicroLite">
// Copyright 2012 - 2013 Project Contributors
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
    /// <summary>
    /// The interface which specifies the and/or methods to extend the where clause in the fluent sql builder syntax.
    /// </summary>
    public interface IAndOrOrderBy : IHideObjectMethods, IGroupBy, IOrderBy, IToSqlQuery
    {
        /// <summary>
        /// Adds a column as an AND to the where clause of the query.
        /// </summary>
        /// <param name="columnName">The column name to use in the where clause.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IWhereSingleColumn AndWhere(string columnName);

        /// <summary>
        /// Adds a predicate as an AND to the where clause of the query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The args.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOrOrderBy AndWhere(string predicate, params object[] args);

        /// <summary>
        /// Adds a column as an OR to the where clause of the query.
        /// </summary>
        /// <param name="columnName">The column name to use in the where clause.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IWhereSingleColumn OrWhere(string columnName);

        /// <summary>
        /// Adds a predicate as an OR to the where clause of the query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The args.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOrOrderBy OrWhere(string predicate, params object[] args);
    }
}