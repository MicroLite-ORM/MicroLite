// -----------------------------------------------------------------------
// <copyright file="ExpandoObjectInfo.cs" company="MicroLite">
// Copyright 2012 - 2015 Project Contributors
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
                throw new NotSupportedException(ExceptionMessages.ExpandoObjectInfo_NotSupportedReason);
            }
        }

        public object CreateInstance(IDataReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            if (log.IsDebug)
            {
                log.Debug(LogMessages.ObjectInfo_CreatingInstance, forType.Name);
            }

            var instance = new ExpandoObject();
            var dictionary = (IDictionary<string, object>)instance;

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var value = reader.IsDBNull(i) ? null : reader.GetValue(i);

                dictionary[columnName] = value;
            }

            return instance;
        }

        public ColumnInfo GetColumnInfo(string columnName)
        {
            throw new NotSupportedException(ExceptionMessages.ExpandoObjectInfo_NotSupportedReason);
        }

        public object GetIdentifierValue(object instance)
        {
            throw new NotSupportedException(ExceptionMessages.ExpandoObjectInfo_NotSupportedReason);
        }

        public SqlArgument[] GetInsertValues(object instance)
        {
            throw new NotSupportedException(ExceptionMessages.ExpandoObjectInfo_NotSupportedReason);
        }

        public SqlArgument[] GetUpdateValues(object instance)
        {
            throw new NotSupportedException(ExceptionMessages.ExpandoObjectInfo_NotSupportedReason);
        }

        public bool HasDefaultIdentifierValue(object instance)
        {
            throw new NotSupportedException(ExceptionMessages.ExpandoObjectInfo_NotSupportedReason);
        }

        public bool IsDefaultIdentifier(object identifier)
        {
            throw new NotSupportedException(ExceptionMessages.ExpandoObjectInfo_NotSupportedReason);
        }

        public void SetIdentifierValue(object instance, object identifier)
        {
            throw new NotSupportedException(ExceptionMessages.ExpandoObjectInfo_NotSupportedReason);
        }

        public void VerifyInstanceForInsert(object instance)
        {
            throw new NotSupportedException(ExceptionMessages.ExpandoObjectInfo_NotSupportedReason);
        }
    }
}