// -----------------------------------------------------------------------
// <copyright file="PagedResult.cs" company="MicroLite">
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
    using System.Collections.Generic;

    /// <summary>
    /// The result of a paged query.
    /// </summary>
    /// <typeparam name="T">The type of object the contained in the results.</typeparam>
    public sealed class PagedResult<T>
    {
        private readonly long page;
        private readonly IList<T> results;
        private readonly long resultsPerPage;

        /// <summary>
        /// Initialises a new instance of the <see cref="PagedResult&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="page">The page number.</param>
        /// <param name="results">The results in the page.</param>
        /// <param name="resultsPerPage">The number of results per page.</param>
        public PagedResult(long page, IList<T> results, long resultsPerPage)
        {
            this.page = page;
            this.results = results;
            this.resultsPerPage = resultsPerPage;
        }

        /// <summary>
        /// Gets the page number.
        /// </summary>
        public long Page
        {
            get
            {
                return this.page;
            }
        }

        /// <summary>
        /// Gets the results in the page.
        /// </summary>
        public IList<T> Results
        {
            get
            {
                return this.results;
            }
        }

        /// <summary>
        /// Gets the number of results per page.
        /// </summary>
        public long ResultsPerPage
        {
            get
            {
                return this.resultsPerPage;
            }
        }
    }
}