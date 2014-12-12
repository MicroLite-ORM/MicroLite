namespace MicroLite.Tests.Core
{
#if NET_4_5

    using System;
    using System.Collections.Generic;
    using System.Data;
    using MicroLite.Builder;
    using MicroLite.Characters;
    using MicroLite.Core;
    using MicroLite.Dialect;
    using MicroLite.Driver;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="AsyncReadOnlySession"/> class.
    /// </summary>
    public class AsyncReadOnlySessionTests : UnitTest
    {
        public AsyncReadOnlySessionTests()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));
        }

        [Fact]
        public void AdvancedReturnsSameSessionByDifferentInterface()
        {
            var session = new AsyncReadOnlySession(
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
            mockReader.As<IDisposable>().Setup(x => x.Dispose());

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader(It.IsAny<CommandBehavior>())).Returns(mockReader.Object);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.SqlCharacters).Returns(SqlCharacters.Empty);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new MockDbConnectionWrapper(mockConnection.Object));
            mockDbDriver.Setup(x => x.BuildCommand(SqlBuilder.Select("*").From(typeof(Customer)).ToSqlQuery())).Returns(new MockDbCommandWrapper(mockCommand.Object));

            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            var customers = session.Include.All<Customer>();

            session.ExecutePendingQueriesAsync().Wait();

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
            mockReader.As<IDisposable>().Setup(x => x.Dispose());

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader(It.IsAny<CommandBehavior>())).Returns(mockReader.Object);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();

            var mockSqlDialect = new Mock<ISqlDialect>();

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new MockDbConnectionWrapper(mockConnection.Object));
            mockDbDriver.Setup(x => x.BuildCommand(sqlQuery)).Returns(new MockDbCommandWrapper(mockCommand.Object));

            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            var customers = session.FetchAsync<Customer>(sqlQuery).Result;

            Assert.Equal(1, customers.Count);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnection.VerifyAll();
            mockSqlDialect.VerifyAll();
        }

        [Fact]
        public void FetchThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            var exception = Assert.Throws<AggregateException>(
                () => session.FetchAsync<Customer>(null).Result);

            Assert.Equal("sqlQuery", ((ArgumentNullException)exception.InnerException).ParamName);
        }

        [Fact]
        public void FetchThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            using (session)
            {
            }

            var exception = Assert.Throws<AggregateException>(
                () => session.FetchAsync<Customer>(null).Result);

            Assert.IsType<ObjectDisposedException>(exception.InnerException);
        }

        [Fact]
        public void IncludeManyThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            SqlQuery sqlQuery = null;

            var exception = Assert.Throws<ArgumentNullException>(
                () => session.Include.Many<Customer>(sqlQuery));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        [Fact]
        public void IncludeReturnsSameSessionByDifferentInterface()
        {
            var session = new AsyncReadOnlySession(
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
            mockReader.As<IDisposable>().Setup(x => x.Dispose());

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader(It.IsAny<CommandBehavior>())).Returns(mockReader.Object);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();

            var mockSqlDialect = new Mock<ISqlDialect>();

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new MockDbConnectionWrapper(mockConnection.Object));
            mockDbDriver.Setup(x => x.BuildCommand(sqlQuery)).Returns(new MockDbCommandWrapper(mockCommand.Object));

            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            var includeScalar = session.Include.Scalar<int>(sqlQuery);

            session.ExecutePendingQueriesAsync().Wait();

            Assert.Equal(10, includeScalar.Value);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnection.VerifyAll();
            mockSqlDialect.VerifyAll();
        }

        [Fact]
        public void IncludeScalarThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            SqlQuery sqlQuery = null;

            var exception = Assert.Throws<ArgumentNullException>(
                () => session.Include.Scalar<int>(sqlQuery));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        [Fact]
        public void IncludeSingleThrowsArgumentNullExceptionForNullIdentifier()
        {
            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            object identifier = null;

            var exception = Assert.Throws<ArgumentNullException>(
                () => session.Include.Single<Customer>(identifier));

            Assert.Equal("identifier", exception.ParamName);
        }

        [Fact]
        public void IncludeSingleThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            SqlQuery sqlQuery = null;

            var exception = Assert.Throws<ArgumentNullException>(
                () => session.Include.Single<Customer>(sqlQuery));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        [Fact]
        public void MicroLiteExceptionsCaughtByExecutePendingQueriesShouldNotBeWrappedInAnotherMicroLiteException()
        {
            var mockSqlDialect = new Mock<ISqlDialect>();

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Throws<MicroLiteException>();

            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            // We need at least 1 queued query otherwise we will get an exception when doing queries.Dequeue() instead.
            session.Include.Scalar<int>(new SqlQuery(""));

            var exception = Assert.Throws<AggregateException>(
                () => session.ExecutePendingQueriesAsync().Wait());

            Assert.IsType<MicroLiteException>(exception.InnerException);
            Assert.IsNotType<MicroLiteException>(exception.InnerException.InnerException);
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
            mockReader.As<IDisposable>().Setup(x => x.Dispose());

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader(It.IsAny<CommandBehavior>())).Returns(mockReader.Object);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CountQuery(sqlQuery)).Returns(countQuery);
            mockSqlDialect.Setup(x => x.PageQuery(sqlQuery, PagingOptions.ForPage(1, 1))).Returns(pagedQuery);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new MockDbConnectionWrapper(mockConnection.Object));
            mockDbDriver.Setup(x => x.SupportsBatchedQueries).Returns(true);
            mockDbDriver.Setup(x => x.Combine(countQuery, pagedQuery)).Returns(combinedQuery);
            mockDbDriver.Setup(x => x.BuildCommand(combinedQuery)).Returns(new MockDbCommandWrapper(mockCommand.Object));

            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            var page = session.PagedAsync<Customer>(sqlQuery, PagingOptions.ForPage(1, 1)).Result;

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
            mockReader.As<IDisposable>().Setup(x => x.Dispose());

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader(It.IsAny<CommandBehavior>())).Returns(mockReader.Object);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CountQuery(sqlQuery)).Returns(countQuery);
            mockSqlDialect.Setup(x => x.PageQuery(sqlQuery, PagingOptions.ForPage(1, 25))).Returns(pagedQuery);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new MockDbConnectionWrapper(mockConnection.Object));
            mockDbDriver.Setup(x => x.SupportsBatchedQueries).Returns(true);
            mockDbDriver.Setup(x => x.Combine(countQuery, pagedQuery)).Returns(combinedQuery);
            mockDbDriver.Setup(x => x.BuildCommand(combinedQuery)).Returns(new MockDbCommandWrapper(mockCommand.Object));

            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            var page = session.PagedAsync<Customer>(sqlQuery, PagingOptions.ForPage(1, 25)).Result;

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
            mockReader.As<IDisposable>().Setup(x => x.Dispose());

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader(It.IsAny<CommandBehavior>())).Returns(mockReader.Object);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CountQuery(sqlQuery)).Returns(countQuery);
            mockSqlDialect.Setup(x => x.PageQuery(sqlQuery, PagingOptions.ForPage(10, 25))).Returns(pagedQuery);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new MockDbConnectionWrapper(mockConnection.Object));
            mockDbDriver.Setup(x => x.SupportsBatchedQueries).Returns(true);
            mockDbDriver.Setup(x => x.Combine(countQuery, pagedQuery)).Returns(combinedQuery);
            mockDbDriver.Setup(x => x.BuildCommand(combinedQuery)).Returns(new MockDbCommandWrapper(mockCommand.Object));

            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            var page = session.PagedAsync<Customer>(sqlQuery, PagingOptions.ForPage(10, 25)).Result;

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
            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            var exception = Assert.Throws<AggregateException>(
                () => session.PagedAsync<Customer>(null, PagingOptions.ForPage(1, 25)).Result);

            Assert.Equal("sqlQuery", ((ArgumentNullException)exception.InnerException).ParamName);
        }

        [Fact]
        public void PagedThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            using (session)
            {
            }

            var exception = Assert.Throws<AggregateException>(
                () => session.PagedAsync<Customer>(null, PagingOptions.ForPage(1, 25)).Result);

            Assert.IsType<ObjectDisposedException>(exception.InnerException);
        }

        [Fact]
        public void SingleIdentifierExecutesAndReturnsNull()
        {
            object identifier = 100;

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(false);
            mockReader.As<IDisposable>().Setup(x => x.Dispose());

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader(It.IsAny<CommandBehavior>())).Returns(mockReader.Object);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildSelectSqlQuery(It.IsNotNull<IObjectInfo>(), identifier)).Returns(new SqlQuery(""));

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new MockDbConnectionWrapper(mockConnection.Object));
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(new MockDbCommandWrapper(mockCommand.Object));

            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            var customer = session.SingleAsync<Customer>(identifier).Result;

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
            mockReader.As<IDisposable>().Setup(x => x.Dispose());

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader(It.IsAny<CommandBehavior>())).Returns(mockReader.Object);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildSelectSqlQuery(It.IsNotNull<IObjectInfo>(), identifier)).Returns(new SqlQuery(""));

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new MockDbConnectionWrapper(mockConnection.Object));
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(new MockDbCommandWrapper(mockCommand.Object));

            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            var customer = session.SingleAsync<Customer>(identifier).Result;

            Assert.NotNull(customer);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnection.VerifyAll();
            mockSqlDialect.VerifyAll();
        }

        [Fact]
        public void SingleIdentifierThrowsArgumentNullExceptionForNullIdentifier()
        {
            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            object identifier = null;

            var exception = Assert.Throws<AggregateException>(
                () => session.SingleAsync<Customer>(identifier).Result);

            Assert.Equal("identifier", ((ArgumentNullException)exception.InnerException).ParamName);
        }

        [Fact]
        public void SingleIdentifierThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            using (session)
            {
            }

            var exception = Assert.Throws<AggregateException>(
                () => session.SingleAsync<Customer>(1).Result);

            Assert.IsType<ObjectDisposedException>(exception.InnerException);
        }

        [Fact]
        public void SingleSqlQueryExecutesAndReturnsNull()
        {
            var sqlQuery = new SqlQuery("");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(false);
            mockReader.As<IDisposable>().Setup(x => x.Dispose());

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader(It.IsAny<CommandBehavior>())).Returns(mockReader.Object);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();

            var mockSqlDialect = new Mock<ISqlDialect>();

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new MockDbConnectionWrapper(mockConnection.Object));
            mockDbDriver.Setup(x => x.BuildCommand(sqlQuery)).Returns(new MockDbCommandWrapper(mockCommand.Object));

            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            var customer = session.SingleAsync<Customer>(sqlQuery).Result;

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
            mockReader.As<IDisposable>().Setup(x => x.Dispose());

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader(It.IsAny<CommandBehavior>())).Returns(mockReader.Object);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();

            var mockSqlDialect = new Mock<ISqlDialect>();

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new MockDbConnectionWrapper(mockConnection.Object));
            mockDbDriver.Setup(x => x.BuildCommand(sqlQuery)).Returns(new MockDbCommandWrapper(mockCommand.Object));

            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object);

            var customer = session.SingleAsync<Customer>(sqlQuery).Result;

            Assert.NotNull(customer);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnection.VerifyAll();
        }

        [Fact]
        public void SingleSqlQueryThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            SqlQuery sqlQuery = null;

            var exception = Assert.Throws<AggregateException>(
                () => session.SingleAsync<Customer>(sqlQuery).Result);

            Assert.Equal("sqlQuery", ((ArgumentNullException)exception.InnerException).ParamName);
        }

        [Fact]
        public void SingleSqlQueryThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new AsyncReadOnlySession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object);

            using (session)
            {
            }

            var exception = Assert.Throws<AggregateException>(
                () => session.SingleAsync<Customer>(new SqlQuery("")).Result);

            Assert.IsType<ObjectDisposedException>(exception.InnerException);
        }

        public class WhenCallingPagedUsingPagingOptionsNone
        {
            [Fact]
            public void AMicroLiteExceptionIsThrown()
            {
                var session = new AsyncReadOnlySession(
                    ConnectionScope.PerTransaction,
                    new Mock<ISqlDialect>().Object,
                    new Mock<IDbDriver>().Object);

                var exception = Assert.Throws<AggregateException>(
                    () => session.PagedAsync<Customer>(new SqlQuery(""), PagingOptions.None).Result);

                Assert.IsType<MicroLiteException>(exception.InnerException);
                Assert.Equal(ExceptionMessages.Session_PagingOptionsMustNotBeNone, exception.InnerException.Message);
            }
        }

        public class WhenExecutingMultipleQueriesAndTheSqlDialectUsedDoesNotSupportBatching
        {
            private Mock<IDbDriver> mockDbDriver = new Mock<IDbDriver>();

            public WhenExecutingMultipleQueriesAndTheSqlDialectUsedDoesNotSupportBatching()
            {
                var mockSqlDialect = new Mock<ISqlDialect>();
                mockSqlDialect.Setup(x => x.BuildSelectSqlQuery(It.IsNotNull<IObjectInfo>(), It.IsNotNull<object>())).Returns(new SqlQuery(""));

                mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new MockDbConnectionWrapper(new Mock<IDbConnection>().Object));
                mockDbDriver.Setup(x => x.SupportsBatchedQueries).Returns(false);
                mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(() =>
                {
                    var mockCommand = new Mock<IDbCommand>();
                    mockCommand.Setup(x => x.ExecuteReader(It.IsAny<CommandBehavior>())).Returns(() =>
                    {
                        var mockReader = new Mock<IDataReader>();
                        mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

                        return mockReader.Object;
                    });

                    return new MockDbCommandWrapper(mockCommand.Object);
                });

                var session = new AsyncReadOnlySession(
                    ConnectionScope.PerTransaction,
                    mockSqlDialect.Object,
                    mockDbDriver.Object);

                var includeCustomer = session.Include.Single<Customer>(2);
                var customer = session.SingleAsync<Customer>(1).Result;
            }

            [Fact]
            public void TheDbDriverShouldBuildTwoIDbCommands()
            {
                this.mockDbDriver.Verify(x => x.BuildCommand(It.IsNotNull<SqlQuery>()), Times.Exactly(2));
            }

            [Fact]
            public void TheDbDriverShouldNotCombineTheQueriesUsingTheIEnumerableOverload()
            {
                this.mockDbDriver.Verify(x => x.Combine(It.IsAny<IEnumerable<SqlQuery>>()), Times.Never());
            }

            [Fact]
            public void TheDbDriverShouldNotCombineTheQueriesUsingTheNonIEnumerableOverload()
            {
                this.mockDbDriver.Verify(x => x.Combine(It.IsAny<SqlQuery>(), It.IsAny<SqlQuery>()), Times.Never());
            }
        }

        public class WhenExecutingMultipleQueriesAndTheSqlDialectUsedSupportsBatching
        {
            private Mock<IDbDriver> mockDbDriver = new Mock<IDbDriver>();

            public WhenExecutingMultipleQueriesAndTheSqlDialectUsedSupportsBatching()
            {
                this.mockDbDriver.Setup(x => x.Combine(It.IsNotNull<IEnumerable<SqlQuery>>())).Returns(new SqlQuery(""));

                var mockReader = new Mock<IDataReader>();
                mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false, true, false }).Dequeue);

                var mockCommand = new Mock<IDbCommand>();
                mockCommand.Setup(x => x.ExecuteReader(It.IsAny<CommandBehavior>())).Returns(mockReader.Object);

                var mockConnection = new Mock<IDbConnection>();
                mockConnection.Setup(x => x.CreateCommand()).Returns(new MockDbCommandWrapper(mockCommand.Object));

                var mockSqlDialect = new Mock<ISqlDialect>();
                mockSqlDialect.Setup(x => x.BuildSelectSqlQuery(It.IsNotNull<IObjectInfo>(), It.IsNotNull<object>())).Returns(new SqlQuery(""));

                mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new MockDbConnectionWrapper(mockConnection.Object));
                mockDbDriver.Setup(x => x.SupportsBatchedQueries).Returns(true);
                mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(new MockDbCommandWrapper(mockCommand.Object));

                var session = new AsyncReadOnlySession(
                    ConnectionScope.PerTransaction,
                    mockSqlDialect.Object,
                    mockDbDriver.Object);

                var includeCustomerA = session.Include.Single<Customer>(3);
                var includeCustomerB = session.Include.Single<Customer>(2);
                var customer = session.SingleAsync<Customer>(1).Result;
            }

            [Fact]
            public void TheDbDriverShouldNotCombineTheQueriesUsingTheNonIEnumerableOverload()
            {
                this.mockDbDriver.Verify(x => x.Combine(It.IsAny<SqlQuery>(), It.IsAny<SqlQuery>()), Times.Never());
            }

            [Fact]
            public void TheSqlDialectShouldBuildOneIDbCommand()
            {
                this.mockDbDriver.Verify(x => x.BuildCommand(It.IsNotNull<SqlQuery>()), Times.Once());
            }

            [Fact]
            public void TheSqlDialectShouldCombineTheQueriesUsingTheIEnumerableOverload()
            {
                this.mockDbDriver.Verify(x => x.Combine(It.IsNotNull<IEnumerable<SqlQuery>>()), Times.Once());
            }
        }

        public class WhenExecutingTwoQueriesAndTheSqlDialectUsedSupportsBatching
        {
            private Mock<IDbDriver> mockDbDriver = new Mock<IDbDriver>();

            public WhenExecutingTwoQueriesAndTheSqlDialectUsedSupportsBatching()
            {
                this.mockDbDriver.Setup(x => x.Combine(It.IsNotNull<SqlQuery>(), It.IsNotNull<SqlQuery>())).Returns(new SqlQuery(""));

                var mockReader = new Mock<IDataReader>();
                mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

                var mockCommand = new Mock<IDbCommand>();
                mockCommand.Setup(x => x.ExecuteReader(It.IsAny<CommandBehavior>())).Returns(mockReader.Object);

                var mockConnection = new Mock<IDbConnection>();
                mockConnection.Setup(x => x.CreateCommand()).Returns(new MockDbCommandWrapper(mockCommand.Object));

                var mockSqlDialect = new Mock<ISqlDialect>();
                mockSqlDialect.Setup(x => x.BuildSelectSqlQuery(It.IsNotNull<IObjectInfo>(), It.IsNotNull<object>())).Returns(new SqlQuery(""));

                mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new MockDbConnectionWrapper(mockConnection.Object));
                mockDbDriver.Setup(x => x.SupportsBatchedQueries).Returns(true);
                mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(new MockDbCommandWrapper(mockCommand.Object));

                var session = new AsyncReadOnlySession(
                    ConnectionScope.PerTransaction,
                    mockSqlDialect.Object,
                    mockDbDriver.Object);

                var includeCustomer = session.Include.Single<Customer>(2);
                var customer = session.SingleAsync<Customer>(1).Result;
            }

            [Fact]
            public void TheDbDriverShouldCombineTheQueriesUsingTheNonIEnumerableOverload()
            {
                this.mockDbDriver.Verify(x => x.Combine(It.IsNotNull<SqlQuery>(), It.IsNotNull<SqlQuery>()), Times.Once());
            }

            [Fact]
            public void TheSqlDialectShouldBuildOneIDbCommand()
            {
                this.mockDbDriver.Verify(x => x.BuildCommand(It.IsNotNull<SqlQuery>()), Times.Once());
            }

            [Fact]
            public void TheSqlDialectShouldNotCombineTheQueriesUsingTheIEnumerableOverload()
            {
                this.mockDbDriver.Verify(x => x.Combine(It.IsAny<IEnumerable<SqlQuery>>()), Times.Never());
            }
        }
    }

#endif
}