// -----------------------------------------------------------------------
// <copyright file="AttributeMappingConvention.cs" company="MicroLite">
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
    /// The implementation of <see cref="IMappingConvention"/> which uses attributes to map tables and columns
    /// to types and properties only maps if an attribute is present (opt-in).
    /// </summary>
    internal sealed class AttributeMappingConvention : IMappingConvention
    {
        private static readonly ILog log = LogManager.GetLog("MicroLite.StrictAttributeMappingConvention");

        public ObjectInfo CreateObjectInfo(Type forType)
        {
            if (forType == null)
            {
                throw new ArgumentNullException("forType");
            }

            var tableAttribute = forType.GetAttribute<TableAttribute>(inherit: false);

            if (tableAttribute == null)
            {
                var message = Messages.StrictAttributeMappingConvention_NoTableAttribute.FormatWith(forType.FullName);
                log.TryLogFatal(message);
                throw new MicroLiteException(message);
            }

            var identifierStrategy = MicroLite.Mapping.IdentifierStrategy.Identity;
            var columns = new List<ColumnInfo>();

            var properties = forType.GetProperties(BindingFlags.Instance | BindingFlags.Public);

            foreach (var property in properties.Where(p => p.CanRead && p.CanWrite))
            {
                var columnAttribute = property.GetAttribute<ColumnAttribute>(inherit: true);

                if (columnAttribute == null)
                {
                    log.TryLogDebug(LogMessages.ObjectInfo_IgnoringProperty, forType.FullName, property.Name);
                    continue;
                }

                var identifierAttribute = property.GetAttribute<IdentifierAttribute>(inherit: true);

                if (identifierAttribute != null)
                {
                    identifierStrategy = identifierAttribute.IdentifierStrategy;
                }

                var columnInfo = new ColumnInfo(
                    columnName: columnAttribute.Name,
                    isIdentifier: identifierAttribute == null ? false : true,
                    propertyInfo: property);

                columns.Add(columnInfo);
            }

            var tableInfo = new TableInfo(columns, identifierStrategy, tableAttribute.Name, tableAttribute.Schema);

            return new ObjectInfo(forType, tableInfo);
        }
    }
}