namespace MicroLite.Tests.Configuration
{
    using System;
    using MicroLite.Configuration;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ConfigureConnectionExtensions"/> class.
    /// </summary>
    public class ConfigureConnectionExtensionsTests
    {
        public class WhenCallingForMsSqlConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForMsSqlConnection()
            {
                ConfigureConnectionExtensions.ForMsSqlConnection(this.mockConfigureConnection.Object, "TestConnection");
            }

            [Fact]
            public void SetMappingConventionIsCalledWithAnInstanceOfAttributeMappingConvention()
            {
                this.mockConfigureConnection.Verify(x => x.ForConnection("TestConnection", "MicroLite.Dialect.MsSqlDialect"), Times.Once());
            }
        }

        public class WhenCallingForMsSqlConnectionAndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => ConfigureConnectionExtensions.ForMsSqlConnection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForMySqlConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForMySqlConnection()
            {
                ConfigureConnectionExtensions.ForMySqlConnection(this.mockConfigureConnection.Object, "TestConnection");
            }

            [Fact]
            public void SetMappingConventionIsCalledWithAnInstanceOfAttributeMappingConvention()
            {
                this.mockConfigureConnection.Verify(x => x.ForConnection("TestConnection", "MicroLite.Dialect.MySqlDialect"), Times.Once());
            }
        }

        public class WhenCallingForMySqlConnectionAndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => ConfigureConnectionExtensions.ForMySqlConnection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForPostgreSqlConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForPostgreSqlConnection()
            {
                ConfigureConnectionExtensions.ForPostgreSqlConnection(this.mockConfigureConnection.Object, "TestConnection");
            }

            [Fact]
            public void SetMappingConventionIsCalledWithAnInstanceOfAttributeMappingConvention()
            {
                this.mockConfigureConnection.Verify(x => x.ForConnection("TestConnection", "MicroLite.Dialect.PostgreSqlDialect"), Times.Once());
            }
        }

        public class WhenCallingForPostgreSqlConnectionAndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => ConfigureConnectionExtensions.ForPostgreSqlConnection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForSQLiteConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForSQLiteConnection()
            {
                ConfigureConnectionExtensions.ForSQLiteConnection(this.mockConfigureConnection.Object, "TestConnection");
            }

            [Fact]
            public void SetMappingConventionIsCalledWithAnInstanceOfAttributeMappingConvention()
            {
                this.mockConfigureConnection.Verify(x => x.ForConnection("TestConnection", "MicroLite.Dialect.SQLiteDialect"), Times.Once());
            }
        }

        public class WhenCallingForSQLiteConnectionAndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => ConfigureConnectionExtensions.ForSQLiteConnection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }
    }
}