// -----------------------------------------------------------------------
// <copyright file="IWhereOrOrderBy.cs" company="MicroLite">
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
    /// The interface which specifies the where method in the fluent sql builder syntax.
    /// </summary>
    public interface IWhereOrOrderBy : IHideObjectMethods, IGroupBy, IOrderBy, IToSqlQuery
    {
        /// <summary>
        /// Specifies the where clause for the query.
        /// </summary>
        /// <param name="columnName">The column name to use in the where clause.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IWhereSingleColumn Where(string columnName);

        /// <summary>
        /// Specifies the where clause for the query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The args.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOrOrderBy Where(string predicate, params object[] args);
    }
}