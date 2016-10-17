// -----------------------------------------------------------------------
// <copyright file="EnumTypeConverter.cs" company="MicroLite">
// Copyright 2012 - 2016 Project Contributors
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
    /// An ITypeConverter which can convert Enum values to and from database values.
    /// </summary>
    /// <remarks>
    /// It ensures that the database value is converted to and from the underlying storage type of the Enum to allow for db
    /// columns being byte, short, integer or long.
    /// </remarks>
    public sealed class EnumTypeConverter : ITypeConverter
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
            if (type?.IsEnum == true)
            {
                return true;
            }

            var actualType = TypeConverter.ResolveActualType(type);

            return actualType?.IsEnum == true;
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
                throw new ArgumentNullException(nameof(type));
            }

            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            var enumType = TypeConverter.ResolveActualType(type);

            var enumStorageType = Enum.GetUnderlyingType(enumType);

            var underlyingValue = Convert.ChangeType(value, enumStorageType, CultureInfo.InvariantCulture);

            var enumValue = Enum.ToObject(enumType, underlyingValue);

            return enumValue;
        }

        /// <summary>
        /// Converts value at the specified index in the IDataReader into an instance of the specified type.
        /// </summary>
        /// <param name="reader">The IDataReader containing the results.</param>
        /// <param name="index">The index of the record to read from the IDataReader.</param>
        /// <param name="type">The type to convert result value to.</param>
        /// <returns>An instance of the specified type containing the specified value.</returns>
        public object ConvertFromDbValue(IDataReader reader, int index, Type type)
        {
            if (reader == null)
            {
                throw new ArgumentNullException(nameof(reader));
            }

            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            if (reader.IsDBNull(index))
            {
                return null;
            }

            object enumValue;

            var enumType = TypeConverter.ResolveActualType(type);

            var enumStorageType = Enum.GetUnderlyingType(enumType);

            switch (enumStorageType.Name)
            {
                case "Byte":
                    enumValue = Enum.ToObject(enumType, reader.GetByte(index));
                    break;

                case "Int16":
                    enumValue = Enum.ToObject(enumType, reader.GetInt16(index));
                    break;

                case "Int32":
                    enumValue = Enum.ToObject(enumType, reader.GetInt32(index));
                    break;

                case "Int64":
                    enumValue = Enum.ToObject(enumType, reader.GetInt64(index));
                    break;

                default:
                    enumValue = Enum.ToObject(enumType, reader[0]);
                    break;
            }

            return enumValue;
        }

        /// <summary>
        /// Converts the specified value into an instance of the database value.
        /// </summary>
        /// <param name="value">The value to be converted.</param>
        /// <param name="type">The type to convert from.</param>
        /// <returns>An instance of the corresponding database type containing the value.</returns>
        public object ConvertToDbValue(object value, Type type)
        {
            if (value == null)
            {
                return value;
            }

            var enumType = TypeConverter.ResolveActualType(type);

            var enumStorageType = Enum.GetUnderlyingType(enumType);

            var underlyingValue = Convert.ChangeType(value, enumStorageType, CultureInfo.InvariantCulture);

            return underlyingValue;
        }
    }
}