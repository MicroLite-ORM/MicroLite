// -----------------------------------------------------------------------
// <copyright file="ObjectBuilder.cs" company="MicroLite">
// Copyright 2012 - 2014 Project Contributors
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
    /// The default implementation of <see cref="IObjectBuilder"/>.
    /// </summary>
    internal sealed class ObjectBuilder : IObjectBuilder
    {
        public T BuildInstance<T>(IObjectInfo objectInfo, IDataReader reader)
        {
            var instance = (T)objectInfo.CreateInstance();

            objectInfo.SetPropertyValues(instance, reader);

            return instance;
        }
    }
}