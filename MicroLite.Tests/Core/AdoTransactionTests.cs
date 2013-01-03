namespace MicroLite.Tests.Core
{
    using System;
    using System.Data;
    using MicroLite.Core;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="AdoTransaction"/> class.
    /// </summary>
    public class AdoTransactionTests
    {
        /// <summary>
        /// Issue #56 - Calling Commit more than once should throw an InvalidOperationException.
        /// </summary>
        [Fact]
        public void CallingCommitTwiceShouldResultInInvalidOperationExceptionBeingThrown()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.Close());

            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(mockConnection.Object);

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Commit();

            var exception = Assert.Throws<InvalidOperationException>(() => transaction.Commit());
            Assert.Equal(Messages.Transaction_Completed, exception.Message);
        }

        /// <summary>
        /// Issue #58 - Calling RollBack if Committed successfully should throw an InvalidOperationException.
        /// </summary>
        [Fact]
        public void CallingRollBackIfCommittedShouldResultInInvalidOperationExceptionBeingThrown()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.Close());

            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(mockConnection.Object);

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Commit();

            var exception = Assert.Throws<InvalidOperationException>(() => transaction.Rollback());
            Assert.Equal(Messages.Transaction_Completed, exception.Message);
        }

        /// <summary>
        /// Issue #57 - Calling RollBack more than once should throw an InvalidOperationException.
        /// </summary>
        [Fact]
        public void CallingRollBackTwiceShouldResultInInvalidOperationExceptionBeingThrown()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.Close());

            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(mockConnection.Object);

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Rollback();

            var exception = Assert.Throws<InvalidOperationException>(() => transaction.Rollback());
            Assert.Equal(Messages.Transaction_Completed, exception.Message);
        }

        [Fact]
        public void CommitCallsDbConnectionClose()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.Close());

            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(mockConnection.Object);

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Commit();

            mockConnection.VerifyAll();
        }

        [Fact]
        public void CommitCallsDbTransactionCommit()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockTransaction.Setup(x => x.Commit());

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Commit();

            mockTransaction.VerifyAll();
        }

        [Fact]
        public void CommitCallsDbTransactionCommitAndReThrowsExceptionIfCommitFails()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockTransaction.Setup(x => x.Commit()).Throws<InvalidOperationException>();

            var transaction = new AdoTransaction(mockTransaction.Object);

            var exception = Assert.Throws<MicroLiteException>(() => transaction.Commit());

            Assert.IsType<InvalidOperationException>(exception.InnerException);
            Assert.Equal(exception.Message, exception.InnerException.Message);
        }

        [Fact]
        public void CommitSetsIsActiveToFalse()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Commit();

            Assert.False(transaction.IsActive);
        }

        [Fact]
        public void CommitSetsWasCommittedToTrue()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Commit();

            Assert.True(transaction.WasCommitted);
        }

        [Fact]
        public void CommitThrowsObjectDisposedExceptionIfDisposed()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            var transaction = new AdoTransaction(mockTransaction.Object);

            using (transaction)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => transaction.Commit());
        }

        /// <summary>
        /// Issue #48 - This SqlTransaction has completed; it is no longer usable. thrown by Transaction.Dispose.
        /// </summary>
        /// <remarks>
        /// This was because dispose also called rollback if the transaction was not committed.
        /// </remarks>
        [Fact]
        public void DisposeDoesNotRollbackDbTransactionIfAlreadyRolledback()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockTransaction.Setup(x => x.Rollback());

            using (var transaction = new AdoTransaction(mockTransaction.Object))
            {
                // Explicitly roll back the transaction.
                transaction.Rollback();
            }

            mockTransaction.Verify(x => x.Rollback(), Times.Once());
        }

        [Fact]
        public void DisposeDoesNotRollbackDbTransactionIfCommittedAndDisposesDbTransaction()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockTransaction.Setup(x => x.Dispose());
            mockTransaction.Setup(x => x.Rollback());

            using (var transaction = new AdoTransaction(mockTransaction.Object))
            {
                transaction.Commit();
            }

            mockTransaction.Verify(x => x.Dispose(), Times.Once());
            mockTransaction.Verify(x => x.Rollback(), Times.Never());
        }

        [Fact]
        public void DisposeRollsbackDbTransactionIfNotCommitedAndDisposesDbTransaction()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            mockTransaction.Setup(x => x.Dispose());
            mockTransaction.Setup(x => x.Rollback());

            using (var transaction = new AdoTransaction(mockTransaction.Object))
            {
            }

            mockTransaction.VerifyAll();
        }

        /// <summary>
        /// This reverts the behaviour of #55 but without breaking the logic that caused #55 initially.
        /// </summary>
        [Fact]
        public void DisposeRollsbackDbTransactionIfNotCommitedFaulted()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockTransaction.Setup(x => x.Commit()).Throws<InvalidOperationException>();

            using (var transaction = new AdoTransaction(mockTransaction.Object))
            {
                // Commit the transaction so that it faults.
                Assert.Throws<MicroLiteException>(() => transaction.Commit());
            }

            mockTransaction.Verify(x => x.Rollback(), Times.Once());
        }

        [Fact]
        public void EnlistDoesNotSetTransactionOnDbCommandIfCommitted()
        {
            var mockCommand = new Mock<IDbCommand>();
            mockCommand.SetupProperty(x => x.Transaction);

            var command = mockCommand.Object;

            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Commit();
            transaction.Enlist(command);

            Assert.Null(command.Transaction);
        }

        [Fact]
        public void EnlistDoesNotSetTransactionOnDbCommandIfRolledBack()
        {
            var mockCommand = new Mock<IDbCommand>();
            mockCommand.SetupProperty(x => x.Transaction);

            var command = mockCommand.Object;

            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Rollback();
            transaction.Enlist(command);

            Assert.Null(command.Transaction);
        }

        [Fact]
        public void EnlistSetsTransactionOnDbCommandIfNotCommitted()
        {
            var mockCommand = new Mock<IDbCommand>();
            mockCommand.SetupProperty(x => x.Transaction);

            var command = mockCommand.Object;

            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            var dbTransaction = mockTransaction.Object;

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Enlist(command);

            Assert.Same(dbTransaction, command.Transaction);
        }

        [Fact]
        public void EnlistThrowsArgumentNullExceptionForNullDbCommand()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            var transaction = new AdoTransaction(mockTransaction.Object);

            var exception = Assert.Throws<ArgumentNullException>(() => transaction.Enlist(null));

            Assert.Equal("command", exception.ParamName);
        }

        [Fact]
        public void IsolationLevelReurnsTransactionIsolationLevel()
        {
            var mockDbTransaction = new Mock<IDbTransaction>();
            mockDbTransaction.Setup(x => x.IsolationLevel).Returns(IsolationLevel.Chaos);

            var dbTransaction = mockDbTransaction.Object;

            var transaction = new AdoTransaction(dbTransaction);

            Assert.Equal(dbTransaction.IsolationLevel, transaction.IsolationLevel);
        }

        [Fact]
        public void RollbackCallsDbConnectionClose()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.Close());

            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(mockConnection.Object);

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Rollback();

            mockConnection.VerifyAll();
        }

        [Fact]
        public void RollbackCallsDbTransactionRollback()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockTransaction.Setup(x => x.Rollback());

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Rollback();

            mockTransaction.VerifyAll();
        }

        [Fact]
        public void RollbackCallsDbTransactionRollbackAndReThrowsExceptionIfRollbackFails()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockTransaction.Setup(x => x.Rollback()).Throws<InvalidOperationException>();

            var transaction = new AdoTransaction(mockTransaction.Object);

            var exception = Assert.Throws<MicroLiteException>(() => transaction.Rollback());

            Assert.IsType<InvalidOperationException>(exception.InnerException);
            Assert.Equal(exception.Message, exception.InnerException.Message);
        }

        [Fact]
        public void RollbackSetsIsActiveToFalse()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Rollback();

            Assert.False(transaction.IsActive);
        }

        [Fact]
        public void RollbackSetsWasRolledBackToTrue()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Rollback();

            Assert.True(transaction.WasRolledBack);
        }

        [Fact]
        public void RollbackThrowsObjectDisposedExceptionIfDisposed()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            var transaction = new AdoTransaction(mockTransaction.Object);

            using (transaction)
            {
            }

            Assert.Throws<ObjectDisposedException>(() => transaction.Rollback());
        }
    }
}