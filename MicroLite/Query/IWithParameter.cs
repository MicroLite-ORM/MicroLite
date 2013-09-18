// -----------------------------------------------------------------------
// <copyright file="IWithParameter.cs" company="MicroLite">
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
namespace MicroLite.Query
{
    /// <summary>
    /// The interface which specifies a parameter and argument for a stored procedure in the fluent sql builder syntax.
    /// </summary>
    public interface IWithParameter : IHideObjectMethods, IToSqlQuery
    {
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
        IWithParameter WithParameter(string parameter, object arg);
    }
}