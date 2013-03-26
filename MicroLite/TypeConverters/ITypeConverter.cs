// -----------------------------------------------------------------------
// <copyright file="ITypeConverter.cs" company="MicroLite">
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
namespace MicroLite.TypeConverters
{
    using System;

    /// <summary>
    /// The interface for a class which can convert types.
    /// </summary>
    internal interface ITypeConverter
    {
        /// <summary>
        /// Determines whether this instance can convert the specified type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///   <c>true</c> if this instance can convert the specified type; otherwise, <c>false</c>.
        /// </returns>
        bool CanConvert(Type type);

        /// <summary>
        /// Converts the specified value into an instance of the specified type.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="type">The type to convert to.</param>
        /// <returns>An instance of the specified type containing the specified value.</returns>
        object Convert(object value, Type type);
    }
}