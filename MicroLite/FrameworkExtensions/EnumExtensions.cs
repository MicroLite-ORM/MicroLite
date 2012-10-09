// -----------------------------------------------------------------------
// <copyright file="EnumExtensions.cs" company="MicroLite">
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
namespace MicroLite.FrameworkExtensions
{
    using System;
    using System.Linq;

    internal static class EnumExtensions
    {
        internal static bool In(this Enum value, params Enum[] values)
        {
            return values.Contains(value);
        }
    }
}