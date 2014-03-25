namespace MicroLite.Tests.Driver
{
    using MicroLite.Driver;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="MySqlDbDriver"/> class.
    /// </summary>
    public class MySqlDbDriverTests : UnitTest
    {
        [Fact]
        public void SupportsBatchedQueriesReturnsTrue()
        {
            var dbDriver = new MySqlDbDriver();

            Assert.True(dbDriver.SupportsBatchedQueries);
        }
    }
}