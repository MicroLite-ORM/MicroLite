namespace MicroLite.Tests.Configuration
{
    using MicroLite.Configuration;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="Configure"/> class.
    /// </summary>

    public class ConfigureTests
    {
        [Fact]
        public void ExtensionsReturnsNewInstanceOnEachCall()
        {
            var extensions1 = Configure.Extensions();
            var extensions2 = Configure.Extensions();

            Assert.NotSame(extensions1, extensions2);
        }

        [Fact]
        public void FluentlyReturnsNewInstanceOnEachCall()
        {
            var configure1 = Configure.Fluently();
            var configure2 = Configure.Fluently();

            Assert.NotSame(configure1, configure2);
        }
    }
}