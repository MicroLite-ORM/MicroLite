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
            public void TheCurrentTransactionShouldNotBeCommitted()
            {
                this.mockSession.Verify(x => x.CurrentTransaction.Commit(), Times.Never());
            }

            [Fact]
            public void TheCurrentTransactionShouldNotBeRolledBack()
            {
                this.mockSession.Verify(x => x.CurrentTransaction.Rollback(), Times.Never());
            }

            [Fact]
            public void TheSessionShouldBeDisposed()
            {
                this.mockSession.Verify(x => x.Dispose(), Times.Once());
            }
        }

        public class WhenCallingOnActionExecutedWithAnActiveBackManagedTransactionAndHasExceptionIsTrue
        {
            private readonly Mock<IReadOnlySession> mockSession = new Mock<IReadOnlySession>();

            public WhenCallingOnActionExecutedWithAnActiveBackManagedTransactionAndHasExceptionIsTrue()
            {
                this.mockSession.Setup(x => x.CurrentTransaction.WasRolledBack).Returns(false);

                var sessionManager = new SessionManager();
                sessionManager.OnActionExecuted(this.mockSession.Object, manageTransaction: true, hasException: true);
            }

            [Fact]
            public void TheCurrentTransactionShouldNotBeCommitted()
            {
                this.mockSession.Verify(x => x.CurrentTransaction.Commit(), Times.Never());
            }

            [Fact]
            public void TheCurrentTransactionShouldNotBeRolledBack()
            {
                this.mockSession.Verify(x => x.CurrentTransaction.Rollback(), Times.Once());
            }

            [Fact]
            public void TheSessionShouldBeDisposed()
            {
                this.mockSession.Verify(x => x.Dispose(), Times.Once());
            }
        }

        public class WhenCallingOnActionExecutedWithAnActiveManagedTransactionAndHasExceptionIsFalse
        {
            private readonly Mock<IReadOnlySession> mockSession = new Mock<IReadOnlySession>();

            public WhenCallingOnActionExecutedWithAnActiveManagedTransactionAndHasExceptionIsFalse()
            {
                this.mockSession.Setup(x => x.CurrentTransaction.IsActive).Returns(true);

                var sessionManager = new SessionManager();
                sessionManager.OnActionExecuted(this.mockSession.Object, manageTransaction: true, hasException: false);
            }

            [Fact]
            public void TheCurrentTransactionShouldBeCommitted()
            {
                this.mockSession.Verify(x => x.CurrentTransaction.Commit(), Times.Once());
            }

            [Fact]
            public void TheCurrentTransactionShouldNotBeRolledBack()
            {
                this.mockSession.Verify(x => x.CurrentTransaction.Rollback(), Times.Never());
            }

            [Fact]
            public void TheSessionShouldBeDisposed()
            {
                this.mockSession.Verify(x => x.Dispose(), Times.Once());
            }
        }

        public class WhenCallingOnActionExecutedWithAnCompletedManagedTransactionAndHasExceptionIsFalse
        {
            private readonly Mock<IReadOnlySession> mockSession = new Mock<IReadOnlySession>();

            public WhenCallingOnActionExecutedWithAnCompletedManagedTransactionAndHasExceptionIsFalse()
            {
                this.mockSession.Setup(x => x.CurrentTransaction.IsActive).Returns(false);

                var sessionManager = new SessionManager();
                sessionManager.OnActionExecuted(this.mockSession.Object, manageTransaction: true, hasException: false);
            }

            [Fact]
            public void TheCurrentTransactionShouldNotBeCommitted()
            {
                this.mockSession.Verify(x => x.CurrentTransaction.Commit(), Times.Never());
            }

            [Fact]
            public void TheCurrentTransactionShouldNotBeRolledBack()
            {
                this.mockSession.Verify(x => x.CurrentTransaction.Rollback(), Times.Never());
            }

            [Fact]
            public void TheSessionShouldBeDisposed()
            {
                this.mockSession.Verify(x => x.Dispose(), Times.Once());
            }
        }

        public class WhenCallingOnActionExecutedWithARolledBackManagedTransactionAndHasExceptionIsTrue
        {
            private readonly Mock<IReadOnlySession> mockSession = new Mock<IReadOnlySession>();

            public WhenCallingOnActionExecutedWithARolledBackManagedTransactionAndHasExceptionIsTrue()
            {
                this.mockSession.Setup(x => x.CurrentTransaction.WasRolledBack).Returns(true);

                var sessionManager = new SessionManager();
                sessionManager.OnActionExecuted(this.mockSession.Object, manageTransaction: true, hasException: true);
            }

            [Fact]
            public void TheCurrentTransactionShouldNotBeCommitted()
            {
                this.mockSession.Verify(x => x.CurrentTransaction.Commit(), Times.Never());
            }

            [Fact]
            public void TheCurrentTransactionShouldNotBeRolledBack()
            {
                this.mockSession.Verify(x => x.CurrentTransaction.Rollback(), Times.Never());
            }

            [Fact]
            public void TheSessionShouldBeDisposed()
            {
                this.mockSession.Verify(x => x.Dispose(), Times.Once());
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