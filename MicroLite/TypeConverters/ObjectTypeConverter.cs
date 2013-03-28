// -----------------------------------------------------------------------
// <copyright file="ObjectTypeConverter.cs" company="MicroLite">
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

    internal sealed class ObjectTypeConverter : TypeConverter
    {
        public override bool CanConvert(Type type)
        {
            var actualType = this.ResolveActualType(type);

            return !actualType.IsEnum;
        }

        public override object ConvertFromDbValue(object value, Type type)
        {
            if (value == DBNull.Value)
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
                var converted = System.Convert.ChangeType(value, type, CultureInfo.InvariantCulture);

                return converted;
            }
        }
    }
}