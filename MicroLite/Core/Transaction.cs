// -----------------------------------------------------------------------
// <copyright file="Transaction.cs" company="MicroLite">
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
namespace MicroLite.Core
{
    using System.Data;

    /// <summary>
    /// The class which manages beginning a transaction.
    /// </summary>
    internal static class Transaction
    {
        internal static ITransaction Begin(IDbConnection connection, IsolationLevel isolationLevel)
        {
            connection.Open();

            var dbTransaction = connection.BeginTransaction(isolationLevel);

            return new AdoTransaction(dbTransaction);
        }
    }
}