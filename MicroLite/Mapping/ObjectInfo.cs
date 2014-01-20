// -----------------------------------------------------------------------
// <copyright file="ObjectInfo.cs" company="MicroLite">
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
    using System;
    using System.Collections.Generic;
    using System.Data;
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
        private static IMappingConvention mappingConvention;

        private static IDictionary<Type, IObjectInfo> objectInfos = new Dictionary<Type, IObjectInfo>
        {
#if !NET_3_5
            { typeof(System.Dynamic.ExpandoObject), new ExpandoObjectInfo() },
            { typeof(object), new ExpandoObjectInfo() } // If the generic argument <dynamic> is used (in ISession.Fetch for example), typeof(T) will return object.
#endif
        };

        private readonly object defaultIdentifierValue;
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

            this.forType = forType;
            this.tableInfo = tableInfo;
            this.propertyAccessors = new Dictionary<string, IPropertyAccessor>(this.tableInfo.Columns.Count);

            for (int i = 0; i < this.tableInfo.Columns.Count; i++)
            {
                var columnInfo = this.tableInfo.Columns[i];

                this.propertyAccessors.Add(columnInfo.PropertyInfo.Name, PropertyAccessor.Create(columnInfo.PropertyInfo));

                if (columnInfo.IsIdentifier && columnInfo.PropertyInfo.PropertyType.IsValueType)
                {
                    this.defaultIdentifierValue = (ValueType)Activator.CreateInstance(columnInfo.PropertyInfo.PropertyType);
                }
            }
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
                if (mappingConvention == null)
                {
                    mappingConvention = mappingConvention = new ConventionMappingConvention(ConventionMappingSettings.Default);
                }

                return mappingConvention;
            }

            set
            {
                mappingConvention = value;
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

                if (log.IsDebug)
                {
                    log.Debug(Messages.ObjectInfo_CreatingObjectInfo, forType.FullName);
                }

                objectInfo = MappingConvention.CreateObjectInfo(forType);

                var newObjectInfos = new Dictionary<Type, IObjectInfo>(objectInfos);
                newObjectInfos[forType] = objectInfo;

                objectInfos = newObjectInfos;
            }

            if (log.IsDebug)
            {
                log.Debug(Messages.ObjectInfo_RetrievingObjectInfo, forType.FullName);
            }

            return objectInfo;
        }

        /// <summary>
        /// Creates a new instance of the type.
        /// </summary>
        /// <returns>A new instance of the type.</returns>
        public object CreateInstance()
        {
            if (log.IsDebug)
            {
                log.Debug(Messages.ObjectInfo_CreatingInstance, this.forType.FullName);
            }

            return Activator.CreateInstance(this.forType);
        }

        /// <summary>
        /// Gets the column information for the column with the specified name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>The ColumnInfo or null if no column is mapped for the object with the specified name.</returns>
        public ColumnInfo GetColumnInfo(string columnName)
        {
            ColumnInfo columnInfo = null;

            for (int i = 0; i < this.tableInfo.Columns.Count; i++)
            {
                if (this.tableInfo.Columns[i].ColumnName == columnName)
                {
                    columnInfo = this.tableInfo.Columns[i];
                    break;
                }
            }

            return columnInfo;
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

            var value = this.GetPropertyValue(instance, this.tableInfo.IdentifierProperty);

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
                var message = Messages.ObjectInfo_UnknownProperty.FormatWith(this.ForType.Name, propertyName);
                log.Error(message);
                throw new MappingException(message);
            }

            if (log.IsDebug)
            {
                log.Debug(Messages.ObjectInfo_GettingPropertyValue, this.ForType.Name, propertyName);
            }

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

            var columnInfo = this.GetColumnInfo(columnName);

            if (columnInfo == null)
            {
                var message = Messages.ObjectInfo_ColumnNotMapped.FormatWith(columnName, this.ForType.Name);
                log.Error(message);
                throw new MappingException(message);
            }

            if (log.IsDebug)
            {
                log.Debug(Messages.ObjectInfo_GettingPropertyValueForColumn, this.ForType.Name, columnInfo.PropertyInfo.Name, columnName);
            }

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

            bool hasDefaultIdentifier = object.Equals(identifierValue, this.defaultIdentifierValue);

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
                var message = Messages.ObjectInfo_UnknownProperty.FormatWith(this.ForType.Name, propertyName);
                log.Error(message);
                throw new MappingException(message);
            }

            if (log.IsDebug)
            {
                log.Debug(Messages.ObjectInfo_SettingPropertyValue, this.ForType.Name, propertyName);
            }

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

            var columnInfo = this.GetColumnInfo(columnName);

            if (columnInfo == null)
            {
                var message = Messages.ObjectInfo_UnknownColumn.FormatWith(this.ForType.Name, columnName);
                log.Error(message);
                throw new MappingException(message);
            }

            if (log.IsDebug)
            {
                log.Debug(Messages.ObjectInfo_SettingPropertyValue, this.ForType.Name, columnInfo.PropertyInfo.Name);
            }

            var typeConverter = TypeConverter.For(columnInfo.PropertyInfo.PropertyType);

            var converted = typeConverter.ConvertFromDbValue(value, columnInfo.PropertyInfo.PropertyType);

            this.propertyAccessors[columnInfo.PropertyInfo.Name].SetValue(instance, converted);
        }

        /// <summary>
        /// Sets the property value for each property mapped to a column in the specified IDataReader after converting it to the correct type for the property.
        /// </summary>
        /// <typeparam name="T">The type of the instance to set the values for.</typeparam>
        /// <param name="instance">The instance to set the property value on.</param>
        /// <param name="reader">The IDataReader containing the query results.</param>
        public void SetPropertyValues<T>(T instance, IDataReader reader)
        {
            this.VerifyInstanceIsCorrectTypeForThisObjectInfo(instance);

            for (int i = 0; i < reader.FieldCount; i++)
            {
                if (reader.IsDBNull(i))
                {
                    continue;
                }

                var columnName = reader.GetName(i);
                var columnInfo = this.GetColumnInfo(columnName);

                if (columnInfo == null)
                {
                    if (log.IsWarn)
                    {
                        log.Warn(Messages.ObjectInfo_UnknownColumn, this.ForType.Name, columnName);
                    }

                    continue;
                }

                switch (columnInfo.PropertyInfo.PropertyType.ResolveActualType().FullName)
                {
                    case "System.Boolean":
                        this.SetBoolean<T>(instance, reader, i, columnInfo);
                        continue;

                    case "System.Byte":
                        this.SetByte<T>(instance, reader, i, columnInfo);
                        continue;

                    case "System.Char":
                        this.SetChar<T>(instance, reader, i, columnInfo);
                        continue;

                    case "System.DateTime":
                        this.SetDateTime<T>(instance, reader, i, columnInfo);
                        continue;

                    case "System.Decimal":
                        this.SetDecimal<T>(instance, reader, i, columnInfo);
                        continue;

                    case "System.Double":
                        this.SetDouble<T>(instance, reader, i, columnInfo);
                        continue;

                    case "System.Single":
                        this.SetSingle<T>(instance, reader, i, columnInfo);
                        continue;

                    case "System.Guid":
                        this.SetGuid<T>(instance, reader, i, columnInfo);
                        continue;

                    case "System.Int16":
                        this.SetInt16<T>(instance, reader, i, columnInfo);
                        continue;

                    case "System.Int32":
                        this.SetInt32<T>(instance, reader, i, columnInfo);
                        continue;

                    case "System.Int64":
                        this.SetInt64<T>(instance, reader, i, columnInfo);
                        continue;

                    case "System.String":
                        ((IPropertyAccessor<T, string>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, reader.GetString(i));
                        continue;

                    default:
                        this.SetPropertyValueForColumn(instance, columnName, reader.GetValue(i));
                        continue;
                }
            }
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
                log.Fatal(message);
                throw new MappingException(message);
            }
        }

        private void SetBoolean<T>(T instance, IDataReader reader, int i, ColumnInfo columnInfo)
        {
            if (columnInfo.PropertyInfo.PropertyType.IsGenericType)
            {
                ((IPropertyAccessor<T, bool?>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, (bool?)reader.GetBoolean(i));
            }
            else
            {
                ((IPropertyAccessor<T, bool>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, reader.GetBoolean(i));
            }
        }

        private void SetByte<T>(T instance, IDataReader reader, int i, ColumnInfo columnInfo)
        {
            if (columnInfo.PropertyInfo.PropertyType.IsGenericType)
            {
                ((IPropertyAccessor<T, byte?>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, (byte?)reader.GetByte(i));
            }
            else
            {
                ((IPropertyAccessor<T, byte>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, reader.GetByte(i));
            }
        }

        private void SetChar<T>(T instance, IDataReader reader, int i, ColumnInfo columnInfo)
        {
            if (columnInfo.PropertyInfo.PropertyType.IsGenericType)
            {
                ((IPropertyAccessor<T, char?>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, (char?)reader.GetChar(i));
            }
            else
            {
                ((IPropertyAccessor<T, char>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, reader.GetChar(i));
            }
        }

        private void SetDateTime<T>(T instance, IDataReader reader, int i, ColumnInfo columnInfo)
        {
            if (columnInfo.PropertyInfo.PropertyType.IsGenericType)
            {
                ((IPropertyAccessor<T, DateTime?>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, (DateTime?)reader.GetDateTime(i));
            }
            else
            {
                ((IPropertyAccessor<T, DateTime>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, reader.GetDateTime(i));
            }
        }

        private void SetDecimal<T>(T instance, IDataReader reader, int i, ColumnInfo columnInfo)
        {
            if (columnInfo.PropertyInfo.PropertyType.IsGenericType)
            {
                ((IPropertyAccessor<T, decimal?>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, (decimal?)reader.GetDecimal(i));
            }
            else
            {
                ((IPropertyAccessor<T, decimal>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, reader.GetDecimal(i));
            }
        }

        private void SetDouble<T>(T instance, IDataReader reader, int i, ColumnInfo columnInfo)
        {
            if (columnInfo.PropertyInfo.PropertyType.IsGenericType)
            {
                ((IPropertyAccessor<T, double?>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, (double?)reader.GetDouble(i));
            }
            else
            {
                ((IPropertyAccessor<T, double>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, reader.GetDouble(i));
            }
        }

        private void SetGuid<T>(T instance, IDataReader reader, int i, ColumnInfo columnInfo)
        {
            if (columnInfo.PropertyInfo.PropertyType.IsGenericType)
            {
                ((IPropertyAccessor<T, Guid?>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, (Guid?)reader.GetGuid(i));
            }
            else
            {
                ((IPropertyAccessor<T, Guid>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, reader.GetGuid(i));
            }
        }

        private void SetInt16<T>(T instance, IDataReader reader, int i, ColumnInfo columnInfo)
        {
            if (columnInfo.PropertyInfo.PropertyType.IsGenericType)
            {
                ((IPropertyAccessor<T, short?>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, (short?)reader.GetInt16(i));
            }
            else
            {
                ((IPropertyAccessor<T, short>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, reader.GetInt16(i));
            }
        }

        private void SetInt32<T>(T instance, IDataReader reader, int i, ColumnInfo columnInfo)
        {
            if (columnInfo.PropertyInfo.PropertyType.IsGenericType)
            {
                ((IPropertyAccessor<T, int?>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, (int?)reader.GetInt32(i));
            }
            else
            {
                ((IPropertyAccessor<T, int>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, reader.GetInt32(i));
            }
        }

        private void SetInt64<T>(T instance, IDataReader reader, int i, ColumnInfo columnInfo)
        {
            if (columnInfo.PropertyInfo.PropertyType.IsGenericType)
            {
                ((IPropertyAccessor<T, long?>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, (long?)reader.GetInt64(i));
            }
            else
            {
                ((IPropertyAccessor<T, long>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, reader.GetInt64(i));
            }
        }

        private void SetSingle<T>(T instance, IDataReader reader, int i, ColumnInfo columnInfo)
        {
            if (columnInfo.PropertyInfo.PropertyType.IsGenericType)
            {
                ((IPropertyAccessor<T, float?>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, (float?)reader.GetFloat(i));
            }
            else
            {
                ((IPropertyAccessor<T, float>)this.propertyAccessors[columnInfo.PropertyInfo.Name]).SetValue(instance, reader.GetFloat(i));
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
                var mesage = Messages.ObjectInfo_TypeMismatch.FormatWith(instance.GetType().Name, this.ForType.Name);
                log.Error(mesage);
                throw new MappingException(mesage);
            }
        }
    }
}