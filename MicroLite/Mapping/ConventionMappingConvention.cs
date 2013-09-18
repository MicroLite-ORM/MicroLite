// -----------------------------------------------------------------------
// <copyright file="ConventionMappingConvention.cs" company="MicroLite">
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
            this.log.TryLogInfo(
                Messages.ConventionMappingConvention_Configuration,
                settings.UsePluralClassNameForTableName.ToString(),
                settings.TableSchema ?? "null",
                settings.IdentifierStrategy.ToString());

            this.settings = settings;
        }

        public IObjectInfo CreateObjectInfo(Type forType)
        {
            if (forType == null)
            {
                throw new ArgumentNullException("forType");
            }

            var columns = this.CreateColumnInfos(forType);

            var tableInfo = new TableInfo(
                columns,
                this.settings.IdentifierStrategy,
                this.settings.UsePluralClassNameForTableName ? this.settings.InflectionService.ToPlural(forType.Name) : forType.Name,
                this.settings.TableSchema);

            return new ObjectInfo(forType, tableInfo);
        }

        private List<ColumnInfo> CreateColumnInfos(Type forType)
        {
            var properties = forType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var columns = new List<ColumnInfo>(properties.Length);

            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite)
                {
                    this.log.TryLogDebug(Messages.MappingConvention_PropertyNotGetAndSet, forType.Name, property.Name);
                    continue;
                }

                if (this.settings.Ignore(property))
                {
                    this.log.TryLogDebug(Messages.ConventionMappingConvention_IgnoringProperty, forType.Name, property.Name);
                    continue;
                }

                var isIdentifier = this.settings.IsIdentifier(property);

                var columnInfo = new ColumnInfo(
                       columnName: isIdentifier ? this.settings.ResolveIdentifierColumnName(property) : this.settings.ResolveColumnName(property),
                       propertyInfo: property,
                       isIdentifier: isIdentifier,
                       allowInsert: isIdentifier ? true : this.settings.AllowInsert(property),
                       allowUpdate: isIdentifier ? false : this.settings.AllowUpdate(property));

                columns.Add(columnInfo);
            }

            return columns;
        }
    }
}