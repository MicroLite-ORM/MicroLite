namespace MicroLite.Tests.Driver
{
    using System.Data.Common;
    using MicroLite.Driver;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SqlServerCeDbDriver"/> class.
    /// </summary>
    public class SqlServerCeDbDriverTests
    {
        [Fact]
        public void BuildCommandDoesNotSetsDbCommandTimeoutToSqlQueryTime()
        {
            var sqlQuery = new SqlQuery("SELECT * FROM Table");
            sqlQuery.Timeout = 42; // Use an oddball time which shouldn't be a default anywhere.

            var mockDbProviderFactory = new Mock<DbProviderFactory>();

            // Frig the timeout here - we're using an OleDbCommand but at runtime it will be an
            // SqlCeCommand which has the timeout set to 0 by default
            mockDbProviderFactory.Setup(x => x.CreateCommand()).Returns(new System.Data.OleDb.OleDbCommand
            {
                CommandTimeout = 0
            });

            var dbDriver = new SqlServerCeDbDriver();
            dbDriver.DbProviderFactory = mockDbProviderFactory.Object;

            var command = dbDriver.BuildCommand(sqlQuery);

            Assert.Equal(0, command.CommandTimeout);
        }

        [Fact]
        public void SupportsBatchedQueriesReturnsFalse()
        {
            var dbDriver = new SqlServerCeDbDriver();

            Assert.False(dbDriver.SupportsBatchedQueries);
        }
    }
}