namespace MicroLite.Tests.Core
{
    using System;
    using System.Data;
    using MicroLite.Core;
    using MicroLite.Dialect;
    using MicroLite.Driver;
    using MicroLite.Listeners;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="Session"/> class.
    /// </summary>
    public class SessionTests : UnitTest
    {
        [Fact]
        public void AdvancedReturnsSameSessionByDifferentInterface()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new IListener[0]);

            var advancedSession = session.Advanced;

            Assert.Same(session, advancedSession);
        }

        [Fact]
        public void DeleteInstanceInvokesListeners()
        {
            var customer = new Customer
            {
                Id = 187224
            };

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildDeleteSqlQuery(It.IsNotNull<IObjectInfo>(), customer.Id)).Returns(new SqlQuery(""));

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(mockCommand.Object);

            int counter = 0;

            var mockListener1 = new Mock<IListener>();
            mockListener1.Setup(x => x.AfterDelete(customer, 1)).Callback(() => Assert.Equal(4, ++counter));
            mockListener1.Setup(x => x.BeforeDelete(customer)).Callback(() => Assert.Equal(1, ++counter));

            var mockListener2 = new Mock<IListener>();
            mockListener2.Setup(x => x.AfterDelete(customer, 1)).Callback(() => Assert.Equal(3, ++counter));
            mockListener2.Setup(x => x.BeforeDelete(customer)).Callback(() => Assert.Equal(2, ++counter));

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new[] { mockListener1.Object, mockListener2.Object });

            session.Delete(customer);

            mockListener1.VerifyAll();
            mockListener2.VerifyAll();
        }

        [Fact]
        public void DeleteInstanceReturnsFalseIfNoRecordsDeleted()
        {
            var customer = new Customer
            {
                Id = 14556
            };

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(0);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildDeleteSqlQuery(It.IsNotNull<IObjectInfo>(), customer.Id)).Returns(new SqlQuery(""));

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new IListener[0]);

            Assert.False(session.Delete(customer));

            mockSqlDialect.VerifyAll();
            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void DeleteInstanceReturnsTrueIfRecordDeleted()
        {
            var customer = new Customer
            {
                Id = 14556
            };

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildDeleteSqlQuery(It.IsNotNull<IObjectInfo>(), customer.Id)).Returns(new SqlQuery(""));

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new IListener[0]);

            Assert.True(session.Delete(customer));

            mockSqlDialect.VerifyAll();
            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void DeleteInstanceThrowsArgumentNullExceptionForNullInstance()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Delete(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void DeleteInstanceThrowsMicroLiteExceptionIfExecuteNonQueryThrowsException()
        {
            var customer = new Customer
            {
                Id = 187224
            };

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Throws<InvalidOperationException>();
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildDeleteSqlQuery(It.IsNotNull<IObjectInfo>(), customer.Id)).Returns(new SqlQuery(""));

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new IListener[0]);

            var exception = Assert.Throws<MicroLiteException>(() => session.Delete(customer));

            Assert.NotNull(exception.InnerException);
            Assert.Equal(exception.InnerException.Message, exception.Message);

            // Command should still be disposed.
            mockCommand.VerifyAll();
        }

        [Fact]
        public void DeleteInstanceThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = 0
            };

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                mockDbDriver.Object,
                new IListener[0]);

            var exception = Assert.Throws<MicroLiteException>(() => session.Delete(customer));

            Assert.Equal(ExceptionMessages.Session_IdentifierNotSetForDelete, exception.Message);
        }

        [Fact]
        public void DeleteInstanceThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
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

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(0);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildDeleteSqlQuery(It.IsNotNull<IObjectInfo>(), identifier)).Returns(new SqlQuery(""));

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new IListener[0]);

            Assert.False(session.Delete(type, identifier));

            mockSqlDialect.VerifyAll();
            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void DeleteTypeByIdentifierReturnsTrueIfRecordDeleted()
        {
            var type = typeof(Customer);
            var identifier = 1234;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildDeleteSqlQuery(It.IsNotNull<IObjectInfo>(), identifier)).Returns(new SqlQuery(""));

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new IListener[0]);

            Assert.True(session.Delete(type, identifier));

            mockSqlDialect.VerifyAll();
            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void DeleteTypeByIdentifierThrowsArgumentNullExceptionForNullIdentifier()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Delete(typeof(Customer), null));

            Assert.Equal("identifier", exception.ParamName);
        }

        [Fact]
        public void DeleteTypeByIdentifierThrowsArgumentNullExceptionForNullType()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Delete(null, 1234));

            Assert.Equal("type", exception.ParamName);
        }

        [Fact]
        public void DeleteTypeByIdentifierThrowsMicroLiteExceptionIfExecuteNonQueryThrowsException()
        {
            var type = typeof(Customer);
            var identifier = 1234;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Throws<InvalidOperationException>();
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildDeleteSqlQuery(It.IsNotNull<IObjectInfo>(), identifier)).Returns(new SqlQuery(""));

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
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
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
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

            var mockSqlDialect = new Mock<ISqlDialect>();

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new IListener[0]);

            Assert.Equal(result, session.Execute(sqlQuery));

            mockDbDriver.VerifyAll();
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

            var mockSqlDialect = new Mock<ISqlDialect>();

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new IListener[0]);

            Assert.Equal(result, session.ExecuteScalar<object>(sqlQuery));

            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void ExecuteScalarThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.ExecuteScalar<object>(null));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        [Fact]
        public void ExecuteScalarThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
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

            var mockSqlDialect = new Mock<ISqlDialect>();

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new IListener[0]);

            Assert.Equal((CustomerStatus)result, session.ExecuteScalar<CustomerStatus>(sqlQuery));

            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void ExecuteThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Execute(null));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        [Fact]
        public void ExecuteThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new IListener[0]);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Execute(new SqlQuery("SELECT")));
        }

        [Fact]
        public void InsertBuildsAndExecutesAnInsertCommandOnlyIfIdentifierStrategyAssigned()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var customer = new Customer
            {
                Id = 12345
            };

            var insertSqlQuery = new SqlQuery("INSERT");

            var mockInsertCommand = new Mock<IDbCommand>();
            mockInsertCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);
            mockInsertCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildInsertSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(insertSqlQuery);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.BuildCommand(insertSqlQuery)).Returns(mockInsertCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new IListener[0]);

            session.Insert(customer);

            mockSqlDialect.Verify(x => x.BuildInsertSqlQuery(It.IsNotNull<IObjectInfo>(), customer), Times.Once());
            mockSqlDialect.Verify(x => x.BuildSelectIdentitySqlQuery(It.IsAny<IObjectInfo>()), Times.Never());

            mockDbDriver.VerifyAll();
            mockInsertCommand.VerifyAll();
        }

        [Fact]
        public void InsertBuildsAndExecutesCombinedQueriesIfIdentifierStrategyDbGeneratedAndTheDbDriverSupportsBatchedQueries()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer();
            var insertSqlQuery = new SqlQuery("INSERT");
            var selectIdSqlQuery = new SqlQuery("SELECT");
            var combinedSqlQuery = new SqlQuery("INSERT;SELECT");
            object identifier = 23543;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(identifier);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildInsertSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(insertSqlQuery);
            mockSqlDialect.Setup(x => x.BuildSelectIdentitySqlQuery(It.IsNotNull<IObjectInfo>())).Returns(selectIdSqlQuery);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.SupportsBatchedQueries).Returns(true);
            mockDbDriver.Setup(x => x.Combine(insertSqlQuery, selectIdSqlQuery)).Returns(combinedSqlQuery);
            mockDbDriver.Setup(x => x.BuildCommand(combinedSqlQuery)).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new IListener[0]);

            session.Insert(customer);

            mockSqlDialect.VerifyAll();
            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void InsertBuildsAndExecutesIndividualQueriesIfIdentifierStrategyDbGeneratedTheDbDriverDoesNotSupportBatchedQueries()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer();
            var insertSqlQuery = new SqlQuery("INSERT");
            var selectIdSqlQuery = new SqlQuery("SELECT");
            object identifier = 23543;

            var mockInsertCommand = new Mock<IDbCommand>();
            mockInsertCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);
            mockInsertCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockSelectCommand = new Mock<IDbCommand>();
            mockSelectCommand.Setup(x => x.ExecuteScalar()).Returns(identifier);
            mockInsertCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildInsertSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(insertSqlQuery);
            mockSqlDialect.Setup(x => x.BuildSelectIdentitySqlQuery(It.IsNotNull<IObjectInfo>())).Returns(selectIdSqlQuery);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.SupportsBatchedQueries).Returns(false);
            mockDbDriver.Setup(x => x.BuildCommand(insertSqlQuery)).Returns(mockInsertCommand.Object);
            mockDbDriver.Setup(x => x.BuildCommand(selectIdSqlQuery)).Returns(mockSelectCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new IListener[0]);

            session.Insert(customer);

            mockSqlDialect.VerifyAll();
            mockDbDriver.VerifyAll();
            mockInsertCommand.VerifyAll();
            mockSelectCommand.VerifyAll();
        }

        [Fact]
        public void InsertInvokesListeners()
        {
            var customer = new Customer();
            object identifier = 23543;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(identifier);

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildInsertSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(new SqlQuery(""));
            mockSqlDialect.Setup(x => x.BuildSelectIdentitySqlQuery(It.IsNotNull<IObjectInfo>())).Returns(new SqlQuery(""));

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(mockCommand.Object);

            int counter = 0;

            var mockListener1 = new Mock<IListener>();
            mockListener1.Setup(x => x.AfterInsert(customer, identifier)).Callback(() => Assert.Equal(4, ++counter));
            mockListener1.Setup(x => x.BeforeInsert(customer)).Callback(() => Assert.Equal(1, ++counter));

            var mockListener2 = new Mock<IListener>();
            mockListener2.Setup(x => x.AfterInsert(customer, identifier)).Callback(() => Assert.Equal(3, ++counter));
            mockListener2.Setup(x => x.BeforeInsert(customer)).Callback(() => Assert.Equal(2, ++counter));

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new[] { mockListener1.Object, mockListener2.Object });

            session.Insert(customer);

            mockListener1.VerifyAll();
            mockListener2.VerifyAll();
        }

        [Fact]
        public void InsertThrowsArgumentNullExceptionForNullInstance()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Insert(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void InsertThrowsMicroLiteExceptionIfExecuteScalarThrowsException()
        {
            var customer = new Customer();

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteScalar()).Throws<InvalidOperationException>();
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildInsertSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(new SqlQuery(""));
            mockSqlDialect.Setup(x => x.BuildSelectIdentitySqlQuery(It.IsNotNull<IObjectInfo>())).Returns(new SqlQuery(""));

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
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
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new IListener[0]);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Insert(new Customer()));
        }

        [Fact]
        public void UpdateInstanceBuildsAndExecutesQuery()
        {
            var customer = new Customer
            {
                Id = 187224
            };

            var sqlQuery = new SqlQuery("");
            var rowsAffected = 1;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(rowsAffected);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildUpdateSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(sqlQuery);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new IListener[0]);

            session.Update(customer);

            mockSqlDialect.VerifyAll();
            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void UpdateInstanceInvokesListeners()
        {
            var customer = new Customer
            {
                Id = 187224
            };

            var sqlQuery = new SqlQuery("");
            var rowsAffected = 1;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(rowsAffected);

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildUpdateSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(sqlQuery);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(mockCommand.Object);

            int counter = 0;

            var mockListener1 = new Mock<IListener>();
            mockListener1.Setup(x => x.AfterUpdate(customer, 1)).Callback(() => Assert.Equal(4, ++counter));
            mockListener1.Setup(x => x.BeforeUpdate(customer)).Callback(() => Assert.Equal(1, ++counter));

            var mockListener2 = new Mock<IListener>();
            mockListener2.Setup(x => x.AfterUpdate(customer, 1)).Callback(() => Assert.Equal(3, ++counter));
            mockListener2.Setup(x => x.BeforeUpdate(customer)).Callback(() => Assert.Equal(2, ++counter));

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new[] { mockListener1.Object, mockListener2.Object });

            session.Update(customer);

            mockListener1.VerifyAll();
            mockListener2.VerifyAll();
        }

        [Fact]
        public void UpdateInstanceReturnsFalseIfNoRecordsUpdated()
        {
            var customer = new Customer
            {
                Id = 187224
            };

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(0);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildUpdateSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(new SqlQuery(""));

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new IListener[0]);

            Assert.False(session.Update(customer));

            mockSqlDialect.VerifyAll();
            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void UpdateInstanceReturnsTrueIfRecordUpdated()
        {
            var customer = new Customer
            {
                Id = 187224
            };

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildUpdateSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(new SqlQuery(""));

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new IListener[0]);

            Assert.True(session.Update(customer));

            mockSqlDialect.VerifyAll();
            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void UpdateInstanceThrowsArgumentNullExceptionForNullInstance()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Update((Customer)null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void UpdateInstanceThrowsMicroLiteExceptionIfExecuteNonQueryThrowsException()
        {
            var customer = new Customer
            {
                Id = 187224
            };

            var sqlQuery = new SqlQuery("");

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Throws<InvalidOperationException>();
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildUpdateSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(sqlQuery);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new IListener[0]);

            var exception = Assert.Throws<MicroLiteException>(() => session.Update(customer));

            Assert.NotNull(exception.InnerException);
            Assert.Equal(exception.InnerException.Message, exception.Message);

            // Command should still be disposed.
            mockCommand.VerifyAll();
        }

        [Fact]
        public void UpdateInstanceThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = 0
            };

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                mockDbDriver.Object,
                new IListener[0]);

            var exception = Assert.Throws<MicroLiteException>(() => session.Update(customer));

            Assert.Equal(ExceptionMessages.Session_IdentifierNotSetForUpdate, exception.Message);
        }

        [Fact]
        public void UpdateInstanceThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
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

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(0);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildUpdateSqlQuery(objectDelta)).Returns(new SqlQuery(""));

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new IListener[0]);

            Assert.False(session.Update(objectDelta));

            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void UpdateObjectDeltaReturnsTrueIfRecordUpdated()
        {
            var objectDelta = new ObjectDelta(typeof(Customer), 1234);
            objectDelta.AddChange("Name", "Fred Flintstone");

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);
            mockCommand.As<IDisposable>().Setup(x => x.Dispose());

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildUpdateSqlQuery(objectDelta)).Returns(new SqlQuery(""));

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerTransaction)).Returns(new Mock<IDbConnection>().Object);
            mockDbDriver.Setup(x => x.BuildCommand(It.IsNotNull<SqlQuery>())).Returns(mockCommand.Object);

            var session = new Session(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new IListener[0]);

            Assert.True(session.Update(objectDelta));

            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public void UpdateObjectDeltaThrowsArgumentNullExceptionForNullObjectDelta()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new IListener[0]);

            var exception = Assert.Throws<ArgumentNullException>(() => session.Update((ObjectDelta)null));

            Assert.Equal("objectDelta", exception.ParamName);
        }

        [Fact]
        public void UpdateObjectDeltaThrowsMicroLiteExceptionIfItContainsNoChanges()
        {
            var objectDelta = new ObjectDelta(typeof(Customer), 1234);

            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new IListener[0]);

            var exception = Assert.Throws<MicroLiteException>(() => session.Update(objectDelta));

            Assert.Equal(ExceptionMessages.ObjectDelta_MustContainAtLeastOneChange, exception.Message);
        }

        [Fact]
        public void UpdateObjectDeltaThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new Session(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new IListener[0]);

            using (session)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => session.Update(new ObjectDelta(typeof(Customer), 1234)));
        }
    }
}