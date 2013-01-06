// -----------------------------------------------------------------------
// <copyright file="ListenerCollection.cs" company="MicroLite">
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
namespace MicroLite.Listeners
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The class which contains the IListeners used by the MicroLite ORM framework.
    /// </summary>
    public sealed class ListenerCollection : IEnumerable<IListener>
    {
        private readonly IList<Func<IListener>> listenerFactories = new List<Func<IListener>>();
        private readonly IList<Type> listenerTypes = new List<Type>();

        /// <summary>
        /// Initialises a new instance of the <see cref="ListenerCollection"/> class.
        /// </summary>
        public ListenerCollection()
        {
            this.Add<DbGeneratedListener>();
            this.Add<AssignedListener>();
            this.Add<GuidListener>();
            this.Add<GuidCombListener>();
        }

        /// <summary>
        /// Adds the IListener to the collection.
        /// </summary>
        /// <typeparam name="T">The type of <see cref="IListener"/> to add.</typeparam>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter", Justification = "The syntax is accepted practice for registering types.")]
        public void Add<T>()
            where T : IListener, new()
        {
            var listenerType = typeof(T);

            if (!this.listenerTypes.Contains(listenerType))
            {
                this.listenerFactories.Add(() =>
                {
                    return new T();
                });

                this.listenerTypes.Add(listenerType);
            }
        }

        /// <summary>
        /// Clears all IListeners registered.
        /// </summary>
        public void Clear()
        {
            this.listenerFactories.Clear();
            this.listenerTypes.Clear();
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        public IEnumerator<IListener> GetEnumerator()
        {
            foreach (var listenerFactory in this.listenerFactories)
            {
                yield return listenerFactory();
            }
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