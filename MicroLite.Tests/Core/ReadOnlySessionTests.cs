namespace MicroLite.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using MicroLite.Builder;
    using MicroLite.Core;
    using MicroLite.Dialect;
    using MicroLite.Driver;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ReadOnlySession"/> class.
    /// </summary>
    public class ReadOnlySessionTests : UnitTest
    {
        public ReadOnlySessionTests()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));
        }

        [Fact]
        public void AdvancedReturnsSameSessionByDifferentInterface()
        {
            var session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            var advancedSession = session.Advanced;

            Assert.Same(session, advancedSession);
        }

        [Fact]
        public void AllCreatesASelectAllQueryExecutesAndReturnsResults()
        {
            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.SqlCharacters).Returns(SqlCharacters.Empty);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(mockConnection.Object);
            mockDbDriver.Setup(x => x.BuildCommand(SqlBuilder.Select("*").From(typeof(Customer)).ToSqlQuery())).Returns(mockCommand.Object);

            var session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            var customers = session.All<Customer>();

            session.ExecutePendingQueries();

            Assert.Equal(1, customers.Values.Count);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnection.VerifyAll();
            mockSqlDialect.VerifyAll();
        }

        [Fact]
        public void FetchExecutesAndReturnsResults()
        {
            var sqlQuery = new SqlQuery("");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();

            var mockSqlDialect = new Mock<ISqlDialect>();

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(mockConnection.Object);
            mockDbDriver.Setup(x => x.BuildCommand(sqlQuery)).Returns(mockCommand.Object);

            var session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            var customers = session.Fetch<Customer>(sqlQuery);

            Assert.Equal(1, customers.Count);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnection.VerifyAll();
            mockSqlDialect.VerifyAll();
        }

        [Fact]
        public void FetchThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            var exception = Assert.Throws<ArgumentNullException>(
                () => session.Fetch<Customer>(null));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        [Fact]
        public void FetchThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(
                () => session.Fetch<Customer>(null));
        }

        [Fact]
        public void IncludeReturnsSameSessionByDifferentInterface()
        {
            var session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            var includeSession = session.Include;

            Assert.Same(session, includeSession);
        }

        [Fact]
        public void IncludeScalarSqlQueryExecutesAndReturnsResult()
        {
            var sqlQuery = new SqlQuery("");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(1);
            mockReader.Setup(x => x[0]).Returns(10);
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();

            var mockSqlDialect = new Mock<ISqlDialect>();

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(mockConnection.Object);
            mockDbDriver.Setup(x => x.BuildCommand(sqlQuery)).Returns(mockCommand.Object);

            var session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            var includeScalar = session.Include.Scalar<int>(sqlQuery);

            session.ExecutePendingQueries();

            Assert.Equal(10, includeScalar.Value);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnection.VerifyAll();
            mockSqlDialect.VerifyAll();
        }

        [Fact]
        public void IncludeScalarThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            SqlQuery sqlQuery = null;

            var exception = Assert.Throws<ArgumentNullException>(
                () => session.Include.Scalar<int>(sqlQuery));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        [Fact]
        public void IncludeScalarThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(
                () => session.Include.Scalar<int>(new SqlQuery("")));
        }

        [Fact]
        public void MicroLiteExceptionsCaughtByExecutePendingQueriesShouldNotBeWrappedInAnotherMicroLiteException()
        {
            var mockSqlDialect = new Mock<ISqlDialect>();

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.BuildCommand(It.IsAny<SqlQuery>())).Throws<MicroLiteException>();

            var session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            // We need at least 1 queued query otherwise we will get an exception when doing queries.Dequeue() instead.
            session.Include.Scalar<int>(new SqlQuery(""));

            var exception = Assert.Throws<MicroLiteException>(() => session.ExecutePendingQueries());

            Assert.IsNotType<MicroLiteException>(exception.InnerException);
        }

        [Fact]
        public void PagedExecutesAndReturnsResultsForFirstPageWithOnePerPage()
        {
            var sqlQuery = new SqlQuery("SELECT * FROM TABLE");
            var countQuery = new SqlQuery("SELECT COUNT(*) FROM TABLE");
            var pagedQuery = new SqlQuery("SELECT * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM Customers) AS Customers");
            var combinedQuery = new SqlQuery("SELECT COUNT(*) FROM TABLE;SELECT Id FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM Customers) AS Customers");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(new Queue<int>(new[] { 1, 0 }).Dequeue);
            mockReader.Setup(x => x[0]).Returns(1000); // Simulate 1000 records in the count query
            mockReader.Setup(x => x.NextResult()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false, true, false }).Dequeue);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CountQuery(sqlQuery)).Returns(countQuery);
            mockSqlDialect.Setup(x => x.PageQuery(sqlQuery, PagingOptions.ForPage(1, 1))).Returns(pagedQuery);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(mockConnection.Object);
            mockDbDriver.Setup(x => x.SupportsBatchedQueries).Returns(true);
            mockDbDriver.Setup(x => x.Combine(It.Is<IEnumerable<SqlQuery>>(c => c.Contains(countQuery) && c.Contains(pagedQuery)))).Returns(combinedQuery);
            mockDbDriver.Setup(x => x.BuildCommand(combinedQuery)).Returns(mockCommand.Object);

            var session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            var page = session.Paged<Customer>(sqlQuery, PagingOptions.ForPage(1, 1));

            Assert.Equal(1, page.Page);
            Assert.Equal(1, page.Results.Count);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnection.VerifyAll();
            mockSqlDialect.VerifyAll();
        }

        [Fact]
        public void PagedExecutesAndReturnsResultsForFirstPageWithTwentyFivePerPage()
        {
            var sqlQuery = new SqlQuery("SELECT * FROM TABLE");
            var countQuery = new SqlQuery("SELECT COUNT(*) FROM TABLE");
            var pagedQuery = new SqlQuery("SELECT * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM Customers) AS Customers");
            var combinedQuery = new SqlQuery("SELECT COUNT(*) FROM TABLE;SELECT * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM Customers) AS Customers");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(new Queue<int>(new[] { 1, 0 }).Dequeue);
            mockReader.Setup(x => x[0]).Returns(1000); // Simulate 1000 records in the count query
            mockReader.Setup(x => x.NextResult()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false, true, false }).Dequeue);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CountQuery(sqlQuery)).Returns(countQuery);
            mockSqlDialect.Setup(x => x.PageQuery(sqlQuery, PagingOptions.ForPage(1, 25))).Returns(pagedQuery);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(mockConnection.Object);
            mockDbDriver.Setup(x => x.SupportsBatchedQueries).Returns(true);
            mockDbDriver.Setup(x => x.Combine(It.Is<IEnumerable<SqlQuery>>(c => c.Contains(countQuery) && c.Contains(pagedQuery)))).Returns(combinedQuery);
            mockDbDriver.Setup(x => x.BuildCommand(combinedQuery)).Returns(mockCommand.Object);

            var session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            var page = session.Paged<Customer>(sqlQuery, PagingOptions.ForPage(1, 25));

            Assert.Equal(1, page.Page);
            Assert.Equal(1, page.Results.Count);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnection.VerifyAll();
            mockSqlDialect.VerifyAll();
        }

        [Fact]
        public void PagedExecutesAndReturnsResultsForTenthPageWithTwentyFivePerPage()
        {
            var sqlQuery = new SqlQuery("SELECT * FROM TABLE");
            var countQuery = new SqlQuery("SELECT COUNT(*) FROM TABLE");
            var pagedQuery = new SqlQuery("SELECT * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM Customers) AS Customers");
            var combinedQuery = new SqlQuery("SELECT COUNT(*) FROM TABLE;SELECT * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM Customers) AS Customers");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(new Queue<int>(new[] { 1, 0 }).Dequeue);
            mockReader.Setup(x => x[0]).Returns(1000); // Simulate 1000 records in the count query
            mockReader.Setup(x => x.NextResult()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false, true, false }).Dequeue);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CountQuery(sqlQuery)).Returns(countQuery);
            mockSqlDialect.Setup(x => x.PageQuery(sqlQuery, PagingOptions.ForPage(10, 25))).Returns(pagedQuery);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(mockConnection.Object);
            mockDbDriver.Setup(x => x.SupportsBatchedQueries).Returns(true);
            mockDbDriver.Setup(x => x.Combine(It.Is<IEnumerable<SqlQuery>>(c => c.Contains(countQuery) && c.Contains(pagedQuery)))).Returns(combinedQuery);
            mockDbDriver.Setup(x => x.BuildCommand(combinedQuery)).Returns(mockCommand.Object);

            var session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            var page = session.Paged<Customer>(sqlQuery, PagingOptions.ForPage(10, 25));

            Assert.Equal(10, page.Page);
            Assert.Equal(1, page.Results.Count);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnection.VerifyAll();
            mockSqlDialect.VerifyAll();
        }

        [Fact]
        public void PagedThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            var exception = Assert.Throws<ArgumentNullException>(
                () => session.Paged<Customer>(null, PagingOptions.ForPage(1, 25)));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        [Fact]
        public void PagedThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(
                () => session.Paged<Customer>(null, PagingOptions.ForPage(1, 25)));
        }

        [Fact]
        public void SingleIdentifierExecutesAndReturnsNull()
        {
            object identifier = 100;

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(false);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Select, typeof(Customer), identifier)).Returns(new SqlQuery(""));

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(mockConnection.Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsAny<SqlQuery>())).Returns(mockCommand.Object);

            IReadOnlySession session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            var customer = session.Single<Customer>(identifier);

            Assert.Null(customer);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnection.VerifyAll();
            mockSqlDialect.VerifyAll();
        }

        [Fact]
        public void SingleIdentifierExecutesAndReturnsResult()
        {
            object identifier = 100;

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Select, typeof(Customer), identifier)).Returns(new SqlQuery(""));

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(mockConnection.Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsAny<SqlQuery>())).Returns(mockCommand.Object);

            IReadOnlySession session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            var customer = session.Single<Customer>(identifier);

            Assert.NotNull(customer);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnection.VerifyAll();
            mockSqlDialect.VerifyAll();
        }

        [Fact]
        public void SingleIdentifierThrowsArgumentNullExceptionForNullIdentifier()
        {
            IReadOnlySession session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            object identifier = null;

            var exception = Assert.Throws<ArgumentNullException>(
                () => session.Single<Customer>(identifier));

            Assert.Equal("identifier", exception.ParamName);
        }

        [Fact]
        public void SingleIdentifierThrowsObjectDisposedExceptionIfDisposed()
        {
            IReadOnlySession session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(
                () => session.Single<Customer>(1));
        }

        [Fact]
        public void SingleSqlQueryExecutesAndReturnsNull()
        {
            var sqlQuery = new SqlQuery("");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(false);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();

            var mockSqlDialect = new Mock<ISqlDialect>();

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(mockConnection.Object);
            mockDbDriver.Setup(x => x.BuildCommand(sqlQuery)).Returns(mockCommand.Object);

            IReadOnlySession session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            var customer = session.Single<Customer>(sqlQuery);

            Assert.Null(customer);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnection.VerifyAll();
        }

        [Fact]
        public void SingleSqlQueryExecutesAndReturnsResult()
        {
            var sqlQuery = new SqlQuery("");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();

            var mockSqlDialect = new Mock<ISqlDialect>();

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(mockConnection.Object);
            mockDbDriver.Setup(x => x.BuildCommand(sqlQuery)).Returns(mockCommand.Object);

            IReadOnlySession session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            var customer = session.Single<Customer>(sqlQuery);

            Assert.NotNull(customer);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnection.VerifyAll();
        }

        [Fact]
        public void SingleSqlQueryThrowsArgumentNullExceptionForNullSqlQuery()
        {
            IReadOnlySession session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            SqlQuery sqlQuery = null;

            var exception = Assert.Throws<ArgumentNullException>(
                () => session.Single<Customer>(sqlQuery));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        [Fact]
        public void SingleSqlQueryThrowsObjectDisposedExceptionIfDisposed()
        {
            IReadOnlySession session = new ReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(
                () => session.Single<Customer>(new SqlQuery("")));
        }

        public class WhenCallingPagedUsingPagingOptionsNone
        {
            [Fact]
            public void AMicroLiteExceptionIsThrown()
            {
                var session = new ReadOnlySession(
                    ConnectionScope.PerTransaction,
                    new Mock<ISqlDialect>().Object,
                    new Mock<IDbDriver>().Object);

                var exception = Assert.Throws<MicroLiteException>(
                    () => session.Paged<Customer>(new SqlQuery(""), PagingOptions.None));

                Assert.Equal(Messages.Session_PagingOptionsMustNotBeNone, exception.Message);
            }
        }

        public class WhenExecutingMultipleQueriesAndTheSqlDialectUsedDoesNotSupportBatching
        {
            private Mock<IDbDriver> mockDbDriver = new Mock<IDbDriver>();

            public WhenExecutingMultipleQueriesAndTheSqlDialectUsedDoesNotSupportBatching()
            {
                var mockSqlDialect = new Mock<ISqlDialect>();
                mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Select, typeof(Customer), It.IsAny<object>())).Returns(new SqlQuery(""));

                mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
                mockDbDriver.Setup(x => x.SupportsBatchedQueries).Returns(false);
                mockDbDriver.Setup(x => x.BuildCommand(It.IsAny<SqlQuery>())).Returns(() =>
                {
                    var mockCommand = new Mock<IDbCommand>();
                    mockCommand.Setup(x => x.ExecuteReader()).Returns(() =>
                    {
                        var mockReader = new Mock<IDataReader>();
                        mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

                        return mockReader.Object;
                    });

                    return mockCommand.Object;
                });

                IReadOnlySession session = new ReadOnlySession(
                    ConnectionScope.PerTransaction,
                    mockSqlDialect.Object,
                    mockDbDriver.Object);

                var includeCustomer = session.Include.Single<Customer>(2);
                var customer = session.Single<Customer>(1);
            }

            [Fact]
            public void TheDbDriverShouldBuildTwoIDbCommands()
            {
                this.mockDbDriver.Verify(x => x.BuildCommand(It.IsAny<SqlQuery>()), Times.Exactly(2));
            }

            [Fact]
            public void TheDbDriverShouldNotCombineTheQueries()
            {
                this.mockDbDriver.Verify(x => x.Combine(It.IsAny<IEnumerable<SqlQuery>>()), Times.Never());
            }
        }

        public class WhenExecutingMultipleQueriesAndTheSqlDialectUsedSupportsBatching
        {
            private Mock<IDbDriver> mockDbDriver = new Mock<IDbDriver>();

            public WhenExecutingMultipleQueriesAndTheSqlDialectUsedSupportsBatching()
            {
                var mockReader = new Mock<IDataReader>();
                mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

                var mockCommand = new Mock<IDbCommand>();
                mockCommand.Setup(x => x.ExecuteReader()).Returns(mockReader.Object);

                var mockConnection = new Mock<IDbConnection>();
                mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

                var mockSqlDialect = new Mock<ISqlDialect>();
                mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Select, typeof(Customer), It.IsAny<object>())).Returns(new SqlQuery(""));

                mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(mockConnection.Object);
                mockDbDriver.Setup(x => x.SupportsBatchedQueries).Returns(true);
                mockDbDriver.Setup(x => x.BuildCommand(It.IsAny<SqlQuery>())).Returns(mockCommand.Object);

                IReadOnlySession session = new ReadOnlySession(
                    ConnectionScope.PerTransaction,
                    mockSqlDialect.Object,
                    mockDbDriver.Object);

                var includeCustomer = session.Include.Single<Customer>(2);
                var customer = session.Single<Customer>(1);
            }

            [Fact]
            public void TheSqlDialectShouldBuildOneIDbCommand()
            {
                this.mockDbDriver.Verify(x => x.BuildCommand(It.IsAny<SqlQuery>()), Times.Once());
            }

            [Fact]
            public void TheSqlDialectShouldCombineTheQueries()
            {
                this.mockDbDriver.Verify(x => x.Combine(It.IsAny<IEnumerable<SqlQuery>>()), Times.Once());
            }
        }
    }
}