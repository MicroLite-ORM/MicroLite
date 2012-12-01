namespace MicroLite.Tests.Core
{
    using System.Data;
    using MicroLite.Core;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="ConnectionManager"/> class.
    /// </summary>
    [TestFixture]
    public class ConnectionManagerTests
    {
        [Test]
        public void BeginTransactionShouldReturnANewTransactionIfTheCurrentTransactionIsNotActive()
        {
            var mockConnection = new Mock<IDbConnection>();
            var mockTransaction = new Mock<IDbTransaction>();

            mockConnection.Setup(x => x.BeginTransaction()).Returns(mockTransaction.Object);

            mockTransaction.Setup(x => x.Connection).Returns(mockConnection.Object);

            var connectionManager = new ConnectionManager(mockConnection.Object);

            var transaction1 = connectionManager.BeginTransaction();
            transaction1.Commit();

            var transaction2 = connectionManager.BeginTransaction();

            Assert.AreNotSame(transaction1, transaction2);
        }

        [Test]
        public void BeginTransactionShouldReturnTheSameTransactionEachTimeItIsCalledWhileTheCurrentTransactionIsActive()
        {
            var mockConnection = new Mock<IDbConnection>();
            var mockTransaction = new Mock<IDbTransaction>();

            mockConnection.Setup(x => x.BeginTransaction()).Returns(mockTransaction.Object);

            mockTransaction.Setup(x => x.Connection).Returns(mockConnection.Object);

            var connectionManager = new ConnectionManager(mockConnection.Object);

            var transaction1 = connectionManager.BeginTransaction();
            var transaction2 = connectionManager.BeginTransaction();

            Assert.AreSame(transaction1, transaction2);
        }

        [Test]
        public void BeginTransactionWithIsolationLevelReturnsNewTransactionIfActive()
        {
            var mockConnection = new Mock<IDbConnection>();
            var mockTransaction = new Mock<IDbTransaction>();

            mockConnection.Setup(x => x.BeginTransaction(IsolationLevel.Chaos)).Returns(mockTransaction.Object);

            mockTransaction.Setup(x => x.Connection).Returns(mockConnection.Object);

            var connectionManager = new ConnectionManager(mockConnection.Object);

            var transaction1 = connectionManager.BeginTransaction(IsolationLevel.Chaos);
            transaction1.Commit();

            var transaction2 = connectionManager.BeginTransaction(IsolationLevel.Chaos);

            Assert.AreNotSame(transaction1, transaction2);
        }

        [Test]
        public void BeginTransactionWithIsolationLevelReturnsSameTransactionIfActive()
        {
            var mockConnection = new Mock<IDbConnection>();
            var mockTransaction = new Mock<IDbTransaction>();

            mockConnection.Setup(x => x.BeginTransaction(IsolationLevel.Chaos)).Returns(mockTransaction.Object);

            mockTransaction.Setup(x => x.Connection).Returns(mockConnection.Object);

            var connectionManager = new ConnectionManager(mockConnection.Object);

            var transaction1 = connectionManager.BeginTransaction(IsolationLevel.Chaos);
            var transaction2 = connectionManager.BeginTransaction(IsolationLevel.Chaos);

            Assert.AreSame(transaction1, transaction2);
        }

        [Test]
        public void CurrentTransactionReturnsCurrentTransactionIfActive()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.BeginTransaction()).Returns(new Mock<IDbTransaction>().Object);

            var connectionManager = new ConnectionManager(mockConnection.Object);
            var transaction = connectionManager.BeginTransaction();

            Assert.AreSame(transaction, connectionManager.CurrentTransaction);
        }

        [Test]
        public void CurrentTransactionReturnsNullIfNoTransactionStarted()
        {
            var connectionManager = new ConnectionManager(new Mock<IDbConnection>().Object);

            Assert.IsNull(connectionManager.CurrentTransaction);
        }

        [Test]
        public void DisposeClosesAndDisposesConnection()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.Close());
            mockConnection.Setup(x => x.Dispose());

            using (new ConnectionManager(mockConnection.Object))
            {
            }

            mockConnection.VerifyAll();
        }

        [Test]
        public void DisposeDisposesCurrentTransaction()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockTransaction.Setup(x => x.Dispose());

            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.BeginTransaction()).Returns(mockTransaction.Object);

            using (var connectionManager = new ConnectionManager(mockConnection.Object))
            {
                connectionManager.BeginTransaction();
            }

            mockTransaction.VerifyAll();
        }

        [TestFixture]
        public class WhenCallingCreateCommandAndATransactionIsActive
        {
            private readonly IDbCommand command;
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly IDbTransaction transaction = new Mock<IDbTransaction>().Object;

            public WhenCallingCreateCommandAndATransactionIsActive()
            {
                this.mockConnection.Setup(x => x.State).Returns(ConnectionState.Open);
                this.mockConnection.Setup(x => x.BeginTransaction()).Returns(this.transaction);

                var mockTransaction = new Mock<IDbCommand>();
                mockTransaction.SetupProperty(x => x.Transaction);
                this.mockConnection.Setup(x => x.CreateCommand()).Returns(mockTransaction.Object);

                var connectionManager = new ConnectionManager(this.mockConnection.Object);
                connectionManager.BeginTransaction();

                this.command = connectionManager.CreateCommand();
            }

            [Test]
            public void TheCommandShouldBeCreated()
            {
                this.mockConnection.Verify(x => x.CreateCommand());
            }

            [Test]
            public void TheCommandShouldBeReturned()
            {
                Assert.NotNull(this.command);
            }

            [Test]
            public void TheTransactionShouldBeSetOnTheCommand()
            {
                Assert.AreEqual(this.transaction, this.command.Transaction);
            }
        }

        [TestFixture]
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

            [Test]
            public void TheCommandShouldBeCreated()
            {
                this.mockConnection.Verify(x => x.CreateCommand());
            }

            [Test]
            public void TheCommandShouldBeReturned()
            {
                Assert.NotNull(this.command);
            }

            [Test]
            public void TheConnectionShouldBeOpened()
            {
                this.mockConnection.Verify(x => x.Open());
            }
        }
    }
}