// -----------------------------------------------------------------------
// <copyright file="XDocumentTypeConverter.cs" company="MicroLite">
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
    using System.Xml.Linq;

    /// <summary>
    /// An ITypeConverter which can convert an XDocument to and from the stored database value of either an xml or string column.
    /// </summary>
    public sealed class XDocumentTypeConverter : TypeConverter
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
            return propertyType == typeof(XDocument);
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

            var document = XDocument.Parse(value.ToString());

            return document;
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

            var document = (XDocument)value;

            var xml = document.ToString(SaveOptions.DisableFormatting);

            return xml;
        }
    }
}