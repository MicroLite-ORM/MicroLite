namespace MicroLite.Tests
{
    using System.Collections.Generic;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="PagedResult"/> class.
    /// </summary>
    [TestFixture]
    public class PagedResultTests
    {
        [Test]
        public void ConstructorSetsProperties()
        {
            var page = 1;
            var results = new List<Customer> { new Customer() };
            var resultsPerPage = 10;
            var totalResults = 100;

            var pagedResults = new PagedResult<Customer>(page, results, resultsPerPage, totalResults);

            Assert.AreEqual(page, pagedResults.Page);
            CollectionAssert.AreEqual(results, pagedResults.Results);
            Assert.AreEqual(resultsPerPage, pagedResults.ResultsPerPage);
            Assert.AreEqual(totalResults, pagedResults.TotalResults);
        }

        [Test]
        public void TotalPages()
        {
            var resultsPerPage = 10;
            var totalResults = 100;

            var pagedResults = new PagedResult<Customer>(1, null, resultsPerPage, totalResults);

            Assert.AreEqual(totalResults / resultsPerPage, pagedResults.TotalPages);
        }

        private class Customer
        {
        }
    }
}