// -----------------------------------------------------------------------
// <copyright file="IWhereEquals.cs" company="MicroLite">
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
namespace MicroLite.Builder
{
    /// <summary>
    /// The interface which specifies the where in method in the fluent sql builder syntax.
    /// </summary>
    public interface IWhereEquals : IHideObjectMethods, IToSqlQuery
    {
        /// <summary>
        /// Specifies that the specified column contains a value which is equal to the specified comparisonValue.
        /// </summary>
        /// <param name="column">The column name to use in the where clause.</param>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IToSqlQuery WhereEquals(string column, object comparisonValue);
    }
}