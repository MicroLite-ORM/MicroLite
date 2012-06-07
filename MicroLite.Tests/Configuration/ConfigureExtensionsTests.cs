namespace MicroLite.Tests.Configuration
{
    using System;
    using MicroLite.Configuration;
    using MicroLite.Logging;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="ConfigureExtensions"/> class.
    /// </summary>
    [TestFixture]
    public class ConfigureExtensionsTests
    {
        [Test]
        public void SetLogResolverSetsLogManagerGetLogger()
        {
            Func<string, ILog> resolver = (s) =>
            {
                return null;
            };

            var configureExtensions = new ConfigureExtensions();
            configureExtensions.SetLogResolver(resolver);

            Assert.AreSame(resolver, LogManager.GetLogger);
        }

        [SetUp]
        public void SetUp()
        {
            // Ensure that the GetLogger method is cleared before each test.
            LogManager.GetLogger = null;
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            // Ensure that the GetLogger method is cleared after all tests have been run.
            LogManager.GetLogger = null;
        }
    }
}