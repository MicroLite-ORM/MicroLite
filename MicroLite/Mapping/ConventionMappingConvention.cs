// -----------------------------------------------------------------------
// <copyright file="ConventionMappingConvention.cs" company="Project Contributors">
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
using MicroLite.Logging;

namespace MicroLite.Mapping
{
    /// <summary>
    /// The implementation of <see cref="IMappingConvention"/> which uses a convention to map tables and columns
    /// to types and properties.
    /// </summary>
    internal sealed class ConventionMappingConvention : IMappingConvention
    {
        private readonly ILog _log = LogManager.GetCurrentClassLog();
        private readonly ConventionMappingSettings _settings;

        internal ConventionMappingConvention(ConventionMappingSettings settings)
            => _settings = settings;

        public IObjectInfo CreateObjectInfo(Type forType)
        {
            if (forType is null)
            {
                throw new ArgumentNullException(nameof(forType));
            }

            IdentifierStrategy identifierStrategy = _settings.ResolveIdentifierStrategy(forType);
            List<ColumnInfo> columns = CreateColumnInfos(forType, identifierStrategy);

            var tableInfo = new TableInfo(
                columns,
                identifierStrategy,
                _settings.ResolveTableName(forType),
                _settings.ResolveTableSchema(forType));

            if (_log.IsDebug)
            {
                _log.Debug(LogMessages.MappingConvention_MappingTypeToTable, forType.FullName, tableInfo.Schema, tableInfo.Name);
            }

            return new PocoObjectInfo(forType, tableInfo);
        }

        private List<ColumnInfo> CreateColumnInfos(Type forType, IdentifierStrategy identifierStrategy)
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

                if (_settings.Ignore(property))
                {
                    if (_log.IsDebug)
                    {
                        _log.Debug(LogMessages.ConventionMappingConvention_IgnoringProperty, forType.Name, property.Name);
                    }

                    continue;
                }

                bool isIdentifier = _settings.IsIdentifier(property);

                var columnInfo = new ColumnInfo(
                    columnName: isIdentifier ? _settings.ResolveIdentifierColumnName(property) : _settings.ResolveColumnName(property),
                    dbType: _settings.ResolveDbType(property),
                    propertyInfo: property,
                    isIdentifier: isIdentifier,
                    allowInsert: isIdentifier ? identifierStrategy == IdentifierStrategy.Assigned : _settings.AllowInsert(property),
                    allowUpdate: isIdentifier ? false : _settings.AllowUpdate(property),
                    sequenceName: isIdentifier && identifierStrategy == IdentifierStrategy.Sequence ? _settings.ResolveSequenceName(property) : null);

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
