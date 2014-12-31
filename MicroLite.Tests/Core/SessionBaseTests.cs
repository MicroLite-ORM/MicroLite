namespace MicroLite.Tests.Core
{
    using System;
    using System.Data;
    using MicroLite.Core;
    using MicroLite.Driver;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SessionBase"/> class.
    /// </summary>
    public class SessionBaseTests
    {
        [Fact]
        public void DisposeDoesNotThrowAnExceptionIfCalledTwice()
        {
            var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerTransaction, new Mock<IDbDriver>().Object);
            mockSessionBase.CallBase = true;

            var sessionBase = mockSessionBase.Object;
            sessionBase.Dispose();

            Assert.DoesNotThrow(() => sessionBase.Dispose());
        }

        public class WhenCallingBeginTransaction
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<IDbDriver> mockDbDriver = new Mock<IDbDriver>();
            private readonly SessionBase sessionBase;
            private readonly ITransaction transaction;

            public WhenCallingBeginTransaction()
            {
                this.mockDbDriver.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerSession, mockDbDriver.Object);
                mockSessionBase.CallBase = true;

                this.sessionBase = mockSessionBase.Object;
                this.transaction = sessionBase.BeginTransaction();
            }

            [Fact]
            public void TheCurrentTransactionPropertyIsSetToTheReturnedTransaction()
            {
                Assert.Same(this.transaction, sessionBase.CurrentTransaction);
            }

            [Fact]
            public void TheIsolationLevelIsReadCommitted()
            {
                this.mockConnection.Verify(x => x.BeginTransaction(IsolationLevel.ReadCommitted), Times.Once());
            }

            [Fact]
            public void TheTransactionIsReturned()
            {
                Assert.NotNull(this.transaction);
                Assert.IsType<Transaction>(this.transaction);
            }
        }

        public class WhenCallingBeginTransaction_AndTheSessionIsDisposed
        {
            private readonly SessionBase sessionBase;

            public WhenCallingBeginTransaction_AndTheSessionIsDisposed()
            {
                var mockDbDriver = new Mock<IDbDriver>();
                mockDbDriver.Setup(x => x.CreateConnection()).Returns(new Mock<IDbConnection>().Object);

                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerSession, mockDbDriver.Object);
                mockSessionBase.CallBase = true;

                this.sessionBase = mockSessionBase.Object;
                this.sessionBase.Dispose();
            }

            [Fact]
            public void AnObjectDisposedExceptionIsThrown()
            {
                Assert.Throws<ObjectDisposedException>(() => this.sessionBase.BeginTransaction());
            }
        }

        public class WhenCallingBeginTransaction_AndTheTransactionScopeIsPerSession
        {
            private readonly IsolationLevel isolationLevel = IsolationLevel.Chaos;
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<IDbDriver> mockDbDriver = new Mock<IDbDriver>();
            private readonly SessionBase sessionBase;
            private readonly ITransaction transaction;

            public WhenCallingBeginTransaction_AndTheTransactionScopeIsPerSession()
            {
                this.mockDbDriver.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerSession, this.mockDbDriver.Object);
                mockSessionBase.CallBase = true;

                this.sessionBase = mockSessionBase.Object;
                this.transaction = sessionBase.BeginTransaction(this.isolationLevel);
            }

            [Fact]
            public void TheConnectionIsOpened()
            {
                this.mockConnection.Verify(x => x.Open(), Times.Once());
            }

            [Fact]
            public void TheCurrentTransactionPropertyIsSetToTheReturnedTransaction()
            {
                Assert.Same(this.transaction, sessionBase.CurrentTransaction);
            }

            [Fact]
            public void TheSpecifiedIsolationLevelIsUsed()
            {
                this.mockConnection.Verify(x => x.BeginTransaction(this.isolationLevel), Times.Once());
            }

            [Fact]
            public void TheTransactionIsReturned()
            {
                Assert.NotNull(this.transaction);
                Assert.IsType<Transaction>(this.transaction);
            }
        }

        public class WhenCallingBeginTransaction_AndTheTransactionScopeIsPerTransaction
        {
            private readonly IsolationLevel isolationLevel = IsolationLevel.Chaos;
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<IDbDriver> mockDbDriver = new Mock<IDbDriver>();
            private readonly SessionBase sessionBase;
            private readonly ITransaction transaction;

            public WhenCallingBeginTransaction_AndTheTransactionScopeIsPerTransaction()
            {
                this.mockDbDriver.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerTransaction, this.mockDbDriver.Object);
                mockSessionBase.CallBase = true;

                this.sessionBase = mockSessionBase.Object;
                this.transaction = sessionBase.BeginTransaction(this.isolationLevel);
            }

            [Fact]
            public void TheConnectionIsOpened()
            {
                this.mockConnection.Verify(x => x.Open(), Times.Once());
            }

            [Fact]
            public void TheCurrentTransactionPropertyIsSetToTheReturnedTransaction()
            {
                Assert.Same(this.transaction, sessionBase.CurrentTransaction);
            }

            [Fact]
            public void TheSpecifiedIsolationLevelIsUsed()
            {
                this.mockConnection.Verify(x => x.BeginTransaction(this.isolationLevel), Times.Once());
            }

            [Fact]
            public void TheTransactionIsReturned()
            {
                Assert.NotNull(this.transaction);
                Assert.IsType<Transaction>(this.transaction);
            }
        }

        public class WhenCallingBeginTransaction_WithAnIsolationLevel
        {
            private readonly IsolationLevel isolationLevel = IsolationLevel.Chaos;
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<IDbDriver> mockDbDriver = new Mock<IDbDriver>();
            private readonly SessionBase sessionBase;
            private readonly ITransaction transaction;

            public WhenCallingBeginTransaction_WithAnIsolationLevel()
            {
                this.mockDbDriver.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerSession, this.mockDbDriver.Object);
                mockSessionBase.CallBase = true;

                this.sessionBase = mockSessionBase.Object;
                this.transaction = sessionBase.BeginTransaction(this.isolationLevel);
            }

            [Fact]
            public void TheCurrentTransactionPropertyIsSetToTheReturnedTransaction()
            {
                Assert.Same(this.transaction, sessionBase.CurrentTransaction);
            }

            [Fact]
            public void TheSpecifiedIsolationLevelIsUsed()
            {
                this.mockConnection.Verify(x => x.BeginTransaction(this.isolationLevel), Times.Once());
            }

            [Fact]
            public void TheTransactionIsReturned()
            {
                Assert.NotNull(this.transaction);
                Assert.IsType<Transaction>(this.transaction);
            }
        }

        public class WhenCallingCommandCompleted_WithConnectionScopePerSession_ThereIsATransaction_AndTheConnectionIsOpen
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();

            public WhenCallingCommandCompleted_WithConnectionScopePerSession_ThereIsATransaction_AndTheConnectionIsOpen()
            {
                this.mockConnection.Setup(x => x.State).Returns(ConnectionState.Open);

                var mockDbDriver = new Mock<IDbDriver>();
                mockDbDriver.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

                var sessionBase = new TestSessionBase(ConnectionScope.PerSession, mockDbDriver.Object);
                sessionBase.BeginTransaction();
                sessionBase.CallCommandCompleted();
            }

            [Fact]
            public void TheConnectionIsNotClosed()
            {
                this.mockConnection.Verify(x => x.Close(), Times.Never());
            }
        }

        public class WhenCallingCommandCompleted_WithConnectionScopePerSession_ThereIsNoTransaction_AndTheConnectionIsOpen
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();

            public WhenCallingCommandCompleted_WithConnectionScopePerSession_ThereIsNoTransaction_AndTheConnectionIsOpen()
            {
                this.mockConnection.Setup(x => x.State).Returns(ConnectionState.Open);

                var mockDbDriver = new Mock<IDbDriver>();
                mockDbDriver.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

                var sessionBase = new TestSessionBase(ConnectionScope.PerSession, mockDbDriver.Object);
                sessionBase.CallCommandCompleted();
            }

            [Fact]
            public void TheConnectionIsNotClosed()
            {
                this.mockConnection.Verify(x => x.Close(), Times.Never());
            }
        }

        public class WhenCallingCommandCompleted_WithConnectionScopePerTransaction_ThereIsATransaction_AndTheConnectionIsOpen
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();

            public WhenCallingCommandCompleted_WithConnectionScopePerTransaction_ThereIsATransaction_AndTheConnectionIsOpen()
            {
                this.mockConnection.Setup(x => x.State).Returns(ConnectionState.Open);

                var mockDbDriver = new Mock<IDbDriver>();
                mockDbDriver.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

                var sessionBase = new TestSessionBase(ConnectionScope.PerTransaction, mockDbDriver.Object);
                sessionBase.BeginTransaction();
                sessionBase.CallCommandCompleted();
            }

            [Fact]
            public void TheConnectionIsNotClosed()
            {
                this.mockConnection.Verify(x => x.Close(), Times.Never());
            }
        }

        public class WhenCallingCommandCompleted_WithConnectionScopePerTransaction_ThereIsNoTransaction_AndTheConnectionIsOpen
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();

            public WhenCallingCommandCompleted_WithConnectionScopePerTransaction_ThereIsNoTransaction_AndTheConnectionIsOpen()
            {
                this.mockConnection.Setup(x => x.State).Returns(ConnectionState.Open);

                var mockDbDriver = new Mock<IDbDriver>();
                mockDbDriver.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

                var sessionBase = new TestSessionBase(ConnectionScope.PerTransaction, mockDbDriver.Object);
                sessionBase.CallCommandCompleted();
            }

            [Fact]
            public void TheConnectionIsClosed()
            {
                this.mockConnection.Verify(x => x.Close(), Times.Once());
            }
        }

        public class WhenCallingCreateCommand_WithConnectionScopePerSession_AndThereIsATransaction
        {
            private readonly Mock<IDbCommand> mockCommand = new Mock<IDbCommand>();
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<IDbDriver> mockDbDriver = new Mock<IDbDriver>();
            private readonly Mock<IDbTransaction> mockTransaction = new Mock<IDbTransaction>();
            private readonly SqlQuery sqlQuery = new SqlQuery("");

            public WhenCallingCreateCommand_WithConnectionScopePerSession_AndThereIsATransaction()
            {
                this.mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(this.mockTransaction.Object);

                this.mockDbDriver.Setup(x => x.CreateConnection()).Returns(this.mockConnection.Object);
                this.mockDbDriver.Setup(x => x.BuildCommand(sqlQuery)).Returns(this.mockCommand.Object);

                var sessionBase = new TestSessionBase(ConnectionScope.PerSession, this.mockDbDriver.Object);
                sessionBase.BeginTransaction();
                sessionBase.CallCreateCommand(sqlQuery);
            }

            [Fact]
            public void DbDriverBuildCommandIsCalled()
            {
                this.mockDbDriver.Verify(x => x.BuildCommand(sqlQuery), Times.Once());
            }

            [Fact]
            public void TheConnectionIsSetOnTheCommand()
            {
                this.mockCommand.VerifySet(x => x.Connection = this.mockConnection.Object, Times.Once());
            }

            [Fact]
            public void TheTransactionIsSetOnTheCommand()
            {
                this.mockCommand.VerifySet(x => x.Transaction = this.mockTransaction.Object, Times.Once());
            }
        }

        public class WhenCallingCreateCommand_WithConnectionScopePerSession_AndThereIsNoTransaction
        {
            private readonly Mock<IDbCommand> mockCommand = new Mock<IDbCommand>();
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<IDbDriver> mockDbDriver = new Mock<IDbDriver>();
            private readonly SqlQuery sqlQuery = new SqlQuery("");

            public WhenCallingCreateCommand_WithConnectionScopePerSession_AndThereIsNoTransaction()
            {
                this.mockDbDriver.Setup(x => x.CreateConnection()).Returns(this.mockConnection.Object);
                this.mockDbDriver.Setup(x => x.BuildCommand(sqlQuery)).Returns(this.mockCommand.Object);

                var sessionBase = new TestSessionBase(ConnectionScope.PerSession, this.mockDbDriver.Object);
                sessionBase.CallCreateCommand(sqlQuery);
            }

            [Fact]
            public void DbDriverBuildCommandIsCalled()
            {
                this.mockDbDriver.Verify(x => x.BuildCommand(sqlQuery), Times.Once());
            }

            [Fact]
            public void TheConnectionIsSetOnTheCommand()
            {
                this.mockCommand.VerifySet(x => x.Connection = this.mockConnection.Object, Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotSetOnTheCommand()
            {
                this.mockCommand.VerifySet(x => x.Transaction = It.IsNotNull<IDbTransaction>(), Times.Never());
            }
        }

        public class WhenCallingCreateCommand_WithConnectionScopePerTransaction_ThereIsATransaction_AndTheConnectionIsClosed
        {
            private readonly Mock<IDbCommand> mockCommand = new Mock<IDbCommand>();
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<IDbDriver> mockDbDriver = new Mock<IDbDriver>();
            private readonly Mock<IDbTransaction> mockTransaction = new Mock<IDbTransaction>();
            private readonly SqlQuery sqlQuery = new SqlQuery("");

            public WhenCallingCreateCommand_WithConnectionScopePerTransaction_ThereIsATransaction_AndTheConnectionIsClosed()
            {
                this.mockConnection.Setup(x => x.State).Returns(ConnectionState.Closed);
                this.mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(this.mockTransaction.Object);

                this.mockDbDriver.Setup(x => x.CreateConnection()).Returns(this.mockConnection.Object);
                this.mockDbDriver.Setup(x => x.BuildCommand(sqlQuery)).Returns(this.mockCommand.Object);

                var sessionBase = new TestSessionBase(ConnectionScope.PerTransaction, this.mockDbDriver.Object);
                sessionBase.BeginTransaction();
                sessionBase.CallCreateCommand(sqlQuery);
            }

            [Fact]
            public void DbDriverBuildCommandIsCalled()
            {
                this.mockDbDriver.Verify(x => x.BuildCommand(sqlQuery), Times.Once());
            }

            [Fact]
            public void TheConnectionIsSetOnTheCommand()
            {
                this.mockCommand.VerifySet(x => x.Connection = this.mockConnection.Object, Times.Once());
            }

            [Fact]
            public void TheTransactionIsSetOnTheCommand()
            {
                this.mockCommand.VerifySet(x => x.Transaction = this.mockTransaction.Object, Times.Once());
            }
        }

        public class WhenCallingCreateCommand_WithConnectionScopePerTransaction_ThereIsNoTransaction_AndTheConnectionIsClosed
        {
            private readonly Mock<IDbCommand> mockCommand = new Mock<IDbCommand>();
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<IDbDriver> mockDbDriver = new Mock<IDbDriver>();
            private readonly SqlQuery sqlQuery = new SqlQuery("");

            public WhenCallingCreateCommand_WithConnectionScopePerTransaction_ThereIsNoTransaction_AndTheConnectionIsClosed()
            {
                this.mockConnection.Setup(x => x.State).Returns(ConnectionState.Closed);

                this.mockDbDriver.Setup(x => x.CreateConnection()).Returns(this.mockConnection.Object);
                this.mockDbDriver.Setup(x => x.BuildCommand(sqlQuery)).Returns(this.mockCommand.Object);

                var sessionBase = new TestSessionBase(ConnectionScope.PerTransaction, this.mockDbDriver.Object);
                sessionBase.CallCreateCommand(sqlQuery);
            }

            [Fact]
            public void DbDriverBuildCommandIsCalled()
            {
                this.mockDbDriver.Verify(x => x.BuildCommand(sqlQuery), Times.Once());
            }

            [Fact]
            public void TheConnectionIsOpened()
            {
                this.mockConnection.Verify(x => x.Open(), Times.Once());
            }

            [Fact]
            public void TheConnectionIsSetOnTheCommand()
            {
                this.mockCommand.VerifySet(x => x.Connection = this.mockConnection.Object, Times.Once());
            }

            [Fact]
            public void TheTransactionIsNotSetOnTheCommand()
            {
                this.mockCommand.VerifySet(x => x.Transaction = It.IsNotNull<IDbTransaction>(), Times.Never());
            }
        }

        public class WhenCallingDispose_AndTheConnectionScopeIsPerSession
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly SessionBase sessionBase;

            public WhenCallingDispose_AndTheConnectionScopeIsPerSession()
            {
                var mockDbDriver = new Mock<IDbDriver>();
                mockDbDriver.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerSession, mockDbDriver.Object);
                mockSessionBase.CallBase = true;

                this.sessionBase = mockSessionBase.Object;
                this.sessionBase.Dispose();
            }

            [Fact]
            public void TheConnectionIsClosed()
            {
                this.mockConnection.Verify(x => x.Close(), Times.Once());
            }

            [Fact]
            public void TheConnectionIsSetToNull()
            {
                Assert.Null(this.sessionBase.Connection);
            }
        }

        public class WhenCallingDispose_AndTheConnectionScopeIsPerTransaction
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly SessionBase sessionBase;

            public WhenCallingDispose_AndTheConnectionScopeIsPerTransaction()
            {
                var mockDbDriver = new Mock<IDbDriver>();
                mockDbDriver.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerTransaction, mockDbDriver.Object);
                mockSessionBase.CallBase = true;

                this.sessionBase = mockSessionBase.Object;
                this.sessionBase.Dispose();
            }

            [Fact]
            public void TheConnectionIsNotClosed()
            {
                this.mockConnection.Verify(x => x.Close(), Times.Never());
            }

            [Fact]
            public void TheConnectionIsSetToNull()
            {
                Assert.Null(this.sessionBase.Connection);
            }
        }

        public class WhenCallingDispose_AndTheTransactionIsNotNull
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly SessionBase sessionBase;

            public WhenCallingDispose_AndTheTransactionIsNotNull()
            {
                this.mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(new Mock<IDbTransaction>().Object);

                var mockDbDriver = new Mock<IDbDriver>();
                mockDbDriver.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerTransaction, mockDbDriver.Object);
                mockSessionBase.CallBase = true;

                this.sessionBase = mockSessionBase.Object;
                this.sessionBase.BeginTransaction();
                this.sessionBase.Dispose();
            }

            [Fact]
            public void TheCurrentTransactionIsSetToNull()
            {
                Assert.Null(this.sessionBase.CurrentTransaction);
            }
        }

        public class WhenCallingTransactionCompleted_WithConnectionScopePerSession
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly SessionBase sessionBase;

            public WhenCallingTransactionCompleted_WithConnectionScopePerSession()
            {
                var mockDbDriver = new Mock<IDbDriver>();
                mockDbDriver.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerSession, mockDbDriver.Object);
                mockSessionBase.CallBase = true;

                this.sessionBase = mockSessionBase.Object;
                this.sessionBase.BeginTransaction();
                this.sessionBase.TransactionCompleted();
            }

            [Fact]
            public void TheConnectionIsNotClosed()
            {
                this.mockConnection.Verify(x => x.Close(), Times.Never());
            }

            [Fact]
            public void TheCurrentTransactionIsSetToNull()
            {
                Assert.Null(this.sessionBase.CurrentTransaction);
            }
        }

        public class WhenCallingTransactionCompleted_WithConnectionScopePerTransaction
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly SessionBase sessionBase;

            public WhenCallingTransactionCompleted_WithConnectionScopePerTransaction()
            {
                var mockDbDriver = new Mock<IDbDriver>();
                mockDbDriver.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerTransaction, mockDbDriver.Object);
                mockSessionBase.CallBase = true;

                this.sessionBase = mockSessionBase.Object;
                this.sessionBase.BeginTransaction();
                this.sessionBase.TransactionCompleted();
            }

            [Fact]
            public void TheConnectionIsClosed()
            {
                this.mockConnection.Verify(x => x.Close(), Times.Once());
            }

            [Fact]
            public void TheCurrentTransactionIsSetToNull()
            {
                Assert.Null(this.sessionBase.CurrentTransaction);
            }
        }

        public class WhenConstructed_WithConnectionScopePerSession
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<IDbDriver> mockDbDriver = new Mock<IDbDriver>();
            private readonly SessionBase sessionBase;

            public WhenConstructed_WithConnectionScopePerSession()
            {
                this.mockDbDriver.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerSession, this.mockDbDriver.Object);
                mockSessionBase.CallBase = true;

                this.sessionBase = mockSessionBase.Object;
            }

            [Fact]
            public void TheConnectionIsOpened()
            {
                this.mockConnection.Verify(x => x.Open(), Times.Once());
            }

            [Fact]
            public void TheConnectionPropertyIsSetToTheConnectionReturnedByDbDriver()
            {
                Assert.Equal(this.mockConnection.Object, this.sessionBase.Connection);
            }

            [Fact]
            public void TheConnectionScopeIsSet()
            {
                Assert.Equal(ConnectionScope.PerSession, this.sessionBase.ConnectionScope);
            }

            [Fact]
            public void TheConstructorCallsDbDriverGetConnection()
            {
                this.mockDbDriver.Verify(x => x.CreateConnection());
            }

            [Fact]
            public void TheCurrentTransactionIsNull()
            {
                Assert.Null(this.sessionBase.CurrentTransaction);
            }
        }

        public class WhenConstructed_WithConnectionScopePerTransaction
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<IDbDriver> mockDbDriver = new Mock<IDbDriver>();
            private readonly SessionBase sessionBase;

            public WhenConstructed_WithConnectionScopePerTransaction()
            {
                this.mockDbDriver.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerTransaction, this.mockDbDriver.Object);
                mockSessionBase.CallBase = true;

                this.sessionBase = mockSessionBase.Object;
            }

            [Fact]
            public void TheConnectionIsNotOpened()
            {
                this.mockConnection.Verify(x => x.Open(), Times.Never());
            }

            [Fact]
            public void TheConnectionPropertyIsSetToTheConnectionReturnedByDbDriver()
            {
                Assert.Equal(this.mockConnection.Object, this.sessionBase.Connection);
            }

            [Fact]
            public void TheConnectionScopeIsSet()
            {
                Assert.Equal(ConnectionScope.PerTransaction, this.sessionBase.ConnectionScope);
            }

            [Fact]
            public void TheConstructorCallsDbDriverGetConnection()
            {
                this.mockDbDriver.Verify(x => x.CreateConnection());
            }

            [Fact]
            public void TheCurrentTransactionIsNull()
            {
                Assert.Null(this.sessionBase.CurrentTransaction);
            }
        }

        private class TestSessionBase : SessionBase
        {
            public TestSessionBase(ConnectionScope connectionScope, IDbDriver dbDriver)
                : base(connectionScope, dbDriver)
            {
            }

            public void CallCommandCompleted()
            {
                base.CommandCompleted();
            }

            public IDbCommand CallCreateCommand(SqlQuery sqlQuery)
            {
                return base.CreateCommand(sqlQuery);
            }
        }
    }
}