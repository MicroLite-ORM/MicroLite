// -----------------------------------------------------------------------
// <copyright file="ObjectBuilder.cs" company="MicroLite">
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
namespace MicroLite.Core
{
    using System;
    using System.Data;
    using MicroLite.Logging;
    using MicroLite.Mapping;

    /// <summary>
    /// The default implementation of <see cref="IObjectBuilder"/>.
    /// </summary>
    internal sealed class ObjectBuilder : IObjectBuilder
    {
        private static readonly ILog log = LogManager.GetCurrentClassLog();

        public T BuildInstance<T>(IObjectInfo objectInfo, IDataReader reader)
        {
            log.TryLogDebug(Messages.ObjectBuilder_CreatingInstance, objectInfo.ForType.FullName);
            var instance = (T)objectInfo.CreateInstance();

            for (int i = reader.FieldCount - 1; i >= 0; i--)
            {
                var columnName = reader.GetName(i);

                try
                {
                    objectInfo.SetPropertyValueForColumn(instance, columnName, reader[i]);
                }
                catch (Exception e)
                {
                    log.TryLogError(e.Message, e);
                    throw new MicroLiteException(e.Message, e);
                }
            }

            return instance;
        }
    }
}