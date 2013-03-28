// -----------------------------------------------------------------------
// <copyright file="EnumTypeConverter.cs" company="MicroLite">
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
    using System.Globalization;

    internal sealed class EnumTypeConverter : TypeConverter
    {
        public override bool CanConvert(Type propertyType)
        {
            var actualType = this.ResolveActualType(propertyType);

            return actualType.IsEnum;
        }

        public override object ConvertFromDbValue(object value, Type propertyType)
        {
            if (value == DBNull.Value)
            {
                return null;
            }

            var enumType = this.ResolveActualType(propertyType);

            var enumStorageType = Enum.GetUnderlyingType(enumType);

            var underlyingValue = System.Convert.ChangeType(value, enumStorageType, CultureInfo.InvariantCulture);

            var enumValue = Enum.ToObject(enumType, underlyingValue);

            return enumValue;
        }

        public override object ConvertToDbValue(object value, Type propertyType)
        {
            if (value == null)
            {
                return value;
            }

            var enumType = this.ResolveActualType(propertyType);

            var enumStorageType = Enum.GetUnderlyingType(enumType);

            var underlyingValue = System.Convert.ChangeType(value, enumStorageType, CultureInfo.InvariantCulture);

            return underlyingValue;
        }
    }
}