namespace MicroLite.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using MicroLite.Core;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="Session"/> class.
    /// </summary>
    [TestFixture]
    public class SessionTests
    {
        [Test]
        public void BeginTransactionCallsConnectionManagerBeginTransaction()
        {
            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.BeginTransaction());

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            session.BeginTransaction();

            mockConnectionManager.VerifyAll();
        }

        [Test]
        public void BeginTransactionWithIsolationLevelCallsConnectionManagerBeginTransactionWithIsolationLevel()
        {
            var isolationLevel = IsolationLevel.Chaos;

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.BeginTransaction(isolationLevel));

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            session.BeginTransaction(isolationLevel);

            mockConnectionManager.VerifyAll();
        }

        [Test]
        public void DeleteInstanceReturnsFalseIfNoRecordsDeleted()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");

            var mockQueryBuilder = new Mock<ISqlQueryBuilder>();
            mockQueryBuilder.Setup(x => x.DeleteQuery(customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(0);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockQueryBuilder.Object);

            Assert.IsFalse(session.Delete(customer));

            mockQueryBuilder.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Test]
        public void DeleteInstanceReturnsTrueIfRecordDeleted()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");

            var mockQueryBuilder = new Mock<ISqlQueryBuilder>();
            mockQueryBuilder.Setup(x => x.DeleteQuery(customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockQueryBuilder.Object);

            Assert.IsTrue(session.Delete(customer));

            mockQueryBuilder.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Test]
        public void DeleteInstanceThrowsArgumentNullExceptionForNullInstance()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Delete<Customer>(null));

            Assert.AreEqual("instance", exception.ParamName);
        }

        [Test]
        public void DeleteInstanceThrowsMicroLiteExceptionIfExecuteNonQueryThrowsException()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");

            var mockQueryBuilder = new Mock<ISqlQueryBuilder>();
            mockQueryBuilder.Setup(x => x.DeleteQuery(customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteNonQuery()).Throws<InvalidOperationException>();
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockQueryBuilder.Object);

            var exception = Assert.Throws<MicroLiteException>(() => session.Delete(customer));

            Assert.NotNull(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);

            // Command should still be disposed.
            mockCommand.VerifyAll();
        }

        [Test]
        public void DeleteInstanceThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Delete(new Customer()));
        }

        [Test]
        public void DisposeDisposesConnectionManager()
        {
            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.Dispose());

            var session = new Session(
            mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            using (session)
            {
            }

            mockConnectionManager.VerifyAll();
        }

        [Test]
        public void ExecuteBuildsAndExecutesCommandNotInTransaction()
        {
            var sqlQuery = new SqlQuery("");
            var result = 1;

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.Open());
            mockConnection.Setup(x => x.Close());

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(mockConnection.Object);
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(result);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            Assert.AreEqual(result, session.Execute(sqlQuery));

            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();
            mockConnection.VerifyAll();
        }

        [Test]
        public void ExecuteDoesNotOpenOrCloseConnectionWhenInTransaction()
        {
            var sqlQuery = new SqlQuery("");
            var result = 1;

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.State).Returns(ConnectionState.Open);
            mockConnection.Setup(x => x.Open());
            mockConnection.Setup(x => x.Close());

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(mockConnection.Object);
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(result);
            mockCommand.Setup(x => x.Transaction).Returns(new Mock<IDbTransaction>().Object);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            Assert.AreEqual(result, session.Execute(sqlQuery));

            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();

            mockConnection.Verify(x => x.Open(), Times.Never());
            mockConnection.Verify(x => x.Close(), Times.Never());
        }

        [Test]
        public void ExecuteScalarBuildsAndExecutesCommand()
        {
            var sqlQuery = new SqlQuery("");
            var result = new object();

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.Open());
            mockConnection.Setup(x => x.Close());

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(mockConnection.Object);
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(result);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            Assert.AreEqual(result, session.ExecuteScalar<object>(sqlQuery));

            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();
            mockConnection.VerifyAll();
        }

        [Test]
        public void ExecuteScalarDoesNotOpenOrCloseConnectionWhenInTransaction()
        {
            var sqlQuery = new SqlQuery("");
            var result = new object();

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.State).Returns(ConnectionState.Open);
            mockConnection.Setup(x => x.Open());
            mockConnection.Setup(x => x.Close());

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(mockConnection.Object);
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(result);
            mockCommand.Setup(x => x.Transaction).Returns(new Mock<IDbTransaction>().Object);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            Assert.AreEqual(result, session.ExecuteScalar<object>(sqlQuery));

            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();

            mockConnection.Verify(x => x.Open(), Times.Never());
            mockConnection.Verify(x => x.Close(), Times.Never());
        }

        [Test]
        public void ExecuteScalarThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            var exception = Assert.Throws<ArgumentNullException>(() => session.ExecuteScalar<object>(null));

            Assert.AreEqual("sqlQuery", exception.ParamName);
        }

        [Test]
        public void ExecuteThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Execute(null));

            Assert.AreEqual("sqlQuery", exception.ParamName);
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
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildNewInstance<Customer>(reader)).Returns(new Customer());

            var session = new Session(
                mockConnectionManager.Object,
                mockObjectBuilder.Object,
                new Mock<ISqlQueryBuilder>().Object);

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
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Fetch<Customer>(null));

            Assert.AreEqual("sqlQuery", exception.ParamName);
        }

        [Test]
        public void FetchThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Fetch<Customer>(null));
        }

        [Test]
        public void InsertBuildsAndExecutesQueryInvokingListeners()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");
            object identifier = 23543;

            var mockQueryBuilder = new Mock<ISqlQueryBuilder>();
            mockQueryBuilder.Setup(x => x.InsertQuery(customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(identifier);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var mockListener = new Mock<IListener>();
            mockListener.Setup(x => x.AfterInsert(customer, identifier));
            mockListener.Setup(x => x.BeforeInsert(typeof(Customer), sqlQuery));
            mockListener.Setup(x => x.BeforeInsert(customer));

            Listeners.Add(() =>
            {
                return mockListener.Object;
            });

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockQueryBuilder.Object);

            session.Insert(customer);

            mockQueryBuilder.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();
            mockListener.VerifyAll();
        }

        [Test]
        public void InsertThrowsArgumentNullExceptionForNullInstance()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Insert<Customer>(null));

            Assert.AreEqual("instance", exception.ParamName);
        }

        [Test]
        public void InsertThrowsMicroLiteExceptionIfExecuteScalarThrowsException()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");

            var mockQueryBuilder = new Mock<ISqlQueryBuilder>();
            mockQueryBuilder.Setup(x => x.InsertQuery(customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteScalar()).Throws<InvalidOperationException>();
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockQueryBuilder.Object);

            var exception = Assert.Throws<MicroLiteException>(() => session.Insert(customer));

            Assert.NotNull(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);

            // Command should still be disposed.
            mockCommand.VerifyAll();
        }

        [Test]
        public void InsertThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Insert(new Customer()));
        }

        [Test]
        public void PagedExecutesAndReturnsResults()
        {
            var sqlQuery = new SqlQuery("");
            var paged = new SqlQuery("");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.Build(paged)).Returns(mockCommand.Object);

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildNewInstance<Customer>(reader)).Returns(new Customer());

            var mockQueryBuilder = new Mock<ISqlQueryBuilder>();
            mockQueryBuilder.Setup(x => x.Page(sqlQuery, 10, 25)).Returns(paged);

            var session = new Session(
                mockConnectionManager.Object,
                mockObjectBuilder.Object,
                mockQueryBuilder.Object);

            var page = session.Paged<Customer>(sqlQuery, 10, 25);

            Assert.AreEqual(10, page.Page);
            Assert.AreEqual(1, page.Results.Count);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockObjectBuilder.VerifyAll();
            mockQueryBuilder.VerifyAll();
        }

        [Test]
        public void PagedThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Paged<Customer>(null, 1, 25));

            Assert.AreEqual("sqlQuery", exception.ParamName);
        }

        [Test]
        public void PagedThrowsArgumentOutOfRangeExceptionIfPageBelow1()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => session.Paged<Customer>(new SqlQuery(""), 0, 25));

            Assert.AreEqual("page", exception.ParamName);
        }

        [Test]
        public void PagedThrowsArgumentOutOfRangeExceptionIfResultsPerPageBelow1()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            var exception = Assert.Throws<ArgumentOutOfRangeException>(
                () => session.Paged<Customer>(new SqlQuery(""), 1, 0));

            Assert.AreEqual("resultsPerPage", exception.ParamName);
        }

        [Test]
        public void PagedThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Paged<Customer>(null, 1, 25));
        }

        [Test]
        public void QueryDoesNotOpenOrCloseConnectionWhenInTransaction()
        {
            var sqlQuery = new SqlQuery("");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Returns(new Queue<bool>(new[] { true, false }).Dequeue);
            var reader = mockReader.Object;

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.State).Returns(ConnectionState.Open);
            mockConnection.Setup(x => x.Open());
            mockConnection.Setup(x => x.Close());

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(mockConnection.Object);
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
            mockCommand.Setup(x => x.Transaction).Returns(new Mock<IDbTransaction>().Object);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildNewInstance<Customer>(reader)).Returns(new Customer());

            var session = new Session(
                mockConnectionManager.Object,
                mockObjectBuilder.Object,
                new Mock<ISqlQueryBuilder>().Object);

            var customers = session.Query<Customer>(sqlQuery).ToArray();

            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();

            mockConnection.Verify(x => x.Open(), Times.Never());
            mockConnection.Verify(x => x.Close(), Times.Never());
        }

        [Test]
        public void QueryExecutesAndReturnsResults()
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
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildNewInstance<Customer>(reader)).Returns(new Customer());

            var session = new Session(
                mockConnectionManager.Object,
                mockObjectBuilder.Object,
                new Mock<ISqlQueryBuilder>().Object);

            var customers = session.Query<Customer>(sqlQuery).ToArray();

            Assert.AreEqual(1, customers.Length);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockObjectBuilder.VerifyAll();
        }

        [Test]
        public void QueryThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Query<Customer>(null).First());

            Assert.AreEqual("sqlQuery", exception.ParamName);
        }

        [Test]
        public void QueryThrowsMicroLiteExceptionIfExecuteReaderThrowsException()
        {
            var sqlQuery = new SqlQuery("");

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteReader()).Throws<InvalidOperationException>();
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            var exception = Assert.Throws<MicroLiteException>(() => session.Query<Customer>(sqlQuery).ToArray());

            Assert.NotNull(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);

            // Command should still be disposed.
            mockCommand.VerifyAll();
        }

        [Test]
        public void QueryThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Query<Customer>(null).First());
        }

        [SetUp]
        public void SetUp()
        {
            Listeners.Clear();
        }

        [Test]
        public void SingleExecutesAndReturnsNull()
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
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var mockQueryBuilder = new Mock<ISqlQueryBuilder>();
            mockQueryBuilder.Setup(x => x.SelectQuery(typeof(Customer), identifier)).Returns(sqlQuery);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockQueryBuilder.Object);

            var customer = session.Single<Customer>(identifier);

            Assert.IsNull(customer);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockQueryBuilder.VerifyAll();
        }

        [Test]
        public void SingleExecutesAndReturnsResult()
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
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildNewInstance<Customer>(reader)).Returns(new Customer());

            var mockQueryBuilder = new Mock<ISqlQueryBuilder>();
            mockQueryBuilder.Setup(x => x.SelectQuery(typeof(Customer), identifier)).Returns(sqlQuery);

            var session = new Session(
                mockConnectionManager.Object,
                mockObjectBuilder.Object,
                mockQueryBuilder.Object);

            var customer = session.Single<Customer>(identifier);

            Assert.NotNull(customer);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockObjectBuilder.VerifyAll();
            mockQueryBuilder.VerifyAll();
        }

        [Test]
        public void SingleThrowsArgumentNullExceptionForNullIdentifier()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Single<Customer>(null));

            Assert.AreEqual("identifier", exception.ParamName);
        }

        [Test]
        public void SingleThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Single<Customer>(1));
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Listeners.Clear();
        }

        [Test]
        public void UpdateBuildsAndExecutesQueryInvokingListeners()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");
            var rowsAffected = 1;

            var mockQueryBuilder = new Mock<ISqlQueryBuilder>();
            mockQueryBuilder.Setup(x => x.UpdateQuery(customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(rowsAffected);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockQueryBuilder.Object);

            var mockListener = new Mock<IListener>();
            mockListener.Setup(x => x.BeforeUpdate(customer));

            Listeners.Add(() =>
            {
                return mockListener.Object;
            });

            session.Update(customer);

            mockQueryBuilder.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();
            mockListener.VerifyAll();
        }

        [Test]
        public void UpdateThrowsArgumentNullExceptionForNullInstance()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Update<Customer>(null));

            Assert.AreEqual("instance", exception.ParamName);
        }

        [Test]
        public void UpdateThrowsMicroLiteExceptionIfExecuteNonQueryThrowsException()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");

            var mockQueryBuilder = new Mock<ISqlQueryBuilder>();
            mockQueryBuilder.Setup(x => x.UpdateQuery(customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteNonQuery()).Throws<InvalidOperationException>();
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockQueryBuilder.Object);

            var exception = Assert.Throws<MicroLiteException>(() => session.Update(customer));

            Assert.NotNull(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);

            // Command should still be disposed.
            mockCommand.VerifyAll();
        }

        [Test]
        public void UpdateThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Update(new Customer()));
        }

        private class Customer
        {
            public int Id
            {
                get;
                set;
            }
        }
    }
}