// -----------------------------------------------------------------------
// <copyright file="IDeleteFrom.cs" company="Project Contributors">
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
    using System;

    /// <summary>
    /// The interface which specifies the from method in the fluent sql builder syntax.
    /// </summary>
    public interface IDeleteFrom : IHideObjectMethods
    {
        /// <summary>
        /// Specifies the table to delete from.
        /// </summary>
        /// <param name="table">The name of the table.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IWhere From(string table);

        /// <summary>
        /// Specifies the type to delete.
        /// </summary>
        /// <param name="forType">The type of object the query relates to.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IWhere From(Type forType);
    }
}