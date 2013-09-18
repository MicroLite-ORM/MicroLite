// -----------------------------------------------------------------------
// <copyright file="IncludeMany.cs" company="MicroLite">
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
    using System.Collections.Generic;
    using System.Data;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;
    using MicroLite.TypeConverters;

    /// <summary>
    /// The default implementation of <see cref="IIncludeMany&lt;T&gt;"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be included.</typeparam>
    internal sealed class IncludeMany<T> : Include, IIncludeMany<T>
    {
        private static readonly Type resultType = typeof(T);
        private readonly IList<T> values = new List<T>();

        public IList<T> Values
        {
            get
            {
                return this.values;
            }
        }

        internal override void BuildValue(IDataReader reader, IObjectBuilder objectBuilder)
        {
            if (resultType.IsNotEntityAndConvertible())
            {
                var typeConverter = TypeConverter.For(resultType);

                while (reader.Read())
                {
                    var value = (T)typeConverter.ConvertFromDbValue(reader[0], resultType);

                    this.values.Add(value);
                }
            }
            else
            {
                var objectInfo = ObjectInfo.For(resultType);

                while (reader.Read())
                {
                    var value = objectBuilder.BuildInstance<T>(objectInfo, reader);

                    this.values.Add(value);
                }
            }

            this.HasValue = this.values.Count > 0;
        }
    }
}