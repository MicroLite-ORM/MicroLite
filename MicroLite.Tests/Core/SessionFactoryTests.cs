namespace MicroLite.Tests.Core
{
    using System.Data.Common;
    using MicroLite.Core;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SessionFactory"/> class.
    /// </summary>

    public class SessionFactoryTests
    {
        [Fact]
        public void ConnectionNameReturnsConnectionNameFromOptions()
        {
            var options = new SessionFactoryOptions
            {
                ConnectionName = "Northwind"
            };

            var sessionFactory = new SessionFactory(options);

            Assert.Equal(options.ConnectionName, sessionFactory.ConnectionName);
        }

        [Fact]
        public void OpenReadOnlySessionCreatesConnectionAndSetsConnectionString()
        {
            var mockConnection = new Mock<DbConnection>();
            mockConnection.SetupProperty(x => x.ConnectionString);

            var mockFactory = new Mock<DbProviderFactory>();
            mockFactory.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

            var options = new SessionFactoryOptions
            {
                ConnectionString = "Data Source=localhost;Initial Catalog=TestDB;",
                ProviderFactory = mockFactory.Object,
                SqlDialect = "MicroLite.Dialect.MsSqlDialect"
            };

            var sessionFactory = new SessionFactory(options);
            var session = sessionFactory.OpenReadOnlySession();

            mockFactory.VerifyAll();
            mockConnection.VerifySet(x => x.ConnectionString = options.ConnectionString);
        }

        [Fact]
        public void OpenReadOnlySessionReturnsNewInstanceOnEachCall()
        {
            var mockConnection = new Mock<DbConnection>();
            mockConnection.SetupProperty(x => x.ConnectionString);

            var mockFactory = new Mock<DbProviderFactory>();
            mockFactory.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

            var options = new SessionFactoryOptions
            {
                ConnectionString = "Data Source=localhost;Initial Catalog=TestDB;",
                ProviderFactory = mockFactory.Object,
                SqlDialect = "MicroLite.Dialect.MsSqlDialect"
            };

            var sessionFactory = new SessionFactory(options);

            var session1 = sessionFactory.OpenReadOnlySession();
            var session2 = sessionFactory.OpenReadOnlySession();

            Assert.NotSame(session1, session2);
        }

        [Fact]
        public void OpenSessionCreatesConnectionAndSetsConnectionString()
        {
            var mockConnection = new Mock<DbConnection>();
            mockConnection.SetupProperty(x => x.ConnectionString);

            var mockFactory = new Mock<DbProviderFactory>();
            mockFactory.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

            var options = new SessionFactoryOptions
            {
                ConnectionString = "Data Source=localhost;Initial Catalog=TestDB;",
                ProviderFactory = mockFactory.Object,
                SqlDialect = "MicroLite.Dialect.MsSqlDialect"
            };

            var sessionFactory = new SessionFactory(options);
            var session = sessionFactory.OpenSession();

            mockFactory.VerifyAll();
            mockConnection.VerifySet(x => x.ConnectionString = options.ConnectionString);
        }

        [Fact]
        public void OpenSessionReturnsNewInstanceOnEachCall()
        {
            var mockConnection = new Mock<DbConnection>();
            mockConnection.SetupProperty(x => x.ConnectionString);

            var mockFactory = new Mock<DbProviderFactory>();
            mockFactory.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

            var options = new SessionFactoryOptions
            {
                ConnectionString = "Data Source=localhost;Initial Catalog=TestDB;",
                ProviderFactory = mockFactory.Object,
                SqlDialect = "MicroLite.Dialect.MsSqlDialect"
            };

            var sessionFactory = new SessionFactory(options);

            var session1 = sessionFactory.OpenSession();
            var session2 = sessionFactory.OpenSession();

            Assert.NotSame(session1, session2);
        }
    }
}