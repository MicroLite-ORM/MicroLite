// -----------------------------------------------------------------------
// <copyright file="TypeConverter.cs" company="MicroLite">
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
namespace MicroLite.TypeConverters
{
    using System;
    using System.Linq;

    /// <summary>
    /// The base class for any implementation of <see cref="ITypeConverter"/>.
    /// </summary>
    public abstract class TypeConverter : ITypeConverter
    {
        private static readonly TypeConverterCollection collection = new TypeConverterCollection();

        /// <summary>
        /// Gets the type converter collection which contains all type converters registered with the MicroLite ORM framework.
        /// </summary>
        public static TypeConverterCollection Converters
        {
            get
            {
                return collection;
            }
        }

        /// <summary>
        /// Gets the <see cref="ITypeConverter"/> for the specified type.
        /// </summary>
        /// <param name="type">The type to get the converter for.</param>
        /// <returns>The <see cref="ITypeConverter"/> for the specified type.</returns>
        public static ITypeConverter For(Type type)
        {
            return Converters.First(c => c.CanConvert(type));
        }

        /// <summary>
        /// Resolves the actual type. If the type is generic (as it would be for a nullable struct) it returns the inner type.
        /// </summary>
        /// <param name="type">The type to resolve.</param>
        /// <returns>The actual type.</returns>
        public static Type ResolveActualType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            return type.IsGenericType ? type.GetGenericArguments()[0] : type;
        }

        /// <summary>
        /// Determines whether this type converter can convert values for the specified property type.
        /// </summary>
        /// <param name="propertyType">The type of the property value to be converted.</param>
        /// <returns>
        ///   <c>true</c> if this instance can convert the specified property type; otherwise, <c>false</c>.
        /// </returns>
        public abstract bool CanConvert(Type propertyType);

        /// <summary>
        /// Converts the specified database value into an instance of the property type.
        /// </summary>
        /// <param name="value">The database value to be converted.</param>
        /// <param name="propertyType">The property type to convert to.</param>
        /// <returns>
        /// An instance of the specified property type containing the specified value.
        /// </returns>
        public abstract object ConvertFromDbValue(object value, Type propertyType);

        /// <summary>
        /// Converts the specified property value into an instance of the database value.
        /// </summary>
        /// <param name="value">The property value to be converted.</param>
        /// <param name="propertyType">The property type to convert from.</param>
        /// <returns>
        /// An instance of the corresponding database type for the property type containing the property value.
        /// </returns>
        public abstract object ConvertToDbValue(object value, Type propertyType);
    }
}