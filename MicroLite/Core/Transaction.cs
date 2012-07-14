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
    using System;
    using System.Data;
    using MicroLite.Logging;

    /// <summary>
    /// The default implementation of <see cref="ITransaction"/>.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Transaction {id} - Active:{IsActive}, Committed:{WasCommitted}, RolledBack:{WasRolledBack}")]
    internal sealed class Transaction : ITransaction
    {
        private static readonly ILog log = LogManager.GetLog("MicroLite.Transaction");
        private readonly string id = Guid.NewGuid().ToString();
        private bool committed;
        private IDbConnection connection;
        private bool disposed;
        private bool failed;
        private bool rolledBack;
        private IDbTransaction transaction;

        /// <summary>
        /// Initialises a new instance of the <see cref="Transaction"/> class.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        /// <remarks>This is to enable easier unit testing only, all production code should call Transaction.Begin().</remarks>
        internal Transaction(IDbTransaction transaction)
        {
            this.transaction = transaction;
            this.connection = transaction.Connection;

            log.TryLogDebug(Messages.Transaction_Created, this.id);
        }

        public bool IsActive
        {
            get
            {
                return !this.committed && !this.rolledBack && !this.failed;
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
                log.TryLogInfo(Messages.Transaction_Committing, this.id);
                this.transaction.Commit();
                log.TryLogInfo(Messages.Transaction_Committed, this.id);

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
                    log.TryLogWarn(Messages.Transaction_DisposedUncommitted, this.id);
                    this.Rollback();
                }

                this.transaction.Dispose();
                this.transaction = null;
                this.connection = null;

                log.TryLogDebug(Messages.Transaction_Disposed, this.id);
                this.disposed = true;
            }
        }

        public void Rollback()
        {
            this.ThrowIfDisposed();
            this.ThrowIfRolledBackOrCommitted();

            try
            {
                log.TryLogInfo(Messages.Transaction_RollingBack, this.id);
                this.transaction.Rollback();
                log.TryLogInfo(Messages.Transaction_RolledBack, this.id);

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

        internal static Transaction Begin(IDbConnection connection)
        {
            connection.Open();

            var dbTransaction = connection.BeginTransaction();

            return new Transaction(dbTransaction);
        }

        internal static Transaction Begin(IDbConnection connection, IsolationLevel isolationLevel)
        {
            connection.Open();

            var dbTransaction = connection.BeginTransaction(isolationLevel);

            return new Transaction(dbTransaction);
        }

        internal void Enlist(IDbCommand command)
        {
            if (this.IsActive)
            {
                log.TryLogInfo(Messages.Transaction_EnlistingCommand, this.id);
                command.Transaction = this.transaction;
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