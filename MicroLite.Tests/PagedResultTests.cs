namespace MicroLite.Tests
{
    using System.Collections.Generic;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="PagedResult"/> class.
    /// </summary>
    public class PagedResultTests
    {
        [Fact]
        public void ConstructorSetsProperties()
        {
            var page = 1;
            var results = new List<Customer> { new Customer() };
            var resultsPerPage = 10;
            var totalResults = 100;

            var pagedResults = new PagedResult<Customer>(page, results, resultsPerPage, totalResults);

            Assert.Equal(page, pagedResults.Page);
            Assert.Equal(results, pagedResults.Results);
            Assert.Equal(resultsPerPage, pagedResults.ResultsPerPage);
            Assert.Equal(totalResults, pagedResults.TotalResults);
        }

        [Fact]
        public void HasResultsReturnsFalseIfNoResults()
        {
            var page = 1;
            var results = new List<Customer>();
            var resultsPerPage = 10;
            var totalResults = 100;

            var pagedResults = new PagedResult<Customer>(page, results, resultsPerPage, totalResults);

            Assert.False(pagedResults.HasResults);
        }

        [Fact]
        public void HasResultsReturnsTrueIfContainsResults()
        {
            var page = 1;
            var results = new List<Customer> { new Customer() };
            var resultsPerPage = 10;
            var totalResults = 100;

            var pagedResults = new PagedResult<Customer>(page, results, resultsPerPage, totalResults);

            Assert.True(pagedResults.HasResults);
        }

        [Fact]
        public void MoreResultsAvailableReturnsFalseIfNoMoreResults()
        {
            var page = 10;
            var results = new List<Customer> { new Customer() };
            var resultsPerPage = 10;
            var totalResults = 100;

            var pagedResults = new PagedResult<Customer>(page, results, resultsPerPage, totalResults);

            Assert.False(pagedResults.MoreResultsAvailable);
        }

        [Fact]
        public void MoreResultsAvailableReturnsTrueIfMoreResults()
        {
            var page = 1;
            var results = new List<Customer> { new Customer() };
            var resultsPerPage = 10;
            var totalResults = 100;

            var pagedResults = new PagedResult<Customer>(page, results, resultsPerPage, totalResults);

            Assert.True(pagedResults.MoreResultsAvailable);
        }

        [Fact]
        public void TotalPages()
        {
            var resultsPerPage = 10;
            var totalResults = 100;

            var pagedResults = new PagedResult<Customer>(1, null, resultsPerPage, totalResults);

            Assert.Equal(10, pagedResults.TotalPages);
        }

        /// <summary>
        /// Issue #130 - PagedResults shows incorrect Total Pages.
        /// </summary>
        [Fact]
        public void TotalPagesReturns1IfTotalResultsLessThanResultsPerPage()
        {
            var resultsPerPage = 10;
            var totalResults = 5;

            var pagedResults = new PagedResult<Customer>(1, null, resultsPerPage, totalResults);

            Assert.Equal(1, pagedResults.TotalPages);
        }

        [Fact]
        public void TotalPagesReturnsOneIfNoResults()
        {
            var resultsPerPage = 10;
            var totalResults = 0;

            var pagedResults = new PagedResult<Customer>(1, null, resultsPerPage, totalResults);

            Assert.Equal(1, pagedResults.TotalPages);
        }

        [Fact]
        public void TotalPagesReturnsOneIfTotalResultsEqualsThanResultsPerPage()
        {
            var resultsPerPage = 10;
            var totalResults = 10;

            var pagedResults = new PagedResult<Customer>(1, null, resultsPerPage, totalResults);

            Assert.Equal(1, pagedResults.TotalPages);
        }

        [Fact]
        public void TotalPagesReturnsOneIfTotalResultsLessThanResultsPerPage()
        {
            var resultsPerPage = 10;
            var totalResults = 7;

            var pagedResults = new PagedResult<Customer>(1, null, resultsPerPage, totalResults);

            Assert.Equal(1, pagedResults.TotalPages);
        }

        /// <summary>
        /// Issue #227 - PagedResults.TotalPages returns incorrect value.
        /// </summary>
        [Fact]
        public void TotalPagesReturnsTwoIfTotalResultsLessThanDoubleResultsPerPage()
        {
            var resultsPerPage = 10;
            var totalResults = 14;

            var pagedResults = new PagedResult<Customer>(1, null, resultsPerPage, totalResults);

            Assert.Equal(2, pagedResults.TotalPages);
        }

        private class Customer
        {
        }
    }
}