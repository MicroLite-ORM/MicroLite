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
namespace MicroLite.Mapping
{
    using System;
    using System.Globalization;

    internal sealed class EnumTypeConverter : TypeConverter
    {
        public override bool CanConvert(Type type)
        {
            var actualType = this.ResolveActualType(type);

            return actualType.IsEnum;
        }

        public override object Convert(object value, Type type)
        {
            if (value == DBNull.Value)
            {
                return null;
            }

            var enumType = this.ResolveActualType(type);

            var enumStorageType = Enum.GetUnderlyingType(enumType);

            var underlyingValue = System.Convert.ChangeType(value, enumStorageType, CultureInfo.InvariantCulture);

            var enumValue = Enum.ToObject(enumType, underlyingValue);

            return enumValue;
        }
    }
}