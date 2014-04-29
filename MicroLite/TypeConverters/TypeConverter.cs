// -----------------------------------------------------------------------
// <copyright file="TypeConverter.cs" company="MicroLite">
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
namespace MicroLite.TypeConverters
{
    using System;

    /// <summary>
    /// A class which allows access to <see cref="ITypeConverter"/>s.
    /// </summary>
    public static class TypeConverter
    {
        private static readonly TypeConverterCollection converters = new TypeConverterCollection();
        private static readonly ITypeConverter defaultConverter = new ObjectTypeConverter();

        /// <summary>
        /// Gets the type converter collection which contains all type converters registered with the MicroLite ORM framework.
        /// </summary>
        public static TypeConverterCollection Converters
        {
            get
            {
                return converters;
            }
        }

        /// <summary>
        /// Gets the default type converter which can be used if there is no specific type converter for a given type.
        /// </summary>
        public static ITypeConverter Default
        {
            get
            {
                return defaultConverter;
            }
        }

        /// <summary>
        /// Gets the <see cref="ITypeConverter"/> for the specified type.
        /// </summary>
        /// <param name="type">The type to get the converter for.</param>
        /// <returns>The <see cref="ITypeConverter"/> for the specified type, or null if no specific type converter exists for the type.</returns>
        /// <remarks>
        /// If For returns null, the TypeConverter.Default can be used.
        /// </remarks>
        public static ITypeConverter For(Type type)
        {
            for (int i = 0; i < Converters.Count; i++)
            {
                var typeConverter = Converters[i];

                if (typeConverter.CanConvert(type))
                {
                    return typeConverter;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether the type is not an entity type and is a convertible type.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <returns>
        /// true if the type is not an entity and can be converted.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown if type is null.</exception>
        public static bool IsNotEntityAndConvertible(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (type.IsValueType || type == typeof(string))
            {
                return true;
            }

            for (int i = 0; i < Converters.Count; i++)
            {
                var typeConverter = Converters[i];

                if (typeConverter.CanConvert(type))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Resolves the actual type.
        /// </summary>
        /// <param name="type">The type to resolve.</param>
        /// <returns>
        /// The actual type (e.g. the inner type if it is a nullable value).
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown if type is null.</exception>
        public static Type ResolveActualType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var actualType = type;

            if (type.IsGenericType)
            {
                actualType = Nullable.GetUnderlyingType(type);
            }

            return actualType;
        }
    }
}