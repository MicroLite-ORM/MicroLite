// -----------------------------------------------------------------------
// <copyright file="IAdvancedSession.cs" company="MicroLite">
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
namespace MicroLite
{
    using System;

    /// <summary>
    /// The interface which provides access to advanced session operations.
    /// </summary>
    /// <remarks>
    /// These operations allow for more advanced use and have been moved to a separate interface to avoid
    /// cluttering the ISession API.
    /// </remarks>
    public interface IAdvancedSession : IHideObjectMethods, IAdvancedReadOnlySession
    {
        /// <summary>
        /// Deletes the database record of the specified type with the specified identifier.
        /// </summary>
        /// <param name="type">The type to delete.</param>
        /// <param name="identifier">The identifier of the record to delete.</param>
        /// <returns>true if the object was deleted successfully; otherwise false.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified type or identifier is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the delete command.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         bool wasDeleted = session.Advanced.Delete(type: typeof(Customer), identifier: 12823);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        bool Delete(Type type, object identifier);

        /// <summary>
        /// Executes the specified SQL query and returns the number of rows affected.
        /// </summary>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <returns>The number of rows affected by the SQL query.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified SqlQuery is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the command.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         var query = new SqlQuery("UPDATE Customers SET Locked = @p0 WHERE Locked = @p1", false, true);
        ///
        ///         int unlockedRowCount = session.Advanced.Execute(query);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        int Execute(SqlQuery sqlQuery);

        /// <summary>
        /// Executes the specified SQL query as a scalar command.
        /// </summary>
        /// <typeparam name="T">The type of result to be returned.</typeparam>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <returns>The result of the scalar query (the first column in the first row returned).</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified SqlQuery is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the command.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         // Create a query which returns a single result.
        ///         var query = new SqlQuery("SELECT COUNT(CustomerId) FROM Customers");
        ///
        ///         int customerCount = session.Advanced.ExecuteScalar&lt;int&gt;(query);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        T ExecuteScalar<T>(SqlQuery sqlQuery);

        /// <summary>
        /// Performs a partial update on a table row based upon the values specified in the object delta.
        /// </summary>
        /// <param name="objectDelta">The object delta containing the changes to be applied.</param>
        /// <returns>true if the object was updated successfully; otherwise false.</returns>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         // Create an ObjectDelta which only updates specific properties:
        ///         var objectDelta = new ObjectDelta(type: typeof(Customer), identifier: 12823);
        ///         objectDelta.AddChange(propertyName: "Locked", newValue: false); // Add 1 or more changes.
        ///
        ///         bool wasUpdated = session.Advanced.Update(objectDelta);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        bool Update(ObjectDelta objectDelta);
    }
}