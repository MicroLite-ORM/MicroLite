// -----------------------------------------------------------------------
// <copyright file="GuidListener.cs" company="MicroLite">
// Copyright 2012 - 2013 Project Contributors
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
        private static readonly ILog log = LogManager.GetCurrentClassLog();

        /// <summary>
        /// Invoked before the SqlQuery to delete the record from the database is created.
        /// </summary>
        /// <param name="instance">The instance to be deleted.</param>
        /// <exception cref="MicroLiteException">Thrown if the identifier value for the object has not been set.</exception>
        public override void BeforeDelete(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.Guid)
            {
                if (objectInfo.HasDefaultIdentifierValue(instance))
                {
                    throw new MicroLiteException(Messages.IListener_IdentifierNotSetForDelete);
                }
            }
        }

        /// <summary>
        /// Invoked before the SqlQuery to insert the record into the database is created.
        /// </summary>
        /// <param name="instance">The instance to be inserted.</param>
        public override void BeforeInsert(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.Guid)
            {
                if (!objectInfo.HasDefaultIdentifierValue(instance))
                {
                    throw new MicroLiteException(Messages.IListener_IdentifierSetForInsert);
                }

                var identifierValue = Guid.NewGuid();

                log.TryLogDebug(Messages.IListener_SettingIdentifierValue, objectInfo.ForType.FullName, identifierValue.ToString());
                objectInfo.SetPropertyValue(instance, objectInfo.TableInfo.IdentifierProperty, identifierValue);
            }
        }

        /// <summary>
        /// Invoked before the SqlQuery to update the record in the database is created.
        /// </summary>
        /// <param name="instance">The instance to be updated.</param>
        /// <exception cref="MicroLiteException">Thrown if the identifier value for the object has not been set.</exception>
        public override void BeforeUpdate(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

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