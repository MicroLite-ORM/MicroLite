// -----------------------------------------------------------------------
// <copyright file="IInsertValue.cs" company="MicroLite">
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
    /// The interface which specifies the value method in the fluent insert sql builder syntax.
    /// </summary>
    public interface IInsertValue : IHideObjectMethods, IToSqlQuery
    {
        /// <summary>
        /// Specifies the values to be inserted into the columns.
        /// </summary>
        /// <param name="columnValues">Values for the columns to be inserted.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IToSqlQuery Values(params object[] columnValues);
    }
}