// -----------------------------------------------------------------------
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
        private static readonly ILog log = LogManager.GetLog("MicroLite.ObjectInfo");
        private static readonly IDictionary<Type, ObjectInfo> objectInfos = new Dictionary<Type, ObjectInfo>();
        private static IMappingConvention mappingConvention = new AttributeMappingConvention();

        // Key is the column name, value is the property info for the property.
        private readonly IDictionary<string, PropertyInfo> columnProperties = new Dictionary<string, PropertyInfo>();

        private readonly Type forType;
        private readonly TableInfo tableInfo;

        /// <summary>
        /// Initialises a new instance of the <see cref="ObjectInfo"/> class.
        /// </summary>
        /// <param name="forType">The type the object info relates to.</param>
        /// <param name="tableInfo">The table info.</param>
        /// <exception cref="ArgumentNullException">Thrown if forType or tableInfo are null.</exception>
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

            log.TryLogDebug(Messages.ObjectInfo_MappingTypeToTable, forType.FullName, tableInfo.Schema, tableInfo.Name);
            this.forType = forType;
            this.tableInfo = tableInfo;

            foreach (var columnInfo in this.tableInfo.Columns)
            {
                log.TryLogDebug(Messages.ObjectInfo_MappingColumnToProperty, forType.Name, columnInfo.PropertyInfo.Name, columnInfo.ColumnName);
                this.columnProperties.Add(columnInfo.ColumnName, columnInfo.PropertyInfo);
            }

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
        /// <exception cref="ArgumentNullException">Thrown if forType is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the specified type cannot be used with MicroLite.</exception>
        public static ObjectInfo For(Type forType)
        {
            if (forType == null)
            {
                throw new ArgumentNullException("forType");
            }

            ObjectInfo objectInfo;

            if (!objectInfos.TryGetValue(forType, out objectInfo))
            {
                VerifyType(forType);

                log.TryLogDebug(Messages.ObjectInfo_CreatingObjectInfo, forType.FullName);
                objectInfo = MappingConvention.CreateObjectInfo(forType);

                lock (locker)
                {
                    objectInfos[forType] = objectInfo;
                }
            }

            log.TryLogDebug(Messages.ObjectInfo_RetrievingObjectInfo, forType.FullName);
            return objectInfo;
        }

        /// <summary>
        /// Gets the property info for specified column name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The <see cref="PropertyInfo"/> for the column, or null if it is not a mapped column.</returns>
        public PropertyInfo GetPropertyInfoForColumn(string columnName)
        {
            PropertyInfo propertyInfo;

            this.columnProperties.TryGetValue(columnName, out propertyInfo);

            return propertyInfo;
        }

        /// <summary>
        /// Determines whether the specified instance has the default identifier value.
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
                message = Messages.ObjectInfo_TypeMustBeClass.FormatWith(forType.Name);
            }
            else if (forType.IsAbstract)
            {
                message = Messages.ObjectInfo_TypeMustNotBeAbstract.FormatWith(forType.Name);
            }
            else if (forType.GetConstructor(Type.EmptyTypes) == null)
            {
                message = Messages.ObjectInfo_TypeMustHaveDefaultConstructor.FormatWith(forType.Name);
            }

            if (message != null)
            {
                log.TryLogFatal(message);
                throw new MicroLiteException(message);
            }
        }
    }
}