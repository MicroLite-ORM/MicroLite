namespace MicroLite.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Dynamic;
    using MicroLite.Core;
    using MicroLite.Dialect;
    using MicroLite.Mapping;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="ReadOnlySession"/> class.
    /// </summary>
    [TestFixture]
    public class ReadOnlySessionTests
    {
        [Test]
        public void BeginTransactionCallsConnectionManagerBeginTransaction()
        {
            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.BeginTransaction());

            var session = new ReadOnlySession(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            session.BeginTransaction();

            mockConnectionManager.VerifyAll();
        }

        [Test]
        public void BeginTransactionThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new ReadOnlySession(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.BeginTransaction());
        }

        [Test]
        public void BeginTransactionWithIsolationLevelCallsConnectionManagerBeginTransactionWithIsolationLevel()
        {
            var isolationLevel = IsolationLevel.Chaos;

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.BeginTransaction(isolationLevel));

            var session = new ReadOnlySession(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            session.BeginTransaction(isolationLevel);

            mockConnectionManager.VerifyAll();
        }

        [Test]
        public void BeginTransactionWithIsolationLevelThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new ReadOnlySession(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.BeginTransaction(IsolationLevel.ReadCommitted));
        }

        [Test]
        public void DisposeDisposesConnectionManager()
        {
            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.Dispose());

            var session = new ReadOnlySession(
            mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            using (session)
            {
            }

            mockConnectionManager.VerifyAll();
        }

        [Test]
        public void FetchExecutesAndReturnsResults()
        {
            var sqlQuery = new SqlQuery("");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.BuildCommand(sqlQuery)).Returns(mockCommand.Object);

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

            var session = new ReadOnlySession(
                mockConnectionManager.Object,
                mockObjectBuilder.Object,
                new Mock<ISqlDialect>().Object);

            var customers = session.Fetch<Customer>(sqlQuery);

            Assert.AreEqual(1, customers.Count);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockObjectBuilder.VerifyAll();
        }

        [Test]
        public void FetchThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new ReadOnlySession(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Fetch<Customer>(null));

            Assert.AreEqual("sqlQuery", exception.ParamName);
        }

        [Test]
        public void FetchThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new ReadOnlySession(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Fetch<Customer>(null));
        }

        [Test]
        public void IncludeReturnsSameSessionByDifferentInterface()
        {
            var session = new ReadOnlySession(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            var includeSession = session.Include;

            Assert.AreSame(session, includeSession);
        }

        [Test]
        public void IncludeScalarSqlQueryExecutesAndReturnsResult()
        {
            var sqlQuery = new SqlQuery("");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(1);
            mockReader.Setup(x => x[0]).Returns(10);
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.BuildCommand(sqlQuery)).Returns(mockCommand.Object);

            var session = new ReadOnlySession(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            var includeScalar = session.Include.Scalar<int>(sqlQuery);

            // HACK: to force session to execute the query.
            // TODO: find a better way, we don't really want to make the method non private just for testing though...
            typeof(ReadOnlySession).GetMethod("ExecuteAllQueries", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).Invoke(session, null);

            Assert.AreEqual(10, includeScalar.Value);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
        }

        [Test]
        public void IncludeScalarThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new ReadOnlySession(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            SqlQuery sqlQuery = null;

            var exception = Assert.Throws<ArgumentNullException>(() => session.Include.Scalar<int>(sqlQuery));

            Assert.AreEqual("sqlQuery", exception.ParamName);
        }

        [Test]
        public void IncludeScalarThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new ReadOnlySession(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Include.Scalar<int>(new SqlQuery("")));
        }

        [Test]
        public void PagedExecutesAndReturnsResults()
        {
            var sqlQuery = new SqlQuery("SELECT * FROM TABLE");
            var countQuery = new SqlQuery("SELECT COUNT(*) FROM TABLE");
            var pagedQuery = new SqlQuery("SELECT * FROM (SELECT *, ROW_NUMBER() OVER(ORDER BY (SELECT NULL)) AS RowNumber FROM Customers) AS Customers");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(1);
            mockReader.Setup(x => x[0]).Returns(100);
            mockReader.Setup(x => x.NextResult()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false, true, false }).Dequeue);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.BuildCommand(It.IsAny<SqlQuery>())).Returns(mockCommand.Object);

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CountQuery(sqlQuery)).Returns(countQuery);
            mockSqlDialect.Setup(x => x.PageQuery(sqlQuery, 10, 25)).Returns(pagedQuery);

            var session = new ReadOnlySession(
                mockConnectionManager.Object,
                mockObjectBuilder.Object,
                mockSqlDialect.Object);

            var page = session.Paged<Customer>(sqlQuery, 10, 25);

            Assert.AreEqual(10, page.Page);
            Assert.AreEqual(1, page.Results.Count);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockObjectBuilder.VerifyAll();
            mockSqlDialect.VerifyAll();
        }

        [Test]
        public void PagedThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new ReadOnlySession(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Paged<Customer>(null, 1, 25));

            Assert.AreEqual("sqlQuery", exception.ParamName);
        }

        [Test]
        public void PagedThrowsArgumentOutOfRangeExceptionIfPageBelow1()
        {
            var session = new ReadOnlySession(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => session.Paged<Customer>(new SqlQuery(""), 0, 25));

            Assert.AreEqual("page", exception.ParamName);
            Assert.IsTrue(exception.Message.StartsWith(Messages.Session_PagesStartAtOne));
        }

        [Test]
        public void PagedThrowsArgumentOutOfRangeExceptionIfResultsPerPageBelow1()
        {
            var session = new ReadOnlySession(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => session.Paged<Customer>(new SqlQuery(""), 1, 0));

            Assert.AreEqual("resultsPerPage", exception.ParamName);
            Assert.IsTrue(exception.Message.StartsWith(Messages.Session_MustHaveAtLeast1Result));
        }

        [Test]
        public void PagedThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new ReadOnlySession(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Paged<Customer>(null, 1, 25));
        }

        [Test]
        public void ProjectionExecutesAndReturnsResults()
        {
            var sqlQuery = new SqlQuery("");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.BuildCommand(sqlQuery)).Returns(mockCommand.Object);

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildDynamic(reader)).Returns(new ExpandoObject());

            var session = new ReadOnlySession(
                mockConnectionManager.Object,
                mockObjectBuilder.Object,
                new Mock<ISqlDialect>().Object);

            var results = session.Projection(sqlQuery);

            Assert.AreEqual(1, results.Count);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockObjectBuilder.VerifyAll();
        }

        [Test]
        public void ProjectionThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new ReadOnlySession(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Projection(null));

            Assert.AreEqual("sqlQuery", exception.ParamName);
        }

        [Test]
        public void ProjectionThrowsMicroLiteExceptionIfExecuteReaderThrowsException()
        {
            var sqlQuery = new SqlQuery("");

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteReader()).Throws<InvalidOperationException>();
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.BuildCommand(sqlQuery)).Returns(mockCommand.Object);

            var session = new ReadOnlySession(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            var exception = Assert.Throws<MicroLiteException>(() => session.Projection(sqlQuery));

            Assert.NotNull(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);

            // Command should still be disposed.
            mockCommand.Verify(x => x.Dispose());
        }

        [Test]
        public void ProjectionThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new ReadOnlySession(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Projection(null));
        }

        [Test]
        public void SingleIdentifierExecutesAndReturnsNull()
        {
            var identifier = 100;
            var sqlQuery = new SqlQuery("");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(false);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.BuildCommand(sqlQuery)).Returns(mockCommand.Object);

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Select, typeof(Customer), identifier)).Returns(sqlQuery);

            IReadOnlySession session = new ReadOnlySession(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockSqlDialect.Object);

            var customer = session.Single<Customer>(identifier);

            Assert.IsNull(customer);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockSqlDialect.VerifyAll();
        }

        [Test]
        public void SingleIdentifierExecutesAndReturnsResult()
        {
            var identifier = 100;
            var sqlQuery = new SqlQuery("");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.BuildCommand(sqlQuery)).Returns(mockCommand.Object);

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Select, typeof(Customer), identifier)).Returns(sqlQuery);

            IReadOnlySession session = new ReadOnlySession(
                mockConnectionManager.Object,
                mockObjectBuilder.Object,
                mockSqlDialect.Object);

            var customer = session.Single<Customer>(identifier);

            Assert.NotNull(customer);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockObjectBuilder.VerifyAll();
            mockSqlDialect.VerifyAll();
        }

        [Test]
        public void SingleIdentifierThrowsArgumentNullExceptionForNullIdentifier()
        {
            IReadOnlySession session = new ReadOnlySession(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            object identifier = null;

            var exception = Assert.Throws<ArgumentNullException>(() => session.Single<Customer>(identifier));

            Assert.AreEqual("identifier", exception.ParamName);
        }

        [Test]
        public void SingleIdentifierThrowsObjectDisposedExceptionIfDisposed()
        {
            IReadOnlySession session = new ReadOnlySession(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Single<Customer>(1));
        }

        [Test]
        public void SingleSqlQueryExecutesAndReturnsNull()
        {
            var sqlQuery = new SqlQuery("");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(false);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.BuildCommand(sqlQuery)).Returns(mockCommand.Object);

            IReadOnlySession session = new ReadOnlySession(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            var customer = session.Single<Customer>(sqlQuery);

            Assert.IsNull(customer);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
        }

        [Test]
        public void SingleSqlQueryExecutesAndReturnsResult()
        {
            var sqlQuery = new SqlQuery("");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.BuildCommand(sqlQuery)).Returns(mockCommand.Object);

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

            IReadOnlySession session = new ReadOnlySession(
                mockConnectionManager.Object,
                mockObjectBuilder.Object,
                new Mock<ISqlDialect>().Object);

            var customer = session.Single<Customer>(sqlQuery);

            Assert.NotNull(customer);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockObjectBuilder.VerifyAll();
        }

        [Test]
        public void SingleSqlQueryThrowsArgumentNullExceptionForNullSqlQuery()
        {
            IReadOnlySession session = new ReadOnlySession(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            SqlQuery sqlQuery = null;

            var exception = Assert.Throws<ArgumentNullException>(() => session.Single<Customer>(sqlQuery));

            Assert.AreEqual("sqlQuery", exception.ParamName);
        }

        [Test]
        public void SingleSqlQueryThrowsObjectDisposedExceptionIfDisposed()
        {
            IReadOnlySession session = new ReadOnlySession(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Single<Customer>(new SqlQuery("")));
        }

        [Test]
        public void TransactionReturnsConnectionManagerCurrentTransaction()
        {
            var transaction = new Mock<ITransaction>().Object;

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CurrentTransaction).Returns(transaction);

            var session = new ReadOnlySession(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object);

            Assert.AreSame(transaction, session.Transaction);
        }

        [MicroLite.Mapping.Table("dbo", "Customers")]
        private class Customer
        {
            [MicroLite.Mapping.Column("CustomerId")]
            [MicroLite.Mapping.Identifier(MicroLite.Mapping.IdentifierStrategy.Identity)]
            public int Id
            {
                get;
                set;
            }
        }
    }
}