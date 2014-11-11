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
        /// Uses the specified argument to filter the column.
        /// </summary>
        /// <param name="comparisonValue">The value to compare with.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOr IsEqualTo(object comparisonValue);
    }
}