﻿// -----------------------------------------------------------------------
// <copyright file="ObjectInfo.cs" company="MicroLite">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Logging;

    /// <summary>
    /// The class which describes a type and the table it is mapped to.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ObjectInfo for {ForType}")]
    public sealed class ObjectInfo
    {
        private static readonly object locker = new object();
        private static readonly ILog log = LogManager.GetLog("ObjectInfo");
        private static readonly IDictionary<Type, ObjectInfo> objectInfos = new Dictionary<Type, ObjectInfo>();
        private static IMappingConvention mappingConvention = new AttributeMappingConvention();

        // Key is the column name, value is the property info for the property.
        private readonly IDictionary<string, PropertyInfo> columnProperties;

        private readonly Type forType;
        private readonly TableInfo tableInfo;

        /// <summary>
        /// Initialises a new instance of the <see cref="ObjectInfo"/> class.
        /// </summary>
        /// <param name="forType">The type the object info relates to.</param>
        /// <param name="tableInfo">The table info.</param>
        public ObjectInfo(Type forType, TableInfo tableInfo)
        {
            if (forType == null)
            {
                throw new ArgumentNullException("forType");
            }

            if (tableInfo == null)
            {
                throw new ArgumentNullException("tableInfo");
            }

            this.forType = forType;
            this.tableInfo = tableInfo;
            this.columnProperties = this.tableInfo.Columns.ToDictionary(x => x.ColumnName, x => x.PropertyInfo);

            var identifierPropertyInfo = this.columnProperties[this.tableInfo.IdentifierColumn];
            if (identifierPropertyInfo.PropertyType.IsValueType)
            {
                this.DefaultIdentifierValue = (ValueType)Activator.CreateInstance(identifierPropertyInfo.PropertyType);
            }
        }

        /// <summary>
        /// Gets the default identifier value.
        /// </summary>
        public object DefaultIdentifierValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets type the object info deals with.
        /// </summary>
        public Type ForType
        {
            get
            {
                return this.forType;
            }
        }

        /// <summary>
        /// Gets the table info.
        /// </summary>
        public TableInfo TableInfo
        {
            get
            {
                return this.tableInfo;
            }
        }

        internal static IMappingConvention MappingConvention
        {
            get
            {
                return ObjectInfo.mappingConvention;
            }

            set
            {
                ObjectInfo.mappingConvention = value;
            }
        }

        /// <summary>
        /// Gets the object info for the specified type.
        /// </summary>
        /// <param name="forType">The type to get the object info for.</param>
        /// <returns>The <see cref="ObjectInfo"/> for the specified <see cref="Type"/>.</returns>
        public static ObjectInfo For(Type forType)
        {
            if (forType == null)
            {
                throw new ArgumentNullException("forType");
            }

            lock (locker)
            {
                if (!objectInfos.ContainsKey(forType))
                {
                    VerifyType(forType);

                    log.TryLogDebug(LogMessages.ObjectInfo_CreatingObjectInfo, forType.FullName);
                    var objectInfo = MappingConvention.CreateObjectInfo(forType);
                    objectInfos.Add(forType, objectInfo);
                }

                log.TryLogDebug(LogMessages.ObjectInfo_RetrievingObjectInfo, forType.FullName);
                return objectInfos[forType];
            }
        }

        /// <summary>
        /// Gets the property info for supplied column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The <see cref="PropertyInfo"/> for the column, or null if it is not a mapped column.</returns>
        public PropertyInfo GetPropertyInfoForColumn(string columnName)
        {
            if (this.columnProperties.ContainsKey(columnName))
            {
                return this.columnProperties[columnName];
            }

            return null;
        }

        /// <summary>
        /// Determines whether the supplied instance has the default identifier value.
        /// </summary>
        /// <param name="instance">The instance.</param>
        /// <returns>
        ///   <c>true</c> if the instance has the default identifier value; otherwise, <c>false</c>.
        /// </returns>
        public bool HasDefaultIdentifierValue(object instance)
        {
            var identifierPropertyInfo = this.GetPropertyInfoForColumn(this.TableInfo.IdentifierColumn);

            var identifierValue = identifierPropertyInfo.GetValue(instance);

            return object.Equals(identifierValue, this.DefaultIdentifierValue);
        }

        private static void VerifyType(Type forType)
        {
            string message = null;

            if (!forType.IsClass)
            {
                message = LogMessages.TypeMustBeClass.FormatWith(forType.Name);
            }
            else if (forType.IsAbstract)
            {
                message = LogMessages.TypeMustNotBeAbstract.FormatWith(forType.Name);
            }
            else if (forType.GetConstructor(Type.EmptyTypes) == null)
            {
                message = LogMessages.TypeMustHaveDefaultConstructor.FormatWith(forType.Name);
            }

            if (message != null)
            {
                log.TryLogFatal(message);
                throw new MicroLiteException(message);
            }
        }
    }
}