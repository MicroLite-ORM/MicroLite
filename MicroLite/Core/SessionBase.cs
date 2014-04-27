// -----------------------------------------------------------------------
// <copyright file="SessionBase.cs" company="MicroLite">
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
    using System;
    using System.Data;
    using MicroLite.Driver;
    using MicroLite.Logging;

    /// <summary>
    /// The base class for a session.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ConnectionScope: {ConnectionScope}")]
    internal abstract class SessionBase : ISessionBase, IDisposable
    {
        protected static readonly ILog Log = LogManager.GetCurrentClassLog();
        private Transaction currentTransaction;
        private bool disposed;

        protected SessionBase(ConnectionScope connectionScope, IDbDriver dbDriver)
        {
            this.Connection = dbDriver.GetConnection(connectionScope);
            this.ConnectionScope = connectionScope;
            this.DbDriver = dbDriver;
        }

        public IDbConnection Connection
        {
            get;
            private set;
        }

        public ConnectionScope ConnectionScope
        {
            get;
            private set;
        }

        public ITransaction CurrentTransaction
        {
            get
            {
                return this.currentTransaction;
            }
        }

        protected IDbDriver DbDriver
        {
            get;
            private set;
        }

        public ITransaction BeginTransaction()
        {
            return this.BeginTransaction(IsolationLevel.ReadCommitted);
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            this.ThrowIfDisposed();

            this.currentTransaction = new Transaction(this, isolationLevel);

            return this.currentTransaction;
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void TransactionCompleted()
        {
            if (this.ConnectionScope == ConnectionScope.PerTransaction)
            {
                if (Log.IsDebug)
                {
                    Log.Debug(LogMessages.ClosingConnection);
                }

                this.Connection.Close();
            }

            this.currentTransaction = null;
        }

        protected void CommandCompleted()
        {
            if (this.ConnectionScope == ConnectionScope.PerTransaction
                && this.Connection.State == ConnectionState.Open
                && this.currentTransaction == null)
            {
                if (Log.IsDebug)
                {
                    Log.Debug(LogMessages.ClosingConnection);
                }

                this.Connection.Close();
            }
        }

        protected IDbCommand CreateCommand(SqlQuery sqlQuery)
        {
            if (this.ConnectionScope == ConnectionScope.PerTransaction
                && this.Connection.State == ConnectionState.Closed
                && this.currentTransaction == null)
            {
                if (Log.IsDebug)
                {
                    Log.Debug(LogMessages.OpeningConnection);
                }

                this.Connection.Open();
            }

            var command = this.DbDriver.BuildCommand(sqlQuery);
            command.Connection = this.Connection;

            if (this.currentTransaction != null)
            {
                this.currentTransaction.Enlist(command);
            }

            return command;
        }

        protected void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                this.disposed = true;

                if (this.currentTransaction != null)
                {
                    this.currentTransaction.Dispose();
                    this.currentTransaction = null;
                }

                if (this.ConnectionScope == ConnectionScope.PerSession)
                {
                    if (this.Connection != null)
                    {
                        if (Log.IsDebug)
                        {
                            Log.Debug(LogMessages.ClosingConnection);
                        }

                        this.Connection.Close();
                    }
                }

                if (this.Connection != null)
                {
                    this.Connection.Dispose();
                    this.Connection = null;
                }
            }
        }

        protected void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }
    }
}