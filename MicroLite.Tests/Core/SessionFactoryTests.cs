namespace MicroLite.Tests.Core
{
    using System.Data;
    using MicroLite.Characters;
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
        public class WhenCallingOpenAsyncReadOnlySession : UnitTest
        {
            private readonly IAsyncReadOnlySession readOnlyAsyncSession;
            private readonly SqlCharacters sqlCharacters = new Mock<SqlCharacters>().Object;

            public WhenCallingOpenAsyncReadOnlySession()
            {
                var mockDbDriver = new Mock<IDbDriver>();
                mockDbDriver.Setup(x => x.CreateConnection());

                var mockSqlDialect = new Mock<ISqlDialect>();
                mockSqlDialect.Setup(x => x.SqlCharacters).Returns(this.sqlCharacters);

                var sessionFactory = new SessionFactory("SqlConnection", mockDbDriver.Object, mockSqlDialect.Object);

                this.readOnlyAsyncSession = sessionFactory.OpenAsyncReadOnlySession();
            }

            [Fact]
            public void AAsyncReadOnlySessionIsReturned()
            {
                Assert.NotNull(this.readOnlyAsyncSession);
                Assert.IsType<AsyncReadOnlySession>(this.readOnlyAsyncSession);
            }

            [Fact]
            public void TheConnectionScopeOfTheAsyncSessionIsPerTransactionByDefault()
            {
                Assert.Equal(ConnectionScope.PerTransaction, ((SessionBase)this.readOnlyAsyncSession).ConnectionScope);
            }

            [Fact]
            public void TheSqlCharactersCurrentPropertyShouldBeSetToTheSqlDialectSqlCharacters()
            {
                Assert.Equal(this.sqlCharacters, SqlCharacters.Current);
            }
        }

        public class WhenCallingOpenAsyncReadOnlySession_MultipleTimes : UnitTest
        {
            private readonly IAsyncReadOnlySession readOnlyAsyncSession1;
            private readonly IAsyncReadOnlySession readOnlyAsyncSession2;

            public WhenCallingOpenAsyncReadOnlySession_MultipleTimes()
            {
                var sessionFactory = new SessionFactory("SqlConnection", new Mock<IDbDriver>().Object, new Mock<ISqlDialect>().Object);

                this.readOnlyAsyncSession1 = sessionFactory.OpenAsyncReadOnlySession();
                this.readOnlyAsyncSession2 = sessionFactory.OpenAsyncReadOnlySession();
            }

            [Fact]
            public void ANewAsyncSessionIsReturnedEachTime()
            {
                Assert.NotSame(this.readOnlyAsyncSession1, this.readOnlyAsyncSession2);
            }
        }

        public class WhenCallingOpenAsyncReadOnlySession_SpecifyingConnectionScope : UnitTest
        {
            private readonly IAsyncReadOnlySession readOnlyAsyncSession;
            private readonly SqlCharacters sqlCharacters = new Mock<SqlCharacters>().Object;

            public WhenCallingOpenAsyncReadOnlySession_SpecifyingConnectionScope()
            {
                var mockDbDriver = new Mock<IDbDriver>();
                mockDbDriver.Setup(x => x.CreateConnection()).Returns(new Mock<IDbConnection>().Object);

                var mockSqlDialect = new Mock<ISqlDialect>();
                mockSqlDialect.Setup(x => x.SqlCharacters).Returns(this.sqlCharacters);

                var sessionFactory = new SessionFactory("SqlConnection", mockDbDriver.Object, mockSqlDialect.Object);

                this.readOnlyAsyncSession = sessionFactory.OpenAsyncReadOnlySession(ConnectionScope.PerSession);
            }

            [Fact]
            public void AAsyncReadOnlySessionIsReturned()
            {
                Assert.NotNull(this.readOnlyAsyncSession);
                Assert.IsType<AsyncReadOnlySession>(this.readOnlyAsyncSession);
            }

            [Fact]
            public void TheConnectionScopeOfTheAsyncSessionIsSetCorrectly()
            {
                Assert.Equal(ConnectionScope.PerSession, ((SessionBase)this.readOnlyAsyncSession).ConnectionScope);
            }

            [Fact]
            public void TheSqlCharactersCurrentPropertyShouldBeSetToTheSqlDialectSqlCharacters()
            {
                Assert.Equal(this.sqlCharacters, SqlCharacters.Current);
            }
        }

        public class WhenCallingOpenAsyncSession : UnitTest
        {
            private readonly IAsyncSession session;
            private readonly SqlCharacters sqlCharacters = new Mock<SqlCharacters>().Object;

            public WhenCallingOpenAsyncSession()
            {
                var mockDbDriver = new Mock<IDbDriver>();
                mockDbDriver.Setup(x => x.CreateConnection());

                var mockSqlDialect = new Mock<ISqlDialect>();
                mockSqlDialect.Setup(x => x.SqlCharacters).Returns(this.sqlCharacters);

                var sessionFactory = new SessionFactory("SqlConnection", mockDbDriver.Object, mockSqlDialect.Object);

                this.session = sessionFactory.OpenAsyncSession();
            }

            [Fact]
            public void AAsyncSessionIsReturned()
            {
                Assert.NotNull(this.session);
                Assert.IsType<AsyncSession>(this.session);
            }

            [Fact]
            public void TheConnectionScopeOfTheAsyncSessionIsPerTransactionByDefault()
            {
                Assert.Equal(ConnectionScope.PerTransaction, ((SessionBase)this.session).ConnectionScope);
            }

            [Fact]
            public void TheSqlCharactersCurrentPropertyShouldBeSetToTheSqlDialectSqlCharacters()
            {
                Assert.Equal(this.sqlCharacters, SqlCharacters.Current);
            }
        }

        public class WhenCallingOpenAsyncSession_MultipleTimes : UnitTest
        {
            private readonly IAsyncSession session1;
            private readonly IAsyncSession session2;

            public WhenCallingOpenAsyncSession_MultipleTimes()
            {
                var sessionFactory = new SessionFactory("SqlConnection", new Mock<IDbDriver>().Object, new Mock<ISqlDialect>().Object);

                this.session1 = sessionFactory.OpenAsyncSession();
                this.session2 = sessionFactory.OpenAsyncSession();
            }

            [Fact]
            public void ANewAsyncSessionIsReturnedEachTime()
            {
                Assert.NotSame(this.session1, this.session2);
            }
        }

        public class WhenCallingOpenAsyncSession_SpecifyingConnectionScope : UnitTest
        {
            private readonly IAsyncSession session;
            private readonly SqlCharacters sqlCharacters = new Mock<SqlCharacters>().Object;

            public WhenCallingOpenAsyncSession_SpecifyingConnectionScope()
            {
                var mockDbDriver = new Mock<IDbDriver>();
                mockDbDriver.Setup(x => x.CreateConnection()).Returns(new Mock<IDbConnection>().Object);

                var mockSqlDialect = new Mock<ISqlDialect>();
                mockSqlDialect.Setup(x => x.SqlCharacters).Returns(this.sqlCharacters);

                var sessionFactory = new SessionFactory("SqlConnection", mockDbDriver.Object, mockSqlDialect.Object);

                this.session = sessionFactory.OpenAsyncSession(ConnectionScope.PerSession);
            }

            [Fact]
            public void AAsyncSessionIsReturned()
            {
                Assert.NotNull(this.session);
                Assert.IsType<AsyncSession>(this.session);
            }

            [Fact]
            public void TheConnectionScopeOfTheAsyncSessionIsSetCorrectly()
            {
                Assert.Equal(ConnectionScope.PerSession, ((SessionBase)this.session).ConnectionScope);
            }

            [Fact]
            public void TheSqlCharactersCurrentPropertyShouldBeSetToTheSqlDialectSqlCharacters()
            {
                Assert.Equal(this.sqlCharacters, SqlCharacters.Current);
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