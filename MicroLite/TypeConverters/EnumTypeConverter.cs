// -----------------------------------------------------------------------
// <copyright file="EnumTypeConverter.cs" company="MicroLite">
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
    using System.Globalization;

    /// <summary>
    /// An ITypeConverter which can convert Enum values to and from database values.
    /// </summary>
    /// <remarks>It ensures that the database value is converted to and from the underlying storage type of the Enum to allow for db columns being byte, short, integer or long.</remarks>
    public sealed class EnumTypeConverter : TypeConverter
    {
        /// <summary>
        /// Determines whether this type converter can convert values for the specified property type.
        /// </summary>
        /// <param name="propertyType">The type of the property value to be converted.</param>
        /// <returns>
        ///   <c>true</c> if this instance can convert the specified property type; otherwise, <c>false</c>.
        /// </returns>
        public override bool CanConvert(Type propertyType)
        {
            var actualType = TypeConverter.ResolveActualType(propertyType);

            return actualType.IsEnum;
        }

        /// <summary>
        /// Converts the specified database value into an instance of the property type.
        /// </summary>
        /// <param name="value">The database value to be converted.</param>
        /// <param name="propertyType">The property type to convert to.</param>
        /// <returns>
        /// An instance of the specified property type containing the specified value.
        /// </returns>
        public override object ConvertFromDbValue(object value, Type propertyType)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            var enumType = TypeConverter.ResolveActualType(propertyType);

            var enumStorageType = Enum.GetUnderlyingType(enumType);

            var underlyingValue = System.Convert.ChangeType(value, enumStorageType, CultureInfo.InvariantCulture);

            var enumValue = Enum.ToObject(enumType, underlyingValue);

            return enumValue;
        }

        /// <summary>
        /// Converts the specified property value into an instance of the database value.
        /// </summary>
        /// <param name="value">The property value to be converted.</param>
        /// <param name="propertyType">The property type to convert from.</param>
        /// <returns>
        /// An instance of the corresponding database type for the property type containing the property value.
        /// </returns>
        public override object ConvertToDbValue(object value, Type propertyType)
        {
            if (value == null)
            {
                return value;
            }

            var enumType = TypeConverter.ResolveActualType(propertyType);

            var enumStorageType = Enum.GetUnderlyingType(enumType);

            var underlyingValue = System.Convert.ChangeType(value, enumStorageType, CultureInfo.InvariantCulture);

            return underlyingValue;
        }
    }
}