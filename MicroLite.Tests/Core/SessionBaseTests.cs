namespace MicroLite.Tests.Core
{
    using System;
    using System.Data;
    using MicroLite.Core;
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
            var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerTransaction, new Mock<IDbConnection>().Object);
            mockSessionBase.CallBase = true;

            var sessionBase = mockSessionBase.Object;
            sessionBase.Dispose();

            Assert.DoesNotThrow(() => sessionBase.Dispose());
        }

        public class WhenCallingBeginTransaction
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly SessionBase sessionBase;
            private readonly ITransaction transaction;

            public WhenCallingBeginTransaction()
            {
                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerSession, mockConnection.Object);
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
                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerSession, new Mock<IDbConnection>().Object);
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

        public class WhenCallingBeginTransaction_WithAnIsolationLevel
        {
            private readonly IsolationLevel isolationLevel = IsolationLevel.Chaos;
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly SessionBase sessionBase;
            private readonly ITransaction transaction;

            public WhenCallingBeginTransaction_WithAnIsolationLevel()
            {
                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerSession, mockConnection.Object);
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

        public class WhenCallingCommandCompleted_TheConnectionScopeIsPerTransaction_ThereIsNoTransaction_AndTheConnectionIsOpen
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly TestSessionBase sessionBase;

            public WhenCallingCommandCompleted_TheConnectionScopeIsPerTransaction_ThereIsNoTransaction_AndTheConnectionIsOpen()
            {
                this.mockConnection.Setup(x => x.State).Returns(ConnectionState.Open);

                this.sessionBase = new TestSessionBase(ConnectionScope.PerTransaction, mockConnection.Object);
                this.sessionBase.CallCommandCompleted();
            }

            [Fact]
            public void TheConnectionIsClosed()
            {
                this.mockConnection.Verify(x => x.Close(), Times.Once());
            }
        }

        public class WhenCallingCreateCommand_AndTheTransactionIsNotNull
        {
            private readonly Mock<IDbCommand> mockCommand = new Mock<IDbCommand>();
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<IDbTransaction> mockTransaction = new Mock<IDbTransaction>();
            private readonly TestSessionBase sessionBase;

            public WhenCallingCreateCommand_AndTheTransactionIsNotNull()
            {
                this.mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(mockTransaction.Object);
                this.mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

                this.sessionBase = new TestSessionBase(ConnectionScope.PerSession, mockConnection.Object);
                this.sessionBase.BeginTransaction();

                this.sessionBase.CallCreateCommand();
            }

            [Fact]
            public void TheCommandIsEnlistedInTheTransaction()
            {
                this.mockCommand.VerifySet(x => x.Transaction = this.mockTransaction.Object);
            }
        }

        public class WhenCallingCreateCommand_AndTheTransactionIsNull
        {
            private readonly IDbCommand command;
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly TestSessionBase sessionBase;

            public WhenCallingCreateCommand_AndTheTransactionIsNull()
            {
                this.mockConnection.Setup(x => x.CreateCommand()).Returns(new Mock<IDbCommand>().Object);

                this.sessionBase = new TestSessionBase(ConnectionScope.PerSession, mockConnection.Object);

                this.command = this.sessionBase.CallCreateCommand();
            }

            [Fact]
            public void TheCommandIsNotEnlistedInTheTransaction()
            {
                Assert.Null(this.command.Transaction);
            }
        }

        public class WhenCallingCreateCommand_TheConnectionScopeIsPerTransaction_ThereIsNoTransaction_AndTheConnectionIsClosed
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly TestSessionBase sessionBase;

            public WhenCallingCreateCommand_TheConnectionScopeIsPerTransaction_ThereIsNoTransaction_AndTheConnectionIsClosed()
            {
                this.mockConnection.Setup(x => x.State).Returns(ConnectionState.Closed);

                this.sessionBase = new TestSessionBase(ConnectionScope.PerTransaction, mockConnection.Object);
                this.sessionBase.CallCreateCommand();
            }

            [Fact]
            public void TheConnectionIsOpenend()
            {
                this.mockConnection.Verify(x => x.Open(), Times.Once());
            }
        }

        public class WhenCallingDispose_AndTheConnectionScopeIsPerSession
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly SessionBase sessionBase;

            public WhenCallingDispose_AndTheConnectionScopeIsPerSession()
            {
                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerSession, mockConnection.Object);
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
                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerTransaction, mockConnection.Object);
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

                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerTransaction, mockConnection.Object);
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

        public class WhenCallingTransactionComplete
        {
            private readonly SessionBase sessionBase;

            public WhenCallingTransactionComplete()
            {
                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerSession, new Mock<IDbConnection>().Object);
                mockSessionBase.CallBase = true;

                this.sessionBase = mockSessionBase.Object;
                this.sessionBase.BeginTransaction();
                this.sessionBase.TransactionCompleted();
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
            private readonly SessionBase sessionBase;

            public WhenConstructed_WithConnectionScopePerSession()
            {
                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerSession, mockConnection.Object);
                mockSessionBase.CallBase = true;

                this.sessionBase = mockSessionBase.Object;
            }

            [Fact]
            public void TheConnectionIsOpened()
            {
                mockConnection.Verify(x => x.Open(), Times.Once());
            }

            [Fact]
            public void TheConnectionReturnsTheDbConnection()
            {
                Assert.Equal(this.mockConnection.Object, this.sessionBase.Connection);
            }

            [Fact]
            public void TheConnectionScopeReturnsPerSession()
            {
                Assert.Equal(ConnectionScope.PerSession, this.sessionBase.ConnectionScope);
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
            private readonly SessionBase sessionBase;

            public WhenConstructed_WithConnectionScopePerTransaction()
            {
                var mockSessionBase = new Mock<SessionBase>(ConnectionScope.PerTransaction, mockConnection.Object);
                mockSessionBase.CallBase = true;

                this.sessionBase = mockSessionBase.Object;
            }

            [Fact]
            public void TheConnectionIsNotOpened()
            {
                mockConnection.Verify(x => x.Open(), Times.Never());
            }

            [Fact]
            public void TheConnectionReturnsTheDbConnection()
            {
                Assert.Equal(this.mockConnection.Object, this.sessionBase.Connection);
            }

            [Fact]
            public void TheConnectionScopeReturnsPerTransaction()
            {
                Assert.Equal(ConnectionScope.PerTransaction, this.sessionBase.ConnectionScope);
            }

            [Fact]
            public void TheCurrentTransactionIsNull()
            {
                Assert.Null(this.sessionBase.CurrentTransaction);
            }
        }

        private class TestSessionBase : SessionBase
        {
            public TestSessionBase(ConnectionScope connectionScope, IDbConnection connection)
                : base(connectionScope, connection)
            {
            }

            public void CallCommandCompleted()
            {
                base.CommandCompleted();
            }

            public IDbCommand CallCreateCommand()
            {
                return base.CreateCommand();
            }
        }
    }
}