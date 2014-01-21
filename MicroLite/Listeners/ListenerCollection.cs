// -----------------------------------------------------------------------
// <copyright file="ListenerCollection.cs" company="MicroLite">
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
    /// The class which contains the IListeners used by the MicroLite ORM framework.
    /// </summary>
    public sealed class ListenerCollection : Collection<IListener>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ListenerCollection"/> class.
        /// </summary>
        public ListenerCollection()
        {
            this.Add(new DbGeneratedListener());
            this.Add(new AssignedListener());
        }
    }
}