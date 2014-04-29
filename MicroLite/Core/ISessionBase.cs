// -----------------------------------------------------------------------
// <copyright file="ISessionBase.cs" company="MicroLite">
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
namespace MicroLite.Core
{
    using System.Data;

    /// <summary>
    /// An abstraction for the session base which de-couples the dependency for the Transaction class.
    /// </summary>
    internal interface ISessionBase
    {
        /// <summary>
        /// Gets the connection.
        /// </summary>
        IDbConnection Connection
        {
            get;
        }

        /// <summary>
        /// Informs the session that the Transaction has been completed.
        /// </summary>
        void TransactionCompleted();
    }
}