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
        internal static void Each<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var item in source)
            {
                action(item);
            }
        }

        internal static IEnumerable<T> Reverse<T>(this IList<T> source)
        {
            for (int i = source.Count - 1; i >= 0; i--)
            {
                yield return source[i];
            }
        }
    }
}