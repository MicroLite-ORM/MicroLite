// -----------------------------------------------------------------------
// <copyright file="AdoTransaction.cs" company="MicroLite">
// Copyright 2012 - 2013 Project Contributors
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
    /// The an implementation of <see cref="ITransaction"/> which manages an ADO transaction.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Transaction - Active:{IsActive}, Committed:{WasCommitted}, RolledBack:{WasRolledBack}")]
    internal sealed class AdoTransaction : ITransaction
    {
        private static readonly ILog log = LogManager.GetCurrentClassLog();
        private readonly IsolationLevel isolationLevel;
        private bool committed;
        private IDbConnection connection;
        private bool disposed;
        private bool failed;
        private bool rolledBack;
        private IDbTransaction transaction;

        /// <summary>
        /// Initialises a new instance of the <see cref="AdoTransaction"/> class.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <remarks>This is to enable easier unit testing only, all production code should call Transaction.Begin().</remarks>
        internal AdoTransaction(IDbTransaction transaction)
        {
            this.transaction = transaction;
            this.connection = transaction.Connection;
            this.isolationLevel = transaction.IsolationLevel;
        }

        public bool IsActive
        {
            get
            {
                return !this.committed && !this.rolledBack && !this.failed;
            }
        }

        public IsolationLevel IsolationLevel
        {
            get
            {
                return this.isolationLevel;
            }
        }

        public bool WasCommitted
        {
            get
            {
                return this.committed;
            }
        }

        public bool WasRolledBack
        {
            get
            {
                return this.rolledBack;
            }
        }

        public void Commit()
        {
            this.ThrowIfDisposed();
            this.ThrowIfNotActive();

            try
            {
                log.TryLogDebug(Messages.Transaction_Committing);
                this.transaction.Commit();
                log.TryLogDebug(Messages.Transaction_Committed);

                this.committed = true;

                this.connection.Close();
            }
            catch (Exception e)
            {
                this.failed = true;

                log.TryLogError(e.Message, e);
                throw new MicroLiteException(e.Message, e);
            }
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                if (this.IsActive)
                {
                    log.TryLogWarn(Messages.Transaction_DisposedUncommitted);
                    this.Rollback();
                }
                else if (this.failed && !this.rolledBack)
                {
                    log.TryLogWarn(Messages.Transaction_RollingBackFailedCommit);
                    this.Rollback();
                }

                this.transaction.Dispose();
                this.transaction = null;
                this.connection = null;

                log.TryLogDebug(Messages.Transaction_Disposed);
                this.disposed = true;
            }
        }

        public void Enlist(IDbCommand command)
        {
            if (command == null)
            {
                throw new ArgumentNullException("command");
            }

            if (this.IsActive)
            {
                log.TryLogDebug(Messages.Transaction_EnlistingCommand);
                command.Transaction = this.transaction;
            }
        }

        public void Rollback()
        {
            this.ThrowIfDisposed();
            this.ThrowIfRolledBackOrCommitted();

            try
            {
                log.TryLogDebug(Messages.Transaction_RollingBack);
                this.transaction.Rollback();
                log.TryLogDebug(Messages.Transaction_RolledBack);

                this.rolledBack = true;

                this.connection.Close();
            }
            catch (Exception e)
            {
                this.failed = true;

                log.TryLogError(e.Message, e);
                throw new MicroLiteException(e.Message, e);
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
                throw new InvalidOperationException(Messages.Transaction_Completed);
            }
        }

        private void ThrowIfRolledBackOrCommitted()
        {
            if (this.rolledBack || this.committed)
            {
                throw new InvalidOperationException(Messages.Transaction_Completed);
            }
        }
    }
}