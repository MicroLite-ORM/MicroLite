// -----------------------------------------------------------------------
// <copyright file="ExpandoObjectInfo.cs" company="MicroLite">
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
#if !NET_3_5

    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using MicroLite.Logging;

    [System.Diagnostics.DebuggerDisplay("ObjectInfo for {ForType}")]
    internal sealed class ExpandoObjectInfo : IObjectInfo
    {
        private static readonly Type forType = typeof(ExpandoObject);
        private static readonly ILog log = LogManager.GetCurrentClassLog();

        public object DefaultIdentifierValue
        {
            get
            {
                throw new NotSupportedException(Messages.ExpandoObjectInfo_NotSupportedReason);
            }
        }

        public Type ForType
        {
            get
            {
                return forType;
            }
        }

        public TableInfo TableInfo
        {
            get
            {
                throw new NotSupportedException(Messages.ExpandoObjectInfo_NotSupportedReason);
            }
        }

        public object CreateInstance()
        {
            return new ExpandoObject();
        }

        public object GetIdentifierValue(object instance)
        {
            throw new NotSupportedException(Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        public object GetPropertyValue(object instance, string propertyName)
        {
            throw new NotSupportedException(Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        public object GetPropertyValueForColumn(object instance, string columnName)
        {
            throw new NotSupportedException(Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        public bool HasDefaultIdentifierValue(object instance)
        {
            throw new NotSupportedException(Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        public void SetPropertyValue(object instance, string propertyName, object value)
        {
            throw new NotSupportedException(Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        public void SetPropertyValueForColumn(object instance, string columnName, object value)
        {
            var dictionary = (IDictionary<string, object>)instance;

            log.TryLogDebug(Messages.IObjectInfo_SettingPropertyValue, this.ForType.Name, columnName);
            dictionary.Add(columnName, value == DBNull.Value ? null : value);
        }
    }

#endif
}