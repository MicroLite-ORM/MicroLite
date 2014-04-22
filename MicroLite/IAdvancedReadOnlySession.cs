// -----------------------------------------------------------------------
// <copyright file="IAdvancedReadOnlySession.cs" company="MicroLite">
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
    /// <summary>
    /// The interface which provides access to advanced read only session operations.
    /// </summary>
    /// <remarks>
    /// These operations allow for more advanced use and have been moved to a separate interface to avoid
    /// cluttering the IReadOnlySession API.
    /// </remarks>
    public interface IAdvancedReadOnlySession : IHideObjectMethods
    {
        /// <summary>
        /// Executes any pending queries which have been queued using the Include API.
        /// </summary>
        void ExecutePendingQueries();
    }
}