// -----------------------------------------------------------------------
// <copyright file="IgnoreAttribute.cs" company="MicroLite">
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
namespace MicroLite
{
    using System;

    /// <summary>
    /// An attribute which can be applied to a property to specify that it should be ignored by the
    /// MicroLite ORM framework.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class IgnoreAttribute : Attribute
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="IgnoreAttribute"/> class.
        /// </summary>
        public IgnoreAttribute()
        {
        }
    }
}