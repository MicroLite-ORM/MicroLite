// -----------------------------------------------------------------------
// <copyright file="IIncludeMany.cs" company="MicroLite">
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
namespace MicroLite
{
    using System.Collections.Generic;

    /// <summary>
    /// The interface for including a multiple results.
    /// </summary>
    /// <typeparam name="T">The type of object to be included.</typeparam>
    public interface IIncludeMany<T>
    {
        /// <summary>
        /// Gets a value indicating whether this include has a value.
        /// </summary>
        bool HasValue
        {
            get;
        }

        /// <summary>
        /// Gets the included values.
        /// </summary>
        /// <value>
        /// Values will be in one of the following states:
        ///  - If the overall query has not been executed the value will be an empty collection.
        ///  - If the query yielded no results, it will be an empty collection; otherwise it will contain the results of the query.
        /// </value>
        IList<T> Values
        {
            get;
        }
    }
}