// -----------------------------------------------------------------------
// <copyright file="IdentifierStrategyListener.cs" company="Project Contributors">
// Copyright Project Contributors
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Globalization;
using MicroLite.Logging;
using MicroLite.Mapping;

namespace MicroLite.Listeners
{
    /// <summary>
    /// The implementation of <see cref="IInsertListener"/> for setting the instance identifier value if
    /// <see cref="IdentifierStrategy"/>.DbGenerated or <see cref="IdentifierStrategy"/>.Sequence is used.
    /// </summary>
    public sealed class IdentifierStrategyListener : IInsertListener
    {
        private static readonly ILog s_log = LogManager.GetCurrentClassLog();

        /// <summary>
        /// Invoked after the SqlQuery to insert the record for the instance has been executed.
        /// </summary>
        /// <param name="instance">The instance which has been inserted.</param>
        /// <param name="executeScalarResult">The execute scalar result (the identifier value returned by the database
        /// or null if the identifier is <see cref="IdentifierStrategy" />.Assigned.</param>
        /// <exception cref="ArgumentNullException">Thrown if instance is null or IdentifierStrategy is DbGenerated
        /// and executeScalarResult is null.</exception>
        public void AfterInsert(object instance, object executeScalarResult)
        {
            if (instance is null)
            {
                throw new ArgumentNullException(nameof(instance));
            }

            if (executeScalarResult is null)
            {
                return;
            }

            IObjectInfo objectInfo = ObjectInfo.For(instance.GetType());

            if (objectInfo.TableInfo.IdentifierStrategy != IdentifierStrategy.Assigned)
            {
                if (s_log.IsDebug)
                {
                    s_log.Debug(LogMessages.IListener_SettingIdentifierValue, objectInfo.ForType.FullName, executeScalarResult.ToString());
                }

                Type propertyType = objectInfo.TableInfo.IdentifierColumn.PropertyInfo.PropertyType;

                if (executeScalarResult.GetType() != propertyType)
                {
                    object converted = Convert.ChangeType(executeScalarResult, propertyType, CultureInfo.InvariantCulture);

                    objectInfo.SetIdentifierValue(instance, converted);
                }
                else
                {
                    objectInfo.SetIdentifierValue(instance, executeScalarResult);
                }
            }
        }

        /// <summary>
        /// Invoked before the SqlQuery to insert the record into the database is created.
        /// </summary>
        /// <param name="instance">The instance to be inserted.</param>
        public void BeforeInsert(object instance)
        {
            return; // no-op
        }
    }
}
