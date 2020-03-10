// -----------------------------------------------------------------------
// <copyright file="ObjectInfo.cs" company="Project Contributors">
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
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using MicroLite.FrameworkExtensions;
using MicroLite.Logging;

namespace MicroLite.Mapping
{
    /// <summary>
    /// The class which describes a type and the table it is mapped to.
    /// </summary>
    public static class ObjectInfo
    {
        private static readonly ILog s_log = LogManager.GetCurrentClassLog();
        private static IMappingConvention s_mappingConvention;
        private static IDictionary<Type, IObjectInfo> s_objectInfos = GetObjectInfos();

        internal static IMappingConvention MappingConvention
        {
            get
            {
                if (s_mappingConvention is null)
                {
                    s_mappingConvention = s_mappingConvention = new ConventionMappingConvention(ConventionMappingSettings.Default);
                }

                return s_mappingConvention;
            }

            set => s_mappingConvention = value;
        }

        /// <summary>
        /// Gets the object info for the specified type.
        /// </summary>
        /// <param name="forType">The type to get the object info for.</param>
        /// <returns>The <see cref="ObjectInfo" /> for the specified <see cref="Type" />.</returns>
        /// <exception cref="ArgumentNullException">Thrown if forType is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the specified type cannot be used with MicroLite.</exception>
        public static IObjectInfo For(Type forType)
        {
            if (forType is null)
            {
                throw new ArgumentNullException(nameof(forType));
            }

            if (s_log.IsDebug)
            {
                s_log.Debug(LogMessages.ObjectInfo_RetrievingObjectInfo, forType.FullName);
            }

            if (!s_objectInfos.TryGetValue(forType, out IObjectInfo objectInfo))
            {
                if (forType.IsGenericType)
                {
                    Type forGenericType = forType.GetGenericTypeDefinition();

                    if (s_objectInfos.TryGetValue(forGenericType, out objectInfo))
                    {
                        return objectInfo;
                    }
                }

                VerifyType(forType);

                if (s_log.IsDebug)
                {
                    s_log.Debug(LogMessages.ObjectInfo_CreatingObjectInfo, forType.FullName);
                }

                objectInfo = MappingConvention.CreateObjectInfo(forType);

                if (s_log.IsDebug)
                {
                    using (var stringWriter = new StringWriter(CultureInfo.InvariantCulture))
                    {
                        objectInfo.EmitMappings(stringWriter);
                        s_log.Debug(stringWriter.ToString());
                    }
                }

                var newObjectInfos = new Dictionary<Type, IObjectInfo>(s_objectInfos)
                {
                    [forType] = objectInfo,
                };

                s_objectInfos = newObjectInfos;
            }

            return objectInfo;
        }

        private static Dictionary<Type, IObjectInfo> GetObjectInfos()
        {
            var dictionary = new Dictionary<Type, IObjectInfo>();

            var expandoObjectInfo = new ExpandoObjectInfo();
            dictionary.Add(typeof(System.Dynamic.ExpandoObject), expandoObjectInfo);
            dictionary.Add(typeof(object), expandoObjectInfo); // If the generic argument <dynamic> is used (in ISession.Fetch for example), typeof(T) will return object.

            var tupleObjectInfo = new TupleObjectInfo();
            dictionary.Add(typeof(Tuple<>), tupleObjectInfo);
            dictionary.Add(typeof(Tuple<,>), tupleObjectInfo);
            dictionary.Add(typeof(Tuple<,,>), tupleObjectInfo);
            dictionary.Add(typeof(Tuple<,,,>), tupleObjectInfo);
            dictionary.Add(typeof(Tuple<,,,,>), tupleObjectInfo);
            dictionary.Add(typeof(Tuple<,,,,,>), tupleObjectInfo);
            dictionary.Add(typeof(Tuple<,,,,,,>), tupleObjectInfo);

            return dictionary;
        }

        private static void VerifyType(Type forType)
        {
            if (!forType.IsClass)
            {
                throw new MappingException(ExceptionMessages.ObjectInfo_TypeMustBeClass.FormatWith(forType.Name));
            }

            if (forType.IsAbstract)
            {
                throw new MappingException(ExceptionMessages.ObjectInfo_TypeMustNotBeAbstract.FormatWith(forType.Name));
            }

            if (!forType.IsPublic && !forType.IsNestedPublic)
            {
                throw new MappingException(ExceptionMessages.ObjectInfo_TypeMustBePublic.FormatWith(forType.Name));
            }

            if (forType.GetConstructor(Type.EmptyTypes) is null)
            {
                throw new MappingException(ExceptionMessages.ObjectInfo_TypeMustHaveDefaultConstructor.FormatWith(forType.Name));
            }
        }
    }
}
