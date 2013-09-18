// -----------------------------------------------------------------------
// <copyright file="IHavingOrOrderBy.cs" company="MicroLite">
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
    /// The interface which specifies the having method in the fluent sql builder syntax.
    /// </summary>
    public interface IHavingOrOrderBy : IHideObjectMethods, IOrderBy
    {
        /// <summary>
        /// Specifies the having clause for the query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="value">The argument value.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IOrderBy Having(string predicate, object value);
    }
}