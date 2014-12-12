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
        public class WhenCallingForFirebirdConnection_WithConnectionDetails
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForFirebirdConnection_WithConnectionDetails()
            {
                ConfigurationExtensions.ForFirebirdConnection(this.mockConfigureConnection.Object, "TestConnection", "Data Source=.", "FirebirdSql.Data.FirebirdClient");
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
                    () => ConfigurationExtensions.ForFirebirdConnection(null, "TestConnection", "Data Source=.", "FirebirdSql.Data.FirebirdClient"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForFirebirdConnection_WithNamedConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForFirebirdConnection_WithNamedConnection()
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

        public class WhenCallingForFirebirdConnection_WithNamedConnection_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => ConfigurationExtensions.ForFirebirdConnection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForMsSql2005Connection_WithConnectionDetails
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForMsSql2005Connection_WithConnectionDetails()
            {
                ConfigurationExtensions.ForMsSql2005Connection(this.mockConfigureConnection.Object, "TestConnection", "Data Source=.", "System.Data.SqlClient");
            }

            [Fact]
            public void ForConnectionIsCalledWithTheConnectionNameConnectionStringProviderNameAndAnInstanceOfTheSqlDialectAndDbDriver()
            {
                this.mockConfigureConnection.Verify(
                    x => x.ForConnection("TestConnection", "Data Source=.", "System.Data.SqlClient", It.IsNotNull<MsSql2005Dialect>(), It.IsNotNull<MsSqlDbDriver>()),
                    Times.Once());
            }
        }

        public class WhenCallingForMsSql2005Connection_WithConnectionDetails_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => ConfigurationExtensions.ForMsSql2005Connection(null, "TestConnection", "Data Source=.", "System.Data.SqlClient"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForMsSql2005Connection_WithNamedConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForMsSql2005Connection_WithNamedConnection()
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

        public class WhenCallingForMsSql2005Connection_WithNamedConnection_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => ConfigurationExtensions.ForMsSql2005Connection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForMsSql2012Connection_WithConnectionDetails
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForMsSql2012Connection_WithConnectionDetails()
            {
                ConfigurationExtensions.ForMsSql2012Connection(this.mockConfigureConnection.Object, "TestConnection", "Data Source=.", "System.Data.SqlClient");
            }

            [Fact]
            public void ForConnectionIsCalledWithTheConnectionNameConnectionStringProviderNameAndAnInstanceOfTheSqlDialectAndDbDriver()
            {
                this.mockConfigureConnection.Verify(
                    x => x.ForConnection("TestConnection", "Data Source=.", "System.Data.SqlClient", It.IsNotNull<MsSql2012Dialect>(), It.IsNotNull<MsSqlDbDriver>()),
                    Times.Once());
            }
        }

        public class WhenCallingForMsSql2012Connection_WithConnectionDetails_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => ConfigurationExtensions.ForMsSql2012Connection(null, "TestConnection", "Data Source=.", "System.Data.SqlClient"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForMsSql2012Connection_WithNamedConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForMsSql2012Connection_WithNamedConnection()
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

        public class WhenCallingForMsSql2012Connection_WithNamedConnection_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => ConfigurationExtensions.ForMsSql2012Connection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForMySqlConnection_WithConnectionDetails
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForMySqlConnection_WithConnectionDetails()
            {
                ConfigurationExtensions.ForMySqlConnection(this.mockConfigureConnection.Object, "TestConnection", "Data Source=.", "MySql.Data.MySqlClient");
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
                    () => ConfigurationExtensions.ForMySqlConnection(null, "TestConnection", "Data Source=.", "MySql.Data.MySqlClient"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForMySqlConnection_WithNamedConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForMySqlConnection_WithNamedConnection()
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

        public class WhenCallingForMySqlConnection_WithNamedConnection_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => ConfigurationExtensions.ForMySqlConnection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForPostgreSqlConnection_WithConnectionDetails
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForPostgreSqlConnection_WithConnectionDetails()
            {
                ConfigurationExtensions.ForPostgreSqlConnection(this.mockConfigureConnection.Object, "TestConnection", "Data Source=.", "Npgsql");
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
                    () => ConfigurationExtensions.ForPostgreSqlConnection(null, "TestConnection", "Data Source=.", "Npgsql"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForPostgreSqlConnection_WithNamedConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForPostgreSqlConnection_WithNamedConnection()
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

        public class WhenCallingForPostgreSqlConnection_WithNamedConnection_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => ConfigurationExtensions.ForPostgreSqlConnection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForSQLiteConnection_WithConnectionDetails
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForSQLiteConnection_WithConnectionDetails()
            {
                ConfigurationExtensions.ForSQLiteConnection(this.mockConfigureConnection.Object, "TestConnection", "Data Source=.", "System.Data.SQLite");
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
                    () => ConfigurationExtensions.ForSQLiteConnection(null, "TestConnection", "Data Source=.", "System.Data.SQLite"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForSQLiteConnection_WithNamedConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForSQLiteConnection_WithNamedConnection()
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

        public class WhenCallingForSQLiteConnection_WithNamedConnection_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => ConfigurationExtensions.ForSQLiteConnection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForSqlServerCeConnection_WithConnectionDetails
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForSqlServerCeConnection_WithConnectionDetails()
            {
                ConfigurationExtensions.ForSqlServerCeConnection(this.mockConfigureConnection.Object, "TestConnection", "Data Source=.", "System.Data.SqlServerCe.4.0");
            }

            [Fact]
            public void ForConnectionIsCalledWithTheConnectionNameConnectionStringProviderNameAndAnInstanceOfTheSqlDialectAndDbDriver()
            {
                this.mockConfigureConnection.Verify(
                    x => x.ForConnection("TestConnection", "Data Source=.", "System.Data.SqlServerCe.4.0", It.IsNotNull<SqlServerCeDialect>(), It.IsNotNull<SqlServerCeDbDriver>()),
                    Times.Once());
            }
        }

        public class WhenCallingForSqlServerCeConnection_WithConnectionDetails_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => ConfigurationExtensions.ForSqlServerCeConnection(null, "TestConnection", "Data Source=.", "System.Data.SqlServerCe.4.0"));

                Assert.Equal("configureConnection", exception.ParamName);
            }
        }

        public class WhenCallingForSqlServerCeConnection_WithNamedConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForSqlServerCeConnection_WithNamedConnection()
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

        public class WhenCallingForSqlServerCeConnection_WithNamedConnection_AndTheConfigureConnectionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => ConfigurationExtensions.ForSqlServerCeConnection(null, "TestConnection"));

                Assert.Equal("configureConnection", exception.ParamName);
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

        public class WhenCallingWithAttributeBasedMapping_WithNamedConnection
        {
            private readonly Mock<IConfigureExtensions> mockConfigureExtensions = new Mock<IConfigureExtensions>();

            public WhenCallingWithAttributeBasedMapping_WithNamedConnection()
            {
                ConfigurationExtensions.WithAttributeBasedMapping(this.mockConfigureExtensions.Object);
            }

            [Fact]
            public void SetMappingConventionIsCalledWithAnInstanceOfAttributeMappingConvention()
            {
                this.mockConfigureExtensions.Verify(x => x.SetMappingConvention(It.IsNotNull<AttributeMappingConvention>()), Times.Once());
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