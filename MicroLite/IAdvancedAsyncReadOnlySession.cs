// -----------------------------------------------------------------------
// <copyright file="IAdvancedAsyncReadOnlySession.cs" company="MicroLite">
// Copyright 2012 - 2015 Project Contributors
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

    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The interface which provides access to advanced read-only asynchronous session operations.
    /// </summary>
    /// <remarks>
    /// These operations allow for more advanced use and have been moved to a separate interface to avoid
    /// cluttering the IAsyncReadOnlySession API.
    /// </remarks>
    public interface IAdvancedAsyncReadOnlySession : IHideObjectMethods
    {
        /// <summary>
        /// Asynchronously executes any pending queries which have been queued using the Include API.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        /// <remarks>Invokes ExecutePendingQueriesAsync(CancellationToken) with CancellationToken.None.</remarks>
        Task ExecutePendingQueriesAsync();

        /// <summary>
        /// Asynchronously executes any pending queries which have been queued using the Include API.
        /// This method propagates a notification that operations should be cancelled.
        /// </summary>
        /// <param name="cancellationToken">The token to monitor for cancellation requests.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task ExecutePendingQueriesAsync(CancellationToken cancellationToken);
    }

#endif
}