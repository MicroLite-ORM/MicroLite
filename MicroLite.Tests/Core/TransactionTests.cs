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
        public class WhenCallingBegin
        {
            private readonly Mock<IDbConnection> mockConnection = new Mock<IDbConnection>();

            private readonly ITransaction transaction;

            public WhenCallingBegin()
            {
                this.mockConnection.Setup(x => x.BeginTransaction(IsolationLevel.Chaos)).Returns(new Mock<IDbTransaction>().Object);

                this.transaction = Transaction.Begin(mockConnection.Object, IsolationLevel.Chaos);
            }

            [Fact]
            public void TheConnectionShouldBeOpened()
            {
                this.mockConnection.Verify(x => x.Open(), Times.Once());
            }

            [Fact]
            public void TheTransactionShouldBeActive()
            {
                Assert.True(transaction.IsActive);
            }

            [Fact]
            public void TheTransactionShouldNotBeCommitted()
            {
                Assert.False(transaction.WasCommitted);
            }

            [Fact]
            public void TheTransactionShouldNotBeRolledBack()
            {
                Assert.False(transaction.WasRolledBack);
            }
        }
    }
}