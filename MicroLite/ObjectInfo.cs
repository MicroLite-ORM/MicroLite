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
namespace MicroLite
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Logging;
    using MicroLite.Mapping;

    /// <summary>
    /// The class which describes a type and the table it is mapped to.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ObjectInfo for {ForType}")]
    internal sealed class ObjectInfo
    {
        private static readonly object locker = new object();
        private static readonly ILog log = LogManager.GetLog("MicroLite.ObjectInfo");
        private static readonly IDictionary<Type, ObjectInfo> objectInfos = new Dictionary<Type, ObjectInfo>();
        private readonly Type forType;

        // The properties (key is the property name, value is the property info for the property).
        private readonly IDictionary<string, PropertyInfo> properties = new Dictionary<string, PropertyInfo>();

        private readonly TableInfo tableInfo = new TableInfo();

        private ObjectInfo(Type forType)
        {
            this.forType = forType;

            this.ReadTableAttributes();
            this.CaptureProperties();

            if (string.IsNullOrEmpty(this.TableInfo.IdentifierColumn))
            {
                var message = LogMessages.NoIdentifierFoundForType.FormatWith(forType.Name);
                log.TryLogFatal(message);
                throw new MicroLiteException(message);
            }
        }

        internal object DefaultIdentiferValue
        {
            get;
            private set;
        }

        internal Type ForType
        {
            get
            {
                return this.forType;
            }
        }

        internal TableInfo TableInfo
        {
            get
            {
                return this.tableInfo;
            }
        }

        internal static ObjectInfo For(Type forType)
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
                    objectInfos.Add(forType, new ObjectInfo(forType));
                }

                log.TryLogDebug(LogMessages.ObjectInfo_RetrievingObjectInfo, forType.FullName);
                return objectInfos[forType];
            }
        }

        internal PropertyInfo GetPropertyInfoForColumn(string columnName)
        {
            if (this.properties.ContainsKey(columnName))
            {
                return this.properties[columnName];
            }

            return null;
        }

        internal bool HasDefaultIdentifierValue(object instance)
        {
            var identifierPropertyInfo = this.GetPropertyInfoForColumn(this.TableInfo.IdentifierColumn);

            var identifierValue = identifierPropertyInfo.GetValue(instance);

            return object.Equals(identifierValue, this.DefaultIdentiferValue);
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

        private void CaptureProperties()
        {
            foreach (var property in this.ForType.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (property.GetAttribute<IgnoreAttribute>(inherit: true) != null)
                {
                    log.TryLogDebug(LogMessages.ObjectInfo_IgnoringProperty, this.ForType.FullName, property.Name);
                    continue;
                }

                if (property.CanRead && property.CanWrite)
                {
                    var columnAttribute = property.GetAttribute<ColumnAttribute>(inherit: true);

                    var columnName = columnAttribute != null ? columnAttribute.Name : property.Name;

                    log.TryLogDebug(LogMessages.ObjectInfo_MappingColumnToProperty, this.ForType.FullName, property.Name, columnName);
                    this.properties.Add(columnName, property);

                    this.TableInfo.Columns.Add(columnName);

                    if (this.TableInfo.IdentifierColumn == null)
                    {
                        var identifierAttribute = property.GetAttribute<IdentifierAttribute>(inherit: true);

                        if (identifierAttribute != null)
                        {
                            log.TryLogDebug(LogMessages.ObjectInfo_UsingPropertyAsIdentifier, this.ForType.FullName, property.Name, identifierAttribute.IdentifierStrategy.ToString());
                            this.TableInfo.IdentifierColumn = columnName;
                            this.TableInfo.IdentifierStrategy = identifierAttribute.IdentifierStrategy;

                            if (property.PropertyType.IsValueType)
                            {
                                this.DefaultIdentiferValue = (ValueType)Activator.CreateInstance(property.PropertyType);
                            }
                        }
                        else if (property.Name.Equals("Id") || property.Name.Equals(this.ForType.Name + "Id"))
                        {
                            log.TryLogDebug(LogMessages.ObjectInfo_UsingPropertyAsIdentifier, this.ForType.FullName, property.Name, "Identity");
                            this.TableInfo.IdentifierColumn = columnName;
                            this.TableInfo.IdentifierStrategy = IdentifierStrategy.Identity;

                            if (property.PropertyType.IsValueType)
                            {
                                this.DefaultIdentiferValue = (ValueType)Activator.CreateInstance(property.PropertyType);
                            }
                        }
                    }
                }
            }
        }

        private void ReadTableAttributes()
        {
            var tableAttribute = this.ForType.GetAttribute<TableAttribute>(inherit: false);

            if (tableAttribute != null)
            {
                log.TryLogDebug(LogMessages.ObjectInfo_MappingClassToTable, this.ForType.FullName, tableAttribute.Schema + "." + tableAttribute.Name);
                this.TableInfo.Name = tableAttribute.Name;
                this.TableInfo.Schema = tableAttribute.Schema;
            }
            else
            {
                log.TryLogDebug(LogMessages.ObjectInfo_MappingClassToTable, this.ForType.FullName, this.ForType.Name);
                this.TableInfo.Name = this.ForType.Name;
            }
        }
    }
}