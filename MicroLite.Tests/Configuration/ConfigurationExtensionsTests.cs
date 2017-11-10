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