// -----------------------------------------------------------------------
// <copyright file="ISession.cs" company="MicroLite">
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
namespace MicroLite
{
    using System;

    /// <summary>
    /// The interface which provides the write methods to map objects to database records.
    /// </summary>
    public interface ISession : IHideObjectMethods, IReadOnlySession
    {
        /// <summary>
        /// Gets the advanced session operations.
        /// </summary>
        new IAdvancedSession Advanced
        {
            get;
        }

        /// <summary>
        /// Deletes the database record for the specified instance.
        /// </summary>
        /// <param name="instance">The instance to delete from the database.</param>
        /// <returns>true if the object was deleted successfully; otherwise false.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the delete command.</exception>
        /// <example>
        /// <code>
        /// bool deleted = false;
        ///
        /// using (var session = sessionFactory.OpenSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         try
        ///         {
        ///             deleted = session.Delete(customer);
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
        bool Delete(object instance);

        /// <summary>
        /// Inserts a new database record for the specified instance.
        /// </summary>
        /// <param name="instance">The instance to persist the values for.</param>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the insert command.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         session.Insert(customer);
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        void Insert(object instance);

        /// <summary>
        /// Performs an insert or update in the database depending on whether the object is considered new (e.g. it has no identifier set).
        /// </summary>
        /// <param name="instance">The instance to persist the values for.</param>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the update command.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         session.InsertOrUpdate(customer);
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        void InsertOrUpdate(object instance);

        /// <summary>
        /// Updates the database record for the specified instance with the current property values.
        /// </summary>
        /// <param name="instance">The instance to persist the values for.</param>
        /// <returns>true if the object was updated successfully; otherwise false.</returns>
        /// <exception cref="ObjectDisposedException">Thrown if the session has been disposed.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the specified instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error executing the update command.</exception>
        /// <example>
        /// <code>
        /// using (var session = sessionFactory.OpenSession())
        /// {
        ///     using (var transaction = session.BeginTransaction())
        ///     {
        ///         session.Update(customer);
        ///         transaction.Commit();
        ///     }
        /// }
        /// </code>
        /// </example>
        bool Update(object instance);
    }
}