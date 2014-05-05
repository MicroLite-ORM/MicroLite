namespace MicroLite.Tests.Driver
{
    using MicroLite.Driver;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SqlServerCeDbDriver"/> class.
    /// </summary>
    public class SqlServerCeDbDriverTests
    {
        [Fact]
        public void SupportsBatchedQueriesReturnsFalse()
        {
            var dbDriver = new SqlServerCeDbDriver();

            Assert.False(dbDriver.SupportsBatchedQueries);
        }
    }
}