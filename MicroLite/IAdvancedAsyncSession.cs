// -----------------------------------------------------------------------
// <copyright file="IAdvancedAsyncSession.cs" company="MicroLite">
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
namespace MicroLite
{
#if NET_4_5

    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The interface which provides access to advanced asynchronous session operations.
    /// </summary>
    /// <remarks>
    /// These operations allow for more advanced use and have been moved to a separate interface to avoid
    /// cluttering the ISession API.
    /// </remarks>
    public interface IAdvancedAsyncSession : IHideObjectMethods, IAdvancedAsyncReadOnlySession
    {
        /// <summary>
        /// Asynchronously deletes the database record with the specified identifier for the specified type.
        /// </summary>
        /// <param name="type">The type to delete.</param>
        /// <param name="identifier">The identifier of the record to delete.</param>
        /// <returns>A Task which can be awaited containing a result which indicates whether the object was deleted successfully.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified type or identifier is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the delete command.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         bool wasDeleted = await session.Advanced.DeleteAsync(type: typeof(Customer), identifier: 12823);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        Task<bool> DeleteAsync(Type type, object identifier);

        /// <summary>
        /// Asynchronously executes the specified SQL query and returns the number of rows affected.
        /// </summary>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <returns>A Task which can be awaited containing a result which indicates the number of rows affected by the SQL query.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified SqlQuery is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the command.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         var query = new SqlQuery("UPDATE Customers SET Locked = @p0 WHERE Locked = @p1", 0, 1);
        ///
        ///         int unlockedRowCount = await session.Advanced.ExecuteAsync(query);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        Task<int> ExecuteAsync(SqlQuery sqlQuery);

        /// <summary>
        /// Asynchronously executes the specified SQL query as a scalar command.
        /// </summary>
        /// <typeparam name="T">The type of result to be returned.</typeparam>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <returns>A Task which can be awaited containing a result which contains the result of the scalar query (the first column in the first row returned).</returns>
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
        ///         int customerCount = await session.Advanced.ExecuteScalarAsync&lt;int&gt;(query);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        Task<T> ExecuteScalarAsync<T>(SqlQuery sqlQuery);

        /// <summary>
        /// Asynchronously performs a partial update on a table row based upon the values specified in the object delta.
        /// </summary>
        /// <param name="objectDelta">The object delta containing the changes to be applied.</param>
        /// <returns>A Task which can be awaited containing a result which indicates whether the object was updated successfully.</returns>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         // Create an ObjectDelta which only updates specific properties:
        ///         var objectDelta = new ObjectDelta(type: typeof(Customer), identifier: 12823);
        ///         objectDelta.AddChange(propertyName: "Locked", newValue: 0); // Add as many or few changes as required.
        ///
        ///         bool wasUpdated = await session.Advanced.UpdateAsync(objectDelta);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        Task<bool> UpdateAsync(ObjectDelta objectDelta);
    }

#endif
}