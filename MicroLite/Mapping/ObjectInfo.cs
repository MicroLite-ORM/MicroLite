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
    using System.Linq;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Logging;

    /// <summary>
    /// The class which describes a type and the table it is mapped to.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ObjectInfo for {ForType}")]
    public sealed class ObjectInfo
    {
        private static readonly ILog log = LogManager.GetCurrentClassLog();
        private static IMappingConvention mappingConvention = new ConventionMappingConvention(ConventionMappingSettings.Default);
        private static IDictionary<Type, ObjectInfo> objectInfos = new Dictionary<Type, ObjectInfo>();

        private readonly Type forType;
        private readonly Dictionary<string, IPropertyAccessor> propertyAccessors;
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
            this.propertyAccessors = new Dictionary<string, IPropertyAccessor>(this.tableInfo.Columns.Count());

            foreach (var columnInfo in this.tableInfo.Columns)
            {
                log.TryLogDebug(Messages.ObjectInfo_MappingColumnToProperty, forType.Name, columnInfo.PropertyInfo.Name, columnInfo.ColumnName);
                this.propertyAccessors.Add(columnInfo.ColumnName, new PropertyAccessor(columnInfo.PropertyInfo));

                if (columnInfo.IsIdentifier && columnInfo.PropertyInfo.PropertyType.IsValueType)
                {
                    this.DefaultIdentifierValue = (ValueType)Activator.CreateInstance(columnInfo.PropertyInfo.PropertyType);
                }
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

                var newObjectInfos = new Dictionary<Type, ObjectInfo>(objectInfos);
                newObjectInfos[forType] = objectInfo;

                objectInfos = newObjectInfos;
            }

            log.TryLogDebug(Messages.ObjectInfo_RetrievingObjectInfo, forType.FullName);
            return objectInfo;
        }

        /// <summary>
        /// Gets the property value for the object identifier.
        /// </summary>
        /// <param name="instance">The instance to retrieve the value from.</param>
        /// <returns>The value of the identifier property.</returns>
        public object GetIdentifierValue(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            if (instance.GetType() != this.ForType)
            {
                log.TryLogError(Messages.ObjectInfo_TypeMismatch, instance.GetType().Name, this.ForType.Name);
                throw new MicroLiteException(Messages.ObjectInfo_TypeMismatch.FormatWith(instance.GetType().Name, this.ForType.Name));
            }

            return this.GetPropertyValueForColumn(instance, this.TableInfo.IdentifierColumn);
        }

        /// <summary>
        /// Gets the property value for the specified column.
        /// </summary>
        /// <param name="instance">The instance to retrieve the value from.</param>
        /// <param name="columnName">Name of the column to get the value for.</param>
        /// <returns>The value of the property.</returns>
        public object GetPropertyValueForColumn(object instance, string columnName)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            if (instance.GetType() != this.ForType)
            {
                log.TryLogError(Messages.ObjectInfo_TypeMismatch, instance.GetType().Name, this.ForType.Name);
                throw new MicroLiteException(Messages.ObjectInfo_TypeMismatch.FormatWith(instance.GetType().Name, this.ForType.Name));
            }

            var columnInfo = this.TableInfo.Columns.SingleOrDefault(c => c.ColumnName == columnName);

            if (columnInfo == null)
            {
                log.TryLogError(Messages.ObjectInfo_ColumnNotMapped, columnName, this.ForType.Name);
                throw new MicroLiteException(Messages.ObjectInfo_ColumnNotMapped.FormatWith(columnName, this.ForType.Name));
            }

            log.TryLogDebug(Messages.ObjectInfo_GetPropertyValueForColumn, this.ForType.Name, columnInfo.PropertyInfo.Name, columnName);
            var value = this.propertyAccessors[columnName].GetValue(instance);

            return value;
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
            var identifierValue = this.GetPropertyValueForColumn(instance, this.TableInfo.IdentifierColumn);

            return object.Equals(identifierValue, this.DefaultIdentifierValue);
        }

        /// <summary>
        /// Sets the property value mapped to the specified column on the instance.
        /// </summary>
        /// <param name="instance">The instance to set the property value on.</param>
        /// <param name="columnName">The name of the column the property is mapped to.</param>
        /// <param name="value">The value to set the property to.</param>
        public void SetPropertyValueForColumn(object instance, string columnName, object value)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            if (instance.GetType() != this.ForType)
            {
                log.TryLogError(Messages.ObjectInfo_TypeMismatch, instance.GetType().Name, this.ForType.Name);
                throw new MicroLiteException(Messages.ObjectInfo_TypeMismatch.FormatWith(instance.GetType().Name, this.ForType.Name));
            }

            var columnInfo = this.TableInfo.Columns.SingleOrDefault(c => c.ColumnName == columnName);

            if (columnInfo == null)
            {
                log.TryLogWarn(Messages.ObjectInfo_UnknownProperty, this.ForType.Name, columnName);
                return;
            }

            log.TryLogDebug(Messages.ObjectInfo_SettingPropertyValue, this.ForType.Name, columnInfo.PropertyInfo.Name);
            this.propertyAccessors[columnName].SetValue(instance, value);
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