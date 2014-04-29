// -----------------------------------------------------------------------
// <copyright file="ObjectTypeConverter.cs" company="MicroLite">
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
    using System.Data;
    using System.Globalization;

    /// <summary>
    /// An ITypeConverter which uses Convert.ChangeType.
    /// </summary>
    /// <remarks>
    /// It is the default ITypeConverter, which can be used if no suitable specific implementation exists.
    /// </remarks>
    internal sealed class ObjectTypeConverter : ITypeConverter
    {
        /// <summary>
        /// Determines whether this type converter can convert values for the specified type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>
        ///   <c>true</c> if this instance can convert the specified type; otherwise, <c>false</c>.
        /// </returns>
        public bool CanConvert(Type type)
        {
            return true;
        }

        /// <summary>
        /// Converts the specified database value into an instance of the specified type.
        /// </summary>
        /// <param name="value">The database value to be converted.</param>
        /// <param name="type">The type to convert to.</param>
        /// <returns>An instance of the specified type containing the specified value.</returns>
        public object ConvertFromDbValue(object value, Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            if (type.IsValueType && type.IsGenericType)
            {
                ValueType converted = (ValueType)value;

                return converted;
            }
            else
            {
                var converted = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

                return converted;
            }
        }

        /// <summary>
        /// Converts value at the specified index in the IDataReader into an instance of the specified type.
        /// </summary>
        /// <param name="reader">The IDataReader containing the results.</param>
        /// <param name="index">The index of the record to read from the IDataReader.</param>
        /// <param name="type">The type to convert result value to.</param>
        /// <returns>An instance of the specified type containing the specified value.</returns>
        /// <exception cref="System.ArgumentNullException">thrown if propertyType is null.</exception>
        public object ConvertFromDbValue(IDataReader reader, int index, Type type)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (reader.IsDBNull(index))
            {
                return null;
            }

            var value = reader[index];

            if (type.IsValueType && type.IsGenericType)
            {
                ValueType converted = (ValueType)value;

                return converted;
            }
            else
            {
                var converted = Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

                return converted;
            }
        }

        /// <summary>
        /// Converts the specified value into an instance of the database value.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="type">The type to convert from.</param>
        /// <returns>An instance of the corresponding database type containing the value.</returns>
        public object ConvertToDbValue(object value, Type type)
        {
            return value;
        }
    }
}