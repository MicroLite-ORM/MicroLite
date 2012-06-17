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
    [System.Diagnostics.DebuggerDisplay("Transaction {id},  Committed {WasCommitted}")]
    internal sealed class Transaction : ITransaction
    {
        private static readonly ILog log = LogManager.GetLog("MicroLite.Transaction");
        private readonly Guid id = Guid.NewGuid();
        private bool disposed;
        private IDbTransaction transaction;
        private bool wasCommitted;

        internal Transaction(IDbTransaction transaction)
        {
            log.TryLogDebug(LogMessages.Transaction_Created, this.id.ToString());
            this.transaction = transaction;
        }

        internal bool WasCommitted
        {
            get
            {
                return this.wasCommitted;
            }
        }

        public void Commit()
        {
            try
            {
                log.TryLogInfo(LogMessages.Transaction_Committing, this.id.ToString());
                this.transaction.Commit();
                this.transaction.Connection.Close();
                this.wasCommitted = true;
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
                if (!this.wasCommitted)
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
                log.TryLogInfo(LogMessages.Transaction_RollingBack, this.id.ToString());
                this.transaction.Rollback();
                this.transaction.Connection.Close();
            }
            catch (Exception e)
            {
                log.TryLogError(e.Message, e);
                throw new MicroLiteException(e.Message, e);
            }
        }

        internal void Enlist(IDbCommand command)
        {
            if (!this.WasCommitted)
            {
                log.TryLogInfo(LogMessages.Transaction_EnlistingCommand, this.id.ToString());
                command.Transaction = this.transaction;
            }
        }
    }
}