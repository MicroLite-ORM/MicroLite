namespace MicroLite.Tests.Core
{
    using System;
    using System.Data;
    using MicroLite.Core;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="AdoTransaction"/> class.
    /// </summary>
    [TestFixture]
    public class AdoTransactionTests
    {
        /// <summary>
        /// Issue #56 - Calling Commit more than once should throw an InvalidOperationException.
        /// </summary>
        [Test]
        public void CallingCommitTwiceShouldResultInInvalidOperationExceptionBeingThrown()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.Close());

            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(mockConnection.Object);

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Commit();

            var exception = Assert.Throws<InvalidOperationException>(() => transaction.Commit());
            Assert.AreEqual(Messages.Transaction_Completed, exception.Message);
        }

        /// <summary>
        /// Issue #58 - Calling RollBack if Committed successfully should throw an InvalidOperationException.
        /// </summary>
        [Test]
        public void CallingRollBackIfCommittedShouldResultInInvalidOperationExceptionBeingThrown()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.Close());

            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(mockConnection.Object);

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Commit();

            var exception = Assert.Throws<InvalidOperationException>(() => transaction.Rollback());
            Assert.AreEqual(Messages.Transaction_Completed, exception.Message);
        }

        /// <summary>
        /// Issue #57 - Calling RollBack more than once should throw an InvalidOperationException.
        /// </summary>
        [Test]
        public void CallingRollBackTwiceShouldResultInInvalidOperationExceptionBeingThrown()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.Close());

            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(mockConnection.Object);

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Rollback();

            var exception = Assert.Throws<InvalidOperationException>(() => transaction.Rollback());
            Assert.AreEqual(Messages.Transaction_Completed, exception.Message);
        }

        [Test]
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

        [Test]
        public void CommitCallsDbTransactionCommit()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockTransaction.Setup(x => x.Commit());

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Commit();

            mockTransaction.VerifyAll();
        }

        [Test]
        public void CommitCallsDbTransactionCommitAndReThrowsExceptionIfCommitFails()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockTransaction.Setup(x => x.Commit()).Throws<InvalidOperationException>();

            var transaction = new AdoTransaction(mockTransaction.Object);

            var exception = Assert.Throws<MicroLiteException>(() => transaction.Commit());

            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
            Assert.AreEqual(exception.Message, exception.InnerException.Message);
        }

        [Test]
        public void CommitSetsIsActiveToFalse()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Commit();

            Assert.IsFalse(transaction.IsActive);
        }

        [Test]
        public void CommitSetsWasCommittedToTrue()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Commit();

            Assert.IsTrue(transaction.WasCommitted);
        }

        [Test]
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
        [Test]
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

        [Test]
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

        /// <summary>
        /// Issue #55 - Transaction should not try and rollback if faulted.
        /// </summary>
        /// <remarks>This was because dispose was not checking faulted in addition to committed and rolledback.</remarks>
        [Test]
        public void DisposeDoesNotRollbackDbTransactionIfFaulted()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockTransaction.Setup(x => x.Commit()).Throws<InvalidOperationException>();

            using (var transaction = new AdoTransaction(mockTransaction.Object))
            {
                // Commit the transaction so that it faults.
                Assert.Throws<MicroLiteException>(() => transaction.Commit());
            }

            mockTransaction.Verify(x => x.Rollback(), Times.Never());
        }

        [Test]
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

        [Test]
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

            Assert.IsNull(command.Transaction);
        }

        [Test]
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

            Assert.IsNull(command.Transaction);
        }

        [Test]
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

            Assert.AreSame(dbTransaction, command.Transaction);
        }

        [Test]
        public void IsolationLevelReurnsTransactionIsolationLevel()
        {
            var mockDbTransaction = new Mock<IDbTransaction>();
            mockDbTransaction.Setup(x => x.IsolationLevel).Returns(IsolationLevel.Chaos);

            var dbTransaction = mockDbTransaction.Object;

            var transaction = new AdoTransaction(dbTransaction);

            Assert.AreEqual(dbTransaction.IsolationLevel, transaction.IsolationLevel);
        }

        [Test]
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

        [Test]
        public void RollbackCallsDbTransactionRollback()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockTransaction.Setup(x => x.Rollback());

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Rollback();

            mockTransaction.VerifyAll();
        }

        [Test]
        public void RollbackCallsDbTransactionRollbackAndReThrowsExceptionIfRollbackFails()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockTransaction.Setup(x => x.Rollback()).Throws<InvalidOperationException>();

            var transaction = new AdoTransaction(mockTransaction.Object);

            var exception = Assert.Throws<MicroLiteException>(() => transaction.Rollback());

            Assert.IsInstanceOf<InvalidOperationException>(exception.InnerException);
            Assert.AreEqual(exception.Message, exception.InnerException.Message);
        }

        [Test]
        public void RollbackSetsIsActiveToFalse()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Rollback();

            Assert.IsFalse(transaction.IsActive);
        }

        [Test]
        public void RollbackSetsWasRolledBackToTrue()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            var transaction = new AdoTransaction(mockTransaction.Object);
            transaction.Rollback();

            Assert.IsTrue(transaction.WasRolledBack);
        }

        [Test]
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