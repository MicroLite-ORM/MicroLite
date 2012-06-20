namespace MicroLite.Tests.Configuration
{
    using System;
    using System.Linq;
    using MicroLite.Configuration;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Listeners;
    using MicroLite.Logging;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="Configure"/> class.
    /// </summary>
    [TestFixture]
    public class ConfigureTests
    {
        [Test]
        public void CreateSessionFactoryAddsInstanceToStaticProperty()
        {
            var sessionFactory = Configure.Fluently().ForConnection("SqlConnection").CreateSessionFactory();

            CollectionAssert.Contains(Configure.SessionFactories, sessionFactory);
        }

        [Test]
        public void CreateSessionFactoryReturnsSessionFactoryForNamedConnection()
        {
            var sessionFactory = Configure.Fluently().ForConnection("SqlConnection").CreateSessionFactory();

            Assert.AreEqual("SqlConnection", sessionFactory.ConnectionName);
        }

        [Test]
        public void ExtensionsReturnsNewInstanceOnEachCall()
        {
            var extensions1 = Configure.Extensions();
            var extensions2 = Configure.Extensions();

            Assert.AreNotSame(extensions1, extensions2);
        }

        [Test]
        public void FluentlyRegistersAssignedListener()
        {
            var sessionFactory = Configure
                .Fluently();

            var listener = ListenerManager.Create()
                .Single(x => x.GetType() == typeof(AssignedListener));

            Assert.NotNull(listener);
        }

        [Test]
        public void FluentlyRegistersGuidListener()
        {
            var sessionFactory = Configure
                .Fluently();

            var listener = ListenerManager.Create()
                .Single(x => x.GetType() == typeof(GuidListener));

            Assert.NotNull(listener);
        }

        [Test]
        public void FluentlyRegistersIdentityListener()
        {
            var sessionFactory = Configure
                .Fluently();

            var listener = ListenerManager.Create()
                .Single(x => x.GetType() == typeof(IdentityListener));

            Assert.NotNull(listener);
        }

        [Test]
        public void FluentlyReturnsNewInstanceOnEachCall()
        {
            var configure1 = Configure.Fluently();
            var configure2 = Configure.Fluently();

            Assert.AreNotSame(configure1, configure2);
        }

        [Test]
        public void ForConnectionThrowsArgumentNullExceptionForNullConnectionName()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => Configure.Fluently().ForConnection(null));

            Assert.AreEqual(exception.ParamName, "connectionName");
        }

        [Test]
        public void ForConnectionThrowsMicroLiteExceptionIfConnectionNameNotInConfigSection()
        {
            var exception = Assert.Throws<MicroLiteException>(
                () => Configure.Fluently().ForConnection("TestDB"));

            Assert.AreEqual(LogMessages.Configure_ConnectionNotFound.FormatWith("TestDB"), exception.Message);
        }

        [Test]
        public void ForConnectionThrowsNotSupportedExceptionIfProviderNameNotSupported()
        {
            var exception = Assert.Throws<NotSupportedException>(
                () => Configure.Fluently().ForConnection("OleConnection"));

            Assert.AreEqual(LogMessages.Configure_ProviderNotSupported.FormatWith("System.Data.OleDb"), exception.Message);
        }

        [SetUp]
        public void SetUp()
        {
            ListenerManager.Clear();
        }
    }
}