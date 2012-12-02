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
    public class ConfigureExtensionsTests
    {
        public class WhenCallingSetLogResolver : IDisposable
        {
            private readonly Func<string, ILog> resolver = (s) =>
            {
                return null;
            };

            public WhenCallingSetLogResolver()
            {
                // Ensure that the GetLogger method is cleared before each test.
                LogManager.GetLogger = null;

                var configureExtensions = new ConfigureExtensions();
                configureExtensions.SetLogResolver(this.resolver);
            }

            public void Dispose()
            {
                // Ensure that the GetLogger method is cleared after all tests have been run.
                LogManager.GetLogger = null;
            }

            [Fact]
            public void TheLogManagerGetLoggerMethodShouldBeSet()
            {
                Assert.Same(this.resolver, LogManager.GetLogger);
            }
        }

        public class WhenCallingSetMappingConvention : IDisposable
        {
            private readonly IMappingConvention mappingConvention = new Mock<IMappingConvention>().Object;

            public WhenCallingSetMappingConvention()
            {
                // Ensure that the MappingConvention is cleared before each test.
                ObjectInfo.MappingConvention = null;

                var configureExtensions = new ConfigureExtensions();
                configureExtensions.SetMappingConvention(this.mappingConvention);
            }

            public void Dispose()
            {
                // Ensure that the MappingConvention is set to the default after all tests have been run.
                ObjectInfo.MappingConvention = new AttributeMappingConvention();
            }

            [Fact]
            public void TheObjectInfoMappingConventionShouldBeSet()
            {
                Assert.Same(this.mappingConvention, ObjectInfo.MappingConvention);
            }
        }

        public class WhenCallingSetMappingConventionAndTheMappingConventionIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var configureExtensions = new ConfigureExtensions();

                var exception = Assert.Throws<ArgumentNullException>(() => configureExtensions.SetMappingConvention(null));

                Assert.Equal("mappingConvention", exception.ParamName);
            }
        }
    }
}