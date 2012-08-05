// -----------------------------------------------------------------------
// <copyright file="IAdvancedSession.cs" company="MicroLite">
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
namespace MicroLite
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The interface which provides access to advanced operations.
    /// </summary>
    /// <remarks>
    /// These operations allow for more advanded use and have been moved to a separate interface to avoid
    /// cluttering the ISession API.
    /// </remarks>
    public interface IAdvancedSession : IHideObjectMethods
    {
        /// <summary>
        /// Deletes the database record for the type with the specified identifier.
        /// </summary>
        /// <param name="type">The type to delete.</param>
        /// <param name="identifier">The identifier of the record to delete.</param>
        /// <returns>true if the object was deleted successfully; otherwise false.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified type or identifier is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the delete command.</exception>
        bool Delete(Type type, object identifier);

        /// <summary>
        /// Executes the specified SQL query.
        /// </summary>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <returns>The number of rows affected by the sql query.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified SqlQuery is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the command.</exception>
        int Execute(SqlQuery sqlQuery);

        /// <summary>
        /// Executes the specified SQL query as a scalar command.
        /// </summary>
        /// <typeparam name="T">The type of result to be returned.</typeparam>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <returns>The result of the scalar query.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified SqlQuery is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the command.</exception>
        T ExecuteScalar<T>(SqlQuery sqlQuery);

#if !NET_3_5

        /// <summary>
        /// Executes the specified SQL query and returns the results as a list of dynamic objects.
        /// </summary>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <returns>The results as a list of dynamic objects.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified SqlQuery is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the command.</exception>
        IList<dynamic> Projection(SqlQuery sqlQuery);

#endif
    }
}