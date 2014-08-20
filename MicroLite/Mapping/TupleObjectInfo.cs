// -----------------------------------------------------------------------
// <copyright file="TupleObjectInfo.cs" company="MicroLite">
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
    using System.Data;

    [System.Diagnostics.DebuggerDisplay("ObjectInfo for {ForType}")]
    internal sealed class TupleObjectInfo : IObjectInfo
    {
        private static readonly Type forType = typeof(Tuple);

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
                throw new NotSupportedException(ExceptionMessages.TupleObjectInfo_NotSupportedReason);
            }
        }

        public object CreateInstance(IDataReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            var fieldTypes = new Type[reader.FieldCount];
            var values = new object[reader.FieldCount];

            for (int i = 0; i < reader.FieldCount; i++)
            {
                fieldTypes[i] = reader.GetFieldType(i);
                values[i] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            }

            var tupleType = GetTupleType(fieldTypes);

            var tuple = Activator.CreateInstance(tupleType, values);

            return tuple;
        }

        public ColumnInfo GetColumnInfo(string columnName)
        {
            throw new NotSupportedException(ExceptionMessages.TupleObjectInfo_NotSupportedReason);
        }

        public object GetIdentifierValue(object instance)
        {
            throw new NotSupportedException(ExceptionMessages.TupleObjectInfo_NotSupportedReason);
        }

        public object[] GetInsertValues(object instance)
        {
            throw new NotSupportedException(ExceptionMessages.TupleObjectInfo_NotSupportedReason);
        }

        public object[] GetUpdateValues(object instance)
        {
            throw new NotSupportedException(ExceptionMessages.TupleObjectInfo_NotSupportedReason);
        }

        public bool HasDefaultIdentifierValue(object instance)
        {
            throw new NotSupportedException(ExceptionMessages.TupleObjectInfo_NotSupportedReason);
        }

        public bool IsDefaultIdentifier(object identifier)
        {
            throw new NotSupportedException(ExceptionMessages.TupleObjectInfo_NotSupportedReason);
        }

        public void SetIdentifierValue(object instance, object identifier)
        {
            throw new NotSupportedException(ExceptionMessages.TupleObjectInfo_NotSupportedReason);
        }

        public void VerifyInstanceForInsert(object instance)
        {
            throw new NotSupportedException(ExceptionMessages.TupleObjectInfo_NotSupportedReason);
        }

        private static Type GetTupleType(Type[] fieldTypes)
        {
            switch (fieldTypes.Length)
            {
                case 1:
                    return typeof(Tuple<>).MakeGenericType(fieldTypes);

                case 2:
                    return typeof(Tuple<,>).MakeGenericType(fieldTypes);

                case 3:
                    return typeof(Tuple<,,>).MakeGenericType(fieldTypes);

                case 4:
                    return typeof(Tuple<,,,>).MakeGenericType(fieldTypes);

                case 5:
                    return typeof(Tuple<,,,,>).MakeGenericType(fieldTypes);

                case 6:
                    return typeof(Tuple<,,,,,>).MakeGenericType(fieldTypes);

                case 7:
                    return typeof(Tuple<,,,,,,>).MakeGenericType(fieldTypes);

                default:
                    throw new NotSupportedException(ExceptionMessages.TupleObjectInfo_TupleNotSupported);
            }
        }
    }

#endif
}