// -----------------------------------------------------------------------
// <copyright file="PagingOptions.cs" company="Project Contributors">
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

namespace MicroLite
{
    /// <summary>
    /// A struct containing the count and offset to be used for paged queries.
    /// </summary>
    [System.Diagnostics.DebuggerDisplay("Count: {Count}, Offset: {Offset}")]
    public struct PagingOptions : IEquatable<PagingOptions>
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="PagingOptions" /> struct.
        /// </summary>
        /// <param name="count">The count (number of records to return).</param>
        /// <param name="offset">The offset (number of records to skip).</param>
        private PagingOptions(int count, int offset)
        {
            Count = count;
            Offset = offset;
        }

        /// <summary>
        /// Gets the number of record to return.
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// Gets the number of records to skip.
        /// </summary>
        public int Offset { get; }

        /// <summary>
        /// Gets the paging options for when no paging is required.
        /// </summary>
        internal static PagingOptions None => new PagingOptions(count: 0, offset: 0);

        /// <summary>
        /// Checks whether two separate PagingOptions instances are not equal.
        /// </summary>
        /// <param name="pagingOptions1">The paging options to check.</param>
        /// <param name="pagingOptions2">The paging options to check against.</param>
        /// <returns><c>true</c> if the instances are not considered equal; otherwise, <c>false</c>.</returns>
        public static bool operator !=(PagingOptions pagingOptions1, PagingOptions pagingOptions2) => !pagingOptions1.Equals(pagingOptions2);

        /// <summary>
        /// Checks whether two separate PagingOptions instances are equal.
        /// </summary>
        /// <param name="pagingOptions1">The paging options to check.</param>
        /// <param name="pagingOptions2">The paging options to check against.</param>
        /// <returns><c>true</c> if the instances are considered equal; otherwise, <c>false</c>.</returns>
        public static bool operator ==(PagingOptions pagingOptions1, PagingOptions pagingOptions2) => pagingOptions1.Equals(pagingOptions2);

        /// <summary>
        /// Gets the paging options for the specified page number.
        /// </summary>
        /// <param name="page">The page number (starting at 1).</param>
        /// <param name="resultsPerPage">The results per page.</param>
        /// <returns>The paging options for the specified page number.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if page or resultsPerPage are less than 1.</exception>
        public static PagingOptions ForPage(int page, int resultsPerPage)
        {
            if (page < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(page), ExceptionMessages.PagingOptions_PagesMustBeAtleastOne);
            }

            if (resultsPerPage < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(resultsPerPage), ExceptionMessages.PagingOptions_ResultsPerPageMustBeAtLeast1);
            }

            int skip = (page - 1) * resultsPerPage;

            return new PagingOptions(count: resultsPerPage, offset: skip);
        }

        /// <summary>
        /// Gets the paging options for the specified skip and take numbers.
        /// </summary>
        /// <param name="skip">The number of records to skip.</param>
        /// <param name="take">The number of records to take.</param>
        /// <returns>The paging options for the specified skip and take numbers.</returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown if skip is below 0 or take is less than 1.</exception>
        public static PagingOptions SkipTake(int skip, int take)
        {
            if (skip < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(skip), ExceptionMessages.PagingOptions_SkipMustBeZeroOrAbove);
            }

            if (take < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(take), ExceptionMessages.PagingOptions_TakeMustBeZeroOrAbove);
            }

            return new PagingOptions(count: take, offset: skip);
        }

        /// <summary>
        /// Determines whether the specified <see cref="object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            var other = obj as PagingOptions?;

            if (other is null)
            {
                return false;
            }

            return Equals(other.Value);
        }

        /// <summary>
        /// Determines whether the specified <see cref="PagingOptions" /> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="PagingOptions"/> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="PagingOptions" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public bool Equals(PagingOptions other)
            => other.Count == Count && other.Offset == Offset;

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
            => Count.GetHashCode() ^ Offset.GetHashCode();
    }
}
