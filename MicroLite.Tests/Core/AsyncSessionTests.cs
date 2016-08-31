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
    /// Unit Tests for the <see cref="AsyncSession"/> class.
    /// </summary>
    public class AsyncSessionTests : UnitTest
    {
        [Fact]
        public void AdvancedReturnsSameSessionByDifferentInterface()
        {
            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new SessionListeners());

            var advancedSession = session.Advanced;

            Assert.Same(session, advancedSession);
        }

        [Fact]
        public async void DeleteInstanceInvokesListeners()
        {
            var customer = new Customer
            {
                Id = 187224
            };

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildDeleteSqlQuery(It.IsNotNull<IObjectInfo>(), customer.Id)).Returns(new SqlQuery(""));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            int counter = 0;

            var mockListener1 = new Mock<IDeleteListener>();
            mockListener1.Setup(x => x.AfterDelete(customer, 1)).Callback(() => Assert.Equal(4, ++counter));
            mockListener1.Setup(x => x.BeforeDelete(customer)).Callback(() => Assert.Equal(1, ++counter));

            var mockListener2 = new Mock<IDeleteListener>();
            mockListener2.Setup(x => x.AfterDelete(customer, 1)).Callback(() => Assert.Equal(3, ++counter));
            mockListener2.Setup(x => x.BeforeDelete(customer)).Callback(() => Assert.Equal(2, ++counter));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners(
                    new[] { mockListener1.Object, mockListener2.Object },
                    new IInsertListener[0],
                    new IUpdateListener[0]));

            await session.DeleteAsync(customer);

            mockListener1.VerifyAll();
            mockListener2.VerifyAll();
        }

        [Fact]
        public async void DeleteInstanceReturnsFalseIfNoRecordsDeleted()
        {
            var customer = new Customer
            {
                Id = 14556
            };

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildDeleteSqlQuery(It.IsNotNull<IObjectInfo>(), customer.Id)).Returns(new SqlQuery(""));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(0);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners());

            Assert.False(await session.DeleteAsync(customer));

            mockSqlDialect.VerifyAll();
            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public async void DeleteInstanceReturnsTrueIfRecordDeleted()
        {
            var customer = new Customer
            {
                Id = 14556
            };

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildDeleteSqlQuery(It.IsNotNull<IObjectInfo>(), customer.Id)).Returns(new SqlQuery(""));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners());

            Assert.True(await session.DeleteAsync(customer));

            mockSqlDialect.VerifyAll();
            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public async void DeleteInstanceThrowsArgumentNullExceptionForNullInstance()
        {
            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new SessionListeners());

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await session.DeleteAsync(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public async void DeleteInstanceThrowsMicroLiteExceptionIfExecuteNonQueryThrowsException()
        {
            var customer = new Customer
            {
                Id = 187224
            };

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildDeleteSqlQuery(It.IsNotNull<IObjectInfo>(), customer.Id)).Returns(new SqlQuery(""));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Throws<InvalidOperationException>();

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners());

            var exception = await Assert.ThrowsAsync<MicroLiteException>(
                async () => await session.DeleteAsync(customer));

            Assert.IsType<InvalidOperationException>(exception.InnerException);
            Assert.Equal(exception.InnerException.Message, exception.Message);

            // Command should still be disposed.
            mockCommand.VerifyAll();
        }

        [Fact]
        public async void DeleteInstanceThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = 0
            };

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(new Mock<IDbCommand>().Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                mockDbDriver.Object,
                new SessionListeners());

            var exception = await Assert.ThrowsAsync<MicroLiteException>(
                async () => await session.DeleteAsync(customer));

            Assert.Equal(ExceptionMessages.Session_IdentifierNotSetForDelete, exception.Message);
        }

        [Fact]
        public async void DeleteInstanceThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new SessionListeners());

            using (session)
            {
            }

            await Assert.ThrowsAsync<ObjectDisposedException>(
                async () => await session.DeleteAsync(new Customer()));
        }

        [Fact]
        public async void DeleteTypeByIdentifierReturnsFalseIfNoRecordsDeleted()
        {
            var type = typeof(Customer);
            var identifier = 1234;

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildDeleteSqlQuery(It.IsNotNull<IObjectInfo>(), identifier)).Returns(new SqlQuery(""));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(0);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners());

            Assert.False(await session.DeleteAsync(type, identifier));

            mockSqlDialect.VerifyAll();
            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public async void DeleteTypeByIdentifierReturnsTrueIfRecordDeleted()
        {
            var type = typeof(Customer);
            var identifier = 1234;

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildDeleteSqlQuery(It.IsNotNull<IObjectInfo>(), identifier)).Returns(new SqlQuery(""));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners());

            Assert.True(await session.DeleteAsync(type, identifier));

            mockSqlDialect.VerifyAll();
            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public async void DeleteTypeByIdentifierThrowsArgumentNullExceptionForNullIdentifier()
        {
            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new SessionListeners());

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await session.DeleteAsync(typeof(Customer), null));

            Assert.Equal("identifier", exception.ParamName);
        }

        [Fact]
        public async void DeleteTypeByIdentifierThrowsArgumentNullExceptionForNullType()
        {
            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new SessionListeners());

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await session.DeleteAsync(null, 1234));
        }

        [Fact]
        public async void DeleteTypeByIdentifierThrowsMicroLiteExceptionIfExecuteNonQueryThrowsException()
        {
            var type = typeof(Customer);
            var identifier = 1234;

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildDeleteSqlQuery(It.IsNotNull<IObjectInfo>(), identifier)).Returns(new SqlQuery(""));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Throws<InvalidOperationException>();

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners());

            var exception = await Assert.ThrowsAsync<MicroLiteException>(
                async () => await session.DeleteAsync(type, identifier));

            Assert.IsType<InvalidOperationException>(exception.InnerException);
            Assert.Equal(exception.InnerException.Message, exception.Message);

            // Command should still be disposed.
            mockCommand.VerifyAll();
        }

        [Fact]
        public async void DeleteTypeByIdentifierThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new SessionListeners());

            using (session)
            {
            }

            await Assert.ThrowsAsync<ObjectDisposedException>(
                async () => await session.DeleteAsync(typeof(Customer), 1234));
        }

        [Fact]
        public async void ExecuteBuildsAndExecutesCommandNotInTransaction()
        {
            var result = 1;

            var mockSqlDialect = new Mock<ISqlDialect>();

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(result);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners());

            Assert.Equal(result, await session.ExecuteAsync(new SqlQuery("")));

            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public async void ExecuteScalarBuildsAndExecutesCommand()
        {
            var result = new object();

            var mockSqlDialect = new Mock<ISqlDialect>();

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(result);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners());

            Assert.Equal(result, await session.ExecuteScalarAsync<object>(new SqlQuery("")));

            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public async void ExecuteScalarThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new SessionListeners());

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await session.ExecuteScalarAsync<object>(null));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        [Fact]
        public async void ExecuteScalarThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new SessionListeners());

            using (session)
            {
            }

            await Assert.ThrowsAsync<ObjectDisposedException>(
                async () => await session.ExecuteScalarAsync<int>(new SqlQuery("SELECT")));
        }

        [Fact]
        public async void ExecuteScalarUsesTypeConvertersToResolveResultType()
        {
            var result = (byte)1;

            var mockSqlDialect = new Mock<ISqlDialect>();

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(result);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners());

            Assert.Equal((CustomerStatus)result, await session.ExecuteScalarAsync<CustomerStatus>(new SqlQuery("")));

            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public async void ExecuteThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new SessionListeners());

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await session.ExecuteAsync(null));

            Assert.Equal("sqlQuery", exception.ParamName);
        }

        [Fact]
        public async void ExecuteThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new SessionListeners());

            using (session)
            {
            }

            await Assert.ThrowsAsync<ObjectDisposedException>(
                async () => await session.ExecuteAsync(new SqlQuery("SELECT")));
        }

        [Fact]
        public async void InsertBuildsAndExecutesAnInsertCommandOnlyIfIdentifierStrategyAssigned()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var customer = new Customer
            {
                Id = 12345
            };

            var insertSqlQuery = new SqlQuery("INSERT");

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildInsertSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(insertSqlQuery);
            mockSqlDialect.Setup(x => x.SupportsSelectInsertedIdentifier).Returns(false);

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners());

            await session.InsertAsync(customer);

            mockSqlDialect.Verify(x => x.BuildInsertSqlQuery(It.IsNotNull<IObjectInfo>(), customer), Times.Once());
            mockSqlDialect.Verify(x => x.BuildSelectInsertIdSqlQuery(It.IsAny<IObjectInfo>()), Times.Never());

            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public async void InsertBuildsAndExecutesAnInsertCommandOnlyIfIdentifierStrategyNotAssignedAndSqlDialectDoesNotSupportSelectInsertedIdentifier()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer();
            var insertSqlQuery = new SqlQuery("INSERT");
            object identifier = 23543;

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildInsertSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(insertSqlQuery);
            mockSqlDialect.Setup(x => x.SupportsSelectInsertedIdentifier).Returns(false);

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(identifier);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners());

            await session.InsertAsync(customer);

            mockSqlDialect.Verify(x => x.BuildInsertSqlQuery(It.IsNotNull<IObjectInfo>(), customer), Times.Once());
            mockSqlDialect.Verify(x => x.BuildSelectInsertIdSqlQuery(It.IsAny<IObjectInfo>()), Times.Never());

            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public async void InsertBuildsAndExecutesCombinedCommandIfIdentifierStrategyNotAssignedAndSqlDialectSupportsSelectInsertedIdentifierAndDbDriverSupportsBatchedQueries()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer();
            var insertSqlQuery = new SqlQuery("INSERT");
            var selectIdSqlQuery = new SqlQuery("SELECT");
            var combinedSqlQuery = new SqlQuery("INSERT;SELECT");
            object identifier = 23543;

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildInsertSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(insertSqlQuery);
            mockSqlDialect.Setup(x => x.BuildSelectInsertIdSqlQuery(It.IsNotNull<IObjectInfo>())).Returns(selectIdSqlQuery);
            mockSqlDialect.Setup(x => x.SupportsSelectInsertedIdentifier).Returns(true);

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(identifier);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));
            mockDbDriver.Setup(x => x.SupportsBatchedQueries).Returns(true);
            mockDbDriver.Setup(x => x.Combine(insertSqlQuery, selectIdSqlQuery)).Returns(combinedSqlQuery);

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners());

            await session.InsertAsync(customer);

            mockSqlDialect.VerifyAll();
            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public async void InsertBuildsAndExecutesIndividualCommandsIfIdentifierStrategyNotAssignedAndSqlDialectSupportsSelectInsertedIdentifierAndDbDriverDoesNotSupportBatchedQueries()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer();
            var insertSqlQuery = new SqlQuery("INSERT");
            var selectIdSqlQuery = new SqlQuery("SELECT");
            object identifier = 23543;

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildInsertSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(insertSqlQuery);
            mockSqlDialect.Setup(x => x.BuildSelectInsertIdSqlQuery(It.IsNotNull<IObjectInfo>())).Returns(selectIdSqlQuery);
            mockSqlDialect.Setup(x => x.SupportsSelectInsertedIdentifier).Returns(true);

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery());
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(identifier);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));
            mockDbDriver.Setup(x => x.SupportsBatchedQueries).Returns(false);

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners());

            await session.InsertAsync(customer);

            mockSqlDialect.VerifyAll();
            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public async void InsertInvokesListeners()
        {
            var customer = new Customer();
            object identifier = 23543;

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildInsertSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(new SqlQuery(""));
            mockSqlDialect.Setup(x => x.BuildSelectInsertIdSqlQuery(It.IsNotNull<IObjectInfo>())).Returns(new SqlQuery(""));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteScalar()).Returns(identifier);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            int counter = 0;

            var mockListener1 = new Mock<IInsertListener>();
            mockListener1.Setup(x => x.AfterInsert(customer, identifier)).Callback(() => Assert.Equal(4, ++counter));
            mockListener1.Setup(x => x.BeforeInsert(customer)).Callback(() => Assert.Equal(1, ++counter));

            var mockListener2 = new Mock<IInsertListener>();
            mockListener2.Setup(x => x.AfterInsert(customer, identifier)).Callback(() => Assert.Equal(3, ++counter));
            mockListener2.Setup(x => x.BeforeInsert(customer)).Callback(() => Assert.Equal(2, ++counter));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners(
                    new IDeleteListener[0],
                    new[] { mockListener1.Object, mockListener2.Object },
                    new IUpdateListener[0]));

            await session.InsertAsync(customer);

            mockListener1.VerifyAll();
            mockListener2.VerifyAll();
        }

        [Fact]
        public async void InsertThrowsArgumentNullExceptionForNullInstance()
        {
            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new SessionListeners());

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await session.InsertAsync(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public async void InsertThrowsMicroLiteExceptionIfExecuteScalarThrowsException()
        {
            var customer = new Customer();

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildInsertSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(new SqlQuery(""));
            mockSqlDialect.Setup(x => x.BuildSelectInsertIdSqlQuery(It.IsNotNull<IObjectInfo>())).Returns(new SqlQuery(""));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteScalar()).Throws<InvalidOperationException>();

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners());

            var exception = await Assert.ThrowsAsync<MicroLiteException>(
                async () => await session.InsertAsync(customer));

            Assert.IsType<InvalidOperationException>(exception.InnerException);
            Assert.Equal(exception.InnerException.Message, exception.Message);

            // Command should still be disposed.
            mockCommand.VerifyAll();
        }

        [Fact]
        public async void InsertThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new SessionListeners());

            using (session)
            {
            }

            await Assert.ThrowsAsync<ObjectDisposedException>(
                async () => await session.InsertAsync(new Customer()));
        }

        [Fact]
        public async void UpdateInstanceBuildsAndExecutesQuery()
        {
            var customer = new Customer
            {
                Id = 187224
            };

            var rowsAffected = 1;

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildUpdateSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(new SqlQuery(""));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(rowsAffected);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners());

            await session.UpdateAsync(customer);

            mockSqlDialect.VerifyAll();
            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public async void UpdateInstanceInvokesListeners()
        {
            var customer = new Customer
            {
                Id = 187224
            };

            var rowsAffected = 1;

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildUpdateSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(new SqlQuery(""));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(rowsAffected);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            int counter = 0;

            var mockListener1 = new Mock<IUpdateListener>();
            mockListener1.Setup(x => x.AfterUpdate(customer, 1)).Callback(() => Assert.Equal(4, ++counter));
            mockListener1.Setup(x => x.BeforeUpdate(customer)).Callback(() => Assert.Equal(1, ++counter));

            var mockListener2 = new Mock<IUpdateListener>();
            mockListener2.Setup(x => x.AfterUpdate(customer, 1)).Callback(() => Assert.Equal(3, ++counter));
            mockListener2.Setup(x => x.BeforeUpdate(customer)).Callback(() => Assert.Equal(2, ++counter));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners(
                    new IDeleteListener[0],
                    new IInsertListener[0],
                    new[] { mockListener1.Object, mockListener2.Object }));

            await session.UpdateAsync(customer);

            mockListener1.VerifyAll();
            mockListener2.VerifyAll();
        }

        [Fact]
        public async void UpdateInstanceReturnsFalseIfNoRecordsUpdated()
        {
            var customer = new Customer
            {
                Id = 187224
            };

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildUpdateSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(new SqlQuery(""));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(0);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners());

            Assert.False(await session.UpdateAsync(customer));

            mockSqlDialect.VerifyAll();
            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public async void UpdateInstanceReturnsTrueIfRecordUpdated()
        {
            var customer = new Customer
            {
                Id = 187224
            };

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildUpdateSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(new SqlQuery(""));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners());

            Assert.True(await session.UpdateAsync(customer));

            mockSqlDialect.VerifyAll();
            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public async void UpdateInstanceThrowsArgumentNullExceptionForNullInstance()
        {
            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new SessionListeners());

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await session.UpdateAsync((Customer)null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public async void UpdateInstanceThrowsMicroLiteExceptionIfExecuteNonQueryThrowsException()
        {
            var customer = new Customer
            {
                Id = 187224
            };

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildUpdateSqlQuery(It.IsNotNull<IObjectInfo>(), customer)).Returns(new SqlQuery(""));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Throws<InvalidOperationException>();

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners());

            var exception = await Assert.ThrowsAsync<MicroLiteException>(
                async () => await session.UpdateAsync(customer));

            Assert.IsType<InvalidOperationException>(exception.InnerException);
            Assert.Equal(exception.InnerException.Message, exception.Message);

            // Command should still be disposed.
            mockCommand.VerifyAll();
        }

        [Fact]
        public async void UpdateInstanceThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = 0
            };

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(new Mock<IDbConnection>().Object));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                mockDbDriver.Object,
                new SessionListeners());

            var exception = await Assert.ThrowsAsync<MicroLiteException>(
                async () => await session.UpdateAsync(customer));

            Assert.Equal(ExceptionMessages.Session_IdentifierNotSetForUpdate, exception.Message);
        }

        [Fact]
        public async void UpdateInstanceThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new SessionListeners());

            using (session)
            {
            }

            await Assert.ThrowsAsync<ObjectDisposedException>(
                async () => await session.UpdateAsync(new Customer()));
        }

        [Fact]
        public async void UpdateObjectDeltaReturnsFalseIfNoRecordUpdated()
        {
            var objectDelta = new ObjectDelta(typeof(Customer), 1234);
            objectDelta.AddChange("Name", "Fred Flintstone");

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildUpdateSqlQuery(objectDelta)).Returns(new SqlQuery(""));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(0);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners());

            Assert.False(await session.UpdateAsync(objectDelta));

            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public async void UpdateObjectDeltaReturnsTrueIfRecordUpdated()
        {
            var objectDelta = new ObjectDelta(typeof(Customer), 1234);
            objectDelta.AddChange("Name", "Fred Flintstone");

            var mockSqlDialect = new Mock<ISqlDialect>();
            mockSqlDialect.Setup(x => x.BuildUpdateSqlQuery(objectDelta)).Returns(new SqlQuery(""));

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.Setup(x => x.ExecuteNonQuery()).Returns(1);

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

            var mockDbDriver = new Mock<IDbDriver>();
            mockDbDriver.Setup(x => x.CreateConnection()).Returns(new MockDbConnectionWrapper(mockConnection.Object));

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                mockSqlDialect.Object,
                mockDbDriver.Object,
                new SessionListeners());

            Assert.True(await session.UpdateAsync(objectDelta));

            mockDbDriver.VerifyAll();
            mockCommand.VerifyAll();
        }

        [Fact]
        public async void UpdateObjectDeltaThrowsArgumentNullExceptionForNullObjectDelta()
        {
            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new SessionListeners());

            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                async () => await session.UpdateAsync((ObjectDelta)null));

            Assert.Equal("objectDelta", exception.ParamName);
        }

        [Fact]
        public async void UpdateObjectDeltaThrowsMicroLiteExceptionIfItContainsNoChanges()
        {
            var objectDelta = new ObjectDelta(typeof(Customer), 1234);

            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new SessionListeners());

            var exception = await Assert.ThrowsAsync<MicroLiteException>(
                async () => await session.UpdateAsync(objectDelta));

            Assert.Equal(ExceptionMessages.ObjectDelta_MustContainAtLeastOneChange, exception.Message);
        }

        [Fact]
        public async void UpdateObjectDeltaThrowsObjectDisposedExceptionIfDisposed()
        {
            var session = new AsyncSession(
                ConnectionScope.PerTransaction,
                new Mock<ISqlDialect>().Object,
                new Mock<IDbDriver>().Object,
                new SessionListeners());

            using (session)
            {
            }

            await Assert.ThrowsAsync<ObjectDisposedException>(
                async () => await session.UpdateAsync(new ObjectDelta(typeof(Customer), 1234)));
        }
    }
}