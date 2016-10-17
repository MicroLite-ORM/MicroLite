// -----------------------------------------------------------------------
// <copyright file="IncludeMany.cs" company="MicroLite">
// Copyright 2012 - 2016 Project Contributors
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
    using System.Data.Common;
    using System.Threading;
    using System.Threading.Tasks;
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
        private Action<IIncludeMany<T>> callback;

        public IList<T> Values { get; } = new List<T>();

        public void OnLoad(Action<IIncludeMany<T>> action)
        {
            this.callback = action;
        }

        internal override async Task BuildValueAsync(DbDataReader reader, CancellationToken cancellationToken)
        {
            if (TypeConverter.IsNotEntityAndConvertible(resultType))
            {
                var typeConverter = TypeConverter.For(resultType) ?? TypeConverter.Default;

                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var value = (T)typeConverter.ConvertFromDbValue(reader, 0, resultType);

                    this.Values.Add(value);
                }
            }
            else
            {
                var objectInfo = ObjectInfo.For(resultType);

                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var instance = (T)objectInfo.CreateInstance(reader);

                    this.Values.Add(instance);
                }
            }

            this.HasValue = this.Values.Count > 0;

            this.callback?.Invoke(this);
        }
    }
}