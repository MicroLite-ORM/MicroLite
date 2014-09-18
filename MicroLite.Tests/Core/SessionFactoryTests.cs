namespace MicroLite.Tests.Core
{
    using MicroLite.Core;
    using MicroLite.Dialect;
    using MicroLite.Driver;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SessionFactory"/> class.
    /// </summary>
    public class SessionFactoryTests
    {
#if NET_4_5

        public class WhenCallingOpenAsyncReadOnlySession : UnitTest
        {
            private readonly IAsyncReadOnlySession readOnlySession;

            public WhenCallingOpenAsyncReadOnlySession()
            {
                var sessionFactory = new SessionFactory("SqlConnection", new Mock<IDbDriver>().Object, new Mock<ISqlDialect>().Object);

                this.readOnlySession = sessionFactory.OpenAsyncReadOnlySession();
            }

            [Fact]
            public void AReadOnlySessionIsReturned()
            {
                Assert.NotNull(this.readOnlySession);
                Assert.IsType<AsyncReadOnlySession>(this.readOnlySession);
            }

            [Fact]
            public void TheConnectionScopeOfTheSessionIsPerTransactionByDefault()
            {
                Assert.Equal(ConnectionScope.PerTransaction, ((SessionBase)this.readOnlySession).ConnectionScope);
            }
        }

        public class WhenCallingOpenAsyncReadOnlySession_MultipleTimes : UnitTest
        {
            private readonly IAsyncReadOnlySession readOnlySession1;
            private readonly IAsyncReadOnlySession readOnlySession2;

            public WhenCallingOpenAsyncReadOnlySession_MultipleTimes()
            {
                var sessionFactory = new SessionFactory("SqlConnection", new Mock<IDbDriver>().Object, new Mock<ISqlDialect>().Object);

                this.readOnlySession1 = sessionFactory.OpenAsyncReadOnlySession();
                this.readOnlySession2 = sessionFactory.OpenAsyncReadOnlySession();
            }

            [Fact]
            public void ANewSessionIsReturnedEachTime()
            {
                Assert.NotSame(this.readOnlySession1, this.readOnlySession2);
            }
        }

        public class WhenCallingOpenAsyncReadOnlySession_SpecifyingConnectionScope : UnitTest
        {
            private readonly IAsyncReadOnlySession readOnlySession;

            public WhenCallingOpenAsyncReadOnlySession_SpecifyingConnectionScope()
            {
                var mockDbDriver = new Mock<IDbDriver>();
                mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerSession));

                var sessionFactory = new SessionFactory("SqlConnection", mockDbDriver.Object, new Mock<ISqlDialect>().Object);

                this.readOnlySession = sessionFactory.OpenAsyncReadOnlySession(ConnectionScope.PerSession);
            }

            [Fact]
            public void TheConnectionScopeOfTheSessionIsSetCorrectly()
            {
                Assert.Equal(ConnectionScope.PerSession, ((SessionBase)this.readOnlySession).ConnectionScope);
            }
        }

