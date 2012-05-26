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

            var pagedResults = new PagedResult<Customer>(page, results);

            Assert.AreEqual(page, pagedResults.Page);
            CollectionAssert.AreEqual(results, pagedResults.Results);
        }

        private class Customer
        {
        }
    }
}