// -----------------------------------------------------------------------
// <copyright file="ITransaction.cs" company="Project Contributors">
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

namespace MicroLite
{
    /// <summary>
    /// The interface for a database transaction.
    /// </summary>
    public interface ITransaction : IDisposable
    {
        /// <summary>
        /// Gets a value indicating whether this transaction is active.
        /// </summary>
        bool IsActive { get; }

        /// <summary>
        /// Commits the transaction, applying all changes made within the transaction scope.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the transaction has been completed.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error committing the transaction.</exception>
        void Commit();

        /// <summary>
        /// Rollbacks the transaction, undoing all changes made within the transaction scope.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if the transaction has been completed.</exception>
        /// <exception cref="MicroLiteException">Thrown if there is an error rolling back the transaction.</exception>
        void Rollback();
    }
}
