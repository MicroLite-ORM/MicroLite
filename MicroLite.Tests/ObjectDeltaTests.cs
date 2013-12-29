namespace MicroLite.Tests
{
    using Xunit;

    public class ObjectDeltaTests
    {
        public class WhenConstructed
        {
            private readonly ObjectDelta objectDelta;

            public WhenConstructed()
            {
                this.objectDelta = new ObjectDelta(typeof(Customer), 1332);
            }

            [Fact]
            public void ChangesIsEmpty()
            {
                Assert.Empty(this.objectDelta.Changes);
            }

            [Fact]
            public void ForTypeIsSet()
            {
                Assert.Equal(typeof(Customer), this.objectDelta.ForType);
            }

            [Fact]
            public void IdentifierIsSet()
            {
                Assert.Equal(1332, this.objectDelta.Identifier);
            }
        }

        private class Customer
        {
        }
    }
}