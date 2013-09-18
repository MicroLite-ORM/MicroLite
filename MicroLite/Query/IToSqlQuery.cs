// -----------------------------------------------------------------------
// <copyright file="IToSqlQuery.cs" company="MicroLite">
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
    /// The interface to end the fluent build syntax.
    /// </summary>
    /// <remarks>
    /// It's a bit of a verbose hack, need to see if I can use cast operators instead somehow...
    /// </remarks>
    public interface IToSqlQuery : IHideObjectMethods
    {
        /// <summary>
        /// Creates a <see cref="SqlQuery"/> from the values specified.
        /// </summary>
        /// <returns>The created <see cref="SqlQuery"/>.</returns>
        SqlQuery ToSqlQuery();
    }
}