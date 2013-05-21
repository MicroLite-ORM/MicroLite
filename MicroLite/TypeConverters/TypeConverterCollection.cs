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
    /// <remarks>The collection acts in the same way as a stack, the last converter added is the first used if it handles the type.</remarks>
    public sealed class TypeConverterCollection : IEnumerable<ITypeConverter>
    {
        private readonly Stack<ITypeConverter> converters = new Stack<ITypeConverter>();

        /// <summary>
        /// Initialises a new instance of the <see cref="TypeConverterCollection"/> class.
        /// </summary>
        public TypeConverterCollection()
        {
            this.converters.Push(new ObjectTypeConverter());
            this.converters.Push(new EnumTypeConverter());
            this.converters.Push(new XDocumentTypeConverter());
        }

        /// <summary>
        /// Adds the specified type converter to the collection of type converters which can be used by MicroLite.
        /// </summary>
        /// <param name="typeConverter">The type converter to be added.</param>
        public void Add(ITypeConverter typeConverter)
        {
            this.converters.Push(typeConverter);
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