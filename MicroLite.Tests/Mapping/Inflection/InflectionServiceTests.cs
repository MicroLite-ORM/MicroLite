namespace MicroLite.Tests.Mapping.Inflection
{
    using MicroLite.Mapping.Inflection;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="InflectionService" /> class.
    /// </summary>
    public class InflectionServiceTests
    {
        [Fact]
        public void English_ReturnsSameInstanceEachTime()
        {
            var service1 = InflectionService.English;
            var service2 = InflectionService.English;

            Assert.Same(service1, service2);
        }
    }
}