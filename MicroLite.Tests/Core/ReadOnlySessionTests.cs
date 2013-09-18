namespace MicroLite.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Dynamic;
    using System.Linq;
    using MicroLite.Core;
    using MicroLite.Dialect;
    using MicroLite.Mapping;
    using MicroLite.Query;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ReadOnlySession"/> class.
    /// </summary>
    public class ReadOnlySessionTests : IDisposable
    {
        public ReadOnlySessionTests()
        {
            SqlBuilder.SqlCharacters = null;
        }

        [Fact]
        public void AdvancedReturnsSameSessionByDifferentInterface()
        {
            var session = new ReadOnlySession(
                new Mock<ISessionFactory>().Object,
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object);

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

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.SqlCharacters).Returns(SqlCharacters.Empty);
            mockSqlDialect.Setup(x => x.BuildCommand(mockCommand.Object, SqlBuilder.Select("*").From(typeof(Customer)).ToSqlQuery()));

            var mockSessionFactory = new Mock<ISessionFactory>();
            mockSessionFactory.Setup(x => x.SqlDialect).Returns(mockSqlDialect.Object);

            var session = new ReadOnlySession(
                mockSessionFactory.Object,
                mockConnectionManager.Object,
                mockObjectBuilder.Object);

            var customers = session.All<Customer>();

            session.ExecutePendingQueries();

            Assert.Equal(1, customers.Values.Count);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockObjectBuilder.VerifyAll();
            mockSqlDialect.VerifyAll();
        }

        [Fact]
        public void BeginTransactionCallsConnectionManagerBeginTransactionWithReadCommitted()
        {
            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.BeginTransaction(IsolationLevel.ReadCommitted));

            var session = new ReadOnlySession(
                new Mock<ISessionFactory>().Object,
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object);

            session.BeginTransaction();

            mockConnectionManager.VerifyAll();
        }

        [Fact]
        public void BeginTransactionWithIsolationLevelCallsConnectionManagerBeginTransactionWithIsolationLevel()
        {
            var isolationLevel = IsolationLevel.Chaos;

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.BeginTransaction(isolationLevel));

            var session = new ReadOnlySession(
                new Mock<ISessionFactory>().Object,
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object);

            session.BeginTransaction(isolationLevel);

            mockConnectionManager.VerifyAll();
        }

        [Fact]
        public void BeginTransactionWithIsolationLevelThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new ReadOnlySession(
                new Mock<ISessionFactory>().Object,
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.BeginTransaction(IsolationLevel.ReadCommitted));
        }

        public void Dispose()
        {
            SqlBuilder.SqlCharacters = null;
        }

        [Fact]
        public void DisposeDisposesConnectionManager()
        {
            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.Dispose());

            var session = new ReadOnlySession(
                new Mock<ISessionFactory>().Object,
            mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object);

            using (session)
            {
            }

            mockConnectionManager.VerifyAll();
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

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildCommand(mockCommand.Object, sqlQuery));

            var mockSessionFactory = new Mock<ISessionFactory>();
            mockSessionFactory.Setup(x => x.SqlDialect).Returns(mockSqlDialect.Object);

            var session = new ReadOnlySession(
                mockSessionFactory.Object,
                mockConnectionManager.Object,
                mockObjectBuilder.Object);

            var customers = session.Fetch<Customer>(sqlQuery);

            Assert.Equal(1, customers.Count);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockObjectBuilder.VerifyAll();
            mockSqlDialect.VerifyAll();
        }

        [Fact]
        public void FetchThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new ReadOnlySession(
                new Mock<ISessionFactory>().Object,
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Fetch<Customer>(null));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        [Fact]
        public void FetchThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new ReadOnlySession(
                new Mock<ISessionFactory>().Object,
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Fetch<Customer>(null));
        }

        [Fact]
        public void IncludeReturnsSameSessionByDifferentInterface()
        {
            var session = new ReadOnlySession(
                new Mock<ISessionFactory>().Object,
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object);

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

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildCommand(mockCommand.Object, sqlQuery));

            var mockSessionFactory = new Mock<ISessionFactory>();
            mockSessionFactory.Setup(x => x.SqlDialect).Returns(mockSqlDialect.Object);

            var session = new ReadOnlySession(
                mockSessionFactory.Object,
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object);

            var includeScalar = session.Include.Scalar<int>(sqlQuery);

            session.ExecutePendingQueries();

            Assert.Equal(10, includeScalar.Value);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockSqlDialect.VerifyAll();
        }

        [Fact]
        public void IncludeScalarThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new ReadOnlySession(
                new Mock<ISessionFactory>().Object,
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object);

            SqlQuery sqlQuery = null;

            var exception = Assert.Throws<ArgumentNullException>(() => session.Include.Scalar<int>(sqlQuery));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        [Fact]
        public void IncludeScalarThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new ReadOnlySession(
                new Mock<ISessionFactory>().Object,
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Include.Scalar<int>(new SqlQuery("")));
        }

        [Fact]
        public void MicroLiteExceptionsCaughtByExecutePendingQueriesShouldNotBeWrappedInAnotherMicroLiteException()
        {
            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildCommand(It.IsAny<IDbCommand>(), It.IsAny<SqlQuery>())).Throws<MicroLiteException>();

            var mockSessionFactory = new Mock<ISessionFactory>();
            mockSessionFactory.Setup(x => x.SqlDialect).Returns(mockSqlDialect.Object);

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(new Mock<IDbCommand>().Object);

            var session = new ReadOnlySession(
                mockSessionFactory.Object,
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object);

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
            var combinedQuery = new SqlQuery("SELECT COUNT(*) FROM TABLE;SELECT * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM Customers) AS Customers");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(1);
            mockReader.Setup(x => x[0]).Returns(1000); // Simulate 1000 records in the count query
            mockReader.Setup(x => x.NextResult()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false, true, false }).Dequeue);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.SupportsBatchedQueries).Returns(true);
            mockSqlDialect.Setup(x => x.CountQuery(sqlQuery)).Returns(countQuery);
            mockSqlDialect.Setup(x => x.PageQuery(sqlQuery, PagingOptions.ForPage(1, 1))).Returns(pagedQuery);
            mockSqlDialect.Setup(x => x.Combine(It.Is<IEnumerable<SqlQuery>>(c => c.Contains(countQuery) && c.Contains(pagedQuery)))).Returns(combinedQuery);
            mockSqlDialect.Setup(x => x.BuildCommand(mockCommand.Object, combinedQuery));

            var mockSessionFactory = new Mock<ISessionFactory>();
            mockSessionFactory.Setup(x => x.SqlDialect).Returns(mockSqlDialect.Object);

            var session = new ReadOnlySession(
                mockSessionFactory.Object,
                mockConnectionManager.Object,
                mockObjectBuilder.Object);

            var page = session.Paged<Customer>(sqlQuery, PagingOptions.ForPage(1, 1));

            Assert.Equal(1, page.Page);
            Assert.Equal(1, page.Results.Count);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockObjectBuilder.VerifyAll();
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
            mockReader.Setup(x => x.FieldCount).Returns(1);
            mockReader.Setup(x => x[0]).Returns(1000); // Simulate 1000 records in the count query
            mockReader.Setup(x => x.NextResult()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false, true, false }).Dequeue);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.SupportsBatchedQueries).Returns(true);
            mockSqlDialect.Setup(x => x.CountQuery(sqlQuery)).Returns(countQuery);
            mockSqlDialect.Setup(x => x.PageQuery(sqlQuery, PagingOptions.ForPage(1, 25))).Returns(pagedQuery);
            mockSqlDialect.Setup(x => x.Combine(It.Is<IEnumerable<SqlQuery>>(c => c.Contains(countQuery) && c.Contains(pagedQuery)))).Returns(combinedQuery);
            mockSqlDialect.Setup(x => x.BuildCommand(mockCommand.Object, combinedQuery));

            var mockSessionFactory = new Mock<ISessionFactory>();
            mockSessionFactory.Setup(x => x.SqlDialect).Returns(mockSqlDialect.Object);

            var session = new ReadOnlySession(
                mockSessionFactory.Object,
                mockConnectionManager.Object,
                mockObjectBuilder.Object);

            var page = session.Paged<Customer>(sqlQuery, PagingOptions.ForPage(1, 25));

            Assert.Equal(1, page.Page);
            Assert.Equal(1, page.Results.Count);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockObjectBuilder.VerifyAll();
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
            mockReader.Setup(x => x.FieldCount).Returns(1);
            mockReader.Setup(x => x[0]).Returns(1000); // Simulate 1000 records in the count query
            mockReader.Setup(x => x.NextResult()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false, true, false }).Dequeue);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.SupportsBatchedQueries).Returns(true);
            mockSqlDialect.Setup(x => x.CountQuery(sqlQuery)).Returns(countQuery);
            mockSqlDialect.Setup(x => x.PageQuery(sqlQuery, PagingOptions.ForPage(10, 25))).Returns(pagedQuery);
            mockSqlDialect.Setup(x => x.Combine(It.Is<IEnumerable<SqlQuery>>(c => c.Contains(countQuery) && c.Contains(pagedQuery)))).Returns(combinedQuery);
            mockSqlDialect.Setup(x => x.BuildCommand(mockCommand.Object, combinedQuery));

            var mockSessionFactory = new Mock<ISessionFactory>();
            mockSessionFactory.Setup(x => x.SqlDialect).Returns(mockSqlDialect.Object);

            var session = new ReadOnlySession(
                mockSessionFactory.Object,
                mockConnectionManager.Object,
                mockObjectBuilder.Object);

            var page = session.Paged<Customer>(sqlQuery, PagingOptions.ForPage(10, 25));

            Assert.Equal(10, page.Page);
            Assert.Equal(1, page.Results.Count);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockObjectBuilder.VerifyAll();
            mockSqlDialect.VerifyAll();
        }

        [Fact]
        public void PagedThrowsArgumentNullExceptionForNullSqlQuery()
        {
            IReadOnlySession session = new ReadOnlySession(
                new Mock<ISessionFactory>().Object,
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Paged<Customer>(null, PagingOptions.ForPage(1, 25)));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        [Fact]
        public void PagedThrowsObjectDisposedExceptionIfDisposed()
        {
            IReadOnlySession session = new ReadOnlySession(
                new Mock<ISessionFactory>().Object,
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Paged<Customer>(null, PagingOptions.ForPage(1, 25)));
        }

        [Fact]
        public void ProjectionExecutesAndReturnsResults()
        {
            var sqlQuery = new SqlQuery("");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildInstance<dynamic>(It.IsAny<IObjectInfo>(), reader)).Returns(new ExpandoObject());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildCommand(mockCommand.Object, sqlQuery));

            var mockSessionFactory = new Mock<ISessionFactory>();
            mockSessionFactory.Setup(x => x.SqlDialect).Returns(mockSqlDialect.Object);

            var session = new ReadOnlySession(
                mockSessionFactory.Object,
                mockConnectionManager.Object,
                mockObjectBuilder.Object);

            var results = session.Projection(sqlQuery);

            Assert.Equal(1, results.Count);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockObjectBuilder.VerifyAll();
            mockSqlDialect.VerifyAll();
        }

        [Fact]
        public void ProjectionThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new ReadOnlySession(
                new Mock<ISessionFactory>().Object,
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Projection(null));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        [Fact]
        public void ProjectionThrowsMicroLiteExceptionIfExecuteReaderThrowsException()
        {
            var sqlQuery = new SqlQuery("");

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteReader()).Throws<InvalidOperationException>();
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockSessionFactory = new Mock<ISessionFactory>();
            mockSessionFactory.Setup(x => x.SqlDialect).Returns(new Mock<ISqlDialect>().Object);

            var session = new ReadOnlySession(
                mockSessionFactory.Object,
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object);

            var exception = Assert.Throws<MicroLiteException>(() => session.Projection(sqlQuery));

            Assert.NotNull(exception.InnerException);
            Assert.Equal(exception.InnerException.Message, exception.Message);

            // Command should still be disposed.
            mockCommand.Verify(x => x.Dispose());
        }

        [Fact]
        public void ProjectionThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new ReadOnlySession(
                new Mock<ISessionFactory>().Object,
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Projection(null));
        }

        [Fact]
        public void SessionFactoryPassedToConstructorIsExposed()
        {
            var mockSessionFactory = new Mock<ISessionFactory>();

            var sessionFactory = mockSessionFactory.Object;

            var session = new ReadOnlySession(
                sessionFactory,
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object);

            Assert.Same(sessionFactory, session.Advanced.SessionFactory);
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

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.SqlCharacters).Returns(SqlCharacters.Empty);
            mockSqlDialect.Setup(x => x.BuildCommand(mockCommand.Object, It.Is<SqlQuery>(s => s.Arguments[0] == identifier)));

            var mockSessionFactory = new Mock<ISessionFactory>();
            mockSessionFactory.Setup(x => x.SqlDialect).Returns(mockSqlDialect.Object);

            IReadOnlySession session = new ReadOnlySession(
                mockSessionFactory.Object,
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object);

            var customer = session.Single<Customer>(identifier);

            Assert.Null(customer);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
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

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.SqlCharacters).Returns(SqlCharacters.Empty);
            mockSqlDialect.Setup(x => x.BuildCommand(mockCommand.Object, It.Is<SqlQuery>(s => s.Arguments[0] == identifier)));

            var mockSessionFactory = new Mock<ISessionFactory>();
            mockSessionFactory.Setup(x => x.SqlDialect).Returns(mockSqlDialect.Object);

            IReadOnlySession session = new ReadOnlySession(
                mockSessionFactory.Object,
                mockConnectionManager.Object,
                mockObjectBuilder.Object);

            var customer = session.Single<Customer>(identifier);

            Assert.NotNull(customer);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockObjectBuilder.VerifyAll();
            mockSqlDialect.VerifyAll();
        }

        [Fact]
        public void SingleIdentifierThrowsArgumentNullExceptionForNullIdentifier()
        {
            IReadOnlySession session = new ReadOnlySession(
                new Mock<ISessionFactory>().Object,
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object);

            object identifier = null;

            var exception = Assert.Throws<ArgumentNullException>(() => session.Single<Customer>(identifier));

            Assert.Equal("identifier", exception.ParamName);
        }

        [Fact]
        public void SingleIdentifierThrowsObjectDisposedExceptionIfDisposed()
        {
            IReadOnlySession session = new ReadOnlySession(
                new Mock<ISessionFactory>().Object,
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Single<Customer>(1));
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

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildCommand(mockCommand.Object, sqlQuery));

            var mockSessionFactory = new Mock<ISessionFactory>();
            mockSessionFactory.Setup(x => x.SqlDialect).Returns(mockSqlDialect.Object);

            IReadOnlySession session = new ReadOnlySession(
                mockSessionFactory.Object,
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object);

            var customer = session.Single<Customer>(sqlQuery);

            Assert.Null(customer);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
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

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildCommand(mockCommand.Object, sqlQuery));

            var mockSessionFactory = new Mock<ISessionFactory>();
            mockSessionFactory.Setup(x => x.SqlDialect).Returns(mockSqlDialect.Object);

            IReadOnlySession session = new ReadOnlySession(
                mockSessionFactory.Object,
                mockConnectionManager.Object,
                mockObjectBuilder.Object);

            var customer = session.Single<Customer>(sqlQuery);

            Assert.NotNull(customer);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockObjectBuilder.VerifyAll();
        }

        [Fact]
        public void SingleSqlQueryThrowsArgumentNullExceptionForNullSqlQuery()
        {
            IReadOnlySession session = new ReadOnlySession(
                new Mock<ISessionFactory>().Object,
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object);

            SqlQuery sqlQuery = null;

            var exception = Assert.Throws<ArgumentNullException>(() => session.Single<Customer>(sqlQuery));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        [Fact]
        public void SingleSqlQueryThrowsObjectDisposedExceptionIfDisposed()
        {
            IReadOnlySession session = new ReadOnlySession(
                new Mock<ISessionFactory>().Object,
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Single<Customer>(new SqlQuery("")));
        }

        [Fact]
        public void TransactionReturnsConnectionManagerCurrentTransaction()
        {
            var transaction = new Mock<ITransaction>().Object;

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CurrentTransaction).Returns(transaction);

            var session = new ReadOnlySession(
                new Mock<ISessionFactory>().Object,
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object);

            Assert.Same(transaction, session.Transaction);
        }

        public class WhenCallingPagedUsingPagingOptionsNone
        {
            [Fact]
            public void AMicroLiteExceptionIsThrown()
            {
                var session = new ReadOnlySession(
                new Mock<ISessionFactory>().Object,
                    new Mock<IConnectionManager>().Object,
                    new Mock<IObjectBuilder>().Object);

                var exception = Assert.Throws<MicroLiteException>(() => session.Paged<Customer>(new SqlQuery(""), PagingOptions.None));

                Assert.Equal(Messages.Session_PagingOptionsMustNotBeNone, exception.Message);
            }
        }

        public class WhenExecutingMultipleQueriesAndTheSqlDialectUsedDoesNotSupportBatching
        {
            private Mock<ISqlDialect> mockSqlDialect = new Mock<ISqlDialect>();

            public WhenExecutingMultipleQueriesAndTheSqlDialectUsedDoesNotSupportBatching()
            {
                var mockConnectionManager = new Mock<IConnectionManager>();
                mockConnectionManager.Setup(x => x.CreateCommand()).Returns(() =>
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

                var mockObjectBuilder = new Mock<IObjectBuilder>();
                mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), It.IsAny<IDataReader>())).Returns(new Customer());

                mockSqlDialect.Setup(x => x.SupportsBatchedQueries).Returns(false);
                mockSqlDialect.Setup(x => x.SqlCharacters).Returns(SqlCharacters.Empty);
                mockSqlDialect.Setup(x => x.BuildCommand(It.IsAny<IDbCommand>(), It.Is<SqlQuery>(s => s.Arguments[0] == (object)1)));
                mockSqlDialect.Setup(x => x.BuildCommand(It.IsAny<IDbCommand>(), It.Is<SqlQuery>(s => s.Arguments[0] == (object)2)));

                var mockSessionFactory = new Mock<ISessionFactory>();
                mockSessionFactory.Setup(x => x.SqlDialect).Returns(mockSqlDialect.Object);

                IReadOnlySession session = new ReadOnlySession(
                    mockSessionFactory.Object,
                    mockConnectionManager.Object,
                    mockObjectBuilder.Object);

                var includeCustomer = session.Include.Single<Customer>(2);
                var customer = session.Single<Customer>(1);
            }

            [Fact]
            public void TheSqlDialectShouldBuildTwoIDbCommands()
            {
                this.mockSqlDialect.Verify(x => x.BuildCommand(It.IsAny<IDbCommand>(), It.IsAny<SqlQuery>()), Times.Exactly(2));
            }

            [Fact]
            public void TheSqlDialectShouldNotCombineTheQueries()
            {
                this.mockSqlDialect.Verify(x => x.Combine(It.IsAny<IEnumerable<SqlQuery>>()), Times.Never());
            }
        }

        public class WhenExecutingMultipleQueriesAndTheSqlDialectUsedSupportsBatching
        {
            private Mock<ISqlDialect> mockSqlDialect = new Mock<ISqlDialect>();

            public WhenExecutingMultipleQueriesAndTheSqlDialectUsedSupportsBatching()
            {
                var mockReader = new Mock<IDataReader>();
                mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);

                var mockCommand = new Mock<IDbCommand>();
                mockCommand.Setup(x => x.ExecuteReader()).Returns(mockReader.Object);

                var mockConnectionManager = new Mock<IConnectionManager>();
                mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

                var mockObjectBuilder = new Mock<IObjectBuilder>();
                mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), It.IsAny<IDataReader>())).Returns(new Customer());

                mockSqlDialect.Setup(x => x.SupportsBatchedQueries).Returns(true);
                mockSqlDialect.Setup(x => x.SqlCharacters).Returns(SqlCharacters.Empty);
                mockSqlDialect.Setup(x => x.BuildCommand(It.IsAny<IDbCommand>(), It.Is<SqlQuery>(s => s.Arguments[0] == (object)2 && s.Arguments[1] == (object)1)));
                mockSqlDialect.Setup(x => x.Combine(It.IsAny<IEnumerable<SqlQuery>>())).Returns(new SqlQuery("", 2, 1));

                var mockSessionFactory = new Mock<ISessionFactory>();
                mockSessionFactory.Setup(x => x.SqlDialect).Returns(mockSqlDialect.Object);

                IReadOnlySession session = new ReadOnlySession(
                    mockSessionFactory.Object,
                    mockConnectionManager.Object,
                    mockObjectBuilder.Object);

                var includeCustomer = session.Include.Single<Customer>(2);
                var customer = session.Single<Customer>(1);
            }

            [Fact]
            public void TheSqlDialectShouldBuildOneIDbCommand()
            {
                this.mockSqlDialect.Verify(x => x.BuildCommand(It.IsAny<IDbCommand>(), It.IsAny<SqlQuery>()), Times.Once());
            }

            [Fact]
            public void TheSqlDialectShouldCombineTheQueries()
            {
                this.mockSqlDialect.Verify(x => x.Combine(It.IsAny<IEnumerable<SqlQuery>>()), Times.Once());
            }
        }

        [MicroLite.Mapping.Table("dbo", "Customers")]
        private class Customer
        {
            [MicroLite.Mapping.Column("CustomerId")]
            [MicroLite.Mapping.Identifier(MicroLite.Mapping.IdentifierStrategy.DbGenerated)]
            public int Id
            {
                get;
                set;
            }
        }
    }
}