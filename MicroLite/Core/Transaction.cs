// -----------------------------------------------------------------------
// <copyright file="Transaction.cs" company="MicroLite">
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
    /// The default implementation of <see cref="ITransaction"/>.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Active: {IsActive}, Committed: {WasCommitted}, RolledBack: {WasRolledBack}")]
    internal sealed class Transaction : ITransaction
    {
        private static readonly ILog log = LogManager.GetCurrentClassLog();
        private bool committed;
        private bool disposed;
        private bool failed;
        private bool rolledBack;
        private ISessionBase sessionBase;
        private IDbTransaction transaction;

        /// <summary>
        /// Initialises a new instance of the <see cref="Transaction" /> class.
        /// </summary>
        /// <param name="sessionBase">The session that the transaction is being created for.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        internal Transaction(ISessionBase sessionBase, IsolationLevel isolationLevel)
        {
            this.sessionBase = sessionBase;

            if (log.IsDebug)
            {
                log.Debug(LogMessages.Transaction_BeginTransactionWithIsolationLevel, isolationLevel.ToString());
            }

            this.transaction = this.sessionBase.Connection.BeginTransaction(isolationLevel);
        }

        public bool IsActive
        {
            get
            {
                return !this.committed && !this.rolledBack && !this.failed;
            }
        }

        public void Commit()
        {
            this.ThrowIfDisposed();
            this.ThrowIfNotActive();

            try
            {
                if (log.IsDebug)
                {
                    log.Debug(LogMessages.Transaction_Committing);
                }

                this.transaction.Commit();
                this.sessionBase.TransactionCompleted();
                this.committed = true;

                if (log.IsDebug)
                {
                    log.Debug(LogMessages.Transaction_Committed);
                }
            }
            catch (Exception e)
            {
                this.failed = true;

                throw new MicroLiteException(e.Message, e);
            }
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Rollback()
        {
            this.ThrowIfDisposed();
            this.ThrowIfRolledBackOrCommitted();

            try
            {
                if (log.IsDebug)
                {
                    log.Debug(LogMessages.Transaction_RollingBack);
                }

                this.transaction.Rollback();
                this.sessionBase.TransactionCompleted();
                this.rolledBack = true;

                if (log.IsDebug)
                {
                    log.Debug(LogMessages.Transaction_RolledBack);
                }
            }
            catch (Exception e)
            {
                this.failed = true;

                throw new MicroLiteException(e.Message, e);
            }
        }

        internal void Enlist(IDbCommand command)
        {
            if (!this.IsActive)
            {
                throw new InvalidOperationException();
            }

            command.Transaction = this.transaction;
        }

        private void Dispose(bool disposing)
        {
            if (this.disposed)
            {
                return;
            }

            if (disposing)
            {
                if (this.IsActive)
                {
                    log.Warn(LogMessages.Transaction_DisposedUncommitted);
                    this.Rollback();
                }
                else if (this.failed && !this.rolledBack)
                {
                    log.Warn(LogMessages.Transaction_RollingBackFailedCommit);
                    this.Rollback();
                }

                this.transaction.Dispose();
                this.transaction = null;
                this.sessionBase = null;
                this.disposed = true;

                if (log.IsDebug)
                {
                    log.Debug(LogMessages.Transaction_Disposed);
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (this.disposed)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        private void ThrowIfNotActive()
        {
            if (!this.IsActive)
            {
                throw new InvalidOperationException(ExceptionMessages.Transaction_AlreadyCompleted);
            }
        }

        private void ThrowIfRolledBackOrCommitted()
        {
            if (this.rolledBack || this.committed)
            {
                throw new InvalidOperationException(ExceptionMessages.Transaction_AlreadyCompleted);
            }
        }
    }
}