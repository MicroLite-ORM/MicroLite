// -----------------------------------------------------------------------
// <copyright file="ExpandoObjectInfo.cs" company="MicroLite">
// Copyright 2012 - 2014 Project Contributors
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
    using System.Data;
    using System.Dynamic;
    using MicroLite.Logging;

    [System.Diagnostics.DebuggerDisplay("ObjectInfo for {ForType}")]
    internal sealed class ExpandoObjectInfo : IObjectInfo
    {
        private static readonly Type forType = typeof(ExpandoObject);
        private static readonly ILog log = LogManager.GetCurrentClassLog();

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

        public T CreateInstance<T>()
        {
            if (log.IsDebug)
            {
                log.Debug(Messages.ObjectInfo_CreatingInstance, this.ForType.FullName);
            }

            var instance = new ExpandoObject();

            return (dynamic)instance;
        }

        public ColumnInfo GetColumnInfo(string columnName)
        {
            throw new NotSupportedException(Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        public object GetIdentifierValue(object instance)
        {
            throw new NotSupportedException(Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        public object[] GetInsertValues(object instance)
        {
            throw new NotSupportedException(Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        public object[] GetUpdateValues(object instance)
        {
            throw new NotSupportedException(Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        public bool HasDefaultIdentifierValue(object instance)
        {
            throw new NotSupportedException(Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        public void SetIdentifierValue(object instance, object identifier)
        {
            throw new NotSupportedException(Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        public void SetPropertyValues<T>(T instance, IDataReader reader)
        {
            var dictionary = (IDictionary<string, object>)instance;

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);

                if (log.IsDebug)
                {
                    log.Debug(Messages.ObjectInfo_SettingPropertyValue, this.ForType.Name, columnName);
                }

                var value = reader.IsDBNull(i) ? null : reader.GetValue(i);

                dictionary[columnName] = value;
            }
        }
    }

#endif
}