// -----------------------------------------------------------------------
// <copyright file="ConnectionManager.cs" company="MicroLite">
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
    using System.Data;
    using MicroLite.Logging;

    /// <summary>
    /// The default implementation of <see cref="IConnectionManager"/>.
    /// </summary>
    internal sealed class ConnectionManager : IConnectionManager
    {
        private static readonly ILog log = LogManager.GetCurrentClassLog();
        private IDbConnection connection;
        private ITransaction currentTransaction;

        internal ConnectionManager(IDbConnection connection)
        {
            this.connection = connection;
        }

        public ITransaction CurrentTransaction
        {
            get
            {
                return this.currentTransaction;
            }
        }

        public ITransaction BeginTransaction(IsolationLevel isolationLevel)
        {
            if (this.currentTransaction == null || !this.currentTransaction.IsActive)
            {
                log.TryLogDebug(Messages.ConnectionManager_BeginTransactionWithIsolationLevel, isolationLevel.ToString());

                this.connection.Open();

                var dbTransaction = this.connection.BeginTransaction(isolationLevel);

                this.currentTransaction = new AdoTransaction(dbTransaction);
            }

            return this.currentTransaction;
        }

        public void CommandCompleted(IDbCommand command)
        {
            if (command.Transaction == null)
            {
                log.TryLogDebug(Messages.ConnectionManager_ClosingConnection);
                command.Connection.Close();
            }
        }

        public IDbCommand CreateCommand()
        {
            if (this.connection.State == ConnectionState.Closed)
            {
                log.TryLogDebug(Messages.ConnectionManager_OpeningConnection);
                this.connection.Open();
            }

            log.TryLogDebug(Messages.ConnectionManager_CreatingCommand);
            var command = this.connection.CreateCommand();

            if (this.currentTransaction != null)
            {
                log.TryLogDebug(Messages.ConnectionManager_EnlistingInTransaction);
                this.currentTransaction.Enlist(command);
            }

            return command;
        }

        public void Dispose()
        {
            if (this.connection != null)
            {
                this.connection.Close();
                this.connection.Dispose();
                this.connection = null;
            }

            if (this.currentTransaction != null)
            {
                this.currentTransaction.Dispose();
                this.currentTransaction = null;
            }
        }
    }
}