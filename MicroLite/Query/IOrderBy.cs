// -----------------------------------------------------------------------
// <copyright file="IOrderBy.cs" company="MicroLite">
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
    /// The interface which specifies the order by method in the fluent sql builder syntax.
    /// </summary>
    public interface IOrderBy : IHideObjectMethods, IToSqlQuery
    {
        /// <summary>
        /// Orders the results of the query by the specified columns in ascending order.
        /// </summary>
        /// <param name="columns">The columns to order by.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IOrderBy OrderByAscending(params string[] columns);

        /// <summary>
        /// Orders the results of the query by the specified columns in descending order.
        /// </summary>
        /// <param name="columns">The columns to order by.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IOrderBy OrderByDescending(params string[] columns);
    }
}