﻿// -----------------------------------------------------------------------
// <copyright file="PocoObjectInfo.cs" company="MicroLite">
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
    using System.Data;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Logging;

    /// <summary>
    /// The class which describes a type and the table it is mapped to.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ObjectInfo for {ForType}")]
    public sealed class PocoObjectInfo : IObjectInfo
    {
        private static readonly ILog log = LogManager.GetCurrentClassLog();
        private readonly object defaultIdentifierValue;
        private readonly Type forType;
        private readonly Func<object, object> getIdentifierValue;
        private readonly Func<object, SqlArgument[]> getInsertValues;
        private readonly Func<object, SqlArgument[]> getUpdateValues;
        private readonly Func<object, object> getVersionValue;
        private readonly Func<IDataReader, object> instanceFactory;
        private readonly Action<object, object> setIdentifierValue;
        private readonly Action<object, object> setVersionValue;
        private readonly TableInfo tableInfo;

        /// <summary>
        /// Initialises a new instance of the <see cref="PocoObjectInfo" /> class.
        /// </summary>
        /// <param name="forType">The type the object info relates to.</param>
        /// <param name="tableInfo">The table info.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if forType or tableInfo are null.
        /// </exception>
        public PocoObjectInfo(Type forType, TableInfo tableInfo)
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
                if (this.tableInfo.VersionColumn != null)
                {
                    this.getVersionValue = DelegateFactory.CreateGetVersion(this);
                    this.setVersionValue = DelegateFactory.CreateSetVersion(this);
                }
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

            if (log.IsDebug)
            {
                log.Debug(LogMessages.ObjectInfo_CreatingInstance, this.forType.FullName);
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
        public SqlArgument[] GetInsertValues(object instance)
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
        public SqlArgument[] GetUpdateValues(object instance)
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
            this.VerifyIdentifierMapped();

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

            if (this.TableInfo.IdentifierStrategy != IdentifierStrategy.Assigned)
            {
                if (!this.HasDefaultIdentifierValue(instance))
                {
                    throw new MicroLiteException(ExceptionMessages.PocoObjectInfo_IdentifierSetForInsert);
                }
            }
            else if (this.TableInfo.IdentifierStrategy == IdentifierStrategy.Assigned)
            {
                if (this.HasDefaultIdentifierValue(instance))
                {
                    throw new MicroLiteException(ExceptionMessages.PocoObjectInfo_IdentifierNotSetForInsert);
                }
            }
        }

        /// <summary>
        /// Gets the property value for the object version.
        /// </summary>
        /// <param name="instance">The instance to retrieve the value from.</param>
        /// <returns>The value of the version property.</returns>
        /// <exception cref="ArgumentNullException">Thrown if instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the instance is not of the correct type.</exception>
        public object GetVersionValue(object instance)
        {
            this.VerifyInstanceIsCorrectTypeForThisObjectInfo(instance);
            this.VerifyVersionMapped();

            var value = this.getVersionValue(instance);

            return value;
        }

        /// <summary>
        /// Sets the property value for the version to the supplied value.
        /// </summary>
        /// <param name="instance">The instance to set the value for.</param>
        /// <param name="version">The value to set as the version property.</param>
        /// <exception cref="ArgumentNullException">Thrown if instance is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the instance is not of the correct type.</exception>
        public void SetVersionValue(object instance, object version)
        {
            this.VerifyInstanceIsCorrectTypeForThisObjectInfo(instance);
            this.VerifyVersionMapped();

            this.setVersionValue(instance, version);
        }

        private void VerifyIdentifierMapped()
        {
            if (this.tableInfo.IdentifierColumn == null)
            {
                throw new MicroLiteException(
                    ExceptionMessages.PocoObjectInfo_NoIdentifierColumn.FormatWith(this.tableInfo.Schema ?? string.Empty, this.tableInfo.Name));
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
                throw new MappingException(ExceptionMessages.PocoObjectInfo_TypeMismatch.FormatWith(instance.GetType().Name, this.ForType.Name));
            }
        }

        private void VerifyVersionMapped()
        {
            if (this.tableInfo.VersionColumn == null)
            {
                throw new MicroLiteException(
                    ExceptionMessages.PocoObjectInfo_NoVersionColumn.FormatWith(this.tableInfo.Schema ?? string.Empty, this.tableInfo.Name));
            }
        }
    }
}