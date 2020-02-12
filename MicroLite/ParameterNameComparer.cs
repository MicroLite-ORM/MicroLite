// -----------------------------------------------------------------------
// <copyright file="ParameterNameComparer.cs" company="Project Contributors">
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
using System;
using System.Collections.Generic;

namespace MicroLite
{
    /// <summary>
    /// An implementation of <see cref="IComparer&lt;T&gt;"/> to sort parameter names.
    /// </summary>
    /// <remarks>
    /// A special use case to ensure that @p9 is sorted after @p10 which is not the case with standard string sorting.
    /// </remarks>
    internal sealed class ParameterNameComparer : IComparer<string>
    {
        private static readonly IComparer<string> instance = new ParameterNameComparer();

        /// <summary>
        /// Prevents a default instance of the <see cref="ParameterNameComparer"/> class from being created.
        /// </summary>
        private ParameterNameComparer()
        {
        }

        /// <summary>
        /// Gets the parameter name comparer instance.
        /// </summary>
        internal static IComparer<string> Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
        /// </summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>
        /// A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.Value Meaning Less than zero<paramref name="x" /> is less than <paramref name="y" />.Zero<paramref name="x" /> equals <paramref name="y" />.Greater than zero<paramref name="x" /> is greater than <paramref name="y" />.
        /// </returns>
        public int Compare(string x, string y)
        {
            if (object.ReferenceEquals(x, y))
            {
                // As per default string comparison logic
                return 0;
            }

            if (x == null)
            {
                // As per default string comparison logic
                return -1;
            }

            if (y == null)
            {
                // As per default string comparison logic
                return 1;
            }

            int minLength = Math.Min(x.Length, y.Length);

            for (int i = 0; i < minLength; i++)
            {
                if (char.IsDigit(x[i]) && char.IsDigit(y[i]))
                {
                    if (x[i] < y[i] && i < minLength - 1)
                    {
                        // Both strings have a digit at position i, it's not the last character
                        // and the digit in x is lower than the digit in y - sort x before y
                        return -1;
                    }

                    if (x[i] > y[i] && i < minLength - 1)
                    {
                        // Both strings have a digit at position i, it's not the last character
                        // and the digit in x is higher than the digit in y - sort x after y
                        return 1;
                    }
                }
            }

            if (x.Length < y.Length)
            {
                return -1;
            }

            if (x.Length > y.Length)
            {
                return 1;
            }

            // Strings are the same length and contain no digits, fall back to the default comparison logic.
            return Comparer<string>.Default.Compare(x, y);
        }
    }
}
