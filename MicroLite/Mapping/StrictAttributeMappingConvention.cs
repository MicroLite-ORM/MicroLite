// -----------------------------------------------------------------------
// <copyright file="StrictAttributeMappingConvention.cs" company="MicroLite">
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
namespace MicroLite.Mapping
{
    using System;

    /// <summary>
    /// The implementation of <see cref="IMappingConvention"/> which uses attributes to map tables and columns
    /// to types and properties only maps if an attribute is present.
    /// </summary>
    internal sealed class StrictAttributeMappingConvention : IMappingConvention
    {
        public ObjectInfo CreateObjectInfo(Type forType)
        {
            throw new NotImplementedException();
        }
    }
}