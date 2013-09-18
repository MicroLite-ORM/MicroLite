// -----------------------------------------------------------------------
// <copyright file="IWhereSingleColumn.cs" company="MicroLite">
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
    /// The interface which specifies the where in method in the fluent sql builder syntax.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "OrIn", Justification = "In this case we mean OR/IN.")]
    public interface IWhereSingleColumn : IHideObjectMethods
    {
        /// <summary>
        /// Uses the specified arguments to filter the column.
        /// </summary>
        /// <param name="lower">The inclusive lower value.</param>
        /// <param name="upper">The inclusive upper value.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOrOrderBy Between(object lower, object upper);

        /// <summary>
        /// Uses the specified arguments to filter the column.
        /// </summary>
        /// <param name="args">The arguments to filter the column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "In", Justification = "The method is to specify an In list.")]
        IAndOrOrderBy In(params object[] args);

        /// <summary>
        /// Uses the specified SqlQuery as a sub query to filter the column.
        /// </summary>
        /// <param name="subQuery">The sub query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "In", Justification = "The method is to specify an In list.")]
        IAndOrOrderBy In(SqlQuery subQuery);

        /// <summary>
        /// Specifies that the specified column contains a value which is equal to the specified comparisonValue.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOrOrderBy IsEqualTo(object comparisonValue);

        /// <summary>
        /// Specifies that the specified column contains a value which is greater than the specified comparisonValue.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOrOrderBy IsGreaterThan(object comparisonValue);

        /// <summary>
        /// Specifies that the specified column contains a value which is greater than or equal to the specified comparisonValue.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOrOrderBy IsGreaterThanOrEqualTo(object comparisonValue);

        /// <summary>
        /// Specifies that the specified column contains a value which is less than the specified comparisonValue.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOrOrderBy IsLessThan(object comparisonValue);

        /// <summary>
        /// Specifies that the specified column contains a value which is less than or equal to the specified comparisonValue.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOrOrderBy IsLessThanOrEqualTo(object comparisonValue);

        /// <summary>
        /// Specifies that the specified column contains a value which is like the specified comparisonValue.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOrOrderBy IsLike(object comparisonValue);

        /// <summary>
        /// Specifies that the specified column contains a value which is not equal to the specified comparisonValue.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOrOrderBy IsNotEqualTo(object comparisonValue);

        /// <summary>
        /// Specifies that the specified column contains a value which is not null.
        /// </summary>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOrOrderBy IsNotNull();

        /// <summary>
        /// Specifies that the specified column contains a value which is null.
        /// </summary>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOrOrderBy IsNull();

        /// <summary>
        /// Uses the specified arguments to filter the column.
        /// </summary>
        /// <param name="args">The arguments to filter the column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "In", Justification = "The method is to specify an In list.")]
        IAndOrOrderBy NotIn(params object[] args);

        /// <summary>
        /// Uses the specified SqlQuery as a sub query to filter the column.
        /// </summary>
        /// <param name="subQuery">The sub query.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "In", Justification = "The method is to specify an In list.")]
        IAndOrOrderBy NotIn(SqlQuery subQuery);
    }
}