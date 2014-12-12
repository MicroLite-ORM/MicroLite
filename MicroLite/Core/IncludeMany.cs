// -----------------------------------------------------------------------
// <copyright file="IncludeMany.cs" company="MicroLite">
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
namespace MicroLite.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using MicroLite.Mapping;
    using MicroLite.TypeConverters;

    /// <summary>
    /// The default implementation of <see cref="IIncludeMany&lt;T&gt;"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be included.</typeparam>
    [System.Diagnostics.DebuggerDisplay("HasValue: {HasValue}, Values: {Values}")]
    internal sealed class IncludeMany<T> : Include, IIncludeMany<T>
    {
        private static readonly Type resultType = typeof(T);
        private readonly IList<T> values = new List<T>();
        private Action<IIncludeMany<T>> callback;

        public IList<T> Values
        {
            get
            {
                return this.values;
            }
        }

        public void OnLoad(Action<IIncludeMany<T>> action)
        {
            this.callback = action;
        }

        internal override void BuildValue(IDataReader reader)
        {
            if (TypeConverter.IsNotEntityAndConvertible(resultType))
            {
                var typeConverter = TypeConverter.For(resultType) ?? TypeConverter.Default;

                while (reader.Read())
                {
                    var value = (T)typeConverter.ConvertFromDbValue(reader, 0, resultType);

                    this.values.Add(value);
                }
            }
            else
            {
                var objectInfo = ObjectInfo.For(resultType);

                while (reader.Read())
                {
                    var instance = (T)objectInfo.CreateInstance(reader);

                    this.values.Add(instance);
                }
            }

            this.HasValue = this.values.Count > 0;

            if (this.callback != null)
            {
                this.callback(this);
            }
        }

#if NET_4_5

        internal override async System.Threading.Tasks.Task BuildValueAsync(DbDataReader reader, CancellationToken cancellationToken)
        {
            if (TypeConverter.IsNotEntityAndConvertible(resultType))
            {
                var typeConverter = TypeConverter.For(resultType) ?? TypeConverter.Default;

                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var value = (T)typeConverter.ConvertFromDbValue(reader, 0, resultType);

                    this.values.Add(value);
                }
            }
            else
            {
                var objectInfo = ObjectInfo.For(resultType);

                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var instance = (T)objectInfo.CreateInstance(reader);

                    this.values.Add(instance);
                }
            }

            this.HasValue = this.values.Count > 0;

            if (this.callback != null)
            {
                this.callback(this);
            }
        }

#endif
    }
}