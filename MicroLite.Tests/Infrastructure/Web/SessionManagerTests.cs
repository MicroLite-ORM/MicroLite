namespace MicroLite.Infrastructure.Web.Tests
{
    using System.Data;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SessionManager"/> class.
    /// </summary>
    public class SessionManagerTests
    {
        public class WhenCallingOnActionExecutedAndManageTransactionIsFalse
        {
            private readonly Mock<IReadOnlySession> mockSession = new Mock<IReadOnlySession>();

            public WhenCallingOnActionExecutedAndManageTransactionIsFalse()
            {
                var sessionManager = new SessionManager();
                sessionManager.OnActionExecuted(this.mockSession.Object, manageTransaction: false, hasException: false);
            }

            [Fact]
            public void TheSessionShouldBeDisposed()
            {
                this.mockSession.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionShouldNotBeCommitted()
            {
                this.mockSession.Verify(x => x.Transaction.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionShouldNotBeRolledBack()
            {
                this.mockSession.Verify(x => x.Transaction.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecutedWithAnActiveBackManagedTransactionAndHasExceptionIsTrue
        {
            private readonly Mock<IReadOnlySession> mockSession = new Mock<IReadOnlySession>();

            public WhenCallingOnActionExecutedWithAnActiveBackManagedTransactionAndHasExceptionIsTrue()
            {
                this.mockSession.Setup(x => x.Transaction.WasRolledBack).Returns(false);

                var sessionManager = new SessionManager();
                sessionManager.OnActionExecuted(this.mockSession.Object, manageTransaction: true, hasException: true);
            }

            [Fact]
            public void TheSessionShouldBeDisposed()
            {
                this.mockSession.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionShouldNotBeCommitted()
            {
                this.mockSession.Verify(x => x.Transaction.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionShouldNotBeRolledBack()
            {
                this.mockSession.Verify(x => x.Transaction.Rollback(), Times.Once());
            }
        }

        public class WhenCallingOnActionExecutedWithAnActiveManagedTransactionAndHasExceptionIsFalse
        {
            private readonly Mock<IReadOnlySession> mockSession = new Mock<IReadOnlySession>();

            public WhenCallingOnActionExecutedWithAnActiveManagedTransactionAndHasExceptionIsFalse()
            {
                this.mockSession.Setup(x => x.Transaction.IsActive).Returns(true);

                var sessionManager = new SessionManager();
                sessionManager.OnActionExecuted(this.mockSession.Object, manageTransaction: true, hasException: false);
            }

            [Fact]
            public void TheSessionShouldBeDisposed()
            {
                this.mockSession.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionShouldBeCommitted()
            {
                this.mockSession.Verify(x => x.Transaction.Commit(), Times.Once());
            }

            [Fact]
            public void TheTransactionShouldNotBeRolledBack()
            {
                this.mockSession.Verify(x => x.Transaction.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecutedWithAnCompletedManagedTransactionAndHasExceptionIsFalse
        {
            private readonly Mock<IReadOnlySession> mockSession = new Mock<IReadOnlySession>();

            public WhenCallingOnActionExecutedWithAnCompletedManagedTransactionAndHasExceptionIsFalse()
            {
                this.mockSession.Setup(x => x.Transaction.IsActive).Returns(false);

                var sessionManager = new SessionManager();
                sessionManager.OnActionExecuted(this.mockSession.Object, manageTransaction: true, hasException: false);
            }

            [Fact]
            public void TheSessionShouldBeDisposed()
            {
                this.mockSession.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionShouldNotBeCommitted()
            {
                this.mockSession.Verify(x => x.Transaction.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionShouldNotBeRolledBack()
            {
                this.mockSession.Verify(x => x.Transaction.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecutedWithARolledBackManagedTransactionAndHasExceptionIsTrue
        {
            private readonly Mock<IReadOnlySession> mockSession = new Mock<IReadOnlySession>();

            public WhenCallingOnActionExecutedWithARolledBackManagedTransactionAndHasExceptionIsTrue()
            {
                this.mockSession.Setup(x => x.Transaction.WasRolledBack).Returns(true);

                var sessionManager = new SessionManager();
                sessionManager.OnActionExecuted(this.mockSession.Object, manageTransaction: true, hasException: true);
            }

            [Fact]
            public void TheSessionShouldBeDisposed()
            {
                this.mockSession.Verify(x => x.Dispose(), Times.Once());
            }

            [Fact]
            public void TheTransactionShouldNotBeCommitted()
            {
                this.mockSession.Verify(x => x.Transaction.Commit(), Times.Never());
            }

            [Fact]
            public void TheTransactionShouldNotBeRolledBack()
            {
                this.mockSession.Verify(x => x.Transaction.Rollback(), Times.Never());
            }
        }

        public class WhenCallingOnActionExecutingAndManageTransactionIsFalse
        {
            private readonly Mock<IReadOnlySession> mockSession = new Mock<IReadOnlySession>();

            public WhenCallingOnActionExecutingAndManageTransactionIsFalse()
            {
                var sessionManager = new SessionManager();
                sessionManager.OnActionExecuting(this.mockSession.Object, manageTransaction: false, isolationLevel: null);
            }

            [Fact]
            public void ATransactionShouldNotBeStarted()
            {
                this.mockSession.Verify(x => x.BeginTransaction(), Times.Never());
            }

            [Fact]
            public void ATransactionWithSpecifiedIsolationLevelShouldNotBeStarted()
            {
                this.mockSession.Verify(x => x.BeginTransaction(It.IsAny<IsolationLevel>()), Times.Never());
            }
        }

        public class WhenCallingOnActionExecutingAndManageTransactionIsTrue
        {
            private readonly Mock<IReadOnlySession> mockSession = new Mock<IReadOnlySession>();

            public WhenCallingOnActionExecutingAndManageTransactionIsTrue()
            {
                var sessionManager = new SessionManager();
                sessionManager.OnActionExecuting(this.mockSession.Object, manageTransaction: true, isolationLevel: null);
            }

            [Fact]
            public void ATransactionShouldBeStarted()
            {
                this.mockSession.Verify(x => x.BeginTransaction(), Times.Once());
            }

            [Fact]
            public void ATransactionWithSpecifiedIsolationLevelShouldNotBeStarted()
            {
                this.mockSession.Verify(x => x.BeginTransaction(It.IsAny<IsolationLevel>()), Times.Never());
            }
        }

        public class WhenCallingOnActionExecutingAndManageTransactionIsTrueAndAnIsolationLevelIsSpecified
        {
            private readonly IsolationLevel isolationLevel = IsolationLevel.Chaos;
            private readonly Mock<IReadOnlySession> mockSession = new Mock<IReadOnlySession>();

            public WhenCallingOnActionExecutingAndManageTransactionIsTrueAndAnIsolationLevelIsSpecified()
            {
                var sessionManager = new SessionManager();
                sessionManager.OnActionExecuting(this.mockSession.Object, manageTransaction: true, isolationLevel: this.isolationLevel);
            }

            [Fact]
            public void ATransactionShouldNotBeStarted()
            {
                this.mockSession.Verify(x => x.BeginTransaction(), Times.Never());
            }

            [Fact]
            public void ATransactionWithSpecifiedIsolationLevelShouldBeStarted()
            {
                this.mockSession.Verify(x => x.BeginTransaction(It.IsAny<IsolationLevel>()), Times.Once());
            }
        }
    }
}