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
    using System.Data;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Logging;
    using MicroLite.Mapping;

    /// <summary>
    /// The default implementation of <see cref="IObjectBuilder"/>.
    /// </summary>
    internal sealed class ObjectBuilder : IObjectBuilder
    {
        private static readonly ILog log = LogManager.GetLog("MicroLite.ObjectBuilder");

        public T BuildNewInstance<T>(IDataReader reader)
            where T : class, new()
        {
            var objectInfo = ObjectInfo.For(typeof(T));

            log.TryLogDebug(Messages.ObjectBuilder_CreatingInstance, objectInfo.ForType.FullName);
            var instance = new T();

            for (int i = 0; i < reader.FieldCount; i++)
            {
                var columnName = reader.GetName(i);

                var propertyInfo = objectInfo.GetPropertyInfoForColumn(columnName);

                if (propertyInfo != null)
                {
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
                else
                {
                    log.TryLogWarn(Messages.ObjectBuilder_UnknownProperty, objectInfo.ForType.Name, columnName);
                }
            }

            return instance;
        }
    }
}