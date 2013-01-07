// -----------------------------------------------------------------------
// <copyright file="PropertyAccessor.cs" company="MicroLite">
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
    using System.Linq.Expressions;
    using System.Reflection;

    [System.Diagnostics.DebuggerDisplay("PropertyAccessor for {propertyName}")]
    internal sealed class PropertyAccessor
    {
        private readonly Delegate getter;
        private readonly PropertyInfo propertyInfo;
        private readonly Delegate setter;

        internal PropertyAccessor(PropertyInfo propertyInfo)
        {
            this.propertyInfo = propertyInfo;

            var instanceParameter = Expression.Parameter(propertyInfo.DeclaringType, "i");
            var propertyParameter = Expression.Property(instanceParameter, propertyInfo);
            var convert = Expression.TypeAs(propertyParameter, typeof(object));

            this.getter = Expression.Lambda(convert, instanceParameter).Compile();

            var argument = Expression.Parameter(typeof(object), "a");
            var setterCall = Expression.Call(instanceParameter, propertyInfo.GetSetMethod(), Expression.Convert(argument, propertyInfo.PropertyType));

            this.setter = Expression.Lambda(setterCall, instanceParameter, argument).Compile();
        }

        internal object GetValue(object instance)
        {
            var value = this.getter.DynamicInvoke(instance);

            if (this.propertyInfo.PropertyType.IsEnum)
            {
                return (int)value;
            }
            else
            {
                return value;
            }
        }

        internal void SetValue(object instance, object value)
        {
            if (value == DBNull.Value)
            {
                return;
            }

            var typeConverter = TypeConverter.ForType(this.propertyInfo.PropertyType);

            var converted = typeConverter.Convert(value, this.propertyInfo.PropertyType);

            this.setter.DynamicInvoke(instance, converted);
        }
    }
}