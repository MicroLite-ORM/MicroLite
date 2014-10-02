namespace MicroLite.Tests.Driver
{
    using System;
    using System.Data;
    using System.Data.Common;
    using MicroLite.Driver;
    using MicroLite.FrameworkExtensions;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="DbDriver"/> class.
    /// </summary>
    public class DbDriverTests : UnitTest
    {
        [Fact]
        public void BuildCommandForSqlQueryWithSqlText()
        {
            var sqlQuery = new SqlQuery(
                "SELECT * FROM Table WHERE Table.Id = ? AND Table.Value1 = ? AND Table.Value2 = ?",
                100, "hello", null);

            var mockDbProviderFactory = new Mock<DbProviderFactory>();
            mockDbProviderFactory.Setup(x => x.CreateCommand()).Returns(new System.Data.OleDb.OleDbCommand());

            var mockDbDriver = new Mock<DbDriver>(SqlCharacters.Empty);
            mockDbDriver.CallBase = true;
            mockDbDriver.Object.DbProviderFactory = mockDbProviderFactory.Object;

            var command = mockDbDriver.Object.BuildCommand(sqlQuery);

            Assert.Equal(sqlQuery.CommandText, command.CommandText);
            Assert.Equal(CommandType.Text, command.CommandType);
            Assert.Equal(3, command.Parameters.Count);

            var parameter1 = (IDataParameter)command.Parameters[0];
            Assert.Equal(ParameterDirection.Input, parameter1.Direction);
            Assert.Equal("Parameter0", parameter1.ParameterName);
            Assert.Equal(sqlQuery.Arguments[0], parameter1.Value);

            var parameter2 = (IDataParameter)command.Parameters[1];
            Assert.Equal(ParameterDirection.Input, parameter2.Direction);
            Assert.Equal("Parameter1", parameter2.ParameterName);
            Assert.Equal(sqlQuery.Arguments[1], parameter2.Value);

            var parameter3 = (IDataParameter)command.Parameters[2];
            Assert.Equal(ParameterDirection.Input, parameter3.Direction);
            Assert.Equal("Parameter2", parameter3.ParameterName);
            Assert.Equal(DBNull.Value, parameter3.Value);
        }

        /// <summary>
        /// Issue #6 - The argument count check needs to cater for the same argument being used twice.
        /// </summary>
        [Fact]
        public void BuildCommandForSqlQueryWithSqlTextWhichUsesSameParameterTwice()
        {
            var sqlQuery = new SqlQuery(
                "SELECT * FROM [Table] WHERE [Table].[Id] = @p0 AND [Table].[Value1] = @p1 OR @p1 IS NULL",
                100, "hello");

            var mockDbProviderFactory = new Mock<DbProviderFactory>();
            mockDbProviderFactory.Setup(x => x.CreateCommand()).Returns(new System.Data.SqlClient.SqlCommand());

            var mockDbDriver = new Mock<DbDriver>(SqlCharacters.Empty);
            mockDbDriver.CallBase = true;
            mockDbDriver.Object.DbProviderFactory = mockDbProviderFactory.Object;

            var command = mockDbDriver.Object.BuildCommand(sqlQuery);

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
        public void BuildCommandSetsDbCommandTimeoutToSqlQueryTime()
        {
            var sqlQuery = new SqlQuery("SELECT * FROM Table");
            sqlQuery.Timeout = 42; // Use an oddball time which shouldn't be a default anywhere.

            var mockDbProviderFactory = new Mock<DbProviderFactory>();
            mockDbProviderFactory.Setup(x => x.CreateCommand()).Returns(new System.Data.OleDb.OleDbCommand());

            var mockDbDriver = new Mock<DbDriver>(SqlCharacters.Empty);
            mockDbDriver.CallBase = true;
            mockDbDriver.Object.DbProviderFactory = mockDbProviderFactory.Object;

            var command = mockDbDriver.Object.BuildCommand(sqlQuery);

            Assert.Equal(sqlQuery.Timeout, command.CommandTimeout);
        }

        [Fact]
        public void BuildCommandSetsDbTypeToAnsiStringIfHandleStringsAsUnicodeIsFalse()
        {
            var mockDbProviderFactory = new Mock<DbProviderFactory>();
            mockDbProviderFactory.Setup(x => x.CreateCommand()).Returns(new System.Data.OleDb.OleDbCommand());

            var mockDbDriver = new Mock<DbDriver>(SqlCharacters.Empty);
            mockDbDriver.CallBase = true;
            mockDbDriver.Object.DbProviderFactory = mockDbProviderFactory.Object;

            var dbDriver = mockDbDriver.Object;
            dbDriver.HandleStringsAsUnicode = false;

            var sqlQuery = new SqlQuery("SELECT * FROM Table WHERE Name = ?", "John");

            var command = dbDriver.BuildCommand(sqlQuery);

            Assert.Equal(DbType.AnsiString, ((DbParameter)command.Parameters[0]).DbType);
        }

        [Fact]
        public void BuildCommandSetsDbTypeToStringIfHandleStringsAsUnicodeIsTrue()
        {
            var mockDbProviderFactory = new Mock<DbProviderFactory>();
            mockDbProviderFactory.Setup(x => x.CreateCommand()).Returns(new System.Data.OleDb.OleDbCommand());

            var mockDbDriver = new Mock<DbDriver>(SqlCharacters.Empty);
            mockDbDriver.CallBase = true;
            mockDbDriver.Object.DbProviderFactory = mockDbProviderFactory.Object;

            var dbDriver = mockDbDriver.Object;
            dbDriver.HandleStringsAsUnicode = true;

            var sqlQuery = new SqlQuery("SELECT * FROM Table WHERE Name = ?", "John");

            var command = dbDriver.BuildCommand(sqlQuery);

            Assert.Equal(DbType.String, ((DbParameter)command.Parameters[0]).DbType);
        }

        [Fact]
        public void BuildCommandThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var mockDbDriver = new Mock<DbDriver>(SqlCharacters.Empty);
            mockDbDriver.CallBase = true;

            var exception = Assert.Throws<ArgumentNullException>(
                () => mockDbDriver.Object.BuildCommand(null));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        [Fact]
        public void BuildCommandThrowsMicroLiteExceptionForParameterCountMismatch()
        {
            var sqlQuery = new SqlQuery(
                "SELECT * FROM [Table] WHERE [Table].[Id] = @p0 AND [Table].[Value] = @p1",
                100);

            var mockDbDriver = new Mock<DbDriver>(SqlCharacters.Empty);
            mockDbDriver.CallBase = true;

            var exception = Assert.Throws<MicroLiteException>(
                () => mockDbDriver.Object.BuildCommand(sqlQuery));

            Assert.Equal(ExceptionMessages.DbDriver_ArgumentsCountMismatch.FormatWith("2", "1"), exception.Message);
        }

        [Fact]
        public void HandleStringsAsUnicodeReturnsTrueByDefault()
        {
            var mockDbDriver = new Mock<DbDriver>(SqlCharacters.Empty);
            mockDbDriver.CallBase = true;

            Assert.True(mockDbDriver.Object.HandleStringsAsUnicode);
        }

        [Fact]
        public void SupportsBatchedQueriesReturnsFalseByDefault()
        {
            var mockDbDriver = new Mock<DbDriver>(SqlCharacters.Empty);
            mockDbDriver.CallBase = true;

            Assert.False(mockDbDriver.Object.SupportsBatchedQueries);
        }

        public class WhenCallingCombine
        {
            private readonly SqlQuery combinedQuery;
            private readonly SqlQuery sqlQuery1;
            private readonly SqlQuery sqlQuery2;

            public WhenCallingCombine()
            {
                this.sqlQuery1 = new SqlQuery("SELECT \"Column1\", \"Column2\", \"Column3\" FROM \"Table1\" WHERE \"Column1\" = ? AND \"Column2\" > ?", "Foo", 100);
                this.sqlQuery1.Timeout = 38;

                this.sqlQuery2 = new SqlQuery("SELECT \"Column1\", \"Column2\" FROM \"Table2\" WHERE (\"Column1\" = ? OR ? IS NULL) AND \"Column2\" < ?", "Bar", -1);
                this.sqlQuery2.Timeout = 42;

                var mockDbDriver = new Mock<DbDriver>(SqlCharacters.Empty);
                mockDbDriver.CallBase = true;

                this.combinedQuery = mockDbDriver.Object.Combine(this.sqlQuery1, this.sqlQuery2);
            }

            [Fact]
            public void TheCombinedArgumentsShouldContainTheFirstArgumentOfTheFirstQuery()
            {
                Assert.Equal(this.sqlQuery1.Arguments[0], this.combinedQuery.Arguments[0]);
            }

            [Fact]
            public void TheCombinedArgumentsShouldContainTheFirstArgumentOfTheSecondQuery()
            {
                Assert.Equal(this.sqlQuery2.Arguments[0], this.combinedQuery.Arguments[2]);
            }

            [Fact]
            public void TheCombinedArgumentsShouldContainTheNumberOfArgumentsInTheSourceQueries()
            {
                Assert.Equal(this.sqlQuery1.Arguments.Count + this.sqlQuery2.Arguments.Count, this.combinedQuery.Arguments.Count);
            }

            [Fact]
            public void TheCombinedArgumentsShouldContainTheSecondArgumentOfTheFirstQuery()
            {
                Assert.Equal(this.sqlQuery1.Arguments[1], this.combinedQuery.Arguments[1]);
            }

            [Fact]
            public void TheCombinedArgumentsShouldContainTheSecondArgumentOfTheSecondQuery()
            {
                Assert.Equal(this.sqlQuery2.Arguments[1], this.combinedQuery.Arguments[3]);
            }

            [Fact]
            public void TheCombinedCommandTextShouldBeSeparatedUsingTheSelectSeparator()
            {
                Assert.Equal(
                    "SELECT \"Column1\", \"Column2\", \"Column3\" FROM \"Table1\" WHERE \"Column1\" = ? AND \"Column2\" > ?;\r\nSELECT \"Column1\", \"Column2\" FROM \"Table2\" WHERE (\"Column1\" = ? OR ? IS NULL) AND \"Column2\" < ?",
                    this.combinedQuery.CommandText);
            }

            [Fact]
            public void TheTimeoutShouldBeSetToTheLongestTimeoutOfTheSourceQueries()
            {
                Assert.Equal(this.sqlQuery2.Timeout, this.combinedQuery.Timeout);
            }
        }

        public class WhenCallingCombine_AndTheFirstSqlQueryIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var mockDbDriver = new Mock<DbDriver>(SqlCharacters.Empty);
                mockDbDriver.CallBase = true;

                var exception = Assert.Throws<ArgumentNullException>(
                    () => mockDbDriver.Object.Combine(null, new SqlQuery("")));

                Assert.Equal("sqlQuery1", exception.ParamName);
            }
        }

        public class WhenCallingCombine_AndTheSecondSqlQueryIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var mockDbDriver = new Mock<DbDriver>(SqlCharacters.Empty);
                mockDbDriver.CallBase = true;

                var exception = Assert.Throws<ArgumentNullException>(
                    () => mockDbDriver.Object.Combine(new SqlQuery(""), null));

                Assert.Equal("sqlQuery2", exception.ParamName);
            }
        }

        public class WhenCallingCombine_WithAnIEnumerable_AndTheSourceQueriesIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var mockDbDriver = new Mock<DbDriver>(SqlCharacters.Empty);
                mockDbDriver.CallBase = true;

                var exception = Assert.Throws<ArgumentNullException>(
                    () => mockDbDriver.Object.Combine(null));

                Assert.Equal("sqlQueries", exception.ParamName);
            }
        }

        public class WhenCallingCombine_WithAnIEnumerableSqlQueryNotUsingNamedParameters
        {
            private readonly SqlQuery combinedQuery;
            private readonly SqlQuery sqlQuery1;
            private readonly SqlQuery sqlQuery2;

            public WhenCallingCombine_WithAnIEnumerableSqlQueryNotUsingNamedParameters()
            {
                this.sqlQuery1 = new SqlQuery("SELECT \"Column1\", \"Column2\", \"Column3\" FROM \"Table1\" WHERE \"Column1\" = ? AND \"Column2\" > ?", "Foo", 100);
                this.sqlQuery1.Timeout = 38;

                this.sqlQuery2 = new SqlQuery("SELECT \"Column1\", \"Column2\" FROM \"Table2\" WHERE (\"Column1\" = ? OR ? IS NULL) AND \"Column2\" < ?", "Bar", -1);
                this.sqlQuery2.Timeout = 42;

                var mockDbDriver = new Mock<DbDriver>(SqlCharacters.Empty);
                mockDbDriver.CallBase = true;

                this.combinedQuery = mockDbDriver.Object.Combine(new[] { this.sqlQuery1, this.sqlQuery2 });
            }

            [Fact]
            public void TheCombinedArgumentsShouldContainTheFirstArgumentOfTheFirstQuery()
            {
                Assert.Equal(this.sqlQuery1.Arguments[0], this.combinedQuery.Arguments[0]);
            }

            [Fact]
            public void TheCombinedArgumentsShouldContainTheFirstArgumentOfTheSecondQuery()
            {
                Assert.Equal(this.sqlQuery2.Arguments[0], this.combinedQuery.Arguments[2]);
            }

            [Fact]
            public void TheCombinedArgumentsShouldContainTheNumberOfArgumentsInTheSourceQueries()
            {
                Assert.Equal(this.sqlQuery1.Arguments.Count + this.sqlQuery2.Arguments.Count, this.combinedQuery.Arguments.Count);
            }

            [Fact]
            public void TheCombinedArgumentsShouldContainTheSecondArgumentOfTheFirstQuery()
            {
                Assert.Equal(this.sqlQuery1.Arguments[1], this.combinedQuery.Arguments[1]);
            }

            [Fact]
            public void TheCombinedArgumentsShouldContainTheSecondArgumentOfTheSecondQuery()
            {
                Assert.Equal(this.sqlQuery2.Arguments[1], this.combinedQuery.Arguments[3]);
            }

            [Fact]
            public void TheCombinedCommandTextShouldBeSeparatedUsingTheSelectSeparator()
            {
                Assert.Equal(
                    "SELECT \"Column1\", \"Column2\", \"Column3\" FROM \"Table1\" WHERE \"Column1\" = ? AND \"Column2\" > ?;\r\nSELECT \"Column1\", \"Column2\" FROM \"Table2\" WHERE (\"Column1\" = ? OR ? IS NULL) AND \"Column2\" < ?",
                    this.combinedQuery.CommandText);
            }

            [Fact]
            public void TheTimeoutShouldBeSetToTheLongestTimeoutOfTheSourceQueries()
            {
                Assert.Equal(this.sqlQuery2.Timeout, this.combinedQuery.Timeout);
            }
        }

        public class WhenCallingCombine_WithAnIEnumerableSqlQueryUsingNamedParameters
        {
            private readonly SqlQuery combinedQuery;
            private readonly SqlQuery sqlQuery1;
            private readonly SqlQuery sqlQuery2;

            public WhenCallingCombine_WithAnIEnumerableSqlQueryUsingNamedParameters()
            {
                this.sqlQuery1 = new SqlQuery("SELECT \"Column1\", \"Column2\", \"Column3\" FROM \"Table1\" WHERE \"Column1\" = @p0 AND \"Column2\" > @p1", "Foo", 100);
                this.sqlQuery1.Timeout = 38;

                this.sqlQuery2 = new SqlQuery("SELECT \"Column1\", \"Column2\" FROM \"Table2\" WHERE (\"Column1\" = @p0 OR @p0 IS NULL) AND \"Column2\" < @p1", "Bar", -1);
                this.sqlQuery2.Timeout = 42;

                var mockDbDriver = new Mock<DbDriver>(SqlCharacters.Empty);
                mockDbDriver.CallBase = true;

                this.combinedQuery = mockDbDriver.Object.Combine(new[] { this.sqlQuery1, this.sqlQuery2 });
            }

            [Fact]
            public void TheCombinedArgumentsShouldContainTheFirstArgumentOfTheFirstQuery()
            {
                Assert.Equal(this.sqlQuery1.Arguments[0], this.combinedQuery.Arguments[0]);
            }

            [Fact]
            public void TheCombinedArgumentsShouldContainTheFirstArgumentOfTheSecondQuery()
            {
                Assert.Equal(this.sqlQuery2.Arguments[0], this.combinedQuery.Arguments[2]);
            }

            [Fact]
            public void TheCombinedArgumentsShouldContainTheNumberOfArgumentsInTheSourceQueries()
            {
                Assert.Equal(this.sqlQuery1.Arguments.Count + this.sqlQuery2.Arguments.Count, this.combinedQuery.Arguments.Count);
            }

            [Fact]
            public void TheCombinedArgumentsShouldContainTheSecondArgumentOfTheFirstQuery()
            {
                Assert.Equal(this.sqlQuery1.Arguments[1], this.combinedQuery.Arguments[1]);
            }

            [Fact]
            public void TheCombinedArgumentsShouldContainTheSecondArgumentOfTheSecondQuery()
            {
                Assert.Equal(this.sqlQuery2.Arguments[1], this.combinedQuery.Arguments[3]);
            }

            [Fact]
            public void TheCombinedCommandTextShouldBeSeparatedUsingTheSelectSeparator()
            {
                Assert.Equal(
                    "SELECT \"Column1\", \"Column2\", \"Column3\" FROM \"Table1\" WHERE \"Column1\" = @p0 AND \"Column2\" > @p1;\r\nSELECT \"Column1\", \"Column2\" FROM \"Table2\" WHERE (\"Column1\" = @p2 OR @p2 IS NULL) AND \"Column2\" < @p3",
                    this.combinedQuery.CommandText);
            }

            [Fact]
            public void TheTimeoutShouldBeSetToTheLongestTimeoutOfTheSourceQueries()
            {
                Assert.Equal(this.sqlQuery2.Timeout, this.combinedQuery.Timeout);
            }
        }

        public class WhenCallingGetConnection_WithConnectionScopePerSession
        {
            private readonly Mock<DbConnection> mockConnection = new Mock<DbConnection>();

            public WhenCallingGetConnection_WithConnectionScopePerSession()
            {
                this.mockConnection.SetupProperty(x => x.ConnectionString);

                var mockDbProviderFactory = new Mock<DbProviderFactory>();
                mockDbProviderFactory.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

                var mockDbDriver = new Mock<DbDriver>(SqlCharacters.Empty);
                mockDbDriver.CallBase = true;

                mockDbDriver.Object.ConnectionString = "DATA SOURCE=...";
                mockDbDriver.Object.DbProviderFactory = mockDbProviderFactory.Object;

                mockDbDriver.Object.GetConnection(ConnectionScope.PerSession);
            }

            [Fact]
            public void TheConnectionIsOpened()
            {
                this.mockConnection.Verify(x => x.Open(), Times.Once());
            }

            [Fact]
            public void TheConnectionStringIsSet()
            {
                this.mockConnection.VerifySet(x => x.ConnectionString = "DATA SOURCE=...", Times.Once());
            }
        }

        public class WhenCallingGetConnection_WithConnectionScopePerTransaction
        {
            private readonly Mock<DbConnection> mockConnection = new Mock<DbConnection>();

            public WhenCallingGetConnection_WithConnectionScopePerTransaction()
            {
                this.mockConnection.SetupProperty(x => x.ConnectionString);

                var mockDbProviderFactory = new Mock<DbProviderFactory>();
                mockDbProviderFactory.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

                var mockDbDriver = new Mock<DbDriver>(SqlCharacters.Empty);
                mockDbDriver.CallBase = true;

                mockDbDriver.Object.ConnectionString = "DATA SOURCE=...";
                mockDbDriver.Object.DbProviderFactory = mockDbProviderFactory.Object;

                mockDbDriver.Object.GetConnection(ConnectionScope.PerTransaction);
            }

            [Fact]
            public void TheConnectionIsNotOpened()
            {
                this.mockConnection.Verify(x => x.Open(), Times.Never());
            }

            [Fact]
            public void TheConnectionStringIsSet()
            {
                this.mockConnection.VerifySet(x => x.ConnectionString = "DATA SOURCE=...", Times.Once());
            }
        }
    }
}