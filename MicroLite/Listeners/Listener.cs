// -----------------------------------------------------------------------
// <copyright file="Listener.cs" company="MicroLite">
// Copyright 2012 - 2016 Project Contributors
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
    /// <summary>
    /// Static entry point for listener collections.
    /// </summary>
    public static class Listener
    {
        private static readonly DeleteListenerCollection deleteListeners = new DeleteListenerCollection();
        private static readonly InsertListenerCollection insertListeners = new InsertListenerCollection();
        private static readonly UpdateListenerCollection updateListeners = new UpdateListenerCollection();

        /// <summary>
        /// Gets the listener collection which contains all registered <see cref="IDeleteListener"/>s.
        /// </summary>
        public static DeleteListenerCollection DeleteListeners
        {
            get
            {
                return deleteListeners;
            }
        }

        /// <summary>
        /// Gets the listener collection which contains all registered <see cref="IInsertListener"/>s.
        /// </summary>
        public static InsertListenerCollection InsertListener
        {
            get
            {
                return insertListeners;
            }
        }

        /// <summary>
        /// Gets the listener collection which contains all registered <see cref="IUpdateListener"/>s.
        /// </summary>
        public static UpdateListenerCollection UpdateListeners
        {
            get
            {
                return updateListeners;
            }
        }
    }
}