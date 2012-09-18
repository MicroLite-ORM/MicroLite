namespace MicroLite.Tests.Configuration
{
    using System;
    using MicroLite.Configuration;
    using MicroLite.FrameworkExtensions;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="FluentConfiguration"/> class.
    /// </summary>
    [TestFixture]
    public class FluentConfigurationTests
    {
        [Test]
        public void CreateSessionFactoryAddsInstanceToStaticProperty()
        {
            var fluentConfiguration = new FluentConfiguration();

            var sessionFactory = fluentConfiguration.ForConnection("SqlConnection").CreateSessionFactory();

            CollectionAssert.Contains(Configure.SessionFactories, sessionFactory);
        }

        [Test]
        public void CreateSessionFactoryReturnsSameInstanceForSameConnectionName()
        {
            var fluentConfiguration = new FluentConfiguration();

            var sessionFactory1 = fluentConfiguration.ForConnection("SqlConnection").CreateSessionFactory();
            var sessionFactory2 = fluentConfiguration.ForConnection("SqlConnection").CreateSessionFactory();

            Assert.AreSame(sessionFactory1, sessionFactory2);
        }

        [Test]
        public void CreateSessionFactoryReturnsSessionFactoryForNamedConnection()
        {
            var fluentConfiguration = new FluentConfiguration();

            var sessionFactory = fluentConfiguration.ForConnection("SqlConnection").CreateSessionFactory();

            Assert.AreEqual("SqlConnection", sessionFactory.ConnectionName);
        }

        [Test]
        public void ForConnectionThrowsArgumentNullExceptionForNullConnectionName()
        {
            var fluentConfiguration = new FluentConfiguration();

            var exception = Assert.Throws<ArgumentNullException>(
                () => fluentConfiguration.ForConnection(null));

            Assert.AreEqual(exception.ParamName, "connectionName");
        }

        [Test]
        public void ForConnectionThrowsMicroLiteExceptionIfConnectionNameNotInConfigSection()
        {
            var fluentConfiguration = new FluentConfiguration();

            var exception = Assert.Throws<MicroLiteException>(
                () => fluentConfiguration.ForConnection("TestDB"));

            Assert.AreEqual(Messages.FluentConfiguration_ConnectionNotFound.FormatWith("TestDB"), exception.Message);
        }

        [Test]
        public void ForConnectionThrowsNotSupportedExceptionIfProviderNameNotSupported()
        {
            var fluentConfiguration = new FluentConfiguration();

            var exception = Assert.Throws<MicroLiteException>(
                () => fluentConfiguration.ForConnection("ConnectionWithInvalidProviderName"));
        }
    }
}