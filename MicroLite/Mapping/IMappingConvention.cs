﻿// -----------------------------------------------------------------------
// <copyright file="IMappingConvention.cs" company="MicroLite">
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
    /// The interface for a class which implements a mapping convention between a class and a table.
    /// </summary>
    public interface IMappingConvention
    {
        /// <summary>
        /// Creates the object info for the supplied type.
        /// </summary>
        /// <param name="forType">The type to create the object info for.</param>
        /// <returns>The <see cref="ObjectInfo"/> for the supplied type.</returns>
        ObjectInfo CreateObjectInfo(Type forType);
    }
}