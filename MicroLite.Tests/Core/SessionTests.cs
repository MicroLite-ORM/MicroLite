namespace MicroLite.Tests.Core
{
    using System;
    using System.Data;
    using MicroLite.Core;
    using MicroLite.Dialect;
    using MicroLite.Listeners;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="Session"/> class.
    /// </summary>
    public class SessionTests
    {
        private enum CustomerStatus
        {
            Inactive = 0,
            Active = 1
        }

        [Fact]
        public void AdvancedReturnsSameSessionByDifferentInterface()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<IDbConnection>().Object,
                new Mock<ISqlDialect>().Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            var advancedSession = session.Advanced;

            Assert.Same(session, advancedSession);
        }

        [Fact]
        public void DeleteInstanceInvokesListeners()
        {
            var customer = new Customer();

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Delete, customer)).Returns(new SqlQuery(""));
            mockSqlDialect.Setup(x => x.BuildCommand(It.IsAny<IDbCommand>(), It.IsAny<SqlQuery>()));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            int counter = 0;

            var mockListener1 = new Mock<IListener>();
            mockListener1.Setup(x => x.AfterDelete(customer, 1)).Callback(() => Assert.Equal(4, ++counter));
            mockListener1.Setup(x => x.BeforeDelete(customer)).Callback(() => Assert.Equal(1, ++counter));

            var mockListener2 = new Mock<IListener>();
            mockListener2.Setup(x => x.AfterDelete(customer, 1)).Callback(() => Assert.Equal(3, ++counter));
            mockListener2.Setup(x => x.BeforeDelete(customer)).Callback(() => Assert.Equal(2, ++counter));

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new[] { mockListener1.Object, mockListener2.Object });

            session.Delete(customer);

            mockListener1.VerifyAll();
        }

        [Fact]
        public void DeleteInstanceReturnsFalseIfNoRecordsDeleted()
        {
            var customer = new Customer
            {
                Id = 14556
            };

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Delete, customer)).Returns(new SqlQuery(""));
            mockSqlDialect.Setup(x => x.BuildCommand(It.IsAny<IDbCommand>(), It.IsAny<SqlQuery>()));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(0);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            Assert.False(session.Delete(customer));

            mockSqlDialect.VerifyAll();
            mockConnection.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void DeleteInstanceReturnsTrueIfRecordDeleted()
        {
            var customer = new Customer
            {
                Id = 14556
            };

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Delete, customer)).Returns(new SqlQuery(""));
            mockSqlDialect.Setup(x => x.BuildCommand(It.IsAny<IDbCommand>(), It.IsAny<SqlQuery>()));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            Assert.True(session.Delete(customer));

            mockSqlDialect.VerifyAll();
            mockConnection.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void DeleteInstanceThrowsArgumentNullExceptionForNullInstance()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<IDbConnection>().Object,
                new Mock<ISqlDialect>().Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Delete(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void DeleteInstanceThrowsMicroLiteExceptionIfExecuteNonQueryThrowsException()
        {
            var customer = new Customer();

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Delete, customer)).Returns(new SqlQuery(""));
            mockSqlDialect.Setup(x => x.BuildCommand(It.IsAny<IDbCommand>(), It.IsAny<SqlQuery>()));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Throws<InvalidOperationException>();
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            var exception = Assert.Throws<MicroLiteException>(() => session.Delete(customer));

            Assert.NotNull(exception.InnerException);
            Assert.Equal(exception.InnerException.Message, exception.Message);

            // Command should still be disposed.
            mockCommand.VerifyAll();
        }

        [Fact]
        public void DeleteInstanceThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<IDbConnection>().Object,
                new Mock<ISqlDialect>().Object,
                new Mock<IObjectBuilder>().Object,

                new IListener[0]);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Delete(new Customer()));
        }

        [Fact]
        public void DeleteTypeByIdentifierReturnsFalseIfNoRecordsDeleted()
        {
            var type = typeof(Customer);
            var identifier = 1234;

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Delete, type, identifier)).Returns(new SqlQuery(""));
            mockSqlDialect.Setup(x => x.BuildCommand(It.IsAny<IDbCommand>(), It.IsAny<SqlQuery>()));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(0);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            Assert.False(session.Delete(type, identifier));

            mockSqlDialect.VerifyAll();
            mockConnection.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void DeleteTypeByIdentifierReturnsTrueIfRecordDeleted()
        {
            var type = typeof(Customer);
            var identifier = 1234;

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Delete, type, identifier)).Returns(new SqlQuery(""));
            mockSqlDialect.Setup(x => x.BuildCommand(It.IsAny<IDbCommand>(), It.IsAny<SqlQuery>()));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            Assert.True(session.Delete(type, identifier));

            mockSqlDialect.VerifyAll();
            mockConnection.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void DeleteTypeByIdentifierThrowsArgumentNullExceptionForNullIdentifier()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<IDbConnection>().Object,
                new Mock<ISqlDialect>().Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Delete(typeof(Customer), null));

            Assert.Equal("identifier", exception.ParamName);
        }

        [Fact]
        public void DeleteTypeByIdentifierThrowsArgumentNullExceptionForNullType()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<IDbConnection>().Object,
                new Mock<ISqlDialect>().Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Delete(null, 1234));

            Assert.Equal("type", exception.ParamName);
        }

        [Fact]
        public void DeleteTypeByIdentifierThrowsMicroLiteExceptionIfExecuteNonQueryThrowsException()
        {
            var type = typeof(Customer);
            var identifier = 1234;

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Delete, type, identifier)).Returns(new SqlQuery(""));
            mockSqlDialect.Setup(x => x.BuildCommand(It.IsAny<IDbCommand>(), It.IsAny<SqlQuery>()));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Throws<InvalidOperationException>();
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            var exception = Assert.Throws<MicroLiteException>(() => session.Delete(type, identifier));

            Assert.NotNull(exception.InnerException);
            Assert.Equal(exception.InnerException.Message, exception.Message);

            // Command should still be disposed.
            mockCommand.VerifyAll();
        }

        [Fact]
        public void DeleteTypeByIdentifierThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<IDbConnection>().Object,
                new Mock<ISqlDialect>().Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Delete(typeof(Customer), 1234));
        }

        [Fact]
        public void ExecuteBuildsAndExecutesCommandNotInTransaction()
        {
            var sqlQuery = new SqlQuery("");
            var result = 1;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(result);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildCommand(mockCommand.Object, sqlQuery));

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            Assert.Equal(result, session.Execute(sqlQuery));

            mockConnection.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void ExecuteScalarBuildsAndExecutesCommand()
        {
            var sqlQuery = new SqlQuery("");
            var result = new object();

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(result);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildCommand(mockCommand.Object, sqlQuery));

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            Assert.Equal(result, session.ExecuteScalar<object>(sqlQuery));

            mockConnection.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void ExecuteScalarThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<IDbConnection>().Object,
                new Mock<ISqlDialect>().Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.ExecuteScalar<object>(null));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        [Fact]
        public void ExecuteScalarThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<IDbConnection>().Object,
                new Mock<ISqlDialect>().Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.ExecuteScalar<int>(new SqlQuery("SELECT")));
        }

        [Fact]
        public void ExecuteScalarUsesTypeConvertersToResolveResultType()
        {
            var sqlQuery = new SqlQuery("");
            var result = (byte)1;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(result);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildCommand(mockCommand.Object, sqlQuery));

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            Assert.Equal((CustomerStatus)result, session.ExecuteScalar<CustomerStatus>(sqlQuery));

            mockConnection.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void ExecuteThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<IDbConnection>().Object,
                new Mock<ISqlDialect>().Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Execute(null));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        [Fact]
        public void ExecuteThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<IDbConnection>().Object,
                new Mock<ISqlDialect>().Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Execute(new SqlQuery("SELECT")));
        }

        [Fact]
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

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            session.Insert(customer);

            mockSqlDialect.VerifyAll();
            mockConnection.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void InsertInvokesListeners()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");
            object identifier = 23543;

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Insert, customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(identifier);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            int counter = 0;

            var mockListener1 = new Mock<IListener>();
            mockListener1.Setup(x => x.AfterInsert(customer, identifier)).Callback(() => Assert.Equal(4, ++counter));
            mockListener1.Setup(x => x.BeforeInsert(customer)).Callback(() => Assert.Equal(1, ++counter));

            var mockListener2 = new Mock<IListener>();
            mockListener2.Setup(x => x.AfterInsert(customer, identifier)).Callback(() => Assert.Equal(3, ++counter));
            mockListener2.Setup(x => x.BeforeInsert(customer)).Callback(() => Assert.Equal(2, ++counter));

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new[] { mockListener1.Object, mockListener2.Object });

            session.Insert(customer);

            mockListener1.VerifyAll();
        }

        [Fact]
        public void InsertOrUpdateInsertsInstanceIfIdentifierIsNotSet()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(new Mock<IDbCommand>().Object);

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(It.IsAny<StatementType>(), It.IsAny<object>())).Returns(new SqlQuery(""));

            var mockListener = new Mock<IListener>();

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new[] { mockListener.Object });

            var customer = new Customer
            {
                Id = 0
            };

            session.InsertOrUpdate(customer);

            mockListener.Verify(x => x.BeforeInsert(customer), Times.Once());
            mockListener.Verify(x => x.BeforeUpdate(customer), Times.Never());
        }

        [Fact]
        public void InsertOrUpdateThrowsArgumentNullExceptionForNullInstance()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<IDbConnection>().Object,
                new Mock<ISqlDialect>().Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.InsertOrUpdate(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void InsertOrUpdateUpdatesInstanceIfIdentifierIsNotSet()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(new Mock<IDbCommand>().Object);

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(It.IsAny<StatementType>(), It.IsAny<object>())).Returns(new SqlQuery(""));

            var mockListener = new Mock<IListener>();

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new[] { mockListener.Object });

            var customer = new Customer
            {
                Id = 1242
            };

            session.InsertOrUpdate(customer);

            mockListener.Verify(x => x.BeforeInsert(customer), Times.Never());
            mockListener.Verify(x => x.BeforeUpdate(customer), Times.Once());
        }

        [Fact]
        public void InsertThrowsArgumentNullExceptionForNullInstance()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<IDbConnection>().Object,
                new Mock<ISqlDialect>().Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Insert(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void InsertThrowsMicroLiteExceptionIfExecuteScalarThrowsException()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Insert, customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteScalar()).Throws<InvalidOperationException>();
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            var exception = Assert.Throws<MicroLiteException>(() => session.Insert(customer));

            Assert.NotNull(exception.InnerException);
            Assert.Equal(exception.InnerException.Message, exception.Message);

            // Command should still be disposed.
            mockCommand.VerifyAll();
        }

        [Fact]
        public void InsertThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<IDbConnection>().Object,
                new Mock<ISqlDialect>().Object,
                new Mock<IObjectBuilder>().Object,

                new IListener[0]);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Insert(new Customer()));
        }

        [Fact]
        public void UpdateInstanceBuildsAndExecutesQuery()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");
            var rowsAffected = 1;

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Update, customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(rowsAffected);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            session.Update(customer);

            mockSqlDialect.VerifyAll();
            mockConnection.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void UpdateInstanceInvokesListeners()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");
            var rowsAffected = 1;

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Update, customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(rowsAffected);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            int counter = 0;

            var mockListener1 = new Mock<IListener>();
            mockListener1.Setup(x => x.AfterUpdate(customer, 1)).Callback(() => Assert.Equal(4, ++counter));
            mockListener1.Setup(x => x.BeforeUpdate(customer)).Callback(() => Assert.Equal(1, ++counter));

            var mockListener2 = new Mock<IListener>();
            mockListener2.Setup(x => x.AfterUpdate(customer, 1)).Callback(() => Assert.Equal(3, ++counter));
            mockListener2.Setup(x => x.BeforeUpdate(customer)).Callback(() => Assert.Equal(2, ++counter));

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new[] { mockListener1.Object, mockListener2.Object });

            session.Update(customer);

            mockListener1.VerifyAll();
        }

        [Fact]
        public void UpdateInstanceReturnsFalseIfNoRecordsUpdated()
        {
            var customer = new Customer();

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Delete, customer)).Returns(new SqlQuery(""));
            mockSqlDialect.Setup(x => x.BuildCommand(It.IsAny<IDbCommand>(), It.IsAny<SqlQuery>()));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(0);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            Assert.False(session.Delete(customer));

            mockSqlDialect.VerifyAll();
            mockConnection.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void UpdateInstanceReturnsTrueIfRecordUpdated()
        {
            var customer = new Customer();

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Delete, customer)).Returns(new SqlQuery(""));
            mockSqlDialect.Setup(x => x.BuildCommand(It.IsAny<IDbCommand>(), It.IsAny<SqlQuery>()));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            Assert.True(session.Delete(customer));

            mockSqlDialect.VerifyAll();
            mockConnection.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void UpdateInstanceThrowsArgumentNullExceptionForNullInstance()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<IDbConnection>().Object,
                new Mock<ISqlDialect>().Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Update((Customer)null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void UpdateInstanceThrowsMicroLiteExceptionIfExecuteNonQueryThrowsException()
        {
            var customer = new Customer();
            var sqlQuery = new SqlQuery("");

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(StatementType.Update, customer)).Returns(sqlQuery);

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Throws<InvalidOperationException>();
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            var exception = Assert.Throws<MicroLiteException>(() => session.Update(customer));

            Assert.NotNull(exception.InnerException);
            Assert.Equal(exception.InnerException.Message, exception.Message);

            // Command should still be disposed.
            mockCommand.VerifyAll();
        }

        [Fact]
        public void UpdateInstanceThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<IDbConnection>().Object,
                new Mock<ISqlDialect>().Object,
                new Mock<IObjectBuilder>().Object,

                new IListener[0]);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Update(new Customer()));
        }

        [Fact]
        public void UpdateObjectDeltaReturnsFalseIfNoRecordUpdated()
        {
            var objectDelta = new ObjectDelta(typeof(Customer), 1234);
            objectDelta.AddChange("Name", "Fred Flintstone");

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(objectDelta)).Returns(new SqlQuery(""));
            mockSqlDialect.Setup(x => x.BuildCommand(It.IsAny<IDbCommand>(), It.IsAny<SqlQuery>()));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(0);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            Assert.False(session.Update(objectDelta));

            mockConnection.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void UpdateObjectDeltaReturnsTrueIfRecordUpdated()
        {
            var objectDelta = new ObjectDelta(typeof(Customer), 1234);
            objectDelta.AddChange("Name", "Fred Flintstone");

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.CreateQuery(objectDelta)).Returns(new SqlQuery(""));
            mockSqlDialect.Setup(x => x.BuildCommand(It.IsAny<IDbCommand>(), It.IsAny<SqlQuery>()));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockConnection.Object,
                mockSqlDialect.Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            Assert.True(session.Update(objectDelta));

            mockConnection.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void UpdateObjectDeltaThrowsArgumentNullExceptionForNullObjectDelta()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<IDbConnection>().Object,
                new Mock<ISqlDialect>().Object,
                new Mock<IObjectBuilder>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Update((ObjectDelta)null));

            Assert.Equal("objectDelta", exception.ParamName);
        }

        [Fact]
        public void UpdateObjectDeltaThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<IDbConnection>().Object,
                new Mock<ISqlDialect>().Object,
                new Mock<IObjectBuilder>().Object,

                new IListener[0]);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Update(new ObjectDelta(typeof(Customer), 1234)));
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