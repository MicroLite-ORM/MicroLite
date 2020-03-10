// -----------------------------------------------------------------------
// <copyright file="AttributeMappingConvention.cs" company="Project Contributors">
// Copyright Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using MicroLite.FrameworkExtensions;
using MicroLite.Logging;
using MicroLite.TypeConverters;

namespace MicroLite.Mapping.Attributes
{
    /// <summary>
    /// The implementation of <see cref="IMappingConvention"/> which uses attributes to map tables and columns
    /// to types and properties only maps if an attribute is present (opt-in).
    /// </summary>
    internal sealed class AttributeMappingConvention : IMappingConvention
    {
        private readonly ILog _log = LogManager.GetCurrentClassLog();

        public IObjectInfo CreateObjectInfo(Type forType)
        {
            if (forType is null)
            {
                throw new ArgumentNullException(nameof(forType));
            }

            TableAttribute tableAttribute = forType.GetAttribute<TableAttribute>(inherit: false);

            if (tableAttribute is null)
            {
                throw new MappingException(ExceptionMessages.AttributeMappingConvention_NoTableAttribute.FormatWith(forType.FullName));
            }

            IdentifierStrategy identifierStrategy = IdentifierStrategy.DbGenerated;
            List<ColumnInfo> columns = CreateColumnInfos(forType, ref identifierStrategy);

            var tableInfo = new TableInfo(columns, identifierStrategy, tableAttribute.Name, tableAttribute.Schema);

            if (_log.IsDebug)
            {
                _log.Debug(LogMessages.MappingConvention_MappingTypeToTable, forType.FullName, tableInfo.Schema, tableInfo.Name);
            }

            return new PocoObjectInfo(forType, tableInfo);
        }

        private List<ColumnInfo> CreateColumnInfos(Type forType, ref IdentifierStrategy identifierStrategy)
        {
            PropertyInfo[] properties = forType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var columns = new List<ColumnInfo>(properties.Length);

            foreach (PropertyInfo property in properties.OrderBy(p => p.Name))
            {
                if (!property.CanRead || !property.CanWrite)
                {
                    if (_log.IsDebug)
                    {
                        _log.Debug(LogMessages.MappingConvention_PropertyNotGetAndSet, forType.Name, property.Name);
                    }

                    continue;
                }

                ColumnAttribute columnAttribute = property.GetAttribute<ColumnAttribute>(inherit: true);

                if (columnAttribute is null)
                {
                    if (_log.IsDebug)
                    {
                        _log.Debug(LogMessages.AttributeMappingConvention_IgnoringProperty, forType.FullName, property.Name);
                    }

                    continue;
                }

                IdentifierAttribute identifierAttribute = property.GetAttribute<IdentifierAttribute>(inherit: true);

                if (identifierAttribute != null)
                {
                    identifierStrategy = identifierAttribute.IdentifierStrategy;
                }

                var columnInfo = new ColumnInfo(
                    columnName: columnAttribute.Name,
                    dbType: TypeConverter.ResolveDbType(property.PropertyType),
                    propertyInfo: property,
                    isIdentifier: identifierAttribute != null,
                    allowInsert: identifierAttribute != null ? identifierStrategy == IdentifierStrategy.Assigned : columnAttribute.AllowInsert,
                    allowUpdate: identifierAttribute == null && columnAttribute.AllowUpdate,
                    sequenceName: identifierAttribute?.SequenceName);

                if (_log.IsDebug)
                {
                    _log.Debug(LogMessages.MappingConvention_MappingColumnToProperty, forType.Name, columnInfo.PropertyInfo.Name, columnInfo.ColumnName);
                }

                columns.Add(columnInfo);
            }

            return columns;
        }
    }
}
