// -----------------------------------------------------------------------
// <copyright file="ConventionMappingConvention.cs" company="MicroLite">
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
            this.log.TryLogInfo(Messages.ConventionMappingConvention_Configuration, settings.UsePluralClassNameForTableName.ToString(), settings.IdentifierStrategy.ToString());
            this.settings = settings;
        }

        public ObjectInfo CreateObjectInfo(Type forType)
        {
            if (forType == null)
            {
                throw new ArgumentNullException("forType");
            }

            var columns = CreateColumnInfos(forType);

            var tableInfo = new TableInfo(
                columns,
                this.settings.IdentifierStrategy,
                this.settings.UsePluralClassNameForTableName ? InflectionService.ToPlural(forType.Name) : forType.Name,
                null);

            return new ObjectInfo(forType, tableInfo);
        }

        private static List<ColumnInfo> CreateColumnInfos(Type forType)
        {
            var possibleClassIdentifiers = new[] { "Id", forType.Name + "Id" };
            var properties = forType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var columns = new List<ColumnInfo>(properties.Length);

            foreach (var property in properties.Where(p => p.CanRead && p.CanWrite))
            {
                var columnInfo = new ColumnInfo(
                       columnName: property.PropertyType.IsEnum ? property.PropertyType.Name + "Id" : property.Name,
                       propertyInfo: property,
                       isIdentifier: possibleClassIdentifiers.Contains(property.Name),
                       allowInsert: true,
                       allowUpdate: true);

                columns.Add(columnInfo);
            }

            return columns;
        }
    }
}