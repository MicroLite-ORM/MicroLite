// -----------------------------------------------------------------------
// <copyright file="IWhereSingleColumn.cs" company="MicroLite">
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
namespace MicroLite.Builder.Syntax.Write
{
    /// <summary>
    /// The interface which specifies the where in method in the fluent sql builder syntax.
    /// </summary>
    public interface IWhereSingleColumn : IHideObjectMethods
    {
        /// <summary>
        /// Uses the specified arguments to filter the column.
        /// </summary>
        /// <param name="lower">The inclusive lower value.</param>
        /// <param name="upper">The inclusive upper value.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if lower or upper is null.</exception>
        IAndOr Between(object lower, object upper);

        /// <summary>
        /// Uses the specified arguments to filter the column.
        /// </summary>
        /// <param name="args">The arguments to filter the column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if args is null.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "In", Justification = "The method is to specify an In list.")]
        IAndOr In(params object[] args);

        /// <summary>
        /// Uses the specified SQL queries as sub queries to filter the column.
        /// </summary>
        /// <param name="subQueries">The sub queries.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if subQueries is null.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "In", Justification = "The method is to specify an In list.")]
        IAndOr In(params SqlQuery[] subQueries);

        /// <summary>
        /// Uses the specified SqlQuery as a sub query to filter the column.
        /// </summary>
        /// <param name="subQuery">The sub query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if subQuery is null.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "In", Justification = "The method is to specify an In list.")]
        IAndOr In(SqlQuery subQuery);

        /// <summary>
        /// Uses the specified argument to filter the column.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOr IsEqualTo(object comparisonValue);

        /// <summary>
        /// Uses the specified argument to filter the column.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOr IsGreaterThan(object comparisonValue);

        /// <summary>
        /// Uses the specified argument to filter the column.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOr IsGreaterThanOrEqualTo(object comparisonValue);

        /// <summary>
        /// Uses the specified argument to filter the column.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOr IsLessThan(object comparisonValue);

        /// <summary>
        /// Uses the specified argument to filter the column.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOr IsLessThanOrEqualTo(object comparisonValue);

        /// <summary>
        /// Uses the specified argument to filter the column.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOr IsLike(object comparisonValue);

        /// <summary>
        /// Uses the specified argument to filter the column.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOr IsNotEqualTo(object comparisonValue);

        /// <summary>
        /// Uses the specified argument to filter the column.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOr IsNotLike(object comparisonValue);

        /// <summary>
        /// Specifies that the specified column contains a value which is not null.
        /// </summary>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOr IsNotNull();

        /// <summary>
        /// Specifies that the specified column contains a value which is null.
        /// </summary>
        /// <returns>
        /// The next step in the fluent sql builder.
        /// </returns>
        IAndOr IsNull();

        /// <summary>
        /// Uses the specified arguments to filter the column.
        /// </summary>
        /// <param name="lower">The inclusive lower value.</param>
        /// <param name="upper">The inclusive upper value.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if lower or upper is null.</exception>
        IAndOr NotBetween(object lower, object upper);

        /// <summary>
        /// Uses the specified arguments to filter the column.
        /// </summary>
        /// <param name="args">The arguments to filter the column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if args is null.</exception>
        IAndOr NotIn(params object[] args);

        /// <summary>
        /// Uses the specified SQL queries as sub queries to filter the column.
        /// </summary>
        /// <param name="subQueries">The sub queries.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if subQueries is null.</exception>
        IAndOr NotIn(params SqlQuery[] subQueries);

        /// <summary>
        /// Uses the specified SqlQuery as a sub query to filter the column.
        /// </summary>
        /// <param name="subQuery">The sub query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if subQuery is null.</exception>
        IAndOr NotIn(SqlQuery subQuery);
    }
}