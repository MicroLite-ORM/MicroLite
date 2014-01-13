// -----------------------------------------------------------------------
// <copyright file="ConnectionScope.cs" company="MicroLite">
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
    /// Defines the scope of a connection used by MicroLite.
    /// </summary>
    public enum ConnectionScope
    {
        /// <summary>
        /// The connection should be opened at the start of a transaction and closed at the end of the transaction.
        /// </summary>
        PerTransaction = 0,

        /// <summary>
        /// The connection should be opened at the start of a session and closed at the end of the session.
        /// </summary>
        PerSession = 1
    }
}