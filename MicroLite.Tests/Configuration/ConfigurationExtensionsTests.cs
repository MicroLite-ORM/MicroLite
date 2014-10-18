namespace MicroLite.Tests.Configuration
{
    using System;
    using MicroLite.Configuration;
    using MicroLite.Dialect;
    using MicroLite.Driver;
    using MicroLite.Mapping;
    using MicroLite.Mapping.Attributes;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ConfigurationExtensions"/> class.
    /// </summary>
    public class ConfigurationExtensionsTests
    {
        public class WhenCallingForFirebirdConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForFirebirdConnection()
            {
                ConfigurationExtensions.ForFirebirdConnection(this.mockConfigureConnection.Object, "TestConnection");
            }

            [Fact]
            public void ForConnectionIsCalledWithAnInstanceOfTheSqlDialectAndDbDriver()
            {
                this.mockConfigureConnection.Verify(
                    x => x.ForConnection("TestConnection", It.IsNotNull<FirebirdSqlDialect>(), It.IsNotNull<FirebirdDbDriver>()),
                    Times.Once());
            }
        }

        public class WhenCallingForFirebirdConnection_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => ConfigurationExtensions.ForFirebirdConnection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForMsSql2005Connection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForMsSql2005Connection()
            {
                ConfigurationExtensions.ForMsSql2005Connection(this.mockConfigureConnection.Object, "TestConnection");
            }

            [Fact]
            public void ForConnectionIsCalledWithAnInstanceOfTheSqlDialectAndDbDriver()
            {
                this.mockConfigureConnection.Verify(
                    x => x.ForConnection("TestConnection", It.IsNotNull<MsSql2005Dialect>(), It.IsNotNull<MsSqlDbDriver>()),
                    Times.Once());
            }
        }

        public class WhenCallingForMsSql2005Connection_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => ConfigurationExtensions.ForMsSql2005Connection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForMsSql2012Connection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForMsSql2012Connection()
            {
                ConfigurationExtensions.ForMsSql2012Connection(this.mockConfigureConnection.Object, "TestConnection");
            }

            [Fact]
            public void ForConnectionIsCalledWithAnInstanceOfTheSqlDialectAndDbDriver()
            {
                this.mockConfigureConnection.Verify(
                    x => x.ForConnection("TestConnection", It.IsNotNull<MsSql2012Dialect>(), It.IsNotNull<MsSqlDbDriver>()),
                    Times.Once());
            }
        }

        public class WhenCallingForMsSql2012Connection_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => ConfigurationExtensions.ForMsSql2012Connection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForMySqlConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForMySqlConnection()
            {
                ConfigurationExtensions.ForMySqlConnection(this.mockConfigureConnection.Object, "TestConnection");
            }

            [Fact]
            public void ForConnectionIsCalledWithAnInstanceOfTheSqlDialectAndDbDriver()
            {
                this.mockConfigureConnection.Verify(
                    x => x.ForConnection("TestConnection", It.IsNotNull<MySqlDialect>(), It.IsNotNull<MySqlDbDriver>()),
                    Times.Once());
            }
        }

        public class WhenCallingForMySqlConnection_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => ConfigurationExtensions.ForMySqlConnection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForPostgreSqlConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForPostgreSqlConnection()
            {
                ConfigurationExtensions.ForPostgreSqlConnection(this.mockConfigureConnection.Object, "TestConnection");
            }

            [Fact]
            public void ForConnectionIsCalledWithAnInstanceOfTheSqlDialectAndDbDriver()
            {
                this.mockConfigureConnection.Verify(
                    x => x.ForConnection("TestConnection", It.IsNotNull<PostgreSqlDialect>(), It.IsNotNull<PostgreSqlDbDriver>()),
                    Times.Once());
            }
        }

        public class WhenCallingForPostgreSqlConnection_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => ConfigurationExtensions.ForPostgreSqlConnection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForSQLiteConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForSQLiteConnection()
            {
                ConfigurationExtensions.ForSQLiteConnection(this.mockConfigureConnection.Object, "TestConnection");
            }

            [Fact]
            public void ForConnectionIsCalledWithAnInstanceOfTheSqlDialectAndDbDriver()
            {
                this.mockConfigureConnection.Verify(
                    x => x.ForConnection("TestConnection", It.IsNotNull<SQLiteDialect>(), It.IsNotNull<SQLiteDbDriver>()),
                    Times.Once());
            }
        }

        public class WhenCallingForSQLiteConnection_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => ConfigurationExtensions.ForSQLiteConnection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForSqlServerCeConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForSqlServerCeConnection()
            {
                ConfigurationExtensions.ForSqlServerCeConnection(this.mockConfigureConnection.Object, "TestConnection");
            }

            [Fact]
            public void ForConnectionIsCalledWithAnInstanceOfTheSqlDialectAndDbDriver()
            {
                this.mockConfigureConnection.Verify(
                    x => x.ForConnection("TestConnection", It.IsNotNull<SqlServerCeDialect>(), It.IsNotNull<SqlServerCeDbDriver>()),
                    Times.Once());
            }
        }

        public class WhenCallingForSqlServerCeConnection_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => ConfigurationExtensions.ForSqlServerCeConnection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingWithAttributeBasedMapping
        {
            private readonly Mock<IConfigureExtensions> mockConfigureExtensions = new Mock<IConfigureExtensions>();

            public WhenCallingWithAttributeBasedMapping()
            {
                ConfigurationExtensions.WithAttributeBasedMapping(this.mockConfigureExtensions.Object);
            }

            [Fact]
            public void SetMappingConventionIsCalledWithAnInstanceOfAttributeMappingConvention()
            {
                this.mockConfigureExtensions.Verify(x => x.SetMappingConvention(It.IsNotNull<AttributeMappingConvention>()), Times.Once());
            }
        }

        public class WhenCallingWithAttributeBasedMapping_AndTheConfigureExtensionsIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => ConfigurationExtensions.WithAttributeBasedMapping(null));

                Assert.Equal("configureExtensions", exception.ParamName);
            }
        }

        public class WhenCallingWithConventionBasedMapping
        {
            private readonly Mock<IConfigureExtensions> mockConfigureExtensions = new Mock<IConfigureExtensions>();

            public WhenCallingWithConventionBasedMapping()
            {
                ConfigurationExtensions.WithConventionBasedMapping(this.mockConfigureExtensions.Object, new ConventionMappingSettings());
            }

            [Fact]
            public void SetMappingConventionIsCalledWithAnInstanceOfConventionMappingConvention()
            {
                this.mockConfigureExtensions.Verify(x => x.SetMappingConvention(It.IsNotNull<ConventionMappingConvention>()), Times.Once());
            }
        }

        public class WhenCallingWithConventionBasedMapping_AndTheConfigureExtensionsIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => ConfigurationExtensions.WithConventionBasedMapping(null, new ConventionMappingSettings()));

                Assert.Equal("configureExtensions", exception.ParamName);
            }
        }

        public class WhenCallingWithConventionBasedMapping_AndTheSettingsAreNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => ConfigurationExtensions.WithConventionBasedMapping(new Mock<IConfigureExtensions>().Object, null));

                Assert.Equal("settings", exception.ParamName);
            }
        }
    }
}