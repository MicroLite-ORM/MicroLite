// -----------------------------------------------------------------------
// <copyright file="InsertListenerCollection.cs" company="MicroLite">
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
namespace MicroLite.Listeners
{
    using System.Collections.ObjectModel;

    /// <summary>
    /// The class which contains the <see cref="IInsertListener"/>s used by the MicroLite ORM framework.
    /// </summary>
    public sealed class InsertListenerCollection : Collection<IInsertListener>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="InsertListenerCollection"/> class.
        /// </summary>
        public InsertListenerCollection()
        {
            this.Items.Insert(0, new IdentifierStrategyListener());
        }

        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        protected override void InsertItem(int index, IInsertListener item)
        {
            // In order to maintain the behaviour of a stack, keep inserting at position 0 which will shift the items down.
            this.Items.Insert(0, item);
        }
    }
}