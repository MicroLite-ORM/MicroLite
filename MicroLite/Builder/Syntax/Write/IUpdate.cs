// -----------------------------------------------------------------------
// <copyright file="IUpdate.cs" company="MicroLite">
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
    using System;

    /// <summary>
    /// The interface which specifies the table method in the fluent update sql builder syntax.
    /// </summary>
    public interface IUpdate : IHideObjectMethods
    {
        /// <summary>
        /// Specifies the table to perform the query against.
        /// </summary>
        /// <param name="tableName">The name of the table.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        ISetOrWhere Table(string tableName);

        /// <summary>
        /// Specifies the type to perform the query against.
        /// </summary>
        /// <param name="forType">The type of object the query relates to.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        ISetOrWhere Table(Type forType);
    }
}