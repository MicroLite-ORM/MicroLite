﻿// -----------------------------------------------------------------------
// <copyright file="IncludeScalar.cs" company="MicroLite">
// Copyright 2012 Trevor Pilley
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
    using System.Globalization;

    /// <summary>
    /// The default implementation of <see cref="IInclude&lt;T&gt;"/> for scalar results.
    /// </summary>
    /// <typeparam name="T">The type of object to be included.</typeparam>
    internal sealed class IncludeScalar<T> : Include, IInclude<T>
    {
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
                if (reader.FieldCount != 1)
                {
                    throw new MicroLiteException(Messages.IncludeScalar_MultipleColumns);
                }

                this.value = (T)Convert.ChangeType(reader[0], typeof(T), CultureInfo.InvariantCulture);
                this.HasValue = true;

                if (reader.Read())
                {
                    throw new MicroLiteException(Messages.IncludeSingle_SingleResultExpected);
                }
            }
        }
    }
}