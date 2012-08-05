namespace MicroLite.Tests.Core
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Dynamic;
    using System.Linq;
    using MicroLite.Core;
    using MicroLite.Listeners;
    using MicroLite.Mapping;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="Session"/> class.
    /// </summary>
    [TestFixture]
    public class SessionTests
    {
        [Test]
        public void AdvancedReturnsSameSessionByDifferentInterface()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            var advancedSession = session.Advanced;

            Assert.AreSame(session, advancedSession);
        }

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
        public void BeginTransactionThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

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

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            session.BeginTransaction(isolationLevel);

            mockConnectionManager.VerifyAll();
        }

        [Test]
        public void BeginTransactionWithIsolationLevelThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.BeginTransaction(IsolationLevel.ReadCommitted));
        }

        [Test]
        public void DeleteInstanceInvokesListeners()
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

            var mockListener = new Mock<IListener>();
            mockListener.Setup(x => x.BeforeDelete(customer));
            mockListener.Setup(x => x.BeforeDelete(customer, sqlQuery));

            Listener.Listeners.Add(() =>
            {
                return mockListener.Object;
            });

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockQueryBuilder.Object);

            session.Delete(customer);

            mockListener.VerifyAll();
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

            var exception = Assert.Throws<ArgumentNullException>(() => session.Delete(null));

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
        public void DeleteTypeByIdentifierReturnsFalseIfNoRecordsDeleted()
        {
            var type = typeof(Customer);
            var identifier = 1234;
            var sqlQuery = new SqlQuery("");

            var mockQueryBuilder = new Mock<ISqlQueryBuilder>();
            mockQueryBuilder.Setup(x => x.DeleteQuery(type, identifier)).Returns(sqlQuery);

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

            Assert.IsFalse(session.Delete(type, identifier));

            mockQueryBuilder.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Test]
        public void DeleteTypeByIdentifierReturnsTrueIfRecordDeleted()
        {
            var type = typeof(Customer);
            var identifier = 1234;
            var sqlQuery = new SqlQuery("");

            var mockQueryBuilder = new Mock<ISqlQueryBuilder>();
            mockQueryBuilder.Setup(x => x.DeleteQuery(type, identifier)).Returns(sqlQuery);

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

            Assert.IsTrue(session.Delete(type, identifier));

            mockQueryBuilder.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Test]
        public void DeleteTypeByIdentifierThrowsArgumentNullExceptionForNullIdentifier()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Delete(typeof(Customer), null));

            Assert.AreEqual("identifier", exception.ParamName);
        }

        [Test]
        public void DeleteTypeByIdentifierThrowsArgumentNullExceptionForNullType()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Delete(null, 1234));

            Assert.AreEqual("type", exception.ParamName);
        }

        [Test]
        public void DeleteTypeByIdentifierThrowsMicroLiteExceptionIfExecuteNonQueryThrowsException()
        {
            var type = typeof(Customer);
            var identifier = 1234;
            var sqlQuery = new SqlQuery("");

            var mockQueryBuilder = new Mock<ISqlQueryBuilder>();
            mockQueryBuilder.Setup(x => x.DeleteQuery(type, identifier)).Returns(sqlQuery);

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

            var exception = Assert.Throws<MicroLiteException>(() => session.Delete(type, identifier));

            Assert.NotNull(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);

            // Command should still be disposed.
            mockCommand.VerifyAll();
        }

        [Test]
        public void DeleteTypeByIdentifierThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Delete(typeof(Customer), 1234));
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
        public void ExecuteScalarThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.ExecuteScalar<int>(new SqlQuery("SELECT")));
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
        public void ExecuteThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Execute(new SqlQuery("SELECT")));
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
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

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
        public void InsertBuildsAndExecutesQuery()
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

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockQueryBuilder.Object);

            session.Insert(customer);

            mockQueryBuilder.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Test]
        public void InsertInvokesListeners()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");
            object identifier = 23543;

            var mockQueryBuilder = new Mock<ISqlQueryBuilder>();
            mockQueryBuilder.Setup(x => x.InsertQuery(customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(identifier);

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var mockListener = new Mock<IListener>();
            mockListener.Setup(x => x.AfterInsert(customer, identifier));
            mockListener.Setup(x => x.BeforeInsert(customer, sqlQuery));
            mockListener.Setup(x => x.BeforeInsert(customer));

            Listener.Listeners.Add(() =>
            {
                return mockListener.Object;
            });

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockQueryBuilder.Object);

            session.Insert(customer);

            mockListener.VerifyAll();
        }

        [Test]
        public void InsertThrowsArgumentNullExceptionForNullInstance()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Insert(null));

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
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

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
            Assert.IsTrue(exception.Message.StartsWith(Messages.Session_PagesStartAtOne));
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
            Assert.IsTrue(exception.Message.StartsWith(Messages.Session_MustHaveAtLeast1Result));
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
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildDynamic(reader)).Returns(new ExpandoObject());

            var session = new Session(
                mockConnectionManager.Object,
                mockObjectBuilder.Object,
                new Mock<ISqlQueryBuilder>().Object);

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
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

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
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            var exception = Assert.Throws<MicroLiteException>(() => session.Projection(sqlQuery));

            Assert.NotNull(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);

            // Command should still be disposed.
            mockCommand.Verify(x => x.Dispose());
        }

        [Test]
        public void ProjectionThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Projection(null));
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
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

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
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

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

        /// <summary>
        /// Issue #54 - If an exception is thrown by IDbReader.Read() it should be wrapped and thrown inside a MicroLiteException.
        /// Issue #59 - IDbReader is not closed on error.
        /// </summary>
        [Test]
        public void QueryThrowsMicroLiteExceptionIfDataReaderReadThrowsException()
        {
            var sqlQuery = new SqlQuery("");

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.Read()).Throws<InvalidOperationException>();
            mockReader.Setup(x => x.Close());
            var reader = mockReader.Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteReader()).Returns(reader);
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

            // DataReader should be closed if there is an exception.
            mockReader.Verify(x => x.Close());

            // Command should still be disposed.
            mockCommand.Verify(x => x.Dispose());
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
            mockCommand.Verify(x => x.Dispose());
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
            Listener.Listeners.Clear();
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
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var mockObjectBuilder = new Mock<IObjectBuilder>();
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

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
        public void SingleIdentifierThrowsArgumentNullExceptionForNullIdentifier()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            object identifier = null;

            var exception = Assert.Throws<ArgumentNullException>(() => session.Single<Customer>(identifier));

            Assert.AreEqual("identifier", exception.ParamName);
        }

        [Test]
        public void SingleIdentifierThrowsObjectDisposedExceptionIfDisposed()
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

        [Test]
        public void SingleSqlQuertExecutesAndReturnsResult()
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
            mockObjectBuilder.Setup(x => x.BuildInstance<Customer>(It.IsAny<ObjectInfo>(), reader)).Returns(new Customer());

            var session = new Session(
                mockConnectionManager.Object,
                mockObjectBuilder.Object,
                new Mock<ISqlQueryBuilder>().Object);

            var customer = session.Single<Customer>(sqlQuery);

            Assert.NotNull(customer);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockObjectBuilder.VerifyAll();
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
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            var customer = session.Single<Customer>(sqlQuery);

            Assert.IsNull(customer);

            mockReader.VerifyAll();
            mockCommand.VerifyAll();
            mockConnectionManager.VerifyAll();
        }

        [Test]
        public void SingleSqlQueryThrowsArgumentNullExceptionForNullIdentifier()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            SqlQuery sqlQuery = null;

            var exception = Assert.Throws<ArgumentNullException>(() => session.Single<Customer>(sqlQuery));

            Assert.AreEqual("sqlQuery", exception.ParamName);
        }

        [Test]
        public void SingleSqlQueryThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Single<Customer>(new SqlQuery("")));
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Listener.Listeners.Clear();
        }

        [Test]
        public void TransactionReturnsConnectionManagerCurrentTransaction()
        {
            var transaction = new Mock<ITransaction>().Object;

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CurrentTransaction).Returns(transaction);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            Assert.AreSame(transaction, session.Transaction);
        }

        [Test]
        public void UpdateBuildsAndExecutesQuery()
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

            session.Update(customer);

            mockQueryBuilder.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Test]
        public void UpdateInvokesListeners()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");
            var rowsAffected = 1;

            var mockQueryBuilder = new Mock<ISqlQueryBuilder>();
            mockQueryBuilder.Setup(x => x.UpdateQuery(customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(rowsAffected);

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.Build(sqlQuery)).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockQueryBuilder.Object);

            var mockListener = new Mock<IListener>();
            mockListener.Setup(x => x.BeforeUpdate(customer));
            mockListener.Setup(x => x.BeforeUpdate(customer, sqlQuery));

            Listener.Listeners.Add(() =>
            {
                return mockListener.Object;
            });

            session.Update(customer);

            mockListener.VerifyAll();
        }

        [Test]
        public void UpdateThrowsArgumentNullExceptionForNullInstance()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlQueryBuilder>().Object);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Update(null));

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