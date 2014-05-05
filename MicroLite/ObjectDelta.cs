// -----------------------------------------------------------------------
// <copyright file="ObjectDelta.cs" company="MicroLite">
// Copyright 2012 - 2013 Trevor Pilley
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// An class which contains partial (delta) changes to an object.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("{forType}")]
    public sealed class ObjectDelta
    {
        private readonly IDictionary<string, object> changes = new Dictionary<string, object>();
        private readonly Type forType;
        private readonly object identifier;

        /// <summary>
        /// Initialises a new instance of the <see cref="ObjectDelta"/> class.
        /// </summary>
        /// <param name="forType">The type the changes relate to.</param>
        /// <param name="identifier">The identifier for the instance of the type the changes relate to.</param>
        public ObjectDelta(Type forType, object identifier)
        {
            this.forType = forType;
            this.identifier = identifier;
        }

        /// <summary>
        /// Gets the number of changes in the delta.
        /// </summary>
        public int ChangeCount
        {
            get
            {
                return this.changes.Count;
            }
        }

        /// <summary>
        /// Gets the changes contained in the delta.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "Ignored.")]
        public IEnumerable<KeyValuePair<string, object>> Changes
        {
            get
            {
                return this.changes;
            }
        }

        /// <summary>
        /// Gets for type the changes relate to.
        /// </summary>
        public Type ForType
        {
            get
            {
                return this.forType;
            }
        }

        /// <summary>
        /// Gets the identifier for the instance of the type the changes relate to.
        /// </summary>
        public object Identifier
        {
            get
            {
                return this.identifier;
            }
        }

        /// <summary>
        /// Adds the a property value change.
        /// </summary>
        /// <param name="propertyName">The name of the property to change.</param>
        /// <param name="newValue">The new value for the property (can be null).</param>
        public void AddChange(string propertyName, object newValue)
        {
            this.changes.Add(propertyName, newValue);
        }
    }
}