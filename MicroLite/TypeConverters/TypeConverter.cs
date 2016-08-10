// -----------------------------------------------------------------------
// <copyright file="TypeConverter.cs" company="MicroLite">
// Copyright 2012 - 2016 Project Contributors
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
    using System.Collections.Generic;
    using System.Data;

    /// <summary>
    /// A class which allows access to <see cref="ITypeConverter"/>s.
    /// </summary>
    public static class TypeConverter
    {
        private static readonly IDictionary<Type, DbType> dbTypeMap = new Dictionary<Type, DbType>
        {
            { typeof(byte), DbType.Byte },
            { typeof(byte?), DbType.Byte },
            { typeof(sbyte), DbType.SByte },
            { typeof(sbyte?), DbType.SByte },
            { typeof(short), DbType.Int16 },
            { typeof(short?), DbType.Int16 },
            { typeof(ushort), DbType.UInt16 },
            { typeof(ushort?), DbType.UInt16 },
            { typeof(int), DbType.Int32 },
            { typeof(int?), DbType.Int32 },
            { typeof(uint), DbType.UInt32 },
            { typeof(uint?), DbType.UInt32 },
            { typeof(long), DbType.Int64 },
            { typeof(long?), DbType.Int64 },
            { typeof(ulong), DbType.UInt64 },
            { typeof(ulong?), DbType.UInt64 },
            { typeof(float), DbType.Single },
            { typeof(float?), DbType.Single },
            { typeof(decimal), DbType.Decimal },
            { typeof(decimal?), DbType.Decimal },
            { typeof(double), DbType.Double },
            { typeof(double?), DbType.Double },
            { typeof(bool), DbType.Boolean },
            { typeof(bool?), DbType.Boolean },
            { typeof(char), DbType.StringFixedLength },
            { typeof(char?), DbType.StringFixedLength },
            { typeof(string), DbType.String },
            { typeof(byte[]), DbType.Binary },
            { typeof(DateTime), DbType.DateTime2 },
            { typeof(DateTime?), DbType.DateTime2 },
            { typeof(DateTimeOffset), DbType.DateTimeOffset },
            { typeof(DateTimeOffset?), DbType.DateTimeOffset },
            { typeof(Guid), DbType.Guid },
            { typeof(Guid?), DbType.Guid }
        };

        private static readonly ITypeConverter defaultConverter = new ObjectTypeConverter();
        private static readonly TypeConverterCollection typeConverters = new TypeConverterCollection();

        /// <summary>
        /// Gets the type converter collection which contains all type converters registered with the MicroLite ORM framework.
        /// </summary>
        public static TypeConverterCollection Converters
        {
            get
            {
                return typeConverters;
            }
        }

        /// <summary>
        /// Gets the default type converter which can be used if there is no specific type converter for a given type.
        /// </summary>
        public static ITypeConverter Default
        {
            get
            {
                return defaultConverter;
            }
        }

        /// <summary>
        /// Gets the <see cref="ITypeConverter"/> for the specified type.
        /// </summary>
        /// <param name="type">The type to get the converter for.</param>
        /// <returns>The <see cref="ITypeConverter"/> for the specified type, or null if no specific type converter exists for the type.</returns>
        /// <remarks>
        /// If For returns null, the TypeConverter.Default can be used.
        /// </remarks>
        public static ITypeConverter For(Type type)
        {
            for (int i = 0; i < Converters.Count; i++)
            {
                var typeConverter = Converters[i];

                if (typeConverter.CanConvert(type))
                {
                    return typeConverter;
                }
            }

            return null;
        }

        /// <summary>
        /// Determines whether the type is not an entity type and is a convertible type.
        /// </summary>
        /// <param name="type">The type to test.</param>
        /// <returns>
        /// true if the type is not an entity and can be converted.
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown if type is null.</exception>
        public static bool IsNotEntityAndConvertible(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (type.IsValueType || type == typeof(string))
            {
                return true;
            }

            for (int i = 0; i < Converters.Count; i++)
            {
                var typeConverter = Converters[i];

                if (typeConverter.CanConvert(type))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Registers the type mapping between a Type and DbType.
        /// </summary>
        /// <param name="type">The Type to be mapped.</param>
        /// <param name="dbType">The DbType to be mapped to.</param>
        public static void RegisterTypeMapping(Type type, DbType dbType)
        {
            dbTypeMap[type] = dbType;
        }

        /// <summary>
        /// Resolves the actual type.
        /// </summary>
        /// <param name="type">The type to resolve.</param>
        /// <returns>
        /// The actual type (e.g. the inner type if it is a nullable value).
        /// </returns>
        /// <exception cref="System.ArgumentNullException">Thrown if type is null.</exception>
        public static Type ResolveActualType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var actualType = type;

            if (type.IsGenericType && typeof(Nullable<>).IsAssignableFrom(type.GetGenericTypeDefinition()))
            {
                actualType = Nullable.GetUnderlyingType(type);
            }

            return actualType;
        }

        /// <summary>
        /// Resolves the DbType mapped to the Type.
        /// </summary>
        /// <param name="type">The Type to resolve the DbType from.</param>
        /// <returns>The DbType the Type maps to.</returns>
        /// <exception cref="System.NotSupportedException">Thrown if the Type is not mapped to a DbType.</exception>
        public static DbType ResolveDbType(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            var actualType = type;

            if (actualType.IsEnum)
            {
                actualType = Enum.GetUnderlyingType(actualType);
            }

            DbType dbType;

            if (dbTypeMap.TryGetValue(actualType, out dbType))
            {
                return dbType;
            }

            throw new NotSupportedException(type.FullName);
        }
    }
}