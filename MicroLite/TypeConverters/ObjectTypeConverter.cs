// -----------------------------------------------------------------------
// <copyright file="ObjectTypeConverter.cs" company="MicroLite">
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
    /// An ITypeConverter which can uses Convert.ChangeType.
    /// </summary>
    /// <remarks>
    /// It is the default ITypeConverter used if no suitable specific implementation exists.
    /// </remarks>
    public sealed class ObjectTypeConverter : TypeConverter
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
            return true;
        }

        /// <summary>
        /// Converts the specified database value into an instance of the property type.
        /// </summary>
        /// <param name="value">The database value to be converted.</param>
        /// <param name="propertyType">The property type to convert to.</param>
        /// <returns>
        /// An instance of the specified property type containing the specified value.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">thrown if propertyType is null.</exception>
        public override object ConvertFromDbValue(object value, Type propertyType)
        {
            if (propertyType == null)
            {
                throw new ArgumentNullException("propertyType");
            }

            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            if (propertyType.IsValueType && propertyType.IsGenericType)
            {
                ValueType converted = (ValueType)value;

                return converted;
            }
            else
            {
                var converted = System.Convert.ChangeType(value, propertyType, CultureInfo.InvariantCulture);

                return converted;
            }
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
            return value;
        }
    }
}