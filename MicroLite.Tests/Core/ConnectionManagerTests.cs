namespace MicroLite.Tests.Core
{
    using System.Data;
    using MicroLite.Core;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ConnectionManager"/> class.
    /// </summary>
    public class ConnectionManagerTests
    {
        public class WhenCallingBeginTransactionAndThereIsAnActiveTransaction
        {
            private readonly ITransaction transaction1;
            private readonly ITransaction transaction2;

            public WhenCallingBeginTransactionAndThereIsAnActiveTransaction()
            {
                var mockConnection = new Mock<IDbConnection>();
                mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(new Mock<IDbTransaction>().Object);

                var connectionManager = new ConnectionManager(mockConnection.Object);
                this.transaction1 = connectionManager.BeginTransaction(IsolationLevel.ReadCommitted);
                this.transaction2 = connectionManager.BeginTransaction(IsolationLevel.ReadCommitted);
            }

            [Fact]
            public void TheActiveTransactionIsReturnedEachTime()
            {
                Assert.Same(transaction1, transaction2);
            }
        }

        public class WhenCallingBeginTransactionWithAnIsolationLevel
        {
            private readonly IsolationLevel isolationLevel = IsolationLevel.Chaos;
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly ITransaction transaction;

            public WhenCallingBeginTransactionWithAnIsolationLevel()
            {
                var mockTransaction = new Mock<IDbTransaction>();
                mockTransaction.Setup(x => x.IsolationLevel).Returns(this.isolationLevel);

                this.mockConnection.Setup(x => x.BeginTransaction(this.isolationLevel)).Returns(mockTransaction.Object);

                var connectionManager = new ConnectionManager(this.mockConnection.Object);
                this.transaction = connectionManager.BeginTransaction(this.isolationLevel);
            }

            [Fact]
            public void TheSpecifiedIsolationLevelIsUsed()
            {
                Assert.Equal(this.isolationLevel, this.transaction.IsolationLevel);
            }
        }

        public class WhenCallingCommandCompletedAndThereIsATransaction
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();

            public WhenCallingCommandCompletedAndThereIsATransaction()
            {
                var mockCommand = new Mock<IDbCommand>();
                mockCommand.Setup(x => x.Connection).Returns(mockConnection.Object);
                mockCommand.Setup(x => x.Transaction).Returns(new Mock<IDbTransaction>().Object);

                var connectionManager = new ConnectionManager(this.mockConnection.Object);
                connectionManager.CommandCompleted(mockCommand.Object);
            }

            [Fact]
            public void TheConnectionShouldNotBeClosed()
            {
                this.mockConnection.Verify(x => x.Close(), Times.Never());
            }
        }

        public class WhenCallingCommandCompletedAndThereIsNoTransaction
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();

            public WhenCallingCommandCompletedAndThereIsNoTransaction()
            {
                var mockCommand = new Mock<IDbCommand>();
                mockCommand.Setup(x => x.Connection).Returns(mockConnection.Object);

                var connectionManager = new ConnectionManager(this.mockConnection.Object);
                connectionManager.CommandCompleted(mockCommand.Object);
            }

            [Fact]
            public void TheConnectionShouldBeClosed()
            {
                this.mockConnection.Verify(x => x.Close(), Times.Once());
            }
        }

        public class WhenCallingCreateCommandAndTheConnectionIsClosed
        {
            private readonly IDbCommand command;
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();

            public WhenCallingCreateCommandAndTheConnectionIsClosed()
            {
                this.mockConnection.Setup(x => x.State).Returns(ConnectionState.Closed);
                this.mockConnection.Setup(x => x.CreateCommand()).Returns(new Mock<IDbCommand>().Object);

                var connectionManager = new ConnectionManager(this.mockConnection.Object);

                this.command = connectionManager.CreateCommand();
            }

            [Fact]
            public void TheCommandShouldBeCreated()
            {
                this.mockConnection.Verify(x => x.CreateCommand(), Times.Once());
            }

            [Fact]
            public void TheCommandShouldBeReturned()
            {
                Assert.NotNull(this.command);
            }

            [Fact]
            public void TheConnectionShouldBeOpened()
            {
                this.mockConnection.Verify(x => x.Open(), Times.Once());
            }
        }

        public class WhenCallingCreateCommandAndThereIsACurrentTransaction
        {
            private readonly IDbCommand command;
            private readonly Mock<IDbCommand> mockCommand = new Mock<IDbCommand>();
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly IDbTransaction transaction = new Mock<IDbTransaction>().Object;

            public WhenCallingCreateCommandAndThereIsACurrentTransaction()
            {
                this.mockCommand.SetupProperty(x => x.Transaction);

                this.mockConnection.Setup(x => x.State).Returns(ConnectionState.Open);
                this.mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(this.transaction);
                this.mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

                var connectionManager = new ConnectionManager(this.mockConnection.Object);
                connectionManager.BeginTransaction(IsolationLevel.ReadCommitted);

                this.command = connectionManager.CreateCommand();
            }

            [Fact]
            public void TheCommandShouldBeCreated()
            {
                this.mockConnection.Verify(x => x.CreateCommand(), Times.Once());
            }

            [Fact]
            public void TheCommandShouldBeReturned()
            {
                Assert.NotNull(this.command);
            }

            [Fact]
            public void TheTransactionShouldBeSetOnTheCommand()
            {
                Assert.Equal(this.transaction, this.command.Transaction);
            }
        }

        public class WhenCallingCreateCommandAndThereIsNoCurrentTransaction
        {
            private readonly IDbCommand command;
            private readonly Mock<IDbCommand> mockCommand = new Mock<IDbCommand>();
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();

            public WhenCallingCreateCommandAndThereIsNoCurrentTransaction()
            {
                this.mockConnection.Setup(x => x.State).Returns(ConnectionState.Open);
                this.mockConnection.Setup(x => x.CreateCommand()).Returns(mockCommand.Object);

                var connectionManager = new ConnectionManager(this.mockConnection.Object);

                this.command = connectionManager.CreateCommand();
            }

            [Fact]
            public void TheCommandShouldBeCreated()
            {
                this.mockConnection.Verify(x => x.CreateCommand(), Times.Once());
            }

            [Fact]
            public void TheCommandShouldBeReturned()
            {
                Assert.NotNull(this.command);
            }

            [Fact]
            public void TheTransactionShouldNotBeSetOnTheCommand()
            {
                this.mockCommand.VerifySet(x => x.Transaction = It.IsAny<IDbTransaction>(), Times.Never());
            }
        }

        public class WhenConstructed
        {
            private readonly ConnectionManager connectionManager = new ConnectionManager(new Mock<IDbConnection>().Object);

            [Fact]
            public void TheCurrentTransactionShouldBeNull()
            {
                Assert.Null(this.connectionManager.CurrentTransaction);
            }
        }

        public class WhenDisposed
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<IDbTransaction> mockTransaction = new Mock<IDbTransaction>();

            public WhenDisposed()
            {
                this.mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
                this.mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(this.mockTransaction.Object);

                using (var connectionManager = new ConnectionManager(this.mockConnection.Object))
                {
                    connectionManager.BeginTransaction(IsolationLevel.ReadCommitted);
                }
            }

            [Fact]
            public void TheConnectionShouldBeClosed()
            {
                this.mockConnection.Verify(x => x.Close(), Times.Once());
            }

            [Fact]
            public void TheConnectionShouldBeDisposed()
            {
                this.mockConnection.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionShouldBeDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }
        }
    }
}