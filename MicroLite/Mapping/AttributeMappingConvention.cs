// -----------------------------------------------------------------------
// <copyright file="AttributeMappingConvention.cs" company="MicroLite">
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
    using System.Reflection;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Logging;

    /// <summary>
    /// The implementation of <see cref="IMappingConvention"/> which uses attributes to map tables and columns
    /// to types and properties only maps if an attribute is present (opt-in).
    /// </summary>
    internal sealed class AttributeMappingConvention : IMappingConvention
    {
        private readonly ILog log = LogManager.GetCurrentClassLog();

        public IObjectInfo CreateObjectInfo(Type forType)
        {
            if (forType == null)
            {
                throw new ArgumentNullException("forType");
            }

            var tableAttribute = forType.GetAttribute<TableAttribute>(inherit: false);

            if (tableAttribute == null)
            {
                throw new MappingException(ExceptionMessages.AttributeMappingConvention_NoTableAttribute.FormatWith(forType.FullName));
            }

            var identifierStrategy = IdentifierStrategy.DbGenerated;
            var columns = this.CreateColumnInfos(forType, ref identifierStrategy);

            var tableInfo = new TableInfo(columns, identifierStrategy, tableAttribute.Name, tableAttribute.Schema);

            if (this.log.IsDebug)
            {
                this.log.Debug(LogMessages.MappingConvention_MappingTypeToTable, forType.FullName, tableInfo.Schema, tableInfo.Name);
            }

            return new ObjectInfo(forType, tableInfo);
        }

        private List<ColumnInfo> CreateColumnInfos(Type forType, ref IdentifierStrategy identifierStrategy)
        {
            var properties = forType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var columns = new List<ColumnInfo>(properties.Length);

            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite)
                {
                    if (this.log.IsDebug)
                    {
                        this.log.Debug(LogMessages.MappingConvention_PropertyNotGetAndSet, forType.Name, property.Name);
                    }

                    continue;
                }

                var columnAttribute = property.GetAttribute<ColumnAttribute>(inherit: true);

                if (columnAttribute == null)
                {
                    if (this.log.IsDebug)
                    {
                        this.log.Debug(LogMessages.AttributeMappingConvention_IgnoringProperty, forType.FullName, property.Name);
                    }

                    continue;
                }

                var identifierAttribute = property.GetAttribute<IdentifierAttribute>(inherit: true);

                if (identifierAttribute != null)
                {
                    identifierStrategy = identifierAttribute.IdentifierStrategy;
                }

                var columnInfo = new ColumnInfo(
                    columnName: columnAttribute.Name,
                    propertyInfo: property,
                    isIdentifier: identifierAttribute != null,
                    allowInsert: identifierAttribute != null ? identifierStrategy == IdentifierStrategy.Assigned : columnAttribute.AllowInsert,
                    allowUpdate: identifierAttribute != null ? false : columnAttribute.AllowUpdate);

                if (this.log.IsDebug)
                {
                    this.log.Debug(LogMessages.MappingConvention_MappingColumnToProperty, forType.Name, columnInfo.PropertyInfo.Name, columnInfo.ColumnName);
                }

                columns.Add(columnInfo);
            }

            return columns;
        }
    }
}