// -----------------------------------------------------------------------
// <copyright file="VersionAttribute.cs" company="MicroLite">
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
namespace MicroLite.Mapping.Attributes
{
    using System;

    /// <summary>
    /// An attribute which can be applied to a property to specify that it maps to the row version
    /// in the table.
    /// </summary>
    /// <example>
    /// <code>
    /// [Column("Version")]
    /// [Version]
    /// public int Version
    /// {
    ///     get;
    ///     set;
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class VersionAttribute : Attribute
    {
    }
}