namespace MicroLite.Tests.Driver
{
    using MicroLite.Driver;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="FirebirdDbDriver"/> class.
    /// </summary>
    public class FirebirdDbDriverTests : UnitTest
    {
        [Fact]
        public void SupportsBatchedQueriesReturnsFalse()
        {
            var dbDriver = new FirebirdDbDriver();

            Assert.False(dbDriver.SupportsBatchedQueries);
        }
    }
}