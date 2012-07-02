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
    using MicroLite.FrameworkExtensions;
    using MicroLite.Logging;

    /// <summary>
    /// The default implementation of <see cref="ITransaction"/>.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Transaction {id} - Active:{Active}, Committed:{Committed}, RolledBack:{RolledBack}")]
    internal sealed class Transaction : ITransaction
    {
        private static readonly ILog log = LogManager.GetLog("MicroLite.Transaction");
        private readonly string id = Guid.NewGuid().ToString();
        private bool failed;
        private bool committed;
        private bool disposed;
        private bool rolledBack;
        private IDbTransaction transaction;

        internal Transaction(IDbTransaction transaction)
        {
            log.TryLogDebug(Messages.Transaction_Created, this.id);
            this.transaction = transaction;
        }

        internal event EventHandler Complete;

        public bool Committed
        {
            get
            {
                return this.committed;
            }
        }

        public bool IsActive
        {
            get
            {
                return !this.committed && !this.rolledBack && !this.failed;
            }
        }

        public bool RolledBack
        {
            get
            {
                return this.rolledBack;
            }
        }

        public void Commit()
        {
            try
            {
                var connection = this.transaction.Connection;

                log.TryLogInfo(Messages.Transaction_Committing, this.id);
                this.transaction.Commit();

                this.committed = true;
                this.Complete.Raise(this);

                connection.Close();
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
                if (!this.committed && !this.rolledBack)
                {
                    log.TryLogWarn(Messages.Transaction_DisposedUncommitted, this.id);
                    this.Rollback();
                }

                this.transaction.Dispose();
                this.transaction = null;

                log.TryLogDebug(Messages.Transaction_Disposed, this.id);
                this.disposed = true;
            }
        }

        public void Rollback()
        {
            try
            {
                var connection = this.transaction.Connection;

                log.TryLogInfo(Messages.Transaction_RollingBack, this.id);
                this.transaction.Rollback();

                this.rolledBack = true;

                this.Complete.Raise(this);

                connection.Close();
            }
            catch (Exception e)
            {
                this.failed = true;

                log.TryLogError(e.Message, e);
                throw new MicroLiteException(e.Message, e);
            }
        }

        internal void Enlist(IDbCommand command)
        {
            if (!this.committed)
            {
                log.TryLogInfo(Messages.Transaction_EnlistingCommand, this.id);
                command.Transaction = this.transaction;
            }
        }
    }
}