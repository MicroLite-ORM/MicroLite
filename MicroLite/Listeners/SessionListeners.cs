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
namespace MicroLite.Listeners
{
    using System.Collections.Generic;

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
            this.DeleteListeners = deleteListeners;
            this.InsertListeners = insertListeners;
            this.UpdateListeners = updateListeners;
        }

        internal IList<IDeleteListener> DeleteListeners
        {
            get;
            private set;
        }

        internal IList<IInsertListener> InsertListeners
        {
            get;
            private set;
        }

        internal IList<IUpdateListener> UpdateListeners
        {
            get;
            private set;
        }
    }
}