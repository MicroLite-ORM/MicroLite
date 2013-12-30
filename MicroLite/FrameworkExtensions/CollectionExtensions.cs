// -----------------------------------------------------------------------
// <copyright file="CollectionExtensions.cs" company="MicroLite">
// Copyright 2012 - 2013 Project Contributors
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
    using System.Collections.Generic;

    internal static class CollectionExtensions
    {
        internal static void Each<T>(this IList<T> source, Action<T> action)
        {
            for (int i = 0; i < source.Count; i++)
            {
                action(source[i]);
            }
        }

        internal static void Reverse<T>(this IList<T> source, Action<T> action)
        {
            for (int i = source.Count - 1; i >= 0; i--)
            {
                action(source[i]);
            }
        }
    }
}