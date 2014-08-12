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

    /// <summary>
    /// The class which describes a type and the table it is mapped to.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ObjectInfo for {ForType}")]
    public sealed class ObjectInfo : IObjectInfo
    {
        private static readonly ILog log = LogManager.GetCurrentClassLog();
        private static IMappingConvention mappingConvention;
        private static IDictionary<Type, IObjectInfo> objectInfos = GetObjectInfos();
        private readonly object defaultIdentifierValue;
        private readonly Type forType;
        private readonly Func<object, object> getIdentifierValue;
        private readonly Func<object, object[]> getInsertValues;
        private readonly Func<object, object[]> getUpdateValues;
        private readonly Func<IDataReader, object> instanceFactory;
        private readonly Action<object, object> setIdentifierValue;
        private readonly TableInfo tableInfo;

        /// <summary>
        /// Initialises a new instance of the <see cref="ObjectInfo" /> class.
        /// </summary>
        /// <param name="forType">The type the object info relates to.</param>
        /// <param name="tableInfo">The table info.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if forType or tableInfo are null.
        /// </exception>
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

            if (this.tableInfo.IdentifierColumn != null && this.tableInfo.IdentifierColumn.PropertyInfo.PropertyType.IsValueType)
            {
                this.defaultIdentifierValue = (ValueType)Activator.CreateInstance(this.tableInfo.IdentifierColumn.PropertyInfo.PropertyType);
            }

            if (this.tableInfo.IdentifierColumn != null)
            {
                this.getIdentifierValue = DelegateFactory.CreateGetIdentifier(this);
                this.getInsertValues = DelegateFactory.CreateGetInsertValues(this);
                this.getUpdateValues = DelegateFactory.CreateGetUpdateValues(this);
                this.setIdentifierValue = DelegateFactory.CreateSetIdentifier(this);
            }

            this.instanceFactory = DelegateFactory.CreateInstanceFactory(this);
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
        /// <returns>The <see cref="ObjectInfo" /> for the specified <see cref="Type" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown if forType is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the specified type cannot be used with MicroLite.</exception>
        public static IObjectInfo For(Type forType)
        {
            if (forType == null)
            {
                throw new ArgumentNullException("forType");
            }

            if (log.IsDebug)
            {
                log.Debug(LogMessages.ObjectInfo_RetrievingObjectInfo, forType.FullName);
            }

            IObjectInfo objectInfo;

            if (!objectInfos.TryGetValue(forType, out objectInfo))
            {
                if (forType.IsGenericType)
                {
                    var forGenericType = forType.GetGenericTypeDefinition();

                    if (objectInfos.TryGetValue(forGenericType, out objectInfo))
                    {
                        return objectInfo;
                    }
                }

                VerifyType(forType);

                if (log.IsDebug)
                {
                    log.Debug(LogMessages.ObjectInfo_CreatingObjectInfo, forType.FullName);
                }

                objectInfo = MappingConvention.CreateObjectInfo(forType);

                var newObjectInfos = new Dictionary<Type, IObjectInfo>(objectInfos);
                newObjectInfos[forType] = objectInfo;

                objectInfos = newObjectInfos;
            }

            return objectInfo;
        }

        /// <summary>
        /// Creates a new instance of the type populated with the values from the specified IDataReader.
        /// </summary>
        /// <param name="reader">The IDataReader containing the values to build the instance from.</param>
        /// <returns>A new instance of the type populated with the values from the specified IDataReader.</returns>
        /// <exception cref="ArgumentNullException">Thrown if reader is null.</exception>
        public object CreateInstance(IDataReader reader)
        {
            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            var instance = this.instanceFactory(reader);

            return instance;
        }

        /// <summary>
        /// Gets the column information for the column with the specified name.
        /// </summary>
        /// <param name="columnName">Name of the column.</param>
        /// <returns>
        /// The ColumnInfo or null if no column is mapped for the object with the specified name.
        /// </returns>
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
            this.VerifyIdentifierMapped();

            var value = this.getIdentifierValue(instance);

            return value;
        }

        /// <summary>
        /// Gets the insert values for the specified instance.
        /// </summary>
        /// <param name="instance">The instance to retrieve the values from.</param>
        /// <returns>An array of values to be used for the insert command.</returns>
        /// <exception cref="ArgumentNullException">Thrown if instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the instance is not of the correct type.</exception>
        public object[] GetInsertValues(object instance)
        {
            this.VerifyInstanceIsCorrectTypeForThisObjectInfo(instance);
            this.VerifyIdentifierMapped();

            var insertValues = this.getInsertValues(instance);

            return insertValues;
        }

        /// <summary>
        /// Gets the update values for the specified instance.
        /// </summary>
        /// <param name="instance">The instance to retrieve the values from.</param>
        /// <returns>An array of values to be used for the update command.</returns>
        /// <exception cref="ArgumentNullException">Thrown if instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the instance is not of the correct type.</exception>
        public object[] GetUpdateValues(object instance)
        {
            this.VerifyInstanceIsCorrectTypeForThisObjectInfo(instance);
            this.VerifyIdentifierMapped();

            var updateValues = this.getUpdateValues(instance);

            return updateValues;
        }

        /// <summary>
        /// Determines whether the specified instance has the default identifier value.
        /// </summary>
        /// <param name="instance">The instance to verify.</param>
        /// <returns>
        ///   <c>true</c> if the instance has the default identifier value; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the instance is not of the correct type.</exception>
        public bool HasDefaultIdentifierValue(object instance)
        {
            this.VerifyInstanceIsCorrectTypeForThisObjectInfo(instance);
            this.VerifyIdentifierMapped();

            var identifierValue = this.getIdentifierValue(instance);

            bool hasDefaultIdentifier = object.Equals(identifierValue, this.defaultIdentifierValue);

            return hasDefaultIdentifier;
        }

        /// <summary>
        /// Determines whether the specified identifier value is the default identifier value.
        /// </summary>
        /// <param name="identifier">The identifier value to verify.</param>
        /// <returns>
        /// True if the identifier is the default value, otherwise false.
        /// </returns>
        public bool IsDefaultIdentifier(object identifier)
        {
            bool isDefaultIdentifier = object.Equals(identifier, this.defaultIdentifierValue);

            return isDefaultIdentifier;
        }

        /// <summary>
        /// Sets the property value for the object identifier to the supplied value.
        /// </summary>
        /// <param name="instance">The instance to set the value for.</param>
        /// <param name="identifier">The value to set as the identifier property.</param>
        /// <exception cref="ArgumentNullException">Thrown if instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the instance is not of the correct type.</exception>
        public void SetIdentifierValue(object instance, object identifier)
        {
            this.VerifyInstanceIsCorrectTypeForThisObjectInfo(instance);

            this.setIdentifierValue(instance, identifier);
        }

        /// <summary>
        /// Verifies the instance can be inserted.
        /// </summary>
        /// <param name="instance">The instance to verify.</param>
        /// <exception cref="ArgumentNullException">Thrown if instance is null.</exception>
        /// <exception cref="MicroLiteException">
        /// Thrown if the instance is not of the correct type or its state is invalid for the specified StatementType.
        /// </exception>
        public void VerifyInstanceForInsert(object instance)
        {
            this.VerifyIdentifierMapped();

            if (this.TableInfo.IdentifierStrategy == IdentifierStrategy.DbGenerated)
            {
                if (!this.HasDefaultIdentifierValue(instance))
                {
                    throw new MicroLiteException(ExceptionMessages.ObjectInfo_IdentifierSetForInsert);
                }
            }
            else if (this.TableInfo.IdentifierStrategy == IdentifierStrategy.Assigned)
            {
                if (this.HasDefaultIdentifierValue(instance))
                {
                    throw new MicroLiteException(ExceptionMessages.ObjectInfo_IdentifierNotSetForInsert);
                }
            }
        }

        /// <summary>
        /// Resets the object info state, removing any cached object information and restoring the default mapping convention.
        /// </summary>
        /// <remarks>
        /// Makes it easier to unit test using different mapping conventions - should remain an internal method.
        /// </remarks>
        internal static void Reset()
        {
            mappingConvention = null;
            objectInfos = GetObjectInfos();
        }

        private static Dictionary<Type, IObjectInfo> GetObjectInfos()
        {
            var dictionary = new Dictionary<Type, IObjectInfo>();

#if !NET_3_5
            var expandoObjectInfo = new ExpandoObjectInfo();
            dictionary.Add(typeof(System.Dynamic.ExpandoObject), expandoObjectInfo);
            dictionary.Add(typeof(object), expandoObjectInfo); // If the generic argument <dynamic> is used (in ISession.Fetch for example), typeof(T) will return object.

            var tupleObjectInfo = new TupleObjectInfo();
            dictionary.Add(typeof(Tuple<>), tupleObjectInfo);
            dictionary.Add(typeof(Tuple<,>), tupleObjectInfo);
            dictionary.Add(typeof(Tuple<,,>), tupleObjectInfo);
            dictionary.Add(typeof(Tuple<,,,>), tupleObjectInfo);
            dictionary.Add(typeof(Tuple<,,,,>), tupleObjectInfo);
            dictionary.Add(typeof(Tuple<,,,,,>), tupleObjectInfo);
            dictionary.Add(typeof(Tuple<,,,,,,>), tupleObjectInfo);
#endif

            return dictionary;
        }

        private static void VerifyType(Type forType)
        {
            if (!forType.IsClass)
            {
                throw new MappingException(ExceptionMessages.ObjectInfo_TypeMustBeClass.FormatWith(forType.Name));
            }

            if (forType.IsAbstract)
            {
                throw new MappingException(ExceptionMessages.ObjectInfo_TypeMustNotBeAbstract.FormatWith(forType.Name));
            }

            if (!forType.IsPublic && !forType.IsNestedPublic)
            {
                throw new MappingException(ExceptionMessages.ObjectInfo_TypeMustBePublic.FormatWith(forType.Name));
            }

            if (forType.GetConstructor(Type.EmptyTypes) == null)
            {
                throw new MappingException(ExceptionMessages.ObjectInfo_TypeMustHaveDefaultConstructor.FormatWith(forType.Name));
            }
        }

        private void VerifyIdentifierMapped()
        {
            if (this.tableInfo.IdentifierColumn == null)
            {
                throw new MicroLiteException(
                    ExceptionMessages.ObjectInfo_NoIdentifierColumn.FormatWith(this.tableInfo.Schema ?? string.Empty, this.tableInfo.Name));
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
                throw new MappingException(ExceptionMessages.ObjectInfo_TypeMismatch.FormatWith(instance.GetType().Name, this.ForType.Name));
            }
        }
    }
}