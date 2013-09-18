// -----------------------------------------------------------------------
// <copyright file="PropertyAccessor.cs" company="MicroLite">
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
namespace MicroLite.Mapping
{
    using System;
    using System.Reflection;

    internal static class PropertyAccessor
    {
        internal static IPropertyAccessor Create(PropertyInfo propertyInfo)
        {
            var propertyAccessor = (IPropertyAccessor)Activator.CreateInstance(
                typeof(PropertyAccessorImpl<,>).MakeGenericType(propertyInfo.DeclaringType, propertyInfo.PropertyType),
                propertyInfo);

            return propertyAccessor;
        }

        [System.Diagnostics.DebuggerDisplay("PropertyAccessor for {propertyInfo.Name}")]
        private sealed class PropertyAccessorImpl<TObject, TValue> : IPropertyAccessor
        {
            private readonly Func<TObject, TValue> getMethod;
            private readonly Action<TObject, TValue> setMethod;

            public PropertyAccessorImpl(PropertyInfo propertyInfo)
            {
                MethodInfo getMethodInfo = propertyInfo.GetGetMethod(nonPublic: true);
                MethodInfo setMethodInfo = propertyInfo.GetSetMethod(nonPublic: true);

                this.getMethod = (Func<TObject, TValue>)Delegate.CreateDelegate(typeof(Func<TObject, TValue>), getMethodInfo);
                this.setMethod = (Action<TObject, TValue>)Delegate.CreateDelegate(typeof(Action<TObject, TValue>), setMethodInfo);
            }

            public object GetValue(object instance)
            {
                object value = this.getMethod((TObject)instance);

                return value;
            }

            public void SetValue(object instance, object value)
            {
                if (value == DBNull.Value)
                {
                    return;
                }

                this.setMethod((TObject)instance, (TValue)value);
            }
        }
    }
}