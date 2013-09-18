// -----------------------------------------------------------------------
// <copyright file="IConnectionManager.cs" company="MicroLite">
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
namespace MicroLite.Core
{
    using System;
    using System.Data;

    /// <summary>
    /// The interface for a class which manages an IDbConnection.
    /// </summary>
    internal interface IConnectionManager : IDisposable
    {
        /// <summary>
        /// Gets the current transaction or null if no transaction has been started.
        /// </summary>
        ITransaction CurrentTransaction
        {
            get;
        }

        /// <summary>
        /// Begins the transaction with the specified isolation level.
        /// </summary>
        /// <param name="isolationLevel">The isolation level to use for the transaction.</param>
        /// <returns>An <see cref="ITransaction"/> with the specified <see cref="IsolationLevel"/>.</returns>
        ITransaction BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Called when the command is completed to free any resources which are no longer needed.
        /// </summary>
        /// <param name="command">The completed command.</param>
        void CommandCompleted(IDbCommand command);

        /// <summary>
        /// Creates a new IDbCommand for the managed connection which will be enlisted in the active transaction.
        /// </summary>
        /// <returns>The IDbCommand for the connection.</returns>
        IDbCommand CreateCommand();
    }
}