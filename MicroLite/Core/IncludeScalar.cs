// -----------------------------------------------------------------------
// <copyright file="IncludeScalar.cs" company="MicroLite">
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
    using System.Data;
    using System.Data.Common;
    using System.Threading;
    using MicroLite.TypeConverters;

    /// <summary>
    /// The default implementation of <see cref="IInclude&lt;T&gt;"/> for scalar results.
    /// </summary>
    /// <typeparam name="T">The type of object to be included.</typeparam>
    [System.Diagnostics.DebuggerDisplay("HasValue: {HasValue}, Value: {Value}")]
    internal sealed class IncludeScalar<T> : Include, IInclude<T>
    {
        private static readonly Type resultType = typeof(T);
        private Action<IInclude<T>> callback;

        public T Value
        {
            get;
            private set;
        }

        public void OnLoad(Action<IInclude<T>> action)
        {
            this.callback = action;
        }

        internal override void BuildValue(IDataReader reader)
        {
            if (reader.Read())
            {
                if (reader.FieldCount != 1)
                {
                    throw new MicroLiteException(ExceptionMessages.IncludeScalar_MultipleColumns);
                }

                var typeConverter = TypeConverter.For(resultType) ?? TypeConverter.Default;

                this.Value = (T)typeConverter.ConvertFromDbValue(reader, 0, resultType);
                this.HasValue = true;

                if (reader.Read())
                {
                    throw new MicroLiteException(ExceptionMessages.Include_SingleRecordExpected);
                }

                if (this.callback != null)
                {
                    this.callback(this);
                }
            }
        }

#if NET_4_5

        internal override async System.Threading.Tasks.Task BuildValueAsync(DbDataReader reader, CancellationToken cancellationToken)
        {
            if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
            {
                if (reader.FieldCount != 1)
                {
                    throw new MicroLiteException(ExceptionMessages.IncludeScalar_MultipleColumns);
                }

                var typeConverter = TypeConverter.For(resultType) ?? TypeConverter.Default;

                this.Value = (T)typeConverter.ConvertFromDbValue(reader, 0, resultType);
                this.HasValue = true;

                if (await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    throw new MicroLiteException(ExceptionMessages.Include_SingleRecordExpected);
                }

                if (this.callback != null)
                {
                    this.callback(this);
                }
            }
        }

#endif
    }
}