// -----------------------------------------------------------------------
// <copyright file="XDocumentTypeConverter.cs" company="MicroLite">
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
    using System.Xml.Linq;

    internal sealed class XDocumentTypeConverter : TypeConverter
    {
        public override bool CanConvert(Type propertyType)
        {
            return propertyType == typeof(XDocument);
        }

        public override object ConvertFromDbValue(object value, Type propertyType)
        {
            if (value == null || value == DBNull.Value)
            {
                return null;
            }

            var document = XDocument.Parse(value.ToString());

            return document;
        }

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