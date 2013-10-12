// -----------------------------------------------------------------------
// <copyright file="IInsertValue.cs" company="MicroLite">
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
    /// The interface which specifies the value method in the fluent insert sql builder syntax.
    /// </summary>
    public interface IInsertValue : IToSqlQuery
    {
        /// <summary>
        /// Specifies the column in the table and the value to be inserted into it.
        /// </summary>
        /// <param name="columnName">Name of the column to be inserted.</param>
        /// <param name="columnValue">The value to be inserted into the column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IInsertValue Value(string columnName, object columnValue);
    }
}