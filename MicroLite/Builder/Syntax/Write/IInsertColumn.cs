// -----------------------------------------------------------------------
// <copyright file="IInsertColumn.cs" company="Project Contributors">
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
    /// The interface which specifies the column(s) method in the fluent insert sql builder syntax.
    /// </summary>
    public interface IInsertColumn : IHideObjectMethods, IInsertValue, IToSqlQuery
    {
        /// <summary>
        /// Specifies the columns in the table to have values inserted into.
        /// </summary>
        /// <param name="columnNames">Name of the columns to be inserted.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IInsertValue Columns(params string[] columnNames);
    }
}
