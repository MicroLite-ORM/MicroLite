namespace MicroLite.Tests.Driver
{
    using MicroLite.Driver;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SQLiteDbDriver"/> class.
    /// </summary>
    public class SQLiteDbDriverTests : UnitTest
    {
        [Fact]
        public void SupportsBatchedQueriesReturnsTrue()
        {
            var dbDriver = new SQLiteDbDriver();

            Assert.True(dbDriver.SupportsBatchedQueries);
        }
    }
}