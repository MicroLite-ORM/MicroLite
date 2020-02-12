// -----------------------------------------------------------------------
// <copyright file="ISelectFrom.cs" company="Project Contributors">
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

namespace MicroLite.Builder.Syntax.Read
{
    /// <summary>
    /// The interface which specifies the from method in the fluent sql builder syntax.
    /// </summary>
    public interface ISelectFrom : IHideObjectMethods
    {
        /// <summary>
        /// Specifies the table to perform the query against.
        /// </summary>
        /// <param name="table">The name of the table.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="ArgumentException">Thrown if table is null or empty.</exception>
        /// <example>
        /// <code>
        /// var query = SqlBuilder.Select("Col1", "Col2").From("Customers")... // Add remainder of query
        /// </code>
        /// </example>
        IWhereOrOrderBy From(string table);

        /// <summary>
        /// Specifies the type to perform the query against.
        /// </summary>
        /// <param name="forType">The type of object the query relates to.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <example>
        /// If the select criteria is * then all mapped columns will be used in the select list instead, otherwise the specified columns will be used.
        /// <code>
        /// var query = SqlBuilder.Select("Col1", "Col2").From(typeof(Customer))... // Add remainder of query
        /// </code>
        /// </example>
        /// <remarks>Results in all columns being named if the select list is 'SELECT *'.</remarks>
        IWhereOrOrderBy From(Type forType);
    }
}
