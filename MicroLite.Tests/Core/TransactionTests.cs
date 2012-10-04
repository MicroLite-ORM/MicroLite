namespace MicroLite.Tests.Core
{
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
        public void BeginOpensConnectionAndBeginsTransaction()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.BeginTransaction()).Returns(new Mock<IDbTransaction>().Object);
            mockConnection.Setup(x => x.Open());

            var transaction = Transaction.Begin(mockConnection.Object);

            Assert.IsTrue(transaction.IsActive);
            Assert.IsFalse(transaction.WasCommitted);
            Assert.IsFalse(transaction.WasRolledBack);

            mockConnection.VerifyAll();
        }

        [Test]
        public void BeginWithIsolationLevelOpensConnectionAndBeginsTransaction()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.BeginTransaction(IsolationLevel.Chaos)).Returns(new Mock<IDbTransaction>().Object);
            mockConnection.Setup(x => x.Open());

            var transaction = Transaction.Begin(mockConnection.Object, IsolationLevel.Chaos);

            Assert.IsTrue(transaction.IsActive);
            Assert.IsFalse(transaction.WasCommitted);
            Assert.IsFalse(transaction.WasRolledBack);

            mockConnection.VerifyAll();
        }
    }
}