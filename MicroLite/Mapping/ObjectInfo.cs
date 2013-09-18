// -----------------------------------------------------------------------
// <copyright file="ObjectInfo.cs" company="MicroLite">
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
    using System.Collections.Generic;

#if !NET_3_5

    using System.Dynamic;

#endif

    using System.Linq;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Logging;
    using MicroLite.TypeConverters;

    /// <summary>
    /// The class which describes a type and the table it is mapped to.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ObjectInfo for {ForType}")]
    public sealed class ObjectInfo : IObjectInfo
    {
        private static readonly ILog log = LogManager.GetCurrentClassLog();
        private static IMappingConvention mappingConvention = new ConventionMappingConvention(ConventionMappingSettings.Default);

        private static IDictionary<Type, IObjectInfo> objectInfos = new Dictionary<Type, IObjectInfo>
        {
#if !NET_3_5
            { typeof(ExpandoObject), new ExpandoObjectInfo() },
            { typeof(object), new ExpandoObjectInfo() } // If the generic argument <dynamic> is used (in ISession.Fetch for example), typeof(T) will return object.
#endif
        };

        private readonly Type forType;
        private readonly Dictionary<string, IPropertyAccessor> propertyAccessors; // key is property name.
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
            this.propertyAccessors = new Dictionary<string, IPropertyAccessor>(this.tableInfo.Columns.Count);

            foreach (var columnInfo in this.tableInfo.Columns)
            {
                log.TryLogDebug(Messages.ObjectInfo_MappingColumnToProperty, forType.Name, columnInfo.PropertyInfo.Name, columnInfo.ColumnName);
                this.propertyAccessors.Add(columnInfo.PropertyInfo.Name, PropertyAccessor.Create(columnInfo.PropertyInfo));

                if (columnInfo.IsIdentifier && columnInfo.PropertyInfo.PropertyType.IsValueType)
                {
                    this.DefaultIdentifierValue = (ValueType)Activator.CreateInstance(columnInfo.PropertyInfo.PropertyType);
                }
            }
        }

        /// <summary>
        /// Gets an object containing the default value for the type of identifier used by the type.
        /// </summary>
        public object DefaultIdentifierValue
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets type the object info relates to.
        /// </summary>
        public Type ForType
        {
            get
            {
                return this.forType;
            }
        }

        /// <summary>
        /// Gets the table info for the type the object info relates to.
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
        public static IObjectInfo For(Type forType)
        {
            if (forType == null)
            {
                throw new ArgumentNullException("forType");
            }

            IObjectInfo objectInfo;

            if (!objectInfos.TryGetValue(forType, out objectInfo))
            {
                VerifyType(forType);

                log.TryLogDebug(Messages.ObjectInfo_CreatingObjectInfo, forType.FullName);
                objectInfo = ObjectInfo.MappingConvention.CreateObjectInfo(forType);

                var newObjectInfos = new Dictionary<Type, IObjectInfo>(objectInfos);
                newObjectInfos[forType] = objectInfo;

                objectInfos = newObjectInfos;
            }

            log.TryLogDebug(Messages.ObjectInfo_RetrievingObjectInfo, forType.FullName);
            return objectInfo;
        }

        /// <summary>
        /// Creates a new instance of the type.
        /// </summary>
        /// <returns>A new instance of the type.</returns>
        public object CreateInstance()
        {
            return Activator.CreateInstance(this.forType);
        }

        /// <summary>
        /// Gets the property value for the object identifier.
        /// </summary>
        /// <param name="instance">The instance to retrieve the value from.</param>
        /// <returns>The value of the identifier property.</returns>
        /// <exception cref="ArgumentNullException">Thrown if instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the instance is not of the correct type.</exception>
        public object GetIdentifierValue(object instance)
        {
            this.VerifyInstanceIsCorrectTypeForThisObjectInfo(instance);

            var value = this.GetPropertyValueForColumn(instance, this.TableInfo.IdentifierColumn);

            return value;
        }

        /// <summary>
        /// Gets the property value for the specified property on the specified instance.
        /// </summary>
        /// <param name="instance">The instance to retrieve the value from.</param>
        /// <param name="propertyName">Name of the property to get the value for.</param>
        /// <returns>The value of the property.</returns>
        public object GetPropertyValue(object instance, string propertyName)
        {
            this.VerifyInstanceIsCorrectTypeForThisObjectInfo(instance);

            IPropertyAccessor propertyAccessor;

            if (!this.propertyAccessors.TryGetValue(propertyName, out propertyAccessor))
            {
                log.TryLogError(Messages.ObjectInfo_UnknownProperty, this.ForType.Name, propertyName);
                throw new MicroLiteException(Messages.ObjectInfo_UnknownProperty.FormatWith(this.ForType.Name, propertyName));
            }

            log.TryLogDebug(Messages.ObjectInfo_GettingPropertyValue, this.ForType.Name, propertyName);
            var value = propertyAccessor.GetValue(instance);

            return value;
        }

        /// <summary>
        /// Gets the property value from the specified instance and converts it to the correct type for the specified column.
        /// </summary>
        /// <param name="instance">The instance to retrieve the value from.</param>
        /// <param name="columnName">Name of the column to get the value for.</param>
        /// <returns>The column value of the property.</returns>
        /// <exception cref="ArgumentNullException">Thrown if instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the instance is not of the correct type.</exception>
        public object GetPropertyValueForColumn(object instance, string columnName)
        {
            this.VerifyInstanceIsCorrectTypeForThisObjectInfo(instance);

            var columnInfo = this.TableInfo.Columns.SingleOrDefault(c => c.ColumnName == columnName);

            if (columnInfo == null)
            {
                log.TryLogError(Messages.ObjectInfo_ColumnNotMapped, columnName, this.ForType.Name);
                throw new MicroLiteException(Messages.ObjectInfo_ColumnNotMapped.FormatWith(columnName, this.ForType.Name));
            }

            log.TryLogDebug(Messages.ObjectInfo_GettingPropertyValueForColumn, this.ForType.Name, columnInfo.PropertyInfo.Name, columnName);
            var value = this.propertyAccessors[columnInfo.PropertyInfo.Name].GetValue(instance);

            var typeConverter = TypeConverter.For(columnInfo.PropertyInfo.PropertyType);

            var converted = typeConverter.ConvertToDbValue(value, columnInfo.PropertyInfo.PropertyType);

            return converted;
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
            this.VerifyInstanceIsCorrectTypeForThisObjectInfo(instance);

            var identifierValue = this.GetPropertyValue(instance, this.TableInfo.IdentifierProperty);

            bool hasDefaultIdentifier = object.Equals(identifierValue, this.DefaultIdentifierValue);

            return hasDefaultIdentifier;
        }

        /// <summary>
        /// Sets the property value for the specified property on the specified instance to the specified value.
        /// </summary>
        /// <param name="instance">The instance to set the property value on.</param>
        /// <param name="propertyName">Name of the property to set the value for.</param>
        /// <param name="value">The value to be set.</param>
        /// <exception cref="ArgumentNullException">Thrown if instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the instance is not of the correct type.</exception>
        public void SetPropertyValue(object instance, string propertyName, object value)
        {
            this.VerifyInstanceIsCorrectTypeForThisObjectInfo(instance);

            IPropertyAccessor propertyAccessor;

            if (!this.propertyAccessors.TryGetValue(propertyName, out propertyAccessor))
            {
                log.TryLogError(Messages.ObjectInfo_UnknownProperty, this.ForType.Name, propertyName);
                throw new MicroLiteException(Messages.ObjectInfo_UnknownProperty.FormatWith(this.ForType.Name, propertyName));
            }

            log.TryLogDebug(Messages.IObjectInfo_SettingPropertyValue, this.ForType.Name, propertyName);
            propertyAccessor.SetValue(instance, value);
        }

        /// <summary>
        /// Sets the property value of the property mapped to the specified column after converting it to the correct type for the property.
        /// </summary>
        /// <param name="instance">The instance to set the property value on.</param>
        /// <param name="columnName">The name of the column the property is mapped to.</param>
        /// <param name="value">The value from the database column to set the property to.</param>
        /// <exception cref="ArgumentNullException">Thrown if instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the instance is not of the correct type.</exception>
        public void SetPropertyValueForColumn(object instance, string columnName, object value)
        {
            this.VerifyInstanceIsCorrectTypeForThisObjectInfo(instance);

            var columnInfo = this.TableInfo.Columns.SingleOrDefault(c => c.ColumnName == columnName);

            if (columnInfo == null)
            {
                log.TryLogError(Messages.ObjectInfo_UnknownColumn, this.ForType.Name, columnName);
                throw new MicroLiteException(Messages.ObjectInfo_UnknownColumn.FormatWith(this.ForType.Name, columnName));
            }

            var typeConverter = TypeConverter.For(columnInfo.PropertyInfo.PropertyType);

            var converted = typeConverter.ConvertFromDbValue(value, columnInfo.PropertyInfo.PropertyType);

            log.TryLogDebug(Messages.IObjectInfo_SettingPropertyValue, this.ForType.Name, columnInfo.PropertyInfo.Name);
            this.propertyAccessors[columnInfo.PropertyInfo.Name].SetValue(instance, converted);
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

        private void VerifyInstanceIsCorrectTypeForThisObjectInfo(object instance)
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
        }
    }
}