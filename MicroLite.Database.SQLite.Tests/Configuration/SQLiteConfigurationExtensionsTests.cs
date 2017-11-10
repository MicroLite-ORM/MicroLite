namespace MicroLite.Tests.Configuration
{
    using System;
    using MicroLite.Configuration;
    using MicroLite.Dialect;
    using MicroLite.Driver;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SQLiteConfigurationExtensions"/> class.
    /// </summary>
    public class SQLiteConfigurationExtensionsTests
    {
        public class WhenCallingForSQLiteConnection_WithConnectionDetails
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForSQLiteConnection_WithConnectionDetails()
            {
                SQLiteConfigurationExtensions.ForSQLiteConnection(this.mockConfigureConnection.Object, "TestConnection", "Data Source=.", "System.Data.SQLite");
            }

            [Fact]
            public void ForConnectionIsCalledWithTheConnectionNameConnectionStringProviderNameAndAnInstanceOfTheSqlDialectAndDbDriver()
            {
                this.mockConfigureConnection.Verify(
                    x => x.ForConnection("TestConnection", "Data Source=.", "System.Data.SQLite", It.IsNotNull<SQLiteDialect>(), It.IsNotNull<SQLiteDbDriver>()),
                    Times.Once());
            }
        }

        public class WhenCallingForSQLiteConnection_WithConnectionDetails_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => SQLiteConfigurationExtensions.ForSQLiteConnection(null, "TestConnection", "Data Source=.", "System.Data.SQLite"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForSQLiteConnection_WithNamedConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForSQLiteConnection_WithNamedConnection()
            {
                SQLiteConfigurationExtensions.ForSQLiteConnection(this.mockConfigureConnection.Object, "TestConnection");
            }

            [Fact]
            public void ForConnectionIsCalledWithAnInstanceOfTheSqlDialectAndDbDriver()
            {
                this.mockConfigureConnection.Verify(
                    x => x.ForConnection("TestConnection", It.IsNotNull<SQLiteDialect>(), It.IsNotNull<SQLiteDbDriver>()),
                    Times.Once());
            }
        }

        public class WhenCallingForSQLiteConnection_WithNamedConnection_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => SQLiteConfigurationExtensions.ForSQLiteConnection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }
    }
}