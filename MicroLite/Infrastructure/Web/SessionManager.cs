// -----------------------------------------------------------------------
// <copyright file="SessionManager.cs" company="MicroLite">
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
    /// The default implementation of <see cref="ISessionManager"/>.
    /// </summary>
    /// <remarks>
    /// Important that this class remains thread safe as it can exist as a singleton in the MVC and WebAPI extensions
    /// when the action filter is registered in global filters.
    /// </remarks>
    public sealed class SessionManager : ISessionManager
    {
        /// <summary>
        /// Called when the action has been executed.
        /// </summary>
        /// <param name="session">The session used for the request.</param>
        /// <param name="manageTransaction">A value indicating whether the transaction is managed by the session manager.</param>
        /// <param name="hasException">A value indicating whether there was an exception during execution.</param>
        public void OnActionExecuted(IReadOnlySession session, bool manageTransaction, bool hasException)
        {
            if (session != null)
            {
                if (manageTransaction && session.Transaction != null)
                {
                    if (hasException)
                    {
                        if (!session.Transaction.WasRolledBack)
                        {
                            session.Transaction.Rollback();
                        }
                    }
                    else
                    {
                        if (session.Transaction.IsActive)
                        {
                            session.Transaction.Commit();
                        }
                    }
                }

                session.Dispose();
            }
        }

        /// <summary>
        /// Called when when the action is executing.
        /// </summary>
        /// <param name="session">The session used for the request.</param>
        /// <param name="manageTransaction">A value indicating whether the transaction is managed by the session manager.</param>
        /// <param name="isolationLevel">The optional isolation level of the managed transaction.</param>
        public void OnActionExecuting(IReadOnlySession session, bool manageTransaction, IsolationLevel? isolationLevel)
        {
            if (session != null)
            {
                if (manageTransaction)
                {
                    if (isolationLevel.HasValue)
                    {
                        session.BeginTransaction(isolationLevel.Value);
                    }
                    else
                    {
                        session.BeginTransaction();
                    }
                }
            }
        }
    }
}