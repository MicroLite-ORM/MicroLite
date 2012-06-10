// -----------------------------------------------------------------------
// <copyright file="PropertyInfoExtensions.cs" company="MicroLite">
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
namespace MicroLite.FrameworkExtensions
{
    using System;
    using System.Globalization;
    using System.Reflection;

    internal static class PropertyInfoExtensions
    {
        internal static object GetValue(this PropertyInfo propertyInfo, object instance)
        {
            var value = propertyInfo.GetValue(instance, null);

            if (propertyInfo.PropertyType.IsEnum)
            {
                return (int)value;
            }
            else
            {
                return value;
            }
        }

        internal static void SetValue<T>(this PropertyInfo propertyInfo, T instance, object value)
        {
            if (value == null && propertyInfo.PropertyType.IsValueType && propertyInfo.PropertyType.IsGenericType)
            {
                return;
            }

            if (propertyInfo.PropertyType.IsEnum)
            {
                propertyInfo.SetValue(instance, value, null);
            }
            else
            {
                if (propertyInfo.PropertyType.IsValueType && propertyInfo.PropertyType.IsGenericType)
                {
                    ValueType converted = (ValueType)value;
                    propertyInfo.SetValue(instance, converted, null);
                }
                else
                {
                    var converted = Convert.ChangeType(value, propertyInfo.PropertyType, CultureInfo.InvariantCulture);
                    propertyInfo.SetValue(instance, converted, null);
                }
            }
        }
    }
}