#endif

        public class WhenCallingOpenReadOnlySession : UnitTest
        {
            private readonly IReadOnlySession readOnlySession;

            public WhenCallingOpenReadOnlySession()
            {
                var sessionFactory = new SessionFactory("SqlConnection", new Mock<IDbDriver>().Object, new Mock<ISqlDialect>().Object);

                this.readOnlySession = sessionFactory.OpenReadOnlySession();
            }

            [Fact]
            public void AReadOnlySessionIsReturned()
            {
                Assert.NotNull(this.readOnlySession);
                Assert.IsType<ReadOnlySession>(this.readOnlySession);
            }

            [Fact]
            public void TheConnectionScopeOfTheSessionIsPerTransactionByDefault()
            {
                Assert.Equal(ConnectionScope.PerTransaction, ((SessionBase)this.readOnlySession).ConnectionScope);
            }
        }

        public class WhenCallingOpenReadOnlySession_MultipleTimes : UnitTest
        {
            private readonly IReadOnlySession readOnlySession1;
            private readonly IReadOnlySession readOnlySession2;

            public WhenCallingOpenReadOnlySession_MultipleTimes()
            {
                var sessionFactory = new SessionFactory("SqlConnection", new Mock<IDbDriver>().Object, new Mock<ISqlDialect>().Object);

                this.readOnlySession1 = sessionFactory.OpenReadOnlySession();
                this.readOnlySession2 = sessionFactory.OpenReadOnlySession();
            }

            [Fact]
            public void ANewSessionIsReturnedEachTime()
            {
                Assert.NotSame(this.readOnlySession1, this.readOnlySession2);
            }
        }

        public class WhenCallingOpenReadOnlySession_SpecifyingConnectionScope : UnitTest
        {
            private readonly IReadOnlySession readOnlySession;

            public WhenCallingOpenReadOnlySession_SpecifyingConnectionScope()
            {
                var mockDbDriver = new Mock<IDbDriver>();
                mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerSession));

                var sessionFactory = new SessionFactory("SqlConnection", mockDbDriver.Object, new Mock<ISqlDialect>().Object);

                this.readOnlySession = sessionFactory.OpenReadOnlySession(ConnectionScope.PerSession);
            }

            [Fact]
            public void TheConnectionScopeOfTheSessionIsSetCorrectly()
            {
                Assert.Equal(ConnectionScope.PerSession, ((SessionBase)this.readOnlySession).ConnectionScope);
            }
        }

        public class WhenCallingOpenSession : UnitTest
        {
            private readonly ISession session;

            public WhenCallingOpenSession()
            {
                var sessionFactory = new SessionFactory("SqlConnection", new Mock<IDbDriver>().Object, new Mock<ISqlDialect>().Object);

                this.session = sessionFactory.OpenSession();
            }

            [Fact]
            public void ASessionIsReturned()
            {
                Assert.NotNull(this.session);
                Assert.IsType<Session>(this.session);
            }

            [Fact]
            public void TheConnectionScopeOfTheSessionIsPerTransactionByDefault()
            {
                Assert.Equal(ConnectionScope.PerTransaction, ((SessionBase)this.session).ConnectionScope);
            }
        }

        public class WhenCallingOpenSession_MultipleTimes : UnitTest
        {
            private readonly ISession session1;
            private readonly ISession session2;

            public WhenCallingOpenSession_MultipleTimes()
            {
                var sessionFactory = new SessionFactory("SqlConnection", new Mock<IDbDriver>().Object, new Mock<ISqlDialect>().Object);

                this.session1 = sessionFactory.OpenSession();
                this.session2 = sessionFactory.OpenSession();
            }

            [Fact]
            public void ANewSessionIsReturnedEachTime()
            {
                Assert.NotSame(this.session1, this.session2);
            }
        }

        public class WhenCallingOpenSession_SpecifyingConnectionScope : UnitTest
        {
            private readonly ISession session;

            public WhenCallingOpenSession_SpecifyingConnectionScope()
            {
                var mockDbDriver = new Mock<IDbDriver>();
                mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerSession));

                var sessionFactory = new SessionFactory("SqlConnection", mockDbDriver.Object, new Mock<ISqlDialect>().Object);

                this.session = sessionFactory.OpenSession(ConnectionScope.PerSession);
            }

            [Fact]
            public void TheConnectionScopeOfTheSessionIsSetCorrectly()
            {
                Assert.Equal(ConnectionScope.PerSession, ((SessionBase)this.session).ConnectionScope);
            }
        }

        public class WhenConstructed : UnitTest
        {
            private readonly string connectionName = "Northwind";
            private readonly IDbDriver dbDriver = new Mock<IDbDriver>().Object;
            private readonly SessionFactory sessionFactory;
            private readonly ISqlDialect sqlDialect = new Mock<ISqlDialect>().Object;

            public WhenConstructed()
            {
                this.sessionFactory = new SessionFactory(this.connectionName, this.dbDriver, this.sqlDialect);
            }

            [Fact]
            public void ConnectionNameReturnsConnectionNameFromOptions()
            {
                Assert.Equal(this.connectionName, this.sessionFactory.ConnectionName);
            }

            [Fact]
            public void TheDbDriverPropertyReturnsDbDriverFromOptions()
            {
                Assert.Same(this.dbDriver, this.sessionFactory.DbDriver);
            }
        }
    }
}