// -----------------------------------------------------------------------
// <copyright file="IObjectBuilder.cs" company="MicroLite">
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
namespace MicroLite.Core
{
    using System.Data;
    using MicroLite.Mapping;

    /// <summary>
    /// The interface for a class which builds an object instance from the values in a <see cref="IDataReader"/>.
    /// </summary>
    internal interface IObjectBuilder
    {
        /// <summary>
        /// Builds an instance of the specified type, populating it with the values in the specified data reader.
        /// </summary>
        /// <typeparam name="T">The type of object to be built.</typeparam>
        /// <param name="objectInfo">The <see cref="IObjectInfo"/> for the type to be built.</param>
        /// <param name="reader">The <see cref="IDataReader"/> containing the values to populate the object with.</param>
        /// <returns>The new instance populated with the values from the <see cref="IDataReader"/>.</returns>
        /// <exception cref="MicroLiteException">Thrown if there is an exception setting a property value.</exception>
        T BuildInstance<T>(IObjectInfo objectInfo, IDataReader reader);
    }
}