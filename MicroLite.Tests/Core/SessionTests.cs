namespace MicroLite.Tests.Core
{
    using System;
    using System.Data;
    using MicroLite.Core;
    using MicroLite.Dialect;
    using MicroLite.Listeners;
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
                new Mock<ISqlDialect>().Object,
                new IListener[0]);

            var advancedSession = session.Advanced;

            Assert.AreSame(session, advancedSession);
        }

        [Test]
        public void DeleteInstanceInvokesListeners()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Delete, customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            int counter = 0;

            var mockListener1 = new Mock<IListener>();
            mockListener1.Setup(x => x.AfterDelete(customer, 1)).Callback(() => Assert.AreEqual(6, ++counter));
            mockListener1.Setup(x => x.BeforeDelete(customer)).Callback(() => Assert.AreEqual(1, ++counter));
            mockListener1.Setup(x => x.BeforeDelete(customer, sqlQuery)).Callback(() => Assert.AreEqual(3, ++counter));

            var mockListener2 = new Mock<IListener>();
            mockListener2.Setup(x => x.AfterDelete(customer, 1)).Callback(() => Assert.AreEqual(5, ++counter));
            mockListener2.Setup(x => x.BeforeDelete(customer)).Callback(() => Assert.AreEqual(2, ++counter));
            mockListener2.Setup(x => x.BeforeDelete(customer, sqlQuery)).Callback(() => Assert.AreEqual(4, ++counter));

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockSqlDialect.Object,
                new[] { mockListener1.Object, mockListener2.Object });

            session.Delete(customer);

            mockListener1.VerifyAll();
        }

        [Test]
        public void DeleteInstanceReturnsFalseIfNoRecordsDeleted()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Delete, customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(0);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockSqlDialect.Object,
                new IListener[0]);

            Assert.IsFalse(session.Delete(customer));

            mockSqlDialect.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Test]
        public void DeleteInstanceReturnsTrueIfRecordDeleted()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Delete, customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockSqlDialect.Object,
                new IListener[0]);

            Assert.IsTrue(session.Delete(customer));

            mockSqlDialect.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Test]
        public void DeleteInstanceThrowsArgumentNullExceptionForNullInstance()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Delete(null));

            Assert.AreEqual("instance", exception.ParamName);
        }

        [Test]
        public void DeleteInstanceThrowsMicroLiteExceptionIfExecuteNonQueryThrowsException()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Delete, customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteNonQuery()).Throws<InvalidOperationException>();
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockSqlDialect.Object,
                new IListener[0]);

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
                new Mock<ISqlDialect>().Object,
                new IListener[0]);

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

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Delete, type, identifier)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(0);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockSqlDialect.Object,
                new IListener[0]);

            Assert.IsFalse(session.Delete(type, identifier));

            mockSqlDialect.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Test]
        public void DeleteTypeByIdentifierReturnsTrueIfRecordDeleted()
        {
            var type = typeof(Customer);
            var identifier = 1234;
            var sqlQuery = new SqlQuery("");

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Delete, type, identifier)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockSqlDialect.Object,
                new IListener[0]);

            Assert.IsTrue(session.Delete(type, identifier));

            mockSqlDialect.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Test]
        public void DeleteTypeByIdentifierThrowsArgumentNullExceptionForNullIdentifier()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Delete(typeof(Customer), null));

            Assert.AreEqual("identifier", exception.ParamName);
        }

        [Test]
        public void DeleteTypeByIdentifierThrowsArgumentNullExceptionForNullType()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Delete(null, 1234));

            Assert.AreEqual("type", exception.ParamName);
        }

        [Test]
        public void DeleteTypeByIdentifierThrowsMicroLiteExceptionIfExecuteNonQueryThrowsException()
        {
            var type = typeof(Customer);
            var identifier = 1234;
            var sqlQuery = new SqlQuery("");

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Delete, type, identifier)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteNonQuery()).Throws<InvalidOperationException>();
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockSqlDialect.Object,
                new IListener[0]);

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
                new Mock<ISqlDialect>().Object,
                new IListener[0]);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Delete(typeof(Customer), 1234));
        }

        [Test]
        public void ExecuteBuildsAndExecutesCommandNotInTransaction()
        {
            var sqlQuery = new SqlQuery("");
            var result = 1;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(result);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildCommand(mockCommand.Object, sqlQuery));

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockSqlDialect.Object,
                new IListener[0]);

            Assert.AreEqual(result, session.Execute(sqlQuery));

            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Test]
        public void ExecuteDoesNotOpenOrCloseConnectionWhenInTransaction()
        {
            var sqlQuery = new SqlQuery("");
            var result = 1;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(result);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildCommand(mockCommand.Object, sqlQuery));

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockSqlDialect.Object,
                new IListener[0]);

            Assert.AreEqual(result, session.Execute(sqlQuery));

            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Test]
        public void ExecuteScalarBuildsAndExecutesCommand()
        {
            var sqlQuery = new SqlQuery("");
            var result = new object();

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(result);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildCommand(mockCommand.Object, sqlQuery));

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockSqlDialect.Object,
                new IListener[0]);

            Assert.AreEqual(result, session.ExecuteScalar<object>(sqlQuery));

            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();
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
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(result);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildCommand(mockCommand.Object, sqlQuery));

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockSqlDialect.Object,
                new IListener[0]);

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
                new Mock<ISqlDialect>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.ExecuteScalar<object>(null));

            Assert.AreEqual("sqlQuery", exception.ParamName);
        }

        [Test]
        public void ExecuteScalarThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object,
                new IListener[0]);

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
                new Mock<ISqlDialect>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Execute(null));

            Assert.AreEqual("sqlQuery", exception.ParamName);
        }

        [Test]
        public void ExecuteThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object,
                new IListener[0]);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Execute(new SqlQuery("SELECT")));
        }

        [Test]
        public void InsertBuildsAndExecutesQuery()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");
            object identifier = 23543;

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Insert, customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteScalar()).Returns(identifier);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockSqlDialect.Object,
                new IListener[0]);

            session.Insert(customer);

            mockSqlDialect.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Test]
        public void InsertInvokesListeners()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");
            object identifier = 23543;

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Insert, customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteScalar()).Returns(identifier);

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            int counter = 0;

            var mockListener1 = new Mock<IListener>();
            mockListener1.Setup(x => x.AfterInsert(customer, identifier)).Callback(() => Assert.AreEqual(6, ++counter));
            mockListener1.Setup(x => x.BeforeInsert(customer)).Callback(() => Assert.AreEqual(1, ++counter));
            mockListener1.Setup(x => x.BeforeInsert(customer, sqlQuery)).Callback(() => Assert.AreEqual(3, ++counter));

            var mockListener2 = new Mock<IListener>();
            mockListener2.Setup(x => x.AfterInsert(customer, identifier)).Callback(() => Assert.AreEqual(5, ++counter));
            mockListener2.Setup(x => x.BeforeInsert(customer)).Callback(() => Assert.AreEqual(2, ++counter));
            mockListener2.Setup(x => x.BeforeInsert(customer, sqlQuery)).Callback(() => Assert.AreEqual(4, ++counter));

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockSqlDialect.Object,
                new[] { mockListener1.Object, mockListener2.Object });

            session.Insert(customer);

            mockListener1.VerifyAll();
        }

        [Test]
        public void InsertThrowsArgumentNullExceptionForNullInstance()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Insert(null));

            Assert.AreEqual("instance", exception.ParamName);
        }

        [Test]
        public void InsertThrowsMicroLiteExceptionIfExecuteScalarThrowsException()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Insert, customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteScalar()).Throws<InvalidOperationException>();
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockSqlDialect.Object,
                new IListener[0]);

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
                new Mock<ISqlDialect>().Object,
                new IListener[0]);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Insert(new Customer()));
        }

        [Test]
        public void UpdateBuildsAndExecutesQuery()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");
            var rowsAffected = 1;

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Update, customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(rowsAffected);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockSqlDialect.Object,
                new IListener[0]);

            session.Update(customer);

            mockSqlDialect.VerifyAll();
            mockConnectionManager.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Test]
        public void UpdateInvokesListeners()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");
            var rowsAffected = 1;

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Update, customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(rowsAffected);

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            int counter = 0;

            var mockListener1 = new Mock<IListener>();
            mockListener1.Setup(x => x.AfterUpdate(customer, 1)).Callback(() => Assert.AreEqual(6, ++counter));
            mockListener1.Setup(x => x.BeforeUpdate(customer)).Callback(() => Assert.AreEqual(1, ++counter));
            mockListener1.Setup(x => x.BeforeUpdate(customer, sqlQuery)).Callback(() => Assert.AreEqual(3, ++counter));

            var mockListener2 = new Mock<IListener>();
            mockListener2.Setup(x => x.AfterUpdate(customer, 1)).Callback(() => Assert.AreEqual(5, ++counter));
            mockListener2.Setup(x => x.BeforeUpdate(customer)).Callback(() => Assert.AreEqual(2, ++counter));
            mockListener2.Setup(x => x.BeforeUpdate(customer, sqlQuery)).Callback(() => Assert.AreEqual(4, ++counter));

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockSqlDialect.Object,
                new[] { mockListener1.Object, mockListener2.Object });

            session.Update(customer);

            mockListener1.VerifyAll();
        }

        [Test]
        public void UpdateThrowsArgumentNullExceptionForNullInstance()
        {
            var session = new Session(
                new Mock<IConnectionManager>().Object,
                new Mock<IObjectBuilder>().Object,
                new Mock<ISqlDialect>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Update(null));

            Assert.AreEqual("instance", exception.ParamName);
        }

        [Test]
        public void UpdateThrowsMicroLiteExceptionIfExecuteNonQueryThrowsException()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Update, customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();

            mockCommand.Setup(x => x.ExecuteNonQuery()).Throws<InvalidOperationException>();
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnectionManager = new Mock<IConnectionManager>();
            mockConnectionManager.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                mockConnectionManager.Object,
                new Mock<IObjectBuilder>().Object,
                mockSqlDialect.Object,
                new IListener[0]);

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
                new Mock<ISqlDialect>().Object,
                new IListener[0]);

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