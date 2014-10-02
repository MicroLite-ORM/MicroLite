// -----------------------------------------------------------------------
// <copyright file="SqlServerCeDbDriver.cs" company="MicroLite">
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
namespace MicroLite.Driver
{
    using System.Data;

    /// <summary>
    /// The implementation of <see cref="IDbDriver"/> for SQL Server Compact Edition.
    /// </summary>
    internal sealed class SqlServerCeDbDriver : DbDriver
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="SqlServerCeDbDriver" /> class.
        /// </summary>
        internal SqlServerCeDbDriver()
            : base(MicroLite.Dialect.SqlServerCeCharacters.Instance)
        {
        }

        /// <summary>
        /// Creates an IDbCommand command using the values in the specified SqlQuery.
        /// </summary>
        /// <param name="sqlQuery">The SQL query containing the values for the command.</param>
        /// <returns>An IDbCommand with the CommandText and CommandType set.</returns>
        /// <remarks>SQL Server Compact Edition doesn't support command timeout so it is not set.</remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "SqlQuery.CommandText is the parameterised query.")]
        protected override IDbCommand CreateCommand(SqlQuery sqlQuery)
        {
            var command = this.DbProviderFactory.CreateCommand();
            command.CommandText = this.GetCommandText(sqlQuery.CommandText);
            command.CommandType = this.GetCommandType(sqlQuery.CommandText);

            return command;
        }
    }
}