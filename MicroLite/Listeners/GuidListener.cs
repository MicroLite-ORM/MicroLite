﻿// -----------------------------------------------------------------------
// <copyright file="GuidListener.cs" company="MicroLite">
// Copyright 2012 Trevor Pilley
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Listeners
{
    using System;
    using MicroLite.Logging;
    using MicroLite.Mapping;

    /// <summary>
    /// The implementation of <see cref="IListener"/> for setting the instance identifier value if
    /// <see cref="IdentifierStrategy"/>.Guid is used.
    /// </summary>
    public sealed class GuidListener : Listener
    {
        private static readonly ILog log = LogManager.GetLog("MicroLite.GuidListener");

        /// <summary>
        /// Invoked before the SqlQuery to insert the record into the database is created.
        /// </summary>
        /// <param name="instance">The instance to be inserted.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Although the class and method are public, the method will only ever be called by Session which will have already validated the parameters.")]
        public override void BeforeInsert(object instance)
        {
            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.Guid)
            {
                var propertyInfo = objectInfo.GetPropertyInfoForColumn(objectInfo.TableInfo.IdentifierColumn);

                var identifierValue = Guid.NewGuid();

                log.TryLogDebug(Messages.IListener_SettingIdentifierValue, objectInfo.ForType.FullName, identifierValue.ToString());
                propertyInfo.SetValue(instance, identifierValue, null);
            }
        }

        /// <summary>
        /// Invoked before the SqlQuery to update the record in the database is created.
        /// </summary>
        /// <param name="instance">The instance to be updated.</param>
        /// <exception cref="MicroLiteException">Thrown if the identifier value for the object has not been set.</exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Although the class and method are public, the method will only ever be called by Session which will have already validated the parameters.")]
        public override void BeforeUpdate(object instance)
        {
            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.Guid)
            {
                if (objectInfo.HasDefaultIdentifierValue(instance))
                {
                    throw new MicroLiteException(Messages.IListener_IdentifierNotSetForUpdate);
                }
            }
        }
    }
}