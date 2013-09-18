namespace MicroLite.Tests.Configuration
{
    using System;
    using MicroLite.Configuration;
    using MicroLite.Mapping;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ConfigurationExtensions"/> class.
    /// </summary>
    public class ConfigurationExtensionsTests
    {
        public class WhenCallingForMsSqlConnection
        {
            private readonly Mock<IConfigureConnection> mockConfigureConnection = new Mock<IConfigureConnection>();

            public WhenCallingForMsSqlConnection()
            {
                ConfigurationExtensions.ForMsSqlConnection(this.mockConfigureConnection.Object, "TestConnection");
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
                var exception = Assert.Throws<ArgumentNullException>(() => ConfigurationExtensions.ForMsSqlConnection(null, "TestConnection"));

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
                var exception = Assert.Throws<ArgumentNullException>(() => ConfigurationExtensions.ForMySqlConnection(null, "TestConnection"));

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
                var exception = Assert.Throws<ArgumentNullException>(() => ConfigurationExtensions.ForPostgreSqlConnection(null, "TestConnection"));

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
                var exception = Assert.Throws<ArgumentNullException>(() => ConfigurationExtensions.ForSQLiteConnection(null, "TestConnection"));

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
                this.mockConfigureExtensions.Verify(x => x.SetMappingConvention(It.IsAny<AttributeMappingConvention>()), Times.Once());
            }
        }

        public class WhenCallingWithAttributeBasedMappingAndTheConfigureExtensionsIsNull
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
                this.mockConfigureExtensions.Verify(x => x.SetMappingConvention(It.IsAny<ConventionMappingConvention>()), Times.Once());
            }
        }

        public class WhenCallingWithConventionBasedMappingAndTheConfigureExtensionsIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => ConfigurationExtensions.WithConventionBasedMapping(null, new ConventionMappingSettings()));

                Assert.Equal("configureExtensions", exception.ParamName);
            }
        }

        public class WhenCallingWithConventionBasedMappingAndTheSettingsAreNull
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