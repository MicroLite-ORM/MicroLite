namespace MicroLite.Tests.Core
{
    using System;
    using System.Data;
    using MicroLite.Core;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="Transaction"/> class.
    /// </summary>
    public class TransactionTests
    {
        [Fact]
        public void DisposeDoesNotThrowAnExceptionIfCalledTwice()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(new Mock<IDbTransaction>().Object);

            var mockSessionBase = new Mock<ISessionBase>();
            mockSessionBase.Setup(x => x.Connection).Returns(mockConnection.Object);

            var transaction = new Transaction(mockSessionBase.Object, IsolationLevel.ReadCommitted);
            transaction.Commit();
            transaction.Dispose();

            Assert.DoesNotThrow(() => transaction.Dispose());
        }

        public class WhenCallingCommit_AndTheConnectionScopeIsPerSession
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<ISessionBase> mockSessionBase = new Mock<ISessionBase>();
            private readonly Mock<IDbTransaction> mockTransaction = new Mock<IDbTransaction>();
            private readonly Transaction transaction;

            public WhenCallingCommit_AndTheConnectionScopeIsPerSession()
            {
                this.mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(this.mockTransaction.Object);

                this.mockSessionBase.Setup(x => x.ConnectionScope).Returns(ConnectionScope.PerSession);
                this.mockSessionBase.Setup(x => x.Connection).Returns(this.mockConnection.Object);

                this.transaction = new Transaction(this.mockSessionBase.Object, IsolationLevel.ReadCommitted);
                this.transaction.Commit();
            }

            [Fact]
            public void IsActiveReturnsFalse()
            {
                Assert.False(this.transaction.IsActive);
            }

            [Fact]
            public void SessionTransactionCompletedShouldBeCalled()
            {
                this.mockSessionBase.Verify(x => x.TransactionCompleted(), Times.Once());
            }

            [Fact]
            public void TheTransactionShouldBeCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Once());
            }
        }

        public class WhenCallingCommit_AndTheConnectionScopeIsPerTransaction
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<ISessionBase> mockSessionBase = new Mock<ISessionBase>();
            private readonly Mock<IDbTransaction> mockTransaction = new Mock<IDbTransaction>();
            private readonly Transaction transaction;

            public WhenCallingCommit_AndTheConnectionScopeIsPerTransaction()
            {
                this.mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(this.mockTransaction.Object);

                this.mockSessionBase.Setup(x => x.ConnectionScope).Returns(ConnectionScope.PerTransaction);
                this.mockSessionBase.Setup(x => x.Connection).Returns(this.mockConnection.Object);

                this.transaction = new Transaction(this.mockSessionBase.Object, IsolationLevel.ReadCommitted);
                this.transaction.Commit();
            }

            [Fact]
            public void IsActiveReturnsFalse()
            {
                Assert.False(this.transaction.IsActive);
            }

            [Fact]
            public void SessionTransactionCompletedShouldBeCalled()
            {
                this.mockSessionBase.Verify(x => x.TransactionCompleted(), Times.Once());
            }

            [Fact]
            public void TheTransactionShouldBeCommitted()
            {
                this.mockTransaction.Verify(x => x.Commit(), Times.Once());
            }
        }

        public class WhenCallingCommit_AndTheTransactionHasBeenCommitted
        {
            private readonly Transaction transaction;

            public WhenCallingCommit_AndTheTransactionHasBeenCommitted()
            {
                var mockConnection = new Mock<IDbConnection>();
                mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(new Mock<IDbTransaction>().Object);

                var mockSessionBase = new Mock<ISessionBase>();
                mockSessionBase.Setup(x => x.Connection).Returns(mockConnection.Object);

                this.transaction = new Transaction(mockSessionBase.Object, IsolationLevel.ReadCommitted);
                transaction.Commit();
            }

            [Fact]
            public void AnInvalidOperationExceptionIsThrown()
            {
                Assert.Throws<InvalidOperationException>(() => this.transaction.Commit());
            }
        }

        public class WhenCallingCommit_AndTheTransactionHasBeenRolledBack
        {
            private readonly Transaction transaction;

            public WhenCallingCommit_AndTheTransactionHasBeenRolledBack()
            {
                var mockConnection = new Mock<IDbConnection>();
                mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(new Mock<IDbTransaction>().Object);

                var mockSessionBase = new Mock<ISessionBase>();
                mockSessionBase.Setup(x => x.Connection).Returns(mockConnection.Object);

                this.transaction = new Transaction(mockSessionBase.Object, IsolationLevel.ReadCommitted);
                transaction.Rollback();
            }

            [Fact]
            public void AnInvalidOperationExceptionIsThrown()
            {
                Assert.Throws<InvalidOperationException>(() => this.transaction.Commit());
            }
        }

        public class WhenCallingCommit_AndTheTransactionIsDisposed
        {
            private readonly Transaction transaction;

            public WhenCallingCommit_AndTheTransactionIsDisposed()
            {
                var mockConnection = new Mock<IDbConnection>();
                mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(new Mock<IDbTransaction>().Object);

                var mockSessionBase = new Mock<ISessionBase>();
                mockSessionBase.Setup(x => x.Connection).Returns(mockConnection.Object);

                this.transaction = new Transaction(mockSessionBase.Object, IsolationLevel.ReadCommitted);
                transaction.Dispose();
            }

            [Fact]
            public void AnObjectDisposedExceptionIsThrown()
            {
                Assert.Throws<ObjectDisposedException>(() => this.transaction.Commit());
            }
        }

        public class WhenCallingCommit_AndTheTransactionThrowsAnException
        {
            private readonly MicroLiteException exception;
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<ISessionBase> mockSessionBase = new Mock<ISessionBase>();
            private readonly Mock<IDbTransaction> mockTransaction = new Mock<IDbTransaction>();
            private readonly Transaction transaction;

            public WhenCallingCommit_AndTheTransactionThrowsAnException()
            {
                this.mockTransaction.Setup(x => x.Commit()).Throws<InvalidOperationException>();

                this.mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(this.mockTransaction.Object);

                this.mockSessionBase.Setup(x => x.ConnectionScope).Returns(ConnectionScope.PerSession);
                this.mockSessionBase.Setup(x => x.Connection).Returns(this.mockConnection.Object);

                this.transaction = new Transaction(this.mockSessionBase.Object, IsolationLevel.ReadCommitted);

                this.exception = Assert.Throws<MicroLiteException>(() => this.transaction.Commit());
            }

            [Fact]
            public void AMicroLiteExceptionShouldBeThrown()
            {
                Assert.NotNull(this.exception);
            }

            [Fact]
            public void IsActiveReturnsFalse()
            {
                Assert.False(this.transaction.IsActive);
            }
        }

        public class WhenCallingDispose
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<ISessionBase> mockSessionBase = new Mock<ISessionBase>();
            private readonly Mock<IDbTransaction> mockTransaction = new Mock<IDbTransaction>();

            public WhenCallingDispose()
            {
                this.mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(this.mockTransaction.Object);

                this.mockSessionBase.Setup(x => x.Connection).Returns(this.mockConnection.Object);

                var transaction = new Transaction(this.mockSessionBase.Object, IsolationLevel.ReadCommitted);
                transaction.Commit();
                transaction.Dispose();
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }
        }

        public class WhenCallingDispose_AndTheTransactionHasFailed_AndHasNotBeenRolledBack
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<ISessionBase> mockSessionBase = new Mock<ISessionBase>();
            private readonly Mock<IDbTransaction> mockTransaction = new Mock<IDbTransaction>();

            public WhenCallingDispose_AndTheTransactionHasFailed_AndHasNotBeenRolledBack()
            {
                this.mockTransaction.Setup(x => x.Commit()).Throws<InvalidOperationException>();

                this.mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(this.mockTransaction.Object);

                this.mockSessionBase.Setup(x => x.Connection).Returns(this.mockConnection.Object);

                var transaction = new Transaction(this.mockSessionBase.Object, IsolationLevel.ReadCommitted);
                Assert.Throws<MicroLiteException>(() => transaction.Commit());
                transaction.Dispose();
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Once());
            }
        }

        public class WhenCallingDispose_AndTheTransactionHasNotBeenCommitted
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<ISessionBase> mockSessionBase = new Mock<ISessionBase>();
            private readonly Mock<IDbTransaction> mockTransaction = new Mock<IDbTransaction>();

            public WhenCallingDispose_AndTheTransactionHasNotBeenCommitted()
            {
                this.mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(this.mockTransaction.Object);

                this.mockSessionBase.Setup(x => x.Connection).Returns(this.mockConnection.Object);

                var transaction = new Transaction(this.mockSessionBase.Object, IsolationLevel.ReadCommitted);
                transaction.Dispose();
            }

            [Fact]
            public void TheTransactionIsDisposed()
            {
                this.mockTransaction.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionIsRolledBack()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Once());
            }
        }

        public class WhenCallingEnlist
        {
            private readonly Mock<IDbCommand> mockCommand = new Mock<IDbCommand>();
            private readonly Mock<IDbTransaction> mockTransaction = new Mock<IDbTransaction>();
            private readonly Transaction transaction;

            public WhenCallingEnlist()
            {
                var mockConnection = new Mock<IDbConnection>();
                mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(this.mockTransaction.Object);
                mockConnection.Setup(x => x.CreateCommand()).Returns(this.mockCommand.Object);

                var mockSessionBase = new Mock<ISessionBase>();
                mockSessionBase.Setup(x => x.Connection).Returns(mockConnection.Object);

                this.transaction = new Transaction(mockSessionBase.Object, IsolationLevel.ReadCommitted);
                this.transaction.Enlist(this.mockCommand.Object);
            }

            [Fact]
            public void TheTransactionIsSetOnTheCommand()
            {
                this.mockCommand.VerifySet(x => x.Transaction = this.mockTransaction.Object);
            }
        }

        public class WhenCallingEnlist_AndTheTransactionIsNotActive
        {
            private readonly Transaction transaction;

            public WhenCallingEnlist_AndTheTransactionIsNotActive()
            {
                var mockConnection = new Mock<IDbConnection>();
                mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(new Mock<IDbTransaction>().Object);

                var mockSessionBase = new Mock<ISessionBase>();
                mockSessionBase.Setup(x => x.Connection).Returns(mockConnection.Object);

                this.transaction = new Transaction(mockSessionBase.Object, IsolationLevel.ReadCommitted);
                this.transaction.Commit();
            }

            [Fact]
            public void AnInvalidOperationExceptionIsThrown()
            {
                Assert.Throws<InvalidOperationException>(() => this.transaction.Enlist(null));
            }
        }

        public class WhenCallingRollback_AndTheConnectionScopeIsPerSession
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<ISessionBase> mockSessionBase = new Mock<ISessionBase>();
            private readonly Mock<IDbTransaction> mockTransaction = new Mock<IDbTransaction>();
            private readonly Transaction transaction;

            public WhenCallingRollback_AndTheConnectionScopeIsPerSession()
            {
                this.mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(this.mockTransaction.Object);

                this.mockSessionBase.Setup(x => x.ConnectionScope).Returns(ConnectionScope.PerSession);
                this.mockSessionBase.Setup(x => x.Connection).Returns(this.mockConnection.Object);

                this.transaction = new Transaction(this.mockSessionBase.Object, IsolationLevel.ReadCommitted);
                this.transaction.Rollback();
            }

            [Fact]
            public void IsActiveReturnsFalse()
            {
                Assert.False(this.transaction.IsActive);
            }

            [Fact]
            public void SessionTransactionCompletedShouldBeCalled()
            {
                this.mockSessionBase.Verify(x => x.TransactionCompleted(), Times.Once());
            }

            [Fact]
            public void TheTransactionShouldBeRolledback()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Once());
            }
        }

        public class WhenCallingRollback_AndTheConnectionScopeIsPerTransaction
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<ISessionBase> mockSessionBase = new Mock<ISessionBase>();
            private readonly Mock<IDbTransaction> mockTransaction = new Mock<IDbTransaction>();
            private readonly Transaction transaction;

            public WhenCallingRollback_AndTheConnectionScopeIsPerTransaction()
            {
                this.mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(this.mockTransaction.Object);

                this.mockSessionBase.Setup(x => x.ConnectionScope).Returns(ConnectionScope.PerTransaction);
                this.mockSessionBase.Setup(x => x.Connection).Returns(this.mockConnection.Object);

                this.transaction = new Transaction(this.mockSessionBase.Object, IsolationLevel.ReadCommitted);
                this.transaction.Rollback();
            }

            [Fact]
            public void IsActiveReturnsFalse()
            {
                Assert.False(this.transaction.IsActive);
            }

            [Fact]
            public void SessionTransactionCompletedShouldBeCalled()
            {
                this.mockSessionBase.Verify(x => x.TransactionCompleted(), Times.Once());
            }

            [Fact]
            public void TheTransactionShouldBeRolledback()
            {
                this.mockTransaction.Verify(x => x.Rollback(), Times.Once());
            }
        }

        public class WhenCallingRollback_AndTheTransactionHasBeenCommitted
        {
            private readonly Transaction transaction;

            public WhenCallingRollback_AndTheTransactionHasBeenCommitted()
            {
                var mockConnection = new Mock<IDbConnection>();
                mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(new Mock<IDbTransaction>().Object);

                var mockSessionBase = new Mock<ISessionBase>();
                mockSessionBase.Setup(x => x.Connection).Returns(mockConnection.Object);

                this.transaction = new Transaction(mockSessionBase.Object, IsolationLevel.ReadCommitted);
                transaction.Commit();
            }

            [Fact]
            public void AnInvalidOperationExceptionIsThrown()
            {
                Assert.Throws<InvalidOperationException>(() => this.transaction.Rollback());
            }
        }

        public class WhenCallingRollback_AndTheTransactionHasBeenRolledback
        {
            private readonly Transaction transaction;

            public WhenCallingRollback_AndTheTransactionHasBeenRolledback()
            {
                var mockConnection = new Mock<IDbConnection>();
                mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(new Mock<IDbTransaction>().Object);

                var mockSessionBase = new Mock<ISessionBase>();
                mockSessionBase.Setup(x => x.Connection).Returns(mockConnection.Object);

                this.transaction = new Transaction(mockSessionBase.Object, IsolationLevel.ReadCommitted);
                transaction.Rollback();
            }

            [Fact]
            public void AnInvalidOperationExceptionIsThrown()
            {
                Assert.Throws<InvalidOperationException>(() => this.transaction.Rollback());
            }
        }

        public class WhenCallingRollback_AndTheTransactionIsDisposed
        {
            private readonly Transaction transaction;

            public WhenCallingRollback_AndTheTransactionIsDisposed()
            {
                var mockConnection = new Mock<IDbConnection>();
                mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(new Mock<IDbTransaction>().Object);

                var mockSessionBase = new Mock<ISessionBase>();
                mockSessionBase.Setup(x => x.Connection).Returns(mockConnection.Object);

                this.transaction = new Transaction(mockSessionBase.Object, IsolationLevel.ReadCommitted);
                transaction.Dispose();
            }

            [Fact]
            public void AnObjectDisposedExceptionIsThrown()
            {
                Assert.Throws<ObjectDisposedException>(() => this.transaction.Rollback());
            }
        }

        public class WhenCallingRollback_AndTheTransactionThrowsAnException
        {
            private readonly MicroLiteException exception;
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<ISessionBase> mockSessionBase = new Mock<ISessionBase>();
            private readonly Mock<IDbTransaction> mockTransaction = new Mock<IDbTransaction>();
            private readonly Transaction transaction;

            public WhenCallingRollback_AndTheTransactionThrowsAnException()
            {
                this.mockTransaction.Setup(x => x.Rollback()).Throws<InvalidOperationException>();

                this.mockConnection.Setup(x => x.BeginTransaction(It.IsAny<IsolationLevel>())).Returns(this.mockTransaction.Object);

                this.mockSessionBase.Setup(x => x.ConnectionScope).Returns(ConnectionScope.PerSession);
                this.mockSessionBase.Setup(x => x.Connection).Returns(this.mockConnection.Object);

                this.transaction = new Transaction(this.mockSessionBase.Object, IsolationLevel.ReadCommitted);

                this.exception = Assert.Throws<MicroLiteException>(() => this.transaction.Rollback());
            }

            [Fact]
            public void AMicroLiteExceptionShouldBeThrown()
            {
                Assert.NotNull(this.exception);
            }

            [Fact]
            public void IsActiveReturnsFalse()
            {
                Assert.False(this.transaction.IsActive);
            }
        }

        public class WhenConstructed
        {
            private readonly IsolationLevel isolationLevel = IsolationLevel.ReadCommitted;
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<ISessionBase> mockSessionBase = new Mock<ISessionBase>();
            private readonly Transaction transaction;

            public WhenConstructed()
            {
                this.mockSessionBase.Setup(x => x.Connection).Returns(this.mockConnection.Object);
                this.transaction = new Transaction(this.mockSessionBase.Object, this.isolationLevel);
            }

            [Fact]
            public void ATransactionIsStartedWithTheSpecifiedIsolationLevel()
            {
                this.mockConnection.Verify(x => x.BeginTransaction(this.isolationLevel), Times.Once());
            }

            [Fact]
            public void IsActiveReturnsTrue()
            {
                Assert.True(this.transaction.IsActive);
            }
        }

        public class WhenConstructed_AndTheTransactionScopeIsPerSession
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<ISessionBase> mockSessionBase = new Mock<ISessionBase>();

            public WhenConstructed_AndTheTransactionScopeIsPerSession()
            {
                this.mockSessionBase.Setup(x => x.Connection).Returns(this.mockConnection.Object);
                this.mockSessionBase.Setup(x => x.ConnectionScope).Returns(ConnectionScope.PerSession);

                var transaction = new Transaction(mockSessionBase.Object, IsolationLevel.ReadCommitted);
            }

            [Fact]
            public void TheConnectionNotIsOpened()
            {
                this.mockConnection.Verify(x => x.Open(), Times.Never());
            }
        }

        public class WhenConstructed_AndTheTransactionScopeIsPerTransaction
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();
            private readonly Mock<ISessionBase> mockSessionBase = new Mock<ISessionBase>();

            public WhenConstructed_AndTheTransactionScopeIsPerTransaction()
            {
                this.mockSessionBase.Setup(x => x.Connection).Returns(this.mockConnection.Object);
                this.mockSessionBase.Setup(x => x.ConnectionScope).Returns(ConnectionScope.PerTransaction);

                var transaction = new Transaction(mockSessionBase.Object, IsolationLevel.ReadCommitted);
            }

            [Fact]
            public void TheConnectionIsOpened()
            {
                this.mockConnection.Verify(x => x.Open(), Times.Once());
            }
        }
    }
}