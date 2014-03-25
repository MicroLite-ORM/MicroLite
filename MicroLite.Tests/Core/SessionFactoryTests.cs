namespace MicroLite.Tests.Core
{
    using System.Data.Common;
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
        public class WhenCallingOpenReadOnlySession
        {
            private readonly IReadOnlySession readOnlySession;

            public WhenCallingOpenReadOnlySession()
            {
                var options = new SessionFactoryOptions
                {
                    ConnectionName = "SqlConnection",
                    SqlDriver = new Mock<IDbDriver>().Object,
                    SqlDialect = new Mock<ISqlDialect>().Object
                };

                var sessionFactory = new SessionFactory(options);

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

        public class WhenCallingOpenReadOnlySession_MultipleTimes
        {
            private readonly IReadOnlySession readOnlySession1;
            private readonly IReadOnlySession readOnlySession2;

            public WhenCallingOpenReadOnlySession_MultipleTimes()
            {
                var options = new SessionFactoryOptions
                {
                    ConnectionName = "SqlConnection",
                    SqlDriver = new Mock<IDbDriver>().Object,
                    SqlDialect = new Mock<ISqlDialect>().Object
                };

                var sessionFactory = new SessionFactory(options);

                this.readOnlySession1 = sessionFactory.OpenReadOnlySession();
                this.readOnlySession2 = sessionFactory.OpenReadOnlySession();
            }

            [Fact]
            public void ANewSessionIsReturnedEachTime()
            {
                Assert.NotSame(this.readOnlySession1, this.readOnlySession2);
            }
        }

        public class WhenCallingOpenReadOnlySession_SpecifyingConnectionScope
        {
            private readonly IReadOnlySession readOnlySession;

            public WhenCallingOpenReadOnlySession_SpecifyingConnectionScope()
            {
                var mockDbDriver = new Mock<IDbDriver>();
                mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerSession));

                var options = new SessionFactoryOptions
                {
                    ConnectionName = "SqlConnection",
                    SqlDriver = mockDbDriver.Object,
                    SqlDialect = new Mock<ISqlDialect>().Object
                };

                var sessionFactory = new SessionFactory(options);

                this.readOnlySession = sessionFactory.OpenReadOnlySession(ConnectionScope.PerSession);
            }

            [Fact]
            public void TheConnectionScopeOfTheSessionIsSetCorrectly()
            {
                Assert.Equal(ConnectionScope.PerSession, ((SessionBase)this.readOnlySession).ConnectionScope);
            }
        }

        public class WhenCallingOpenSession
        {
            private readonly ISession session;

            public WhenCallingOpenSession()
            {
                var options = new SessionFactoryOptions
                {
                    ConnectionName = "SqlConnection",
                    SqlDriver = new Mock<IDbDriver>().Object,
                    SqlDialect = new Mock<ISqlDialect>().Object
                };

                var sessionFactory = new SessionFactory(options);

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

        public class WhenCallingOpenSession_MultipleTimes
        {
            private readonly ISession session1;
            private readonly ISession session2;

            public WhenCallingOpenSession_MultipleTimes()
            {
                var options = new SessionFactoryOptions
                {
                    ConnectionName = "SqlConnection",
                    SqlDriver = new Mock<IDbDriver>().Object,
                    SqlDialect = new Mock<ISqlDialect>().Object
                };

                var sessionFactory = new SessionFactory(options);

                this.session1 = sessionFactory.OpenSession();
                this.session2 = sessionFactory.OpenSession();
            }

            [Fact]
            public void ANewSessionIsReturnedEachTime()
            {
                Assert.NotSame(this.session1, this.session2);
            }
        }

        public class WhenCallingOpenSession_SpecifyingConnectionScope
        {
            private readonly ISession session;

            public WhenCallingOpenSession_SpecifyingConnectionScope()
            {
                var mockDbDriver = new Mock<IDbDriver>();
                mockDbDriver.Setup(x => x.GetConnection(ConnectionScope.PerSession));

                var options = new SessionFactoryOptions
                {
                    ConnectionName = "SqlConnection",
                    SqlDriver = mockDbDriver.Object,
                    SqlDialect = new Mock<ISqlDialect>().Object
                };

                var sessionFactory = new SessionFactory(options);

                this.session = sessionFactory.OpenSession(ConnectionScope.PerSession);
            }

            [Fact]
            public void TheConnectionScopeOfTheSessionIsSetCorrectly()
            {
                Assert.Equal(ConnectionScope.PerSession, ((SessionBase)this.session).ConnectionScope);
            }
        }

        public class WhenCreated
        {
            private readonly SessionFactoryOptions options = new SessionFactoryOptions
            {
                ConnectionName = "Northwind"
            };

            private readonly SessionFactory sessionFactory;

            public WhenCreated()
            {
                this.sessionFactory = new SessionFactory(this.options);
            }

            [Fact]
            public void ConnectionNameReturnsConnectionNameFromOptions()
            {
                Assert.Equal(this.options.ConnectionName, this.sessionFactory.ConnectionName);
            }
        }
    }
}