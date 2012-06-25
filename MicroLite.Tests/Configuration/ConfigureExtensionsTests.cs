namespace MicroLite.Tests.Configuration
{
    using System;
    using MicroLite.Configuration;
    using MicroLite.Logging;
    using MicroLite.Mapping;
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

        [Test]
        public void SetMappingConventionSetsObjectInfoMappingConvention()
        {
            var configureExtensions = new ConfigureExtensions();
            configureExtensions.SetMappingConvention(new MicroLite.Mapping.StrictAttributeMappingConvention());

            Assert.IsInstanceOf<MicroLite.Mapping.StrictAttributeMappingConvention>(ObjectInfo.MappingConvention);
        }

        [Test]
        public void SetMappingConventionThrowsArgumentNullExceptionForNullMappingConvention()
        {
            var configureExtensions = new ConfigureExtensions();

            var exception = Assert.Throws<ArgumentNullException>(() => configureExtensions.SetMappingConvention(null));

            Assert.AreEqual("mappingConvention", exception.ParamName);
        }

        [SetUp]
        public void SetUp()
        {
            // Ensure that the GetLogger method is cleared before each test.
            LogManager.GetLogger = null;

            // Ensure that the MappingConvention is cleared before each test.
            ObjectInfo.MappingConvention = null;
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            // Ensure that the GetLogger method is cleared after all tests have been run.
            LogManager.GetLogger = null;

            // Ensure that the MappingConvention is set to the default after all tests have been run.
            ObjectInfo.MappingConvention = new MicroLite.Mapping.LooseAttributeMappingConvention();
        }
    }
}