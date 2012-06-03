// -----------------------------------------------------------------------
// <copyright file="ITransaction.cs" company="MicroLite">
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

    /// <summary>
    /// The interface for a database transaction.
    /// </summary>
    public interface ITransaction : IHideObjectMethods, IDisposable
    {
        /// <summary>
        /// Commits the transaction, applying all changes made within the transaction scope.
        /// </summary>
        /// <exception cref="MicroLiteException">Thrown if there is an error committing the transaction.</exception>
        void Commit();

        /// <summary>
        /// Rollbacks the transaction, undoing all changes made within the transaction scope.
        /// </summary>
        /// <exception cref="MicroLiteException">Thrown if there is an error rolling back the transaction.</exception>
        void Rollback();
    }
}