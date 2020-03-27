// -----------------------------------------------------------------------
// <copyright file="SessionListeners.cs" company="Project Contributors">
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
using System.Collections.Generic;

namespace MicroLite.Listeners
{
    internal sealed class SessionListeners
    {
        internal SessionListeners()
            : this(new IDeleteListener[0], new IInsertListener[0], new IUpdateListener[0])
        {
        }

        internal SessionListeners(
            IList<IDeleteListener> deleteListeners,
            IList<IInsertListener> insertListeners,
            IList<IUpdateListener> updateListeners)
        {
            DeleteListeners = deleteListeners;
            InsertListeners = insertListeners;
            UpdateListeners = updateListeners;
        }

        internal IList<IDeleteListener> DeleteListeners { get; }

        internal IList<IInsertListener> InsertListeners { get; }

        internal IList<IUpdateListener> UpdateListeners { get; }
    }
}
