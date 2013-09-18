// -----------------------------------------------------------------------
// <copyright file="AttributeMappingConvention.cs" company="MicroLite">
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
    using MicroLite.FrameworkExtensions;
    using MicroLite.Logging;

    /// <summary>
    /// The implementation of <see cref="IMappingConvention"/> which uses attributes to map tables and columns
    /// to types and properties only maps if an attribute is present (opt-in).
    /// </summary>
    internal sealed class AttributeMappingConvention : IMappingConvention
    {
        private static readonly ILog log = LogManager.GetCurrentClassLog();

        public IObjectInfo CreateObjectInfo(Type forType)
        {
            if (forType == null)
            {
                throw new ArgumentNullException("forType");
            }

            var tableAttribute = forType.GetAttribute<TableAttribute>(inherit: false);

            if (tableAttribute == null)
            {
                log.TryLogFatal(Messages.AttributeMappingConvention_NoTableAttribute, forType.FullName);
                throw new MicroLiteException(Messages.AttributeMappingConvention_NoTableAttribute.FormatWith(forType.FullName));
            }

            var identifierStrategy = MicroLite.Mapping.IdentifierStrategy.DbGenerated;
            var columns = CreateColumnInfos(forType, ref identifierStrategy);

            var tableInfo = new TableInfo(columns, identifierStrategy, tableAttribute.Name, tableAttribute.Schema);

            return new ObjectInfo(forType, tableInfo);
        }

        private static List<ColumnInfo> CreateColumnInfos(Type forType, ref IdentifierStrategy identifierStrategy)
        {
            var properties = forType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var columns = new List<ColumnInfo>(properties.Length);

            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite)
                {
                    log.TryLogDebug(Messages.MappingConvention_PropertyNotGetAndSet, forType.Name, property.Name);
                    continue;
                }

                var columnAttribute = property.GetAttribute<ColumnAttribute>(inherit: true);

                if (columnAttribute == null)
                {
                    log.TryLogInfo(Messages.AttributeMappingConvention_IgnoringProperty, forType.FullName, property.Name);
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
                    allowInsert: columnAttribute.AllowInsert,
                    allowUpdate: columnAttribute.AllowUpdate);

                columns.Add(columnInfo);
            }

            return columns;
        }
    }
}