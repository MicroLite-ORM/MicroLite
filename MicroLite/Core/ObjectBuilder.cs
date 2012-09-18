// -----------------------------------------------------------------------
// <copyright file="ObjectBuilder.cs" company="MicroLite">
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
namespace MicroLite.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;

#if !NET_3_5

    using System.Dynamic;

#endif

    using MicroLite.FrameworkExtensions;
    using MicroLite.Logging;
    using MicroLite.Mapping;

    /// <summary>
    /// The default implementation of <see cref="IObjectBuilder"/>.
    /// </summary>
    internal sealed class ObjectBuilder : IObjectBuilder
    {
        private static readonly ILog log = LogManager.GetLog("MicroLite.ObjectBuilder");

#if !NET_3_5

        public dynamic BuildDynamic(IDataReader reader)
        {
            log.TryLogDebug(Messages.ObjectBuilder_CreatingInstance, "dynamic");
            dynamic expando = new ExpandoObject();

            var dictionary = (IDictionary<string, object>)expando;

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);

                log.TryLogDebug(Messages.ObjectBuilder_SettingPropertyValue, "dynamic", columnName);
                dictionary.Add(columnName, reader[i] == DBNull.Value ? null : reader[i]);
            }

            return expando;
        }

#endif

        public T BuildInstance<T>(ObjectInfo objectInfo, IDataReader reader)
            where T : class, new()
        {
            log.TryLogDebug(Messages.ObjectBuilder_CreatingInstance, objectInfo.ForType.FullName);
            var instance = new T();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);
                var propertyInfo = objectInfo.GetPropertyInfoForColumn(columnName);

                if (propertyInfo == null)
                {
                    log.TryLogWarn(Messages.ObjectBuilder_UnknownProperty, objectInfo.ForType.Name, columnName);
                    continue;
                }

                try
                {
                    log.TryLogDebug(Messages.ObjectBuilder_SettingPropertyValue, objectInfo.ForType.Name, columnName);
                    propertyInfo.SetValue(instance, reader[i]);
                }
                catch (Exception e)
                {
                    log.TryLogFatal(e.Message, e);
                    throw new MicroLiteException(e.Message, e);
                }
            }

            return instance;
        }
    }
}