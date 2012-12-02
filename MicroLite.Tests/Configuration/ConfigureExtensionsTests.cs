namespace MicroLite.Tests.Configuration
{
    using System;
    using MicroLite.Configuration;
    using MicroLite.Logging;
    using MicroLite.Mapping;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ConfigureExtensions"/> class.
    /// </summary>
    public class ConfigureExtensionsTests : IDisposable
    {
        public ConfigureExtensionsTests()
        {
            // Ensure that the GetLogger method is cleared before each test.
            LogManager.GetLogger = null;

            // Ensure that the MappingConvention is cleared before each test.
            ObjectInfo.MappingConvention = null;
        }

        public void Dispose()
        {
            // Ensure that the GetLogger method is cleared after all tests have been run.
            LogManager.GetLogger = null;

            // Ensure that the MappingConvention is set to the default after all tests have been run.
            ObjectInfo.MappingConvention = new AttributeMappingConvention();
        }

        [Fact]
        public void SetLogResolverSetsLogManagerGetLogger()
        {
            Func<string, ILog> resolver = (s) =>
            {
                return null;
            };

            var configureExtensions = new ConfigureExtensions();
            configureExtensions.SetLogResolver(resolver);

            Assert.Same(resolver, LogManager.GetLogger);
        }

        [Fact]
        public void SetMappingConventionSetsObjectInfoMappingConvention()
        {
            var mappingConvention = new Mock<IMappingConvention>().Object;

            var configureExtensions = new ConfigureExtensions();
            configureExtensions.SetMappingConvention(mappingConvention);

            Assert.Same(mappingConvention, ObjectInfo.MappingConvention);
        }

        [Fact]
        public void SetMappingConventionThrowsArgumentNullExceptionForNullMappingConvention()
        {
            var configureExtensions = new ConfigureExtensions();

            var exception = Assert.Throws<ArgumentNullException>(() => configureExtensions.SetMappingConvention(null));

            Assert.Equal("mappingConvention", exception.ParamName);
        }
    }
}