// -----------------------------------------------------------------------
// <copyright file="IFunctionOrFrom.cs" company="MicroLite">
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
    /// <summary>
    /// The interface which specifies the from method or function in the fluent sql builder syntax.
    /// </summary>
    public interface IFunctionOrFrom : IHideObjectMethods, IFrom
    {
        /// <summary>
        /// Selects the average value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IFrom Average(string columnName);

        /// <summary>
        /// Selects the average value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <param name="columnAlias">The alias in the result set for the calculated column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IFrom Average(string columnName, string columnAlias);

        /// <summary>
        /// Selects the number of records which match the specified filter.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IFrom Count(string columnName);

        /// <summary>
        /// Selects the number of records which match the specified filter.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <param name="columnAlias">The alias in the result set for the calculated column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IFrom Count(string columnName, string columnAlias);

        /// <summary>
        /// Selects the maximum value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IFrom Max(string columnName);

        /// <summary>
        /// Selects the maximum value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <param name="columnAlias">The alias in the result set for the calculated column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IFrom Max(string columnName, string columnAlias);

        /// <summary>
        /// Selects the minimum value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IFrom Min(string columnName);

        /// <summary>
        /// Selects the minimum value in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <param name="columnAlias">The alias in the result set for the calculated column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IFrom Min(string columnName, string columnAlias);

        /// <summary>
        /// Selects the sum of the values in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IFrom Sum(string columnName);

        /// <summary>
        /// Selects the sum of the values in the specified column.
        /// </summary>
        /// <param name="columnName">The column to query.</param>
        /// <param name="columnAlias">The alias in the result set for the calculated column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IFrom Sum(string columnName, string columnAlias);
    }
}