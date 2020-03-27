// -----------------------------------------------------------------------
// <copyright file="ConnectionScope.cs" company="Project Contributors">
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
namespace MicroLite
{
    /// <summary>
    /// An enumeration which defines when a connection used by MicroLite is opened and closed.
    /// </summary>
    public enum ConnectionScope
    {
        /// <summary>
        /// The connection should be opened when a transaction is started and closed when the transaction is completed (default).
        /// </summary>
        /// <remarks>This is the default behaviour in 5.0 and the only available behaviour prior to 5.0.</remarks>
        PerTransaction = 0,

        /// <summary>
        /// The connection should be opened when a session is created and and closed when the session is disposed.
        /// </summary>
        /// <remarks>
        /// Use this option with caution, it exists mostly for use where opening a connection is expensive
        /// and multiple transactions are to be used within a single session,
        /// or for SQLite in memory databases which only persist data whilst a connection exists.
        /// </remarks>
        PerSession = 1,
    }
}
