namespace MicroLite.Tests.Configuration
{
    using System;
    using System.Linq;
    using MicroLite.Configuration;
    using MicroLite.Core;
    using MicroLite.Logging;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="Configure"/> class.
    /// </summary>
    [TestFixture]
    public class ConfigureTests
    {
        [Test]
        public void CreateSessionFactoryRegistersIdentityListener()
        {
            var connectionString = "Data Source=localhost;Initial Catalog=TestDB;";

            var sessionFactory = Configure
                .Fluently()
                .ForConnection(connectionString, "System.Data.SqlClient")
                .CreateSessionFactory();

            var listener = ExtensionManager.CreateListeners().Single();

            Assert.IsInstanceOf<IdentityListener>(listener);
        }

        [Test]
        public void CreateSessionFactoryReturnsSessionFactoryForConnectionString()
        {
            var connectionString = "Data Source=localhost;Initial Catalog=TestDB;";

            var sessionFactory = Configure
                .Fluently()
                .ForConnection(connectionString, "System.Data.SqlClient")
                .CreateSessionFactory();

            Assert.AreEqual(connectionString, sessionFactory.ConnectionString);
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
        public void ForConnectionThrowsArgumentNullExceptionForNullConnectionString()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => Configure.Fluently().ForConnection(null, string.Empty));

            Assert.AreEqual(exception.ParamName, "connectionString");
        }

        [Test]
        public void ForConnectionThrowsArgumentNullExceptionForNullProviderName()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => Configure.Fluently().ForConnection(string.Empty, null));

            Assert.AreEqual(exception.ParamName, "providerName");
        }

        [Test]
        public void ForConnectionThrowsMicroLiteExceptionIfConnectionNameNotInConfigSection()
        {
            var exception = Assert.Throws<MicroLiteException>(
                () => Configure.Fluently().ForConnection("TestDB"));

            Assert.AreEqual(string.Format(LogMessages.Configure_ConnectionNotFound, "TestDB"), exception.Message);
        }

        [Test]
        public void ForConnectionThrowsMicroLiteExceptionIfProviderNameIsInvalid()
        {
            var exception = Assert.Throws<NotSupportedException>(
                () => Configure.Fluently().ForConnection("Data Source=localhost;Initial Catalog=TestDB;", "InvalidProviderName"));

            Assert.AreEqual(LogMessages.Configure_ProviderNotSupported.FormatWith("InvalidProviderName"), exception.Message);
        }

        [SetUp]
        public void SetUp()
        {
            ExtensionManager.ClearListeners();
        }
    }
}