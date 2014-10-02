namespace MicroLite.Tests.Driver
{
    using System.Data;
    using System.Data.Common;
    using MicroLite.Driver;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="PostgreSqlDbDriver"/> class.
    /// </summary>
    public class PostgreSqlDbDriverTests : UnitTest
    {
        /// <summary>
        /// Issue #6 - The argument count check needs to cater for the same argument being used twice.
        /// </summary>
        [Fact]
        public void BuildCommandForSqlQueryWithSqlTextWhichUsesSameParameterTwice()
        {
            var sqlQuery = new SqlQuery(
                "SELECT * FROM \"Table\" WHERE \"Table\".\"Id\" = @p0 AND \"Table].\"Value1\" = @p1 OR @p1 IS NULL",
                100, "hello");

            var mockDbProviderFactory = new Mock<DbProviderFactory>();
            mockDbProviderFactory.Setup(x => x.CreateCommand()).Returns(new System.Data.OleDb.OleDbCommand());

            var dbDriver = new PostgreSqlDbDriver();
            dbDriver.DbProviderFactory = mockDbProviderFactory.Object;

            var command = dbDriver.BuildCommand(sqlQuery);

            Assert.Equal(sqlQuery.CommandText, command.CommandText);
            Assert.Equal(CommandType.Text, command.CommandType);
            Assert.Equal(2, command.Parameters.Count);

            var parameter1 = (IDataParameter)command.Parameters[0];
            Assert.Equal(ParameterDirection.Input, parameter1.Direction);
            Assert.Equal("@p0", parameter1.ParameterName);
            Assert.Equal(sqlQuery.Arguments[0], parameter1.Value);

            var parameter2 = (IDataParameter)command.Parameters[1];
            Assert.Equal(ParameterDirection.Input, parameter2.Direction);
            Assert.Equal("@p1", parameter2.ParameterName);
            Assert.Equal(sqlQuery.Arguments[1], parameter2.Value);
        }

        [Fact]
        public void BuildCommandForSqlQueryWithStoredProcedureWithoutParameters()
        {
            var sqlQuery = new SqlQuery("SELECT GetTableContents");

            var mockDbProviderFactory = new Mock<DbProviderFactory>();
            mockDbProviderFactory.Setup(x => x.CreateCommand()).Returns(new System.Data.OleDb.OleDbCommand());

            var dbDriver = new PostgreSqlDbDriver();
            dbDriver.DbProviderFactory = mockDbProviderFactory.Object;

            var command = dbDriver.BuildCommand(sqlQuery);

            // The command text should only contain the stored procedure name.
            Assert.Equal("GetTableContents", command.CommandText);
            Assert.Equal(CommandType.StoredProcedure, command.CommandType);
            Assert.Equal(0, command.Parameters.Count);
        }

        [Fact]
        public void BuildCommandForSqlQueryWithStoredProcedureWithParameters()
        {
            var sqlQuery = new SqlQuery(
                "SELECT GetTableContents (@identifier, @Cust_Name)",
                100, "hello");

            var mockDbProviderFactory = new Mock<DbProviderFactory>();
            mockDbProviderFactory.Setup(x => x.CreateCommand()).Returns(new System.Data.OleDb.OleDbCommand());

            var dbDriver = new PostgreSqlDbDriver();
            dbDriver.DbProviderFactory = mockDbProviderFactory.Object;

            var command = dbDriver.BuildCommand(sqlQuery);

            // The command text should only contain the stored procedure name.
            Assert.Equal("GetTableContents", command.CommandText);
            Assert.Equal(CommandType.StoredProcedure, command.CommandType);
            Assert.Equal(2, command.Parameters.Count);

            var parameter1 = (IDataParameter)command.Parameters[0];
            Assert.Equal("@identifier", parameter1.ParameterName);
            Assert.Equal(sqlQuery.Arguments[0], parameter1.Value);

            var parameter2 = (IDataParameter)command.Parameters[1];
            Assert.Equal("@Cust_Name", parameter2.ParameterName);
            Assert.Equal(sqlQuery.Arguments[1], parameter2.Value);
        }

        [Fact]
        public void SupportsBatchedQueriesReturnsTrue()
        {
            var dbDriver = new PostgreSqlDbDriver();

            Assert.True(dbDriver.SupportsBatchedQueries);
        }
    }
}