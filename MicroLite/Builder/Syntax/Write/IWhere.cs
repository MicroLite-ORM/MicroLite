// -----------------------------------------------------------------------
// <copyright file="IWhere.cs" company="MicroLite">
// Copyright 2012 - 2016 Project Contributors
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
    /// The interface which specifies the where method in the fluent sql builder syntax.
    /// </summary>
    public interface IWhere : IHideObjectMethods, IToSqlQuery
    {
        /// <summary>
        /// Specifies the where clause for the query.
        /// </summary>
        /// <param name="column">The column name to use in the where clause.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        /// <exception cref="System.ArgumentException">Thrown if column is null or empty.</exception>
        IWhereSingleColumn Where(string column);
    }
}