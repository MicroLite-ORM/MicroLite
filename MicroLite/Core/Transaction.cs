// -----------------------------------------------------------------------
// <copyright file="Transaction.cs" company="Project Contributors">
// Copyright Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Data;
using MicroLite.Logging;

namespace MicroLite.Core
{
    /// <summary>
    /// The default implementation of <see cref="ITransaction"/>.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Active: {IsActive}, Committed: {committed}, RolledBack: {rolledBack}")]
    internal sealed class Transaction : ITransaction
    {
        private static readonly ILog s_log = LogManager.GetCurrentClassLog();
        private bool _committed;
        private bool _disposed;
        private bool _failed;
        private bool _rolledBack;
        private ISessionBase _sessionBase;
        private IDbTransaction _transaction;

        /// <summary>
        /// Initialises a new instance of the <see cref="Transaction" /> class.
        /// </summary>
        /// <param name="sessionBase">The session that the transaction is being created for.</param>
        /// <param name="isolationLevel">The isolation level.</param>
        internal Transaction(ISessionBase sessionBase, IsolationLevel isolationLevel)
        {
            _sessionBase = sessionBase;

            if (s_log.IsDebug)
            {
                s_log.Debug(LogMessages.Transaction_BeginTransactionWithIsolationLevel, isolationLevel.ToString());
            }

            _transaction = _sessionBase.Connection.BeginTransaction(isolationLevel);
        }

        public bool IsActive => !_committed && !_rolledBack && !_failed;

        public void Commit()
        {
            ThrowIfDisposed();
            ThrowIfNotActive();

            try
            {
                if (s_log.IsDebug)
                {
                    s_log.Debug(LogMessages.Transaction_Committing);
                }

                _transaction.Commit();
                _sessionBase.TransactionCompleted();
                _committed = true;

                if (s_log.IsDebug)
                {
                    s_log.Debug(LogMessages.Transaction_Committed);
                }
            }
            catch (Exception e)
            {
                _failed = true;

                throw new MicroLiteException(e.Message, e);
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Rollback()
        {
            ThrowIfDisposed();
            ThrowIfRolledBackOrCommitted();

            try
            {
                RollbackTransaction();
            }
            catch (Exception e)
            {
                _failed = true;

                throw new MicroLiteException(e.Message, e);
            }
        }

        internal void Enlist(IDbCommand command)
        {
            if (!IsActive)
            {
                throw new InvalidOperationException();
            }

            command.Transaction = _transaction;
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                try
                {
                    if (IsActive)
                    {
                        s_log.Warn(LogMessages.Transaction_DisposedUncommitted);
                        RollbackTransaction();
                    }
                    else if (_failed && !_rolledBack)
                    {
                        s_log.Warn(LogMessages.Transaction_RollingBackFailedCommit);
                        RollbackTransaction();
                    }
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch
#pragma warning restore CA1031 // Do not catch general exception types
                {
                }
                finally
                {
                    _transaction.Dispose();
                    _transaction = null;
                    _sessionBase = null;
                    _disposed = true;

                    if (s_log.IsDebug)
                    {
                        s_log.Debug(LogMessages.Transaction_Disposed);
                    }
                }
            }
        }

        private void RollbackTransaction()
        {
            if (s_log.IsDebug)
            {
                s_log.Debug(LogMessages.Transaction_RollingBack);
            }

            _transaction.Rollback();
            _sessionBase.TransactionCompleted();
            _rolledBack = true;

            if (s_log.IsDebug)
            {
                s_log.Debug(LogMessages.Transaction_RolledBack);
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        private void ThrowIfNotActive()
        {
            if (!IsActive)
            {
                throw new InvalidOperationException(ExceptionMessages.Transaction_AlreadyCompleted);
            }
        }

        private void ThrowIfRolledBackOrCommitted()
        {
            if (_rolledBack || _committed)
            {
                throw new InvalidOperationException(ExceptionMessages.Transaction_AlreadyCompleted);
            }
        }
    }
}
