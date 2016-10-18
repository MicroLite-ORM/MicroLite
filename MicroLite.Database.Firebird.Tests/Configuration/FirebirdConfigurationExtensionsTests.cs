namespace MicroLite.Tests.Configuration
{
    using System;
    using MicroLite.Configuration;
    using MicroLite.Dialect;
    using MicroLite.Driver;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="FirebirdConfigurationExtensions"/> class.
    /// </summary>
    public class FirebirdConfigurationExtensionsTests
    {
        public class WhenCallingForFirebirdConnection_WithConnectionDetails
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForFirebirdConnection_WithConnectionDetails()
            {
                FirebirdConfigurationExtensions.ForFirebirdConnection(this.mockConfigureConnection.Object, "TestConnection", "Data Source=.", "FirebirdSql.Data.FirebirdClient");
            }

            [Fact]
            public void ForConnectionIsCalledWithTheConnectionNameConnectionStringProviderNameAndAnInstanceOfTheSqlDialectAndDbDriver()
            {
                this.mockConfigureConnection.Verify(
                    x => x.ForConnection("TestConnection", "Data Source=.", "FirebirdSql.Data.FirebirdClient", It.IsNotNull<FirebirdSqlDialect>(), It.IsNotNull<FirebirdDbDriver>()),
                    Times.Once());
            }
        }

        public class WhenCallingForFirebirdConnection_WithConnectionDetails_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => FirebirdConfigurationExtensions.ForFirebirdConnection(null, "TestConnection", "Data Source=.", "FirebirdSql.Data.FirebirdClient"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForFirebirdConnection_WithNamedConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForFirebirdConnection_WithNamedConnection()
            {
                FirebirdConfigurationExtensions.ForFirebirdConnection(this.mockConfigureConnection.Object, "TestConnection");
            }

            [Fact]
            public void ForConnectionIsCalledWithAnInstanceOfTheSqlDialectAndDbDriver()
            {
                this.mockConfigureConnection.Verify(
                    x => x.ForConnection("TestConnection", It.IsNotNull<FirebirdSqlDialect>(), It.IsNotNull<FirebirdDbDriver>()),
                    Times.Once());
            }
        }

        public class WhenCallingForFirebirdConnection_WithNamedConnection_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => FirebirdConfigurationExtensions.ForFirebirdConnection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }
    }
}