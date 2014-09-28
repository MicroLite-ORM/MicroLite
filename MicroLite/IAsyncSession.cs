// -----------------------------------------------------------------------
// <copyright file="IAsyncSession.cs" company="MicroLite">
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
        /// <returns>A Task which can be awaited containing a result which indicates whether the object was deleted successfully.</returns>
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
        Task<bool> DeleteAsync(object instance);

        /// <summary>
        /// Asynchronously inserts a new database record for the specified instance.
        /// </summary>
        /// <param name="instance">The instance to persist the values for.</param>
        /// <returns>A Task which can be awaited.</returns>
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
        Task InsertAsync(object instance);

        /// <summary>
        /// Asynchronously updates the database record for the specified instance with the current property values.
        /// </summary>
        /// <param name="instance">The instance to persist the values for.</param>
        /// <returns>A Task which can be awaited containing a result which indicates whether the object was updated successfully.</returns>
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
        Task<bool> UpdateAsync(object instance);
    }

#endif
}