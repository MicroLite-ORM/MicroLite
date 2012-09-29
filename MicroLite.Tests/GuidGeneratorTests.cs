namespace MicroLite.Tests
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="GuidGenerator"/> class.
    /// </summary>
    [TestFixture]
    public class GuidGeneratorTests
    {
        [Test]
        public void CreateCombDifferentGuidEachTime()
        {
            var guid1 = GuidGenerator.CreateComb();
            var guid2 = GuidGenerator.CreateComb();

            Assert.AreNotEqual(guid1, guid2);
        }

        [Test]
        public void CreateCombDifferentGuidsEachTimeEvenIfSameDateTimeSeedIsUsed()
        {
            var dateTime = DateTime.Now;

            var guid1 = GuidGenerator.CreateComb(dateTime);
            var guid2 = GuidGenerator.CreateComb(dateTime);

            Assert.AreNotEqual(guid1, guid2);
        }

        [Test]
        public void CreateCombDoesNotCreateEmptyGuid()
        {
            Assert.AreNotEqual(Guid.Empty, GuidGenerator.CreateComb());
        }
    }
}