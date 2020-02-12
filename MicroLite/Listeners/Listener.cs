// -----------------------------------------------------------------------
// <copyright file="Listener.cs" company="Project Contributors">
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
namespace MicroLite.Listeners
{
    using System.Collections.Generic;
    using MicroLite.Collections;

    /// <summary>
    /// Static entry point for listener collections.
    /// </summary>
    public static class Listener
    {
        private static readonly StackCollection<IDeleteListener> deleteListeners = new StackCollection<IDeleteListener>();
        private static readonly StackCollection<IInsertListener> insertListeners = new StackCollection<IInsertListener> { new IdentifierStrategyListener() };
        private static readonly StackCollection<IUpdateListener> updateListeners = new StackCollection<IUpdateListener>();

        /// <summary>
        /// Gets the listener collection which contains all registered <see cref="IDeleteListener"/>s.
        /// </summary>
        public static IList<IDeleteListener> DeleteListeners
        {
            get
            {
                return deleteListeners;
            }
        }

        /// <summary>
        /// Gets the listener collection which contains all registered <see cref="IInsertListener"/>s.
        /// </summary>
        public static IList<IInsertListener> InsertListener
        {
            get
            {
                return insertListeners;
            }
        }

        /// <summary>
        /// Gets the listener collection which contains all registered <see cref="IUpdateListener"/>s.
        /// </summary>
        public static IList<IUpdateListener> UpdateListeners
        {
            get
            {
                return updateListeners;
            }
        }
    }
}
