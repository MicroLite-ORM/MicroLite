// -----------------------------------------------------------------------
// <copyright file="IAsyncSession.cs" company="Project Contributors">
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
    /// The interface which provides the asynchronous write methods to map objects to database records.
    /// </summary>
    public interface IAsyncSession : IHideObjectMethods, IAsyncReadOnlySession
    {
        /// <summary>
        /// Gets the advanced session operations.
        /// </summary>
        new IAdvancedAsyncSession Advanced
        {
            get;
        }

        /// <summary>
        /// Asynchronously deletes the database record for the specified instance.
        /// </summary>
        /// <param name="instance">The instance to delete from the database.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the delete command.</exception>
        /// <example>
        /// <code>
        /// bool deleted = false;
        ///
        /// using (var session = sessionFactory.OpenAsyncSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         try
        ///         {
        ///             deleted = await session.DeleteAsync(customer);
        ///
        ///             transaction.Commit();
        ///         }
        ///         catch
        ///         {
        ///             deleted = false;
        ///
        ///             transaction.Rollback();
        ///             // Log or throw the exception.
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <remarks>Invokes DeleteAsync(object, CancellationToken) with CancellationToken.None.</remarks>
        Task<bool> DeleteAsync(object instance);

        /// <summary>
        /// Asynchronously deletes the database record for the specified instance.
        /// This method propagates a notification that operations should be cancelled.
        /// </summary>
        /// <param name="instance">The instance to delete from the database.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the delete command.</exception>
        /// <example>
        /// <code>
        /// bool deleted = false;
        ///
        /// using (var session = sessionFactory.OpenAsyncSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         try
        ///         {
        ///             deleted = await session.DeleteAsync(customer);
        ///
        ///             transaction.Commit();
        ///         }
        ///         catch
        ///         {
        ///             deleted = false;
        ///
        ///             transaction.Rollback();
        ///             // Log or throw the exception.
        ///         }
        ///     }
        /// }
        /// </code>
        /// </example>
        Task<bool> DeleteAsync(object instance, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously inserts a new database record for the specified instance.
        /// </summary>
        /// <param name="instance">The instance to persist the values for.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the insert command.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         await session.InsertAsync(customer);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <remarks>Invokes InsertAsync(object, CancellationToken) with CancellationToken.None.</remarks>
        Task InsertAsync(object instance);

        /// <summary>
        /// Asynchronously inserts a new database record for the specified instance.
        /// This method propagates a notification that operations should be cancelled.
        /// </summary>
        /// <param name="instance">The instance to persist the values for.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the insert command.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         await session.InsertAsync(customer);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        Task InsertAsync(object instance, CancellationToken cancellationToken);

        /// <summary>
        /// Asynchronously updates the database record for the specified instance with the current property values.
        /// </summary>
        /// <param name="instance">The instance to persist the values for.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the update command.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         await session.UpdateAsync(customer);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        /// <remarks>Invokes UpdateAsync(object, CancellationToken) with CancellationToken.None.</remarks>
        Task<bool> UpdateAsync(object instance);

        /// <summary>
        /// Asynchronously updates the database record for the specified instance with the current property values.
        /// This method propagates a notification that operations should be cancelled.
        /// </summary>
        /// <param name="instance">The instance to persist the values for.</param>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the update command.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenAsyncSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         await session.UpdateAsync(customer);
        ///
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        Task<bool> UpdateAsync(object instance, CancellationToken cancellationToken);
    }
}
