// -----------------------------------------------------------------------
// <copyright file="LooseAttributeMappingConvention.cs" company="MicroLite">
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
    using MicroLite.FrameworkExtensions;
    using MicroLite.Logging;

    /// <summary>
    /// The implementation of <see cref="IMappingConvention"/> which uses attributes as hints to map tables and
    /// columns to types and properties.
    /// </summary>
    internal sealed class LooseAttributeMappingConvention : IMappingConvention
    {
        private static readonly ILog log = LogManager.GetLog("MicroLite.LooseAttributeMappingConvention");

        public ObjectInfo CreateObjectInfo(Type forType)
        {
            var identifierStrategy = IdentifierStrategy.Identity;
            var columns = new List<ColumnInfo>();

            var properties = forType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties.Where(p => p.CanRead && p.CanWrite))
            {
                if (property.GetAttribute<IgnoreAttribute>(inherit: true) != null)
                {
                    log.TryLogDebug(LogMessages.ObjectInfo_IgnoringProperty, forType.FullName, property.Name);
                    continue;
                }

                var columnAttribute = property.GetAttribute<ColumnAttribute>(inherit: true);
                var identifierAttribute = property.GetAttribute<IdentifierAttribute>(inherit: true);

                if (identifierAttribute != null)
                {
                    identifierStrategy = identifierAttribute.IdentifierStrategy;
                }

                var columnInfo = new ColumnInfo(
                    columnName: columnAttribute != null ? columnAttribute.Name : property.Name,
                    isIdentifier: identifierAttribute == null ? (property.Name.Equals("Id") || property.Name.Equals(forType.Name + "Id")) : true,
                    propertyInfo: property);

                columns.Add(columnInfo);
            }

            var tableAttribute = forType.GetAttribute<TableAttribute>(inherit: false);

            if (tableAttribute != null)
            {
                log.TryLogDebug(LogMessages.ObjectInfo_MappingClassToTable, forType.FullName, tableAttribute.Schema + "." + tableAttribute.Name);
            }
            else
            {
                log.TryLogDebug(LogMessages.ObjectInfo_MappingClassToTable, forType.FullName, forType.Name);
            }

            var tableInfo = new TableInfo(
                columns,
                identifierStrategy,
                tableAttribute != null ? tableAttribute.Name : forType.Name,
                tableAttribute != null ? tableAttribute.Schema : string.Empty);

            return new ObjectInfo(forType, tableInfo);
        }
    }
}