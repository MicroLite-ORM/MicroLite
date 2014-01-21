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
    using MicroLite.Logging;

    /// <summary>
    /// The base class for a session.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ConnectionScope: {ConnectionScope}")]
    internal abstract class SessionBase : ISessionBase, IDisposable
    {
        protected static readonly ILog Log = LogManager.GetCurrentClassLog();
        private readonly ConnectionScope connectionScope;
        private Transaction currentTransaction;
        private bool disposed;

        protected SessionBase(ConnectionScope connectionScope, IDbConnection connection)
        {
            this.connectionScope = connectionScope;
            this.Connection = connection;

            if (this.connectionScope == ConnectionScope.PerSession)
            {
                if (Log.IsDebug)
                {
                    Log.Debug(Messages.OpeningConnection);
                }

                this.Connection.Open();
            }
        }

        public IDbConnection Connection
        {
            get;
            private set;
        }

        public ConnectionScope ConnectionScope
        {
            get
            {
                return this.connectionScope;
            }
        }

        public ITransaction CurrentTransaction
        {
            get
            {
                return this.currentTransaction;
            }
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
            this.currentTransaction = null;
        }

        protected void CommandCompleted()
        {
            if (this.ConnectionScope == ConnectionScope.PerTransaction
                && this.currentTransaction == null
                && this.Connection.State == ConnectionState.Open)
            {
                if (Log.IsDebug)
                {
                    Log.Debug(Messages.ClosingConnection);
                }

                this.Connection.Close();
            }
        }

        protected IDbCommand CreateCommand()
        {
            if (this.ConnectionScope == ConnectionScope.PerTransaction
                && this.currentTransaction == null
                && this.Connection.State == ConnectionState.Closed)
            {
                if (Log.IsDebug)
                {
                    Log.Debug(Messages.OpeningConnection);
                }

                this.Connection.Open();
            }

            var command = this.Connection.CreateCommand();

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

                if (this.connectionScope == ConnectionScope.PerSession)
                {
                    if (Log.IsDebug)
                    {
                        Log.Debug(Messages.ClosingConnection);
                    }

                    this.Connection.Close();
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