// -----------------------------------------------------------------------
// <copyright file="DbGeneratedListener.cs" company="MicroLite">
// Copyright 2012 - 2014 Project Contributors
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
                if (log.IsDebug)
                {
                    log.Debug(LogMessages.IListener_SettingIdentifierValue, objectInfo.ForType.FullName, executeScalarResult.ToString());
                }

                var propertyType = objectInfo.TableInfo.IdentifierColumn.PropertyInfo.PropertyType;

                if (executeScalarResult.GetType() != propertyType)
                {
                    var converted = Convert.ChangeType(executeScalarResult, propertyType, CultureInfo.InvariantCulture);

                    objectInfo.SetIdentifierValue(instance, converted);
                }
                else
                {
                    objectInfo.SetIdentifierValue(instance, executeScalarResult);
                }
            }
        }
    }
}