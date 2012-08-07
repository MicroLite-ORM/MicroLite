// -----------------------------------------------------------------------
// <copyright file="ISqlDialect.cs" company="MicroLite">
// Copyright 2012 Trevor Pilley
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Dialect
{
    using System;

    /// <summary>
    /// The interface for a class which builds an <see cref="SqlQuery"/> for a object instance.
    /// </summary>
    internal interface ISqlDialect
    {
        /// <summary>
        /// Creates a SqlQuery to perform a delete for the given instance.
        /// </summary>
        /// <param name="instance">The instance to build the query for.</param>
        /// <returns>A <see cref="SqlQuery"/> to delete a record.</returns>
        SqlQuery DeleteQuery(object instance);

        /// <summary>
        /// Creates a SqlQuery to perform a delete for the given identifier.
        /// </summary>
        /// <param name="forType">For type.</param>
        /// <param name="identifier">The identifier value for the target record to delete.</param>
        /// <returns>A <see cref="SqlQuery"/> to delete a record.</returns>
        SqlQuery DeleteQuery(Type forType, object identifier);

        /// <summary>
        /// Creates a SqlQuery to perform an insert for the given instance.
        /// </summary>
        /// <param name="instance">The instance to build the query for.</param>
        /// <returns>A <see cref="SqlQuery"/> to insert a record.</returns>
        SqlQuery InsertQuery(object instance);

        /// <summary>
        /// Pages the specified SQL query.
        /// </summary>
        /// <param name="sqlQuery">The SQL query.</param>
        /// <param name="page">The page number to get the results for.</param>
        /// <param name="resultsPerPage">The number of results to be shown per page.</param>
        /// <returns>A <see cref="SqlQuery"/> to return the paged results of the specified query.</returns>
        SqlQuery Page(SqlQuery sqlQuery, long page, long resultsPerPage);

        /// <summary>
        /// Creates a SqlQuery to perform a select for the given type and identifier value.
        /// </summary>
        /// <param name="forType">The type of object the query is for.</param>
        /// <param name="identifier">The identifier value for the target record.</param>
        /// <returns>A <see cref="SqlQuery"/> to select a specific record.</returns>
        SqlQuery SelectQuery(Type forType, object identifier);

        /// <summary>
        /// Creates a SqlQuery to perform an update for the given instance.
        /// </summary>
        /// <param name="instance">The instance to build the query for.</param>
        /// <returns>A <see cref="SqlQuery"/> to update a record.</returns>
        SqlQuery UpdateQuery(object instance);
    }
}