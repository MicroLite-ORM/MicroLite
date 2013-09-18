// -----------------------------------------------------------------------
// <copyright file="IncludeSingle.cs" company="MicroLite">
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
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;
    using MicroLite.TypeConverters;

    /// <summary>
    /// The default implementation of <see cref="IInclude&lt;T&gt;"/> for mapped objects.
    /// </summary>
    /// <typeparam name="T">The type of object to be included.</typeparam>
    internal sealed class IncludeSingle<T> : Include, IInclude<T>
    {
        private static readonly Type resultType = typeof(T);
        private T value;

        public T Value
        {
            get
            {
                return this.value;
            }
        }

        internal override void BuildValue(IDataReader reader, IObjectBuilder objectBuilder)
        {
            if (reader.Read())
            {
                if (resultType.IsNotEntityAndConvertible())
                {
                    var typeConverter = TypeConverter.For(resultType);

                    this.value = (T)typeConverter.ConvertFromDbValue(reader[0], resultType);
                    this.HasValue = true;
                }
                else
                {
                    var objectInfo = ObjectInfo.For(resultType);

                    this.value = objectBuilder.BuildInstance<T>(objectInfo, reader);
                    this.HasValue = true;
                }

                if (reader.Read())
                {
                    throw new MicroLiteException(Messages.IncludeSingle_SingleResultExpected);
                }
            }
        }
    }
}