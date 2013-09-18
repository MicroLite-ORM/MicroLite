// -----------------------------------------------------------------------
// <copyright file="ITransaction.cs" company="MicroLite">
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
    using System.Data;

    /// <summary>
    /// The interface for a database transaction.
    /// </summary>
    public interface ITransaction : IHideObjectMethods, IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether this transaction is active.
        /// </summary>
        bool IsActive
        {
            get;
        }

        /// <summary>
        /// Gets the isolation level of the transaction.
        /// </summary>
        IsolationLevel IsolationLevel
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether this transaction has been committed.
        /// </summary>
        bool WasCommitted
        {
            get;
        }

        /// <summary>
        /// Gets a value indicating whether this transaction has been rolled back.
        /// </summary>
        bool WasRolledBack
        {
            get;
        }

        /// <summary>
        /// Commits the transaction, applying all changes made within the transaction scope.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the transaction has been completed.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error committing the transaction.</exception>
        void Commit();

        /// <summary>
        /// Enlists the specified command in the transaction if the transaction is active.
        /// </summary>
        /// <param name="command">The command to be enlisted.</param>
        void Enlist(IDbCommand command);

        /// <summary>
        /// Rollbacks the transaction, undoing all changes made within the transaction scope.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the transaction has been completed.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error rolling back the transaction.</exception>
        void Rollback();
    }
}