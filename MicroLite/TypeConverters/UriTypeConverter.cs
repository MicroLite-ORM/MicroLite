// -----------------------------------------------------------------------
// <copyright file="UriTypeConverter.cs" company="MicroLite">
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
    /// An ITypeConverter which can convert a Uri to and from the stored database value of a string column.
    /// </summary>
    public sealed class UriTypeConverter : ITypeConverter
    {
        /// <summary>
        /// Determines whether this type converter can convert values for the specified property type.
        /// </summary>
        /// <param name="propertyType">The type of the property value to be converted.</param>
        /// <returns>
        ///   <c>true</c> if this instance can convert the specified property type; otherwise, <c>false</c>.
        /// </returns>
        public bool CanConvert(Type propertyType)
        {
            return propertyType == typeof(Uri);
        }

        /// <summary>
        /// Converts the specified database value into an instance of the property type.
        /// </summary>
        /// <param name="value">The database value to be converted.</param>
        /// <param name="propertyType">The property type to convert to.</param>
        /// <returns>
        /// An instance of the specified property type containing the specified value.
        /// </returns>
        public object ConvertFromDbValue(object value, Type propertyType)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            var uri = new Uri(value.ToString());

            return uri;
        }

        /// <summary>
        /// Converts the specified property value into an instance of the database value.
        /// </summary>
        /// <param name="value">The property value to be converted.</param>
        /// <param name="propertyType">The property type to convert from.</param>
        /// <returns>
        /// An instance of the corresponding database type for the property type containing the property value.
        /// </returns>
        public object ConvertToDbValue(object value, Type propertyType)
        {
            if (value == null)
            {
                return value;
            }

            var uri = (Uri)value;

            var uriString = uri.ToString();

            return uriString;
        }
    }
}