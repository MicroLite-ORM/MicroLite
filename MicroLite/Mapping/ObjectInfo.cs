// -----------------------------------------------------------------------
// <copyright file="ObjectInfo.cs" company="MicroLite">
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
namespace MicroLite.Mapping
{
    /// <summary>
    /// The class which describes a type and the table it is mapped to.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("ObjectInfo for {ForType}")]
    public sealed class ObjectInfo
    {
        private static IMappingConvention mappingConvention = new LooseAttributeMappingConvention();

        /// <summary>
        /// Initialises a new instance of the <see cref="ObjectInfo"/> class.
        /// </summary>
        /// <param name="tableInfo">The table info.</param>
        public ObjectInfo(TableInfo tableInfo)
        {
        }

        internal static IMappingConvention MappingConvention
        {
            get
            {
                return ObjectInfo.mappingConvention;
            }

            set
            {
                ObjectInfo.mappingConvention = value;
            }
        }
    }
}