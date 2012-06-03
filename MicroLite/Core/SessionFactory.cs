// -----------------------------------------------------------------------
// <copyright file="SessionFactory.cs" company="MicroLite">
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
    using System.Data;
    using System.Data.Common;
    using MicroLite.Logging;

    /// <summary>
    /// The default implementation of <see cref="ISessionFactory"/>.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("SessionFactory for {ConnectionString}")]
    internal sealed class SessionFactory : ISessionFactory
    {
        private static readonly ILog log = LogManager.GetLogInstance("MicroLite.SessionFactory");
        private readonly string connectionString;
        private readonly object locker = new object();
        private readonly DbProviderFactory providerFactory;

        internal SessionFactory(string connectionString, DbProviderFactory providerFactory)
        {
            this.connectionString = connectionString;
            this.providerFactory = providerFactory;
        }

        public string ConnectionString
        {
            get
            {
                return this.connectionString;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "This method is provided to create and return an ISession for the caller to use, it should not dispose of it, that is the responsibility of the caller.")]
        public ISession OpenSession()
        {
            IDbConnection connection;

            lock (this.locker)
            {
                connection = this.providerFactory.CreateConnection();
            }

            connection.ConnectionString = this.ConnectionString;

            log.TryLogDebug(LogMessages.SessionFactory_CreatingSession);
            return new Session(new ConnectionManager(connection), new ObjectBuilder(), new SqlQueryBuilder());
        }
    }
}