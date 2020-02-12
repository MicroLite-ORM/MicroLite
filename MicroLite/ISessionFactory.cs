// -----------------------------------------------------------------------
// <copyright file="ISessionFactory.cs" company="Project Contributors">
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
using MicroLite.Driver;

namespace MicroLite
{
    /// <summary>
    /// The interface which defines the factory methods for creating MicroLite sessions.
    /// </summary>
    public interface ISessionFactory : IHideObjectMethods
    {
        /// <summary>
        /// Gets the name of the connection in the connection strings configuration section used by the session factory.
        /// </summary>
        string ConnectionName
        {
            get;
        }

        /// <summary>
        /// Gets the DB driver used by the session factory.
        /// </summary>
        IDbDriver DbDriver
        {
            get;
        }

        /// <summary>
        /// Opens a new asynchronous read-only session to the database using <see cref="ConnectionScope"/>.PerTransaction.
        /// </summary>
        /// <returns>A new asynchronous read-only database session.</returns>
        IAsyncReadOnlySession OpenAsyncReadOnlySession();

        /// <summary>
        /// Opens a new asynchronous read-only session to the database using the specified <see cref="ConnectionScope"/>.
        /// </summary>
        /// <param name="connectionScope">The connection scope to use for the session.</param>
        /// <returns>A new asynchronous read-only database session.</returns>
        IAsyncReadOnlySession OpenAsyncReadOnlySession(ConnectionScope connectionScope);

        /// <summary>
        /// Opens a new asynchronous session to the database using <see cref="ConnectionScope"/>.PerTransaction.
        /// </summary>
        /// <returns>A new asynchronous database session.</returns>
        IAsyncSession OpenAsyncSession();

        /// <summary>
        /// Opens a new asynchronous session to the database using the specified <see cref="ConnectionScope"/>.
        /// </summary>
        /// <param name="connectionScope">The connection scope to use for the session.</param>
        /// <returns>A new asynchronous database session.</returns>
        IAsyncSession OpenAsyncSession(ConnectionScope connectionScope);
    }
}
