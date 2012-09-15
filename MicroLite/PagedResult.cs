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
    /// A class which contains the result of a paged query.
    /// </summary>
    /// <typeparam name="T">The type of object the contained in the results.</typeparam>
    [System.Diagnostics.DebuggerDisplay("Page {Page} of {TotalPages} showing {ResultsPerPage} results per page with a total of {TotalResults} results")]
    public sealed class PagedResult<T>
    {
        private readonly long page;
        private readonly IList<T> results;
        private readonly long resultsPerPage;
        private readonly long totalResults;

        /// <summary>
        /// Initialises a new instance of the <see cref="PagedResult&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="page">The page number for the results.</param>
        /// <param name="results">The results in the page.</param>
        /// <param name="resultsPerPage">The number of results per page.</param>
        /// <param name="totalResults">The total number of results for the query.</param>
        public PagedResult(long page, IList<T> results, long resultsPerPage, long totalResults)
        {
            this.page = page;
            this.results = results;
            this.resultsPerPage = resultsPerPage;
            this.totalResults = totalResults;
        }

        /// <summary>
        /// Gets the page number for the results.
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

        /// <summary>
        /// Gets the total number of pages for the query.
        /// </summary>
        public long TotalPages
        {
            get
            {
                return this.TotalResults / this.ResultsPerPage;
            }
        }

        /// <summary>
        /// Gets the total number of results for the query.
        /// </summary>
        public long TotalResults
        {
            get
            {
                return this.totalResults;
            }
        }
    }
}