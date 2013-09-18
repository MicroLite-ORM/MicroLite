// -----------------------------------------------------------------------
// <copyright file="ISessionManager.cs" company="MicroLite">
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
namespace MicroLite.Infrastructure.Web
{
    using System.Data;

    /// <summary>
    /// The interface for a class which manages a session during the action execution context of a web request.
    /// </summary>
    public interface ISessionManager
    {
        /// <summary>
        /// Called when the action has been executed.
        /// </summary>
        /// <param name="session">The session used for the request.</param>
        /// <param name="manageTransaction">A value indicating whether the transaction is managed by the session manager.</param>
        /// <param name="hasException">A value indicating whether there was an exception during execution.</param>
        void OnActionExecuted(IReadOnlySession session, bool manageTransaction, bool hasException);

        /// <summary>
        /// Called when when the action is executing.
        /// </summary>
        /// <param name="session">The session used for the request.</param>
        /// <param name="manageTransaction">A value indicating whether the transaction is managed by the session manager.</param>
        /// <param name="isolationLevel">The optional isolation level of the managed transaction.</param>
        void OnActionExecuting(IReadOnlySession session, bool manageTransaction, IsolationLevel? isolationLevel);
    }
}