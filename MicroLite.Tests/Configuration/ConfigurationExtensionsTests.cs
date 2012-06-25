namespace MicroLite.Tests.Configuration
{
    using MicroLite.Configuration;
    using MicroLite.Mapping;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="ConfigurationExtensionsTests"/> class.
    /// </summary>
    [TestFixture]
    public class ConfigurationExtensionsTests
    {
        [SetUp]
        public void SetUp()
        {
            // Ensure that the MappingConvention is cleared before each test.
            ObjectInfo.MappingConvention = null;
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            // Ensure that the MappingConvention is set to the default after all tests have been run.
            ObjectInfo.MappingConvention = new MicroLite.Mapping.LooseAttributeMappingConvention();
        }

        [Test]
        public void WithStrictAttributeMapping()
        {
            var configureExtensions = new ConfigureExtensions();
            configureExtensions.WithStrictAttributeMapping();

            Assert.IsInstanceOf<MicroLite.Mapping.StrictAttributeMappingConvention>(ObjectInfo.MappingConvention);
        }
    }
}