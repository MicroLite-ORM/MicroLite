namespace MicroLite.Tests
{
    using System;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="GuidGenerator"/> class.
    /// </summary>

    public class GuidGeneratorTests
    {
        [Fact]
        public void CreateCombDifferentGuidEachTime()
        {
            var guid1 = GuidGenerator.CreateComb();
            var guid2 = GuidGenerator.CreateComb();

            Assert.NotEqual(guid1, guid2);
        }

        [Fact]
        public void CreateCombDifferentGuidsEachTimeEvenIfSameDateTimeSeedIsUsed()
        {
            var dateTime = DateTime.Now;

            var guid1 = GuidGenerator.CreateComb(dateTime);
            var guid2 = GuidGenerator.CreateComb(dateTime);

            Assert.NotEqual(guid1, guid2);
        }

        [Fact]
        public void CreateCombDoesNotCreateEmptyGuid()
        {
            Assert.NotEqual(Guid.Empty, GuidGenerator.CreateComb());
        }
    }
}