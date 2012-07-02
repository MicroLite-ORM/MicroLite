namespace MicroLite.Tests.Core
{
    using System;
    using System.Data;
    using MicroLite.Core;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="Transaction"/> class.
    /// </summary>
    [TestFixture]
    public class TransactionTests
    {
        [Test]
        public void CommitCallsDbTransactionCommit()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Commit());
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            var transaction = new Transaction(mockTransaction.Object);
            transaction.Commit();

            mockTransaction.VerifyAll();
        }

        [Test]
        public void CommitCallsDbTransactionCommitAndReThrowsExceptionIfCommitFails()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Commit()).Throws<InvalidOperationException>();

            var transaction = new Transaction(mockTransaction.Object);

            var exception = Assert.Throws<MicroLiteException>(() => transaction.Commit());

            Assert.NotNull(exception.InnerException);
            Assert.AreEqual(exception.Message, exception.InnerException.Message);
        }

        /// <summary>
        /// Issue #21 - Committing transaction should close connection
        /// </summary>
        [Test]
        public void CommitClosesConnection()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.Close());

            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(mockConnection.Object);

            var transaction = new Transaction(mockTransaction.Object);
            transaction.Commit();

            mockConnection.VerifyAll();
        }

        [Test]
        public void CommitDoesNotRaiseCompleteEventIfCommitFails()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Commit()).Throws<InvalidOperationException>();

            bool completed = false;

            var transaction = new Transaction(mockTransaction.Object);
            transaction.Complete += (s, e) => completed = true;

            Assert.Throws<MicroLiteException>(() => transaction.Commit());

            Assert.IsFalse(completed);
        }

        [Test]
        public void CommitRaisesCompleteEventIfCommitSuccessful()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Commit());
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            bool completed = false;

            var transaction = new Transaction(mockTransaction.Object);
            transaction.Complete += (s, e) => completed = true;
            transaction.Commit();

            Assert.IsTrue(completed);
        }

        [Test]
        public void CommitSetsCommittedToTrue()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Commit());
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            var transaction = new Transaction(mockTransaction.Object);
            transaction.Commit();

            Assert.IsTrue(transaction.Committed);
        }

        [Test]
        public void CommitSetsIsActiveToFalseIfDbTransactionCommitErrors()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Commit()).Throws<InvalidOperationException>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            var transaction = new Transaction(mockTransaction.Object);

            try
            {
                transaction.Commit();
            }
            catch
            {
            }

            Assert.IsFalse(transaction.IsActive);
        }

        [Test]
        public void CommitSetsIsActiveToFalseIfDbTransactionCommitsSuccessfully()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Commit());
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            var transaction = new Transaction(mockTransaction.Object);
            transaction.Commit();

            Assert.IsFalse(transaction.IsActive);
        }

        [Test]
        public void ConstructorSetsCommittedToFalse()
        {
            var transaction = new Transaction(new Mock<IDbTransaction>().Object);

            Assert.IsFalse(transaction.Committed);
        }

        [Test]
        public void ConstructorSetsIsActiveToTrue()
        {
            var transaction = new Transaction(new Mock<IDbTransaction>().Object);

            Assert.IsTrue(transaction.IsActive);
        }

        [Test]
        public void ConstructorSetsRolledBackToFalse()
        {
            var transaction = new Transaction(new Mock<IDbTransaction>().Object);

            Assert.IsFalse(transaction.RolledBack);
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

            using (var transaction = new Transaction(mockTransaction.Object))
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

            using (var transaction = new Transaction(mockTransaction.Object))
            {
                transaction.Commit();
            }

            mockTransaction.Verify(x => x.Dispose(), Times.Once());
            mockTransaction.Verify(x => x.Rollback(), Times.Never());
        }

        [Test]
        public void DisposeRollsbackDbTransactionIfNotCommitedAndDisposesDbTransaction()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockTransaction.Setup(x => x.Dispose());
            mockTransaction.Setup(x => x.Rollback());

            using (new Transaction(mockTransaction.Object))
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

            var transaction = new Transaction(mockTransaction.Object);
            transaction.Commit();
            transaction.Enlist(command);

            Assert.IsNull(command.Transaction);
        }

        [Test]
        public void EnlistSetsTransactionOnDbCommandIfNotCommitted()
        {
            var dbTransaction = new Mock<IDbTransaction>().Object;

            var mockCommand = new Mock<IDbCommand>();
            mockCommand.SetupProperty(x => x.Transaction);

            var command = mockCommand.Object;

            var transaction = new Transaction(dbTransaction);
            transaction.Enlist(command);

            Assert.AreSame(dbTransaction, command.Transaction);
        }

        [Test]
        public void RollbackCallsDbTransactionRollback()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockTransaction.Setup(x => x.Rollback());

            var transaction = new Transaction(mockTransaction.Object);
            transaction.Rollback();

            mockTransaction.VerifyAll();
        }

        [Test]
        public void RollbackCallsDbTransactionRollbackAndReThrowsExceptionIfRollbackFails()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Rollback()).Throws<InvalidOperationException>();

            var transaction = new Transaction(mockTransaction.Object);

            var exception = Assert.Throws<MicroLiteException>(() => transaction.Rollback());

            Assert.NotNull(exception.InnerException);
            Assert.AreEqual(exception.Message, exception.InnerException.Message);
        }

        /// <summary>
        /// Issue #21 - Committing transaction should close connection
        /// </summary>
        [Test]
        public void RollbackClosesConnection()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.Close());

            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(mockConnection.Object);

            var transaction = new Transaction(mockTransaction.Object);
            transaction.Rollback();

            mockConnection.VerifyAll();
        }

        [Test]
        public void RollbackRaisesCompleteEventIfRollbackSuccessful()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Commit());
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);

            bool completed = false;

            var transaction = new Transaction(mockTransaction.Object);
            transaction.Complete += (s, e) => completed = true;
            transaction.Rollback();

            Assert.IsTrue(completed);
        }

        [Test]
        public void RollbackSetsIsActiveToFalseIfDbTransactionRollsBackErrors()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockTransaction.Setup(x => x.Rollback()).Throws<InvalidOperationException>();

            var transaction = new Transaction(mockTransaction.Object);

            try
            {
                transaction.Rollback();
            }
            catch
            {
            }

            Assert.IsFalse(transaction.IsActive);
        }

        [Test]
        public void RollbackSetsIsActiveToFalseIfDbTransactionRollsBackSuccessfully()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockTransaction.Setup(x => x.Rollback());

            var transaction = new Transaction(mockTransaction.Object);
            transaction.Rollback();

            Assert.IsFalse(transaction.IsActive);
        }

        [Test]
        public void RollbackSetsRolledBackToTrue()
        {
            var mockTransaction = new Mock<IDbTransaction>();
            mockTransaction.Setup(x => x.Connection).Returns(new Mock<IDbConnection>().Object);
            mockTransaction.Setup(x => x.Rollback());

            var transaction = new Transaction(mockTransaction.Object);
            transaction.Rollback();

            Assert.IsTrue(transaction.RolledBack);
        }
    }
}