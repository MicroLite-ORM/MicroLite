// -----------------------------------------------------------------------
// <copyright file="IncludeSingle.cs" company="Project Contributors">
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
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;
using MicroLite.Mapping;
using MicroLite.TypeConverters;

namespace MicroLite.Core
{
    /// <summary>
    /// The default implementation of <see cref="IInclude&lt;T&gt;"/> for mapped objects.
    /// </summary>
    /// <typeparam name="T">The type of object to be included.</typeparam>
    [System.Diagnostics.DebuggerDisplay("HasValue: {HasValue}, Value: {Value}")]
    internal sealed class IncludeSingle<T> : Include, IInclude<T>
    {
        private static readonly Type resultType = typeof(T);
        private Action<IInclude<T>> callback;

        public T Value { get; private set; }

        public void OnLoad(Action<IInclude<T>> action) => this.callback = action;

        internal override async Task BuildValueAsync(DbDataReader reader, CancellationToken cancellationToken)
        {
            if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                if (TypeConverter.IsNotEntityAndConvertible(resultType))
                {
                    var typeConverter = TypeConverter.For(resultType) ?? TypeConverter.Default;

                    this.Value = (T)typeConverter.ConvertFromDbValue(reader, 0, resultType);
                }
                else
                {
                    var objectInfo = ObjectInfo.For(resultType);

                    this.Value = (T)objectInfo.CreateInstance(reader);
                }

                this.HasValue = true;

                if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    throw new MicroLiteException(ExceptionMessages.Include_SingleRecordExpected);
                }

                this.callback?.Invoke(this);
            }
        }
    }
}
