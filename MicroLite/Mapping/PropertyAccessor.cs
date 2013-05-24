// -----------------------------------------------------------------------
// <copyright file="PropertyAccessor.cs" company="MicroLite">
// Copyright 2012 - 2013 Trevor Pilley
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

    [System.Diagnostics.DebuggerDisplay("PropertyAccessor for {propertyName}")]
    internal sealed class PropertyAccessor : IPropertyAccessor
    {
        private readonly IPropertyAccessor propertyAccessor;

        internal PropertyAccessor(PropertyInfo propertyInfo)
        {
            this.propertyAccessor = (IPropertyAccessor)Activator.CreateInstance(
                typeof(PropertyAccessorImpl<,>).MakeGenericType(propertyInfo.DeclaringType, propertyInfo.PropertyType),
                propertyInfo);
        }

        public object GetValue(object instance)
        {
            return this.propertyAccessor.GetValue(instance);
        }

        public void SetValue(object instance, object value)
        {
            if (value == DBNull.Value)
            {
                return;
            }

            this.propertyAccessor.SetValue(instance, value);
        }

        private sealed class PropertyAccessorImpl<TObject, TValue> : IPropertyAccessor
        {
            private readonly Func<TObject, TValue> getMethod;
            private readonly PropertyInfo propertyInfo;
            private readonly Action<TObject, TValue> setMethod;

            public PropertyAccessorImpl(PropertyInfo propertyInfo)
            {
                this.propertyInfo = propertyInfo;

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
                this.setMethod((TObject)instance, (TValue)value);
            }
        }
    }
}