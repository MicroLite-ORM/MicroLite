// -----------------------------------------------------------------------
// <copyright file="IFunction.cs" company="MicroLite">
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
namespace MicroLite.Query
{
    using System;

    /// <summary>
    /// The interface which specifies the from method or function in the fluent sql builder syntax.
    /// </summary>
    public interface IFunction : IHideObjectMethods
    {
        /// <summary>
        /// Selects the average value in the specified column.
        /// </summary>
        /// <param name="column">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IFrom Average(string column);

        /// <summary>
        /// Specifies the type of object to count records for which match the specified filter.
        /// </summary>
        /// <param name="forType">The type of object the query relates to.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IWhereOrOrderBy Count(Type forType);

        /// <summary>
        /// Selects the maximum value in the specified column.
        /// </summary>
        /// <param name="column">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IFrom Max(string column);

        /// <summary>
        /// Selects the minimum value in the specified column.
        /// </summary>
        /// <param name="column">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IFrom Min(string column);

        /// <summary>
        /// Selects the sum of the values in the specified column.
        /// </summary>
        /// <param name="column">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IFrom Sum(string column);
    }
}