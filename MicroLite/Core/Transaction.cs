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
    [System.Diagnostics.DebuggerDisplay("Transaction {id},  Committed {committed}")]
    internal sealed class Transaction : ITransaction
    {
        private static readonly ILog log = LogManager.GetLog("MicroLite.Transaction");
        private readonly Guid id = Guid.NewGuid();
        private bool committed;
        private bool disposed;
        private IDbTransaction transaction;

        internal Transaction(IDbTransaction transaction)
        {
            log.TryLogDebug(LogMessages.Transaction_Created, this.id.ToString());
            this.transaction = transaction;
        }

        internal event EventHandler Complete;

        public void Commit()
        {
            try
            {
                var connection = this.transaction.Connection;

                log.TryLogInfo(LogMessages.Transaction_Committing, this.id.ToString());
                this.transaction.Commit();

                this.committed = true;
                this.Complete.Raise(this);

                connection.Close();
            }
            catch (Exception e)
            {
                log.TryLogError(e.Message, e);
                throw new MicroLiteException(e.Message, e);
            }
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                if (!this.committed)
                {
                    log.TryLogWarn(LogMessages.Transaction_DisposedUncommitted, this.id.ToString());
                    this.Rollback();
                }

                this.transaction.Dispose();
                this.transaction = null;

                log.TryLogDebug(LogMessages.Transaction_Disposed, this.id.ToString());
                this.disposed = true;
            }
        }

        public void Rollback()
        {
            try
            {
                var connection = this.transaction.Connection;

                log.TryLogInfo(LogMessages.Transaction_RollingBack, this.id.ToString());
                this.transaction.Rollback();

                this.Complete.Raise(this);

                connection.Close();
            }
            catch (Exception e)
            {
                log.TryLogError(e.Message, e);
                throw new MicroLiteException(e.Message, e);
            }
        }

        internal void Enlist(IDbCommand command)
        {
            if (!this.committed)
            {
                log.TryLogInfo(LogMessages.Transaction_EnlistingCommand, this.id.ToString());
                command.Transaction = this.transaction;
            }
        }
    }
}