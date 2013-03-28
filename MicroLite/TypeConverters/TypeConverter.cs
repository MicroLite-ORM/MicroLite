﻿// -----------------------------------------------------------------------
// <copyright file="TypeConverter.cs" company="MicroLite">
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
    using System.Linq;

    /// <summary>
    /// The base class for any implementation of <see cref="ITypeConverter"/>.
    /// </summary>
    internal abstract class TypeConverter : ITypeConverter
    {
        private static readonly TypeConverterCollection collection = new TypeConverterCollection();

        /// <summary>
        /// Gets the type converter collection which contains all type converters registered with the MicroLite ORM framework.
        /// </summary>
        internal static TypeConverterCollection Converters
        {
            get
            {
                return collection;
            }
        }

        /// <summary>
        /// Determines whether this instance can convert the specified type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///   <c>true</c> if this instance can convert the specified type; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool CanConvert(Type type);

        /// <summary>
        /// Converts the specified value into an instance of the specified type.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="type">The type to convert to.</param>
        /// <returns>
        /// An instance of the specified type containing the specified value.
        /// </returns>
        public abstract object ConvertFromDbValue(object value, Type type);

        /// <summary>
        /// Gets the <see cref="ITypeConverter"/> for the specified type.
        /// </summary>
        /// <param name="type">The type to get the converter for.</param>
        /// <returns>The <see cref="ITypeConverter"/> for the specified type.</returns>
        internal static ITypeConverter For(Type type)
        {
            return Converters.First(c => c.CanConvert(type));
        }

        /// <summary>
        /// Resolves the actual type. If the type is generic (as it would be for a nullable struct) it returns the inner type.
        /// </summary>
        /// <param name="type">The type to resolve.</param>
        /// <returns>The actual type.</returns>
        protected Type ResolveActualType(Type type)
        {
            return type.IsGenericType ? type.GetGenericArguments()[0] : type;
        }
    }
}