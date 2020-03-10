// -----------------------------------------------------------------------
// <copyright file="IAdvancedSession.cs" company="Project Contributors">
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
using System;
using System.Threading;
using System.Threading.Tasks;

namespace MicroLite
{
    /// <summary>
    /// The interface which provides access to advanced session operations.
    /// </summary>
    /// <remarks>
    /// These operations allow for more advanced use and have been moved to a separate interface to avoid
    /// cluttering the IAsyncSession API.
    /// </remarks>
    public interface IAdvancedSession : IAdvancedReadOnlySession
    {
        /// <summary>
        /// Asynchronously deletes the database record of the specified type with the specified identifier.
        /// </summary>
        /// <param name="type">The type to delete.</param>
        /// <param name="identifier">The identifier of the record to delete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified type or identifier is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the delete command.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncSession())
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
        /// <remarks>Invokes DeleteAsync(Type, object, CancellationToken) with CancellationToken.None.</remarks>
        Task<bool> DeleteAsync(Type type, object identifier);

        /// <summary>
        /// Asynchronously deletes the database record of the specified type with the specified identifier.
        /// This method propagates a notification that operations should be cancelled.
        /// </summary>
        /// <param name="type">The type to delete.</param>
        /// <param name="identifier">The identifier of the record to delete.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified type or identifier is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the delete command.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncSession())
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
        Task<bool> DeleteAsync(Type type, object identifier, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously executes the specified SQL query and returns the number of rows affected.
        /// </summary>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified SqlQuery is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the command.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         var query = new SqlQuery("UPDATE Customers SET Locked = @p0 WHERE Locked = @p1", false, true);
        ///
        ///         int unlockedRowCount = await session.Advanced.ExecuteAsync(query);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <remarks>Invokes ExecuteAsync(SqlQuery, CancellationToken) with CancellationToken.None.</remarks>
        Task<int> ExecuteAsync(SqlQuery sqlQuery);

        /// <summary>
        /// Asynchronously executes the specified SQL query and returns the number of rows affected.
        /// This method propagates a notification that operations should be cancelled.
        /// </summary>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified SqlQuery is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the command.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         var query = new SqlQuery("UPDATE Customers SET Locked = @p0 WHERE Locked = @p1", false, true);
        ///
        ///         int unlockedRowCount = await session.Advanced.ExecuteAsync(query);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        Task<int> ExecuteAsync(SqlQuery sqlQuery, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously executes the specified SQL query as a scalar command.
        /// </summary>
        /// <typeparam name="T">The type of result to be returned.</typeparam>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified SqlQuery is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the command.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncSession())
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
        /// <remarks>Invokes ExecuteScalarAsync(SqlQuery, CancellationToken) with CancellationToken.None.</remarks>
        Task<T> ExecuteScalarAsync<T>(SqlQuery sqlQuery);

        /// <summary>
        /// Asynchronously executes the specified SQL query as a scalar command.
        /// This method propagates a notification that operations should be cancelled.
        /// </summary>
        /// <typeparam name="T">The type of result to be returned.</typeparam>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified SqlQuery is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the command.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncSession())
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
        Task<T> ExecuteScalarAsync<T>(SqlQuery sqlQuery, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously performs a partial update on a table row based upon the values specified in the object delta.
        /// </summary>
        /// <param name="objectDelta">The object delta containing the changes to be applied.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         // Create an ObjectDelta which only updates specific properties:
        ///         var objectDelta = new ObjectDelta(type: typeof(Customer), identifier: 12823);
        ///         objectDelta.AddChange(propertyName: "Locked", newValue: false); // Add 1 or more changes.
        ///
        ///         bool wasUpdated = await session.Advanced.UpdateAsync(objectDelta);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <remarks>Invokes UpdateAsync(ObjectDelta, CancellationToken) with CancellationToken.None.</remarks>
        Task<bool> UpdateAsync(ObjectDelta objectDelta);

        /// <summary>
        /// Asynchronously performs a partial update on a table row based upon the values specified in the object delta.
        /// This method propagates a notification that operations should be cancelled.
        /// </summary>
        /// <param name="objectDelta">The object delta containing the changes to be applied.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         // Create an ObjectDelta which only updates specific properties:
        ///         var objectDelta = new ObjectDelta(type: typeof(Customer), identifier: 12823);
        ///         objectDelta.AddChange(propertyName: "Locked", newValue: false); // Add 1 or more changes.
        ///
        ///         bool wasUpdated = await session.Advanced.UpdateAsync(objectDelta);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        Task<bool> UpdateAsync(ObjectDelta objectDelta, CancellationToken cancellationToken);
    }
}
