// -----------------------------------------------------------------------
// <copyright file="TypeConverterCollection.cs" company="MicroLite">
// Copyright 2012 - 2015 Project Contributors
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
    using System.Collections.ObjectModel;

    /// <summary>
    /// The class which contains the ITypeConverters used by the MicroLite ORM framework.
    /// </summary>
    /// <remarks>The collection acts in the same way as a stack, the last converter added is the first used if it handles the type.</remarks>
    public sealed class TypeConverterCollection : Collection<ITypeConverter>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="TypeConverterCollection"/> class.
        /// </summary>
        public TypeConverterCollection()
        {
            // In order to maintain the behaviour of a stack, keep inserting at position 0 which will shift the items down.
            this.Items.Insert(0, new XDocumentTypeConverter());
            this.Items.Insert(0, new TimeSpanTypeConverter());
            this.Items.Insert(0, new UriTypeConverter());
            this.Items.Insert(0, new EnumTypeConverter()); // Enum is the most common of the custom types so put that at the top.
        }

        /// <summary>
        /// Inserts an element into the <see cref="T:System.Collections.ObjectModel.Collection`1" /> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which <paramref name="item" /> should be inserted.</param>
        /// <param name="item">The object to insert. The value can be null for reference types.</param>
        protected override void InsertItem(int index, ITypeConverter item)
        {
            this.Items.Insert(0, item);
        }
    }
}