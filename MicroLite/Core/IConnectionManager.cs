// -----------------------------------------------------------------------
// <copyright file="IConnectionManager.cs" company="MicroLite">
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
    using System;
    using System.Data;

    internal interface IConnectionManager : IDisposable
    {
        ITransaction CurrentTransaction
        {
            get;
        }

        ITransaction BeginTransaction();

        ITransaction BeginTransaction(System.Data.IsolationLevel isolationLevel);

        IDbCommand BuildCommand(SqlQuery sqlQuery);
    }
}