// -----------------------------------------------------------------------
// <copyright file="TypeConverterCollection.cs" company="MicroLite">
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
namespace MicroLite.TypeConverters
{
    using System.Collections.Generic;

    /// <summary>
    /// The class which contains the ITypeConverters used by the MicroLite ORM framework.
    /// </summary>
    /// <remarks>The collection acts in the same way as a stack, the last converter added is the first used if it handles the type.</remarks>
    public sealed class TypeConverterCollection : ICollection<ITypeConverter>
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
        /// Gets the number of type converters contained in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return this.converters.Count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the collection is read-only.
        /// </summary>
        /// <returns>true if the collection is read-only; otherwise, false.</returns>
        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Adds the specified type converter to the collection of type converters which can be used by MicroLite.
        /// </summary>
        /// <param name="item">The type converter to be added.</param>
        public void Add(ITypeConverter item)
        {
            this.converters.Push(item);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            this.converters.Clear();
        }

        /// <summary>
        /// Determines whether the collection contains the specified type converter.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>
        /// true if the item exists in the collection; otherwise, false.
        /// </returns>
        public bool Contains(ITypeConverter item)
        {
            return this.converters.Contains(item);
        }

        /// <summary>
        /// Copies the items in the collection to the specified one dimension array at the specified index.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(ITypeConverter[] array, int arrayIndex)
        {
            this.converters.CopyTo(array, arrayIndex);
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
        /// Removes the specified type converter from the collection.
        /// </summary>
        /// <param name="item">The type converter to be removed.</param>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the collection; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original collection.
        /// </returns>
        public bool Remove(ITypeConverter item)
        {
            var existing = this.converters.ToArray();
            this.converters.Clear();

            bool removed = false;

            foreach (var typeConverter in existing)
            {
                if (typeConverter == item)
                {
                    removed = true;
                }
                else
                {
                    this.converters.Push(typeConverter);
                }
            }

            return removed;
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