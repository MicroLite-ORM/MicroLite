namespace MicroLite.Tests.Configuration
{
    using System;
    using MicroLite.Configuration;
    using MicroLite.Dialect;
    using MicroLite.Driver;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="PostgreSqlConfigurationExtensions"/> class.
    /// </summary>
    public class PostgreSqlConfigurationExtensionsTests
    {
        public class WhenCallingForPostgreSqlConnection_WithConnectionDetails
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForPostgreSqlConnection_WithConnectionDetails()
            {
                PostgreSqlConfigurationExtensions.ForPostgreSqlConnection(this.mockConfigureConnection.Object, "TestConnection", "Data Source=.", "Npgsql");
            }

            [Fact]
            public void ForConnectionIsCalledWithTheConnectionNameConnectionStringProviderNameAndAnInstanceOfTheSqlDialectAndDbDriver()
            {
                this.mockConfigureConnection.Verify(
                    x => x.ForConnection("TestConnection", "Data Source=.", "Npgsql", It.IsNotNull<PostgreSqlDialect>(), It.IsNotNull<PostgreSqlDbDriver>()),
                    Times.Once());
            }
        }

        public class WhenCallingForPostgreSqlConnection_WithConnectionDetails_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => PostgreSqlConfigurationExtensions.ForPostgreSqlConnection(null, "TestConnection", "Data Source=.", "Npgsql"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForPostgreSqlConnection_WithNamedConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForPostgreSqlConnection_WithNamedConnection()
            {
                PostgreSqlConfigurationExtensions.ForPostgreSqlConnection(this.mockConfigureConnection.Object, "TestConnection");
            }

            [Fact]
            public void ForConnectionIsCalledWithAnInstanceOfTheSqlDialectAndDbDriver()
            {
                this.mockConfigureConnection.Verify(
                    x => x.ForConnection("TestConnection", It.IsNotNull<PostgreSqlDialect>(), It.IsNotNull<PostgreSqlDbDriver>()),
                    Times.Once());
            }
        }

        public class WhenCallingForPostgreSqlConnection_WithNamedConnection_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => PostgreSqlConfigurationExtensions.ForPostgreSqlConnection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }
    }
}