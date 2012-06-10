// -----------------------------------------------------------------------
// <copyright file="DbGeneratedListener.cs" company="MicroLite">
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
namespace MicroLite.Listeners
{
    using System;
    using System.Globalization;
    using MicroLite.Logging;

    /// <summary>
    /// The implementation of <see cref="IListener"/> for setting the instance identifier value if
    /// <see cref="IdentifierStrategy"/>.DbGenerated is used.
    /// </summary>
    internal sealed class DbGeneratedListener : Listener
    {
        private static readonly ILog log = LogManager.GetLog("MicroLite.DbGeneratedListener");

        public override void AfterInsert(object instance, object executeScalarResult)
        {
            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.DbGenerated)
            {
                var propertyInfo = objectInfo.GetPropertyInfoForColumn(objectInfo.TableInfo.IdentifierColumn);

                var identifierValue = Convert.ChangeType(executeScalarResult, propertyInfo.PropertyType, CultureInfo.InvariantCulture);

                log.TryLogDebug(LogMessages.DbGeneratedListener_SettingIdentifierValue, objectInfo.ForType.FullName, identifierValue.ToString());
                propertyInfo.SetValue(instance, identifierValue, null);
            }
        }

        public override void BeforeInsert(object instance)
        {
            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.DbGenerated)
            {
                if (!objectInfo.HasDefaultIdentifierValue(instance))
                {
                    throw new MicroLiteException(Messages.DbGenerated_IdentifierSetForInsert);
                }
            }
        }

        public override void BeforeInsert(Type forType, SqlQuery sqlQuery)
        {
            var objectInfo = ObjectInfo.For(forType);

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.DbGenerated)
            {
                sqlQuery.CommandText += ";SELECT SCOPE_IDENTITY()";
            }
        }

        public override void BeforeUpdate(object instance)
        {
            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.DbGenerated)
            {
                if (objectInfo.HasDefaultIdentifierValue(instance))
                {
                    throw new MicroLiteException(Messages.DbGenerated_IdentifierNotSetForUpdate);
                }
            }
        }
    }
}