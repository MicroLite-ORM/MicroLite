namespace MicroLite.Tests.Core
{
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
        public void BeginOpensConnectionAndBeginsTransaction()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.BeginTransaction()).Returns(new Mock<IDbTransaction>().Object);
            mockConnection.Setup(x => x.Open());

            var transaction = Transaction.Begin(mockConnection.Object);

            Assert.True(transaction.IsActive);
            Assert.False(transaction.WasCommitted);
            Assert.False(transaction.WasRolledBack);

            mockConnection.VerifyAll();
        }

        [Fact]
        public void BeginWithIsolationLevelOpensConnectionAndBeginsTransaction()
        {
            var mockConnection = new Mock<IDbConnection>();
            mockConnection.Setup(x => x.BeginTransaction(IsolationLevel.Chaos)).Returns(new Mock<IDbTransaction>().Object);
            mockConnection.Setup(x => x.Open());

            var transaction = Transaction.Begin(mockConnection.Object, IsolationLevel.Chaos);

            Assert.True(transaction.IsActive);
            Assert.False(transaction.WasCommitted);
            Assert.False(transaction.WasRolledBack);

            mockConnection.VerifyAll();
        }
    }
}