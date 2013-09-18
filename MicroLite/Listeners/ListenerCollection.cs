// -----------------------------------------------------------------------
// <copyright file="ListenerCollection.cs" company="MicroLite">
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
namespace MicroLite.Listeners
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The class which contains the IListeners used by the MicroLite ORM framework.
    /// </summary>
    public sealed class ListenerCollection : ICollection<IListener>
    {
        private readonly Stack<IListener> listeners = new Stack<IListener>();

        /// <summary>
        /// Initialises a new instance of the <see cref="ListenerCollection"/> class.
        /// </summary>
        public ListenerCollection()
        {
            this.listeners.Push(new DbGeneratedListener());
            this.listeners.Push(new AssignedListener());
            this.listeners.Push(new GuidListener());
            this.listeners.Push(new GuidCombListener());
        }

        /// <summary>
        /// Gets the number of listeners contained in the collection.
        /// </summary>
        public int Count
        {
            get
            {
                return this.listeners.Count;
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
        /// Adds the IListener to the collection.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IListener"/> to add.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The syntax is accepted practice for registering types.")]
        [Obsolete("This method has been replaced by Add(IListener).", error: false)]
        public void Add<T>()
            where T : IListener, new()
        {
            var listener = new T();

            this.Add(listener);
        }

        /// <summary>
        /// Adds the specified listener to the collection of listeners which can be used by MicroLite.
        /// </summary>
        /// <param name="item">The listener to be added.</param>
        public void Add(IListener item)
        {
            this.listeners.Push(item);
        }

        /// <summary>
        /// Removes all items from the collection.
        /// </summary>
        public void Clear()
        {
            this.listeners.Clear();
        }

        /// <summary>
        /// Determines whether the collection contains the specified listener.
        /// </summary>
        /// <param name="item">The object to locate in the collection.</param>
        /// <returns>
        /// true if the item exists in the collection; otherwise, false.
        /// </returns>
        public bool Contains(IListener item)
        {
            return this.listeners.Contains(item);
        }

        /// <summary>
        /// Copies the items in the collection to the specified one dimension array at the specified index.
        /// </summary>
        /// <param name="array">The array.</param>
        /// <param name="arrayIndex">Index of the array.</param>
        public void CopyTo(IListener[] array, int arrayIndex)
        {
            this.listeners.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IListener> GetEnumerator()
        {
            return this.listeners.GetEnumerator();
        }

        /// <summary>
        /// Removes the specified listener from the collection.
        /// </summary>
        /// <param name="item">The listener to be removed.</param>
        /// <returns>
        /// true if <paramref name="item" /> was successfully removed from the collection; otherwise, false. This method also returns false if <paramref name="item" /> is not found in the original collection.
        /// </returns>
        public bool Remove(IListener item)
        {
            var existing = this.listeners.ToArray();
            this.listeners.Clear();

            bool removed = false;

            foreach (var listener in existing)
            {
                if (listener == item)
                {
                    removed = true;
                }
                else
                {
                    this.listeners.Push(listener);
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