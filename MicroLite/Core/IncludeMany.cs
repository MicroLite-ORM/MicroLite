// -----------------------------------------------------------------------
// <copyright file="IncludeMany.cs" company="Project Contributors">
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
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using MicroLite.Mapping;
using MicroLite.TypeConverters;

namespace MicroLite.Core
{
    /// <summary>
    /// The default implementation of <see cref="IIncludeMany&lt;T&gt;"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be included.</typeparam>
    [System.Diagnostics.DebuggerDisplay("HasValue: {HasValue}, Values: {Values}")]
    internal sealed class IncludeMany<T> : Include, IIncludeMany<T>
    {
        private static readonly Type s_resultType = typeof(T);
        private Action<IIncludeMany<T>> _callback;

        public IList<T> Values { get; } = new List<T>();

        public void OnLoad(Action<IIncludeMany<T>> action) => _callback = action;

        internal override async Task BuildValueAsync(DbDataReader reader, CancellationToken cancellationToken)
        {
            if (TypeConverter.IsNotEntityAndConvertible(s_resultType))
            {
                ITypeConverter typeConverter = TypeConverter.For(s_resultType) ?? TypeConverter.Default;

                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var value = (T)typeConverter.ConvertFromDbValue(reader, 0, s_resultType);

                    Values.Add(value);
                }
            }
            else
            {
                IObjectInfo objectInfo = ObjectInfo.For(s_resultType);

                while (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var instance = (T)objectInfo.CreateInstance(reader);

                    Values.Add(instance);
                }
            }

            HasValue = Values.Count > 0;

            _callback?.Invoke(this);
        }
    }
}
