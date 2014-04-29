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
        public class WhenCallingSetLogResolver : UnitTest
        {
            private readonly Func<string, ILog> resolver = (s) =>
            {
                return new EmptyLog();
            };

            public WhenCallingSetLogResolver()
            {
                var configureExtensions = new ConfigureExtensions();
                configureExtensions.SetLogResolver(this.resolver);
            }

            [Fact]
            public void TheLogManagerGetLoggerMethodShouldBeSet()
            {
                Assert.Same(this.resolver, LogManager.GetLogger);
            }
        }

        public class WhenCallingSetMappingConvention : UnitTest
        {
            private readonly IMappingConvention mappingConvention = new Mock<IMappingConvention>().Object;

            public WhenCallingSetMappingConvention()
            {
                var configureExtensions = new ConfigureExtensions();
                configureExtensions.SetMappingConvention(this.mappingConvention);
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