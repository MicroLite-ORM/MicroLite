namespace MicroLite.Tests.Driver
{
    using System.Data.OleDb;
    using MicroLite.Driver;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SqlServerCeDbDriver"/> class.
    /// </summary>
    public class SqlServerCeDbDriverTests
    {
        [Fact]
        public void BuildCommandDoesNotSetsDbCommandTimeoutToSqlQueryTime()
        {
            // Frig the timeout here - we're using an OleDbCommand but at runtime it will be an
            // SqlCeCommand which has the timeout set to 0 by default
            var command = new OleDbCommand
            {
                CommandTimeout = 0
            };

            var sqlQuery = new SqlQuery("SELECT * FROM Table");
            sqlQuery.Timeout = 42; // Use an oddball time which shouldn't be a default anywhere.

            var dbDriver = new SqlServerCeDbDriver();
            dbDriver.BuildCommand(command, sqlQuery);

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