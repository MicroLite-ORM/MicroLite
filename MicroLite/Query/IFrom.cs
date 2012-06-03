// -----------------------------------------------------------------------
// <copyright file="IFrom.cs" company="MicroLite">
// Copyright 2012 Trevor Pilley
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
    /// The interface which specifies the from method in the fluent sql builder syntax.
    /// </summary>
    public interface IFrom : IHideObjectMethods
    {
        /// <summary>
        /// Specifies the table to perform the query against.
        /// </summary>
        /// <param name="table">The name of the table.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IWhereOrOrderBy From(string table);
    }
}