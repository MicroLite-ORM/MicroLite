namespace MicroLite.Tests
{
    using System;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="GuidGenerator"/> class.
    /// </summary>
    public class GuidGeneratorTests
    {
        public class WhenCallingCreateComb
        {
            private readonly Guid guid1 = GuidGenerator.CreateComb();
            private readonly Guid guid2 = GuidGenerator.CreateComb();

            [Fact]
            public void EachGuidReturnedShouldBeUnique()
            {
                Assert.NotEqual(this.guid1, this.guid2);
            }

            [Fact]
            public void TheGuidShouldNotBeAnEmptyGuid()
            {
                Assert.NotEqual(Guid.Empty, this.guid1);
                Assert.NotEqual(Guid.Empty, this.guid2);
            }
        }

        public class WhenCallingCreateCombUsingTheSameSeed
        {
            private readonly Guid guid1;
            private readonly Guid guid2;

            public WhenCallingCreateCombUsingTheSameSeed()
            {
                var dateTime = DateTime.Now;

                this.guid1 = GuidGenerator.CreateComb(dateTime);
                this.guid2 = GuidGenerator.CreateComb(dateTime);
            }

            [Fact]
            public void EachGuidReturnedShouldBeUnique()
            {
                Assert.NotEqual(this.guid1, this.guid2);
            }

            [Fact]
            public void TheGuidShouldNotBeAnEmptyGuid()
            {
                Assert.NotEqual(Guid.Empty, this.guid1);
                Assert.NotEqual(Guid.Empty, this.guid2);
            }
        }
    }
}