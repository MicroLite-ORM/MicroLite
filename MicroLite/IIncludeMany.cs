// -----------------------------------------------------------------------
// <copyright file="IIncludeMany.cs" company="Project Contributors">
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
namespace MicroLite
{
    using System;
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

        /// <summary>
        /// Called when the included value is loaded.
        /// </summary>
        /// <param name="action">The action to be invoked.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is for a callback where the parameter for the action is an instance of the IInclude")]
        void OnLoad(Action<IIncludeMany<T>> action);
    }
}