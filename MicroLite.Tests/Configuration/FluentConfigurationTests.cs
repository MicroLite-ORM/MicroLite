namespace MicroLite.Tests.Configuration
{
    using System;
    using MicroLite.Configuration;
    using MicroLite.FrameworkExtensions;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="FluentConfiguration"/> class.
    /// </summary>

    public class FluentConfigurationTests
    {
        [Fact]
        public void CreateSessionFactoryAddsInstanceToStaticProperty()
        {
            var fluentConfiguration = new FluentConfiguration();

            var sessionFactory = fluentConfiguration.ForConnection("SqlConnection").CreateSessionFactory();

            Assert.Contains(sessionFactory, Configure.SessionFactories);
        }

        [Fact]
        public void CreateSessionFactoryReturnsSameInstanceForSameConnectionName()
        {
            var fluentConfiguration = new FluentConfiguration();

            var sessionFactory1 = fluentConfiguration.ForConnection("SqlConnection").CreateSessionFactory();
            var sessionFactory2 = fluentConfiguration.ForConnection("SqlConnection").CreateSessionFactory();

            Assert.Same(sessionFactory1, sessionFactory2);
        }

        [Fact]
        public void CreateSessionFactoryReturnsSessionFactoryForNamedConnection()
        {
            var fluentConfiguration = new FluentConfiguration();

            var sessionFactory = fluentConfiguration.ForConnection("SqlConnection").CreateSessionFactory();

            Assert.Equal("SqlConnection", sessionFactory.ConnectionName);
        }

        [Fact]
        public void ForConnectionThrowsArgumentNullExceptionForNullConnectionName()
        {
            var fluentConfiguration = new FluentConfiguration();

            var exception = Assert.Throws<ArgumentNullException>(
                () => fluentConfiguration.ForConnection(null));

            Assert.Equal(exception.ParamName, "connectionName");
        }

        [Fact]
        public void ForConnectionThrowsMicroLiteExceptionIfConnectionNameNotInConfigSection()
        {
            var fluentConfiguration = new FluentConfiguration();

            var exception = Assert.Throws<MicroLiteException>(
                () => fluentConfiguration.ForConnection("TestDB"));

            Assert.Equal(Messages.FluentConfiguration_ConnectionNotFound.FormatWith("TestDB"), exception.Message);
        }

        [Fact]
        public void ForConnectionThrowsNotSupportedExceptionIfProviderNameNotSupported()
        {
            var fluentConfiguration = new FluentConfiguration();

            var exception = Assert.Throws<MicroLiteException>(
                () => fluentConfiguration.ForConnection("ConnectionWithInvalidProviderName"));
        }

        [Fact]
        public void ForConnectionThrowsNotSupportedExceptionIfSqlDialectNotSupported()
        {
            var fluentConfiguration = new FluentConfiguration();

            var exception = Assert.Throws<NotSupportedException>(
                () => fluentConfiguration.ForConnection("SqlConnection", "MicroLite.Dialect.DB2"));

            Assert.Equal(Messages.SqlDialectFactory_DialectNotSupported.FormatWith("MicroLite.Dialect.DB2"), exception.Message);
        }
    }
}