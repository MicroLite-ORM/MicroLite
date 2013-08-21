// -----------------------------------------------------------------------
// <copyright file="StoredProcedureSqlBuilder.cs" company="MicroLite">
// Copyright 2012 - 2013 Trevor Pilley
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Query
{
    using System.Collections.Generic;
    using System.Text;

    [System.Diagnostics.DebuggerDisplay("{innerSql}")]
    internal sealed class StoredProcedureSqlBuilder : IWithParameter
    {
        private readonly List<object> arguments = new List<object>();
        private readonly StringBuilder innerSql = new StringBuilder(capacity: 60);

        internal StoredProcedureSqlBuilder(string procedureName)
        {
            this.innerSql.Append("EXEC ");
            this.innerSql.Append(procedureName);
        }

        /// <summary>
        /// Creates a <see cref="SqlQuery"/> from the values specified.
        /// </summary>
        /// <returns>The created <see cref="SqlQuery"/>.</returns>
        /// <remarks>This method is called to return an SqlQuery once query has been defined.</remarks>
        public SqlQuery ToSqlQuery()
        {
            return new SqlQuery(this.innerSql.ToString(), this.arguments.ToArray());
        }

        /// <summary>
        /// Specifies that the stored procedure should be executed the specified parameter and argument.
        /// </summary>
        /// <param name="parameter">The parameter to be added.</param>
        /// <param name="arg">The argument value for the parameter.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// Add each parameter separately, specifying the parameter name and value.
        /// <code>
        /// var sqlQuery = SqlBuilder
        ///     .Execute("GetCustomerInvoices")
        ///     .WithParameter("@CustomerId", 7633245)
        ///     .WithParameter("@StartDate", DateTime.Today.AddMonths(-3))
        ///     .WithParameter("@EndDate", DateTime.Today)
        ///     .ToSqlQuery();
        /// </code>
        /// </example>
        public IWithParameter WithParameter(string parameter, object arg)
        {
            if (this.arguments.Count > 0)
            {
                this.innerSql.Append(",");
            }

            this.arguments.Add(arg);
            this.innerSql.Append(" ");
            this.innerSql.Append(parameter);

            return this;
        }
    }
}