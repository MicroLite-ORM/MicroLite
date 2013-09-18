// -----------------------------------------------------------------------
// <copyright file="DbGeneratedListener.cs" company="MicroLite">
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
namespace MicroLite.Listeners
{
    using System;
    using MicroLite.Logging;
    using MicroLite.Mapping;

    /// <summary>
    /// The implementation of <see cref="IListener"/> for setting the instance identifier value if
    /// <see cref="IdentifierStrategy"/>.DbGenerated is used.
    /// </summary>
    public sealed class DbGeneratedListener : Listener
    {
        private static readonly ILog log = LogManager.GetCurrentClassLog();

        /// <summary>
        /// Invoked after the SqlQuery to insert the record for the instance has been executed.
        /// </summary>
        /// <param name="instance">The instance which has been inserted.</param>
        /// <param name="executeScalarResult">The execute scalar result.</param>
        public override void AfterInsert(object instance, object executeScalarResult)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            if (executeScalarResult == null)
            {
                throw new ArgumentNullException("executeScalarResult");
            }

            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.DbGenerated)
            {
                log.TryLogDebug(Messages.IListener_SettingIdentifierValue, objectInfo.ForType.FullName, executeScalarResult.ToString());
                objectInfo.SetPropertyValueForColumn(instance, objectInfo.TableInfo.IdentifierColumn, executeScalarResult);
            }
        }

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

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.DbGenerated)
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
        /// <exception cref="MicroLiteException">Thrown if the identifier value for the object has already been set.</exception>
        public override void BeforeInsert(object instance)
        {
            if (instance == null)
            {
                throw new ArgumentNullException("instance");
            }

            var objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.DbGenerated)
            {
                if (!objectInfo.HasDefaultIdentifierValue(instance))
                {
                    throw new MicroLiteException(Messages.IListener_IdentifierSetForInsert);
                }
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

            if (objectInfo.TableInfo.IdentifierStrategy == IdentifierStrategy.DbGenerated)
            {
                if (objectInfo.HasDefaultIdentifierValue(instance))
                {
                    throw new MicroLiteException(Messages.IListener_IdentifierNotSetForUpdate);
                }
            }
        }
    }
}