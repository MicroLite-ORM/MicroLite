// -----------------------------------------------------------------------
// <copyright file="SessionBase.cs" company="Project Contributors">
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
using MicroLite.Driver;
using MicroLite.Logging;

namespace MicroLite.Core
{
    /// <summary>
    /// The base class for a session.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ConnectionScope: {ConnectionScope}")]
    internal abstract class SessionBase : ISessionBase, IDisposable
    {
        protected static readonly ILog Log = LogManager.GetCurrentClassLog();
        private Transaction _currentTransaction;
        private bool _disposed;

        protected SessionBase(ConnectionScope connectionScope, IDbDriver dbDriver)
        {
            ConnectionScope = connectionScope;
            DbDriver = dbDriver;

            Connection = dbDriver.CreateConnection();

            if (ConnectionScope == ConnectionScope.PerSession)
            {
                if (Log.IsDebug)
                {
                    Log.Debug(LogMessages.Session_OpeningConnection);
                }

                Connection.Open();
            }
        }

        public IDbConnection Connection { get; private set; }

        public ITransaction CurrentTransaction => _currentTransaction;

        internal ConnectionScope ConnectionScope { get; }

        protected IDbCommand Command { get; private set; }

        protected IDbDriver DbDriver { get; }

        public ITransaction BeginTransaction() => BeginTransaction(IsolationLevel.ReadCommitted);

        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            ThrowIfDisposed();

            if (ConnectionScope == ConnectionScope.PerTransaction)
            {
                if (Log.IsDebug)
                {
                    Log.Debug(LogMessages.Session_OpeningConnection);
                }

                Connection.Open();
            }

            _currentTransaction = new Transaction(this, isolationLevel);

            return _currentTransaction;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void TransactionCompleted()
        {
            if (ConnectionScope == ConnectionScope.PerTransaction)
            {
                if (Log.IsDebug)
                {
                    Log.Debug(LogMessages.Session_ClosingConnection);
                }

                Connection.Close();
            }

            _currentTransaction = null;
        }

        protected void CommandCompleted()
        {
            if (ConnectionScope == ConnectionScope.PerTransaction
                && Connection.State == ConnectionState.Open
                && _currentTransaction is null)
            {
                if (Log.IsDebug)
                {
                    Log.Debug(LogMessages.Session_ClosingConnection);
                }

                Connection.Close();
            }
        }

        protected void ConfigureCommand(SqlQuery sqlQuery)
        {
            if (ConnectionScope == ConnectionScope.PerTransaction
                && Connection.State == ConnectionState.Closed
                && _currentTransaction is null)
            {
                if (Log.IsDebug)
                {
                    Log.Debug(LogMessages.Session_OpeningConnection);
                }

                Connection.Open();
            }

            if (Command is null)
            {
                Command = Connection.CreateCommand();
                Command.Connection = Connection;
            }

            DbDriver.BuildCommand(Command, sqlQuery);

            if (_currentTransaction != null)
            {
                _currentTransaction.Enlist(Command);
            }
        }

        protected void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (Command != null)
                {
                    Command.Dispose();
                    Command = null;
                }

                if (_currentTransaction != null)
                {
                    _currentTransaction.Dispose();
                    _currentTransaction = null;
                }

                if (Connection != null)
                {
                    if (ConnectionScope == ConnectionScope.PerSession)
                    {
                        if (Log.IsDebug)
                        {
                            Log.Debug(LogMessages.Session_ClosingConnection);
                        }

                        Connection.Close();
                    }

                    Connection.Dispose();
                    Connection = null;
                }

                _disposed = true;

                if (Log.IsDebug)
                {
                    Log.Debug(LogMessages.Session_Disposed);
                }
            }
        }

        protected void ThrowIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }
    }
}
