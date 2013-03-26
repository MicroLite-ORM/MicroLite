// -----------------------------------------------------------------------
// <copyright file="TypeConverterCollection.cs" company="MicroLite">
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
namespace MicroLite.TypeConverters
{
    using System.Collections.Generic;

    /// <summary>
    /// The class which contains the ITypeConverters used by the MicroLite ORM framework.
    /// </summary>
    internal sealed class TypeConverterCollection : IEnumerable<ITypeConverter>
    {
        private readonly IList<ITypeConverter> converters = new List<ITypeConverter>();

        /// <summary>
        /// Initialises a new instance of the <see cref="TypeConverterCollection"/> class.
        /// </summary>
        internal TypeConverterCollection()
        {
            this.converters.Add(new EnumTypeConverter());
            this.converters.Add(new ObjectTypeConverter());
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<ITypeConverter> GetEnumerator()
        {
            return this.converters.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}