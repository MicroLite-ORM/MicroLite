namespace MicroLite.Tests.Configuration
{
    using System;
    using MicroLite.Configuration;
    using MicroLite.Dialect;
    using MicroLite.Driver;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="MySqlConfigurationExtensions"/> class.
    /// </summary>
    public class MySqlConfigurationExtensionsTests
    {
        public class WhenCallingForMySqlConnection_WithConnectionDetails
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForMySqlConnection_WithConnectionDetails()
            {
                MySqlConfigurationExtensions.ForMySqlConnection(this.mockConfigureConnection.Object, "TestConnection", "Data Source=.", "MySql.Data.MySqlClient");
            }

            [Fact]
            public void ForConnectionIsCalledWithTheConnectionNameConnectionStringProviderNameAndAnInstanceOfTheSqlDialectAndDbDriver()
            {
                this.mockConfigureConnection.Verify(
                    x => x.ForConnection("TestConnection", "Data Source=.", "MySql.Data.MySqlClient", It.IsNotNull<MySqlDialect>(), It.IsNotNull<MySqlDbDriver>()),
                    Times.Once());
            }
        }

        public class WhenCallingForMySqlConnection_WithConnectionDetails_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => MySqlConfigurationExtensions.ForMySqlConnection(null, "TestConnection", "Data Source=.", "MySql.Data.MySqlClient"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForMySqlConnection_WithNamedConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForMySqlConnection_WithNamedConnection()
            {
                MySqlConfigurationExtensions.ForMySqlConnection(this.mockConfigureConnection.Object, "TestConnection");
            }

            [Fact]
            public void ForConnectionIsCalledWithAnInstanceOfTheSqlDialectAndDbDriver()
            {
                this.mockConfigureConnection.Verify(
                    x => x.ForConnection("TestConnection", It.IsNotNull<MySqlDialect>(), It.IsNotNull<MySqlDbDriver>()),
                    Times.Once());
            }
        }

        public class WhenCallingForMySqlConnection_WithNamedConnection_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => MySqlConfigurationExtensions.ForMySqlConnection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }
    }
}