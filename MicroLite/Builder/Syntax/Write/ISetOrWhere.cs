// -----------------------------------------------------------------------
// <copyright file="ISetOrWhere.cs" company="Project Contributors">
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
namespace MicroLite.Builder.Syntax.Write
{
    /// <summary>
    /// The interface which specifies the value method in the fluent update sql builder syntax.
    /// </summary>
    public interface ISetOrWhere : IHideObjectMethods, IWhere
    {
        /// <summary>
        /// Specifies the column in the table and the new value for it.
        /// </summary>
        /// <param name="columnName">Name of the column to be updated.</param>
        /// <param name="columnValue">The new value for the column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        ISetOrWhere SetColumnValue(string columnName, object columnValue);
    }
}