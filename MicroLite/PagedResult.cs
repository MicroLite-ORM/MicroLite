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

        /// <summary>
        /// Initialises a new instance of the <see cref="PagedResult&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <param name="results">The results.</param>
        public PagedResult(long page, IList<T> results)
        {
            this.page = page;
            this.results = results;
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
    }
}