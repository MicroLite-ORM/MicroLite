// -----------------------------------------------------------------------
// <copyright file="IInclude.cs" company="MicroLite">
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
namespace MicroLite
{
    /// <summary>
    /// The interface for including a single result.
    /// </summary>
    /// <typeparam name="T">The type of object to be included.</typeparam>
    public interface IInclude<T>
    {
        /// <summary>
        /// Gets the included value.
        /// </summary>
        /// <value>
        /// If the overall query has not been executed, or if the query did not return a result the value will be null.
        /// Otherwise the result of the query.
        /// </value>
        T Value
        {
            get;
        }
    }
}