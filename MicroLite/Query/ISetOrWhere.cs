// -----------------------------------------------------------------------
// <copyright file="ISetOrWhere.cs" company="MicroLite">
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
    /// The interface which specifies the value method in the fluent update sql builder syntax.
    /// </summary>
    public interface ISetOrWhere : IToSqlQuery
    {
        /// <summary>
        /// Specifies the column in the table and the new value for it.
        /// </summary>
        /// <param name="columnName">Name of the column to be updated.</param>
        /// <param name="columnValue">The new value for the column.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        ISetOrWhere SetColumnValue(string columnName, object columnValue);

        /// <summary>
        /// Specifies the where clause for the query.
        /// </summary>
        /// <param name="columnName">The column name to use in the where clause.</param>
        /// <param name="columnValue">The new value for the query to match using.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IToSqlQuery Where(string columnName, object columnValue);
    }
}