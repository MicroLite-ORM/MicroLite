// -----------------------------------------------------------------------
// <copyright file="IncludeSingle.cs" company="MicroLite">
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
    using System.Data;
    using MicroLite.Mapping;

    /// <summary>
    /// The default implementation of <see cref="IInclude&lt;T&gt;"/>.
    /// </summary>
    /// <typeparam name="T">The type of object to be included.</typeparam>
    internal sealed class IncludeSingle<T> : Include, IInclude<T> where T : class, new()
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
                var objectInfo = ObjectInfo.For(typeof(T));

                this.value = objectBuilder.BuildInstance<T>(objectInfo, reader);

                if (reader.Read())
                {
                    throw new MicroLiteException(Messages.IncludeSingle_SingleResultExpected);
                }
            }
        }
    }
}