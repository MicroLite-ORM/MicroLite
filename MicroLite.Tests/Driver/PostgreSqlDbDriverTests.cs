namespace MicroLite.Tests.Driver
{
    using MicroLite.Driver;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="PostgreSqlDbDriver"/> class.
    /// </summary>
    public class PostgreSqlDbDriverTests : UnitTest
    {
        [Fact]
        public void SupportsBatchedQueriesReturnsTrue()
        {
            var dbDriver = new PostgreSqlDbDriver();

            Assert.True(dbDriver.SupportsBatchedQueries);
        }
    }
}