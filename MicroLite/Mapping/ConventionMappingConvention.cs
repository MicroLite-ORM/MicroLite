// -----------------------------------------------------------------------
// <copyright file="ConventionMappingConvention.cs" company="MicroLite">
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using MicroLite.Logging;

    /// <summary>
    /// The implementation of <see cref="IMappingConvention"/> which uses a convention to map tables and columns
    /// to types and properties.
    /// </summary>
    internal sealed class ConventionMappingConvention : IMappingConvention
    {
        private readonly ILog log = LogManager.GetCurrentClassLog();
        private readonly ConventionMappingSettings settings;

        internal ConventionMappingConvention(ConventionMappingSettings settings)
        {
            this.settings = settings;
        }

        public IObjectInfo CreateObjectInfo(Type forType)
        {
            if (forType == null)
            {
                throw new ArgumentNullException("forType");
            }

            var identifierStrategy = this.settings.ResolveIdentifierStrategy(forType);
            var columns = this.CreateColumnInfos(forType, identifierStrategy);

            var tableInfo = new TableInfo(
                columns,
                identifierStrategy,
                this.settings.ResolveTableName(forType),
                this.settings.ResolveTableSchema(forType));

            if (this.log.IsDebug)
            {
                this.log.Debug(LogMessages.MappingConvention_MappingTypeToTable, forType.FullName, tableInfo.Schema, tableInfo.Name);
            }

            return new PocoObjectInfo(forType, tableInfo);
        }

        private List<ColumnInfo> CreateColumnInfos(Type forType, IdentifierStrategy identifierStrategy)
        {
            var properties = forType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var columns = new List<ColumnInfo>(properties.Length);

            foreach (var property in properties.OrderBy(p => p.Name))
            {
                if (!property.CanRead || !property.CanWrite)
                {
                    if (this.log.IsDebug)
                    {
                        this.log.Debug(LogMessages.MappingConvention_PropertyNotGetAndSet, forType.Name, property.Name);
                    }

                    continue;
                }

                if (this.settings.Ignore(property))
                {
                    if (this.log.IsDebug)
                    {
                        this.log.Debug(LogMessages.ConventionMappingConvention_IgnoringProperty, forType.Name, property.Name);
                    }

                    continue;
                }

                var isIdentifier = this.settings.IsIdentifier(property);
                var isVersion = this.settings.IsVersion(property);

                var columnInfo = new ColumnInfo(
                    columnName: isIdentifier ? this.settings.ResolveIdentifierColumnName(property) : this.settings.ResolveColumnName(property),
                    dbType: this.settings.ResolveDbType(property),
                    propertyInfo: property,
                    isIdentifier: isIdentifier,
                    allowInsert: isIdentifier ? identifierStrategy == IdentifierStrategy.Assigned : isVersion || this.settings.AllowInsert(property),
                    allowUpdate: isIdentifier ? false : isVersion || this.settings.AllowUpdate(property),
                    sequenceName: isIdentifier && identifierStrategy == IdentifierStrategy.Sequence ? this.settings.ResolveSequenceName(property) : null,
                    isVersion: isVersion);

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