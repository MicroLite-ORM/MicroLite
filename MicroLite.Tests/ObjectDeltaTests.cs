namespace MicroLite.Tests
{
    using System.Linq;
    using MicroLite.Tests.TestEntities;
    using Xunit;

    public class ObjectDeltaTests
    {
        public class WhenCallingAddChange
        {
            private readonly ObjectDelta objectDelta = new ObjectDelta(typeof(Customer), 1332);

            public WhenCallingAddChange()
            {
                this.objectDelta.AddChange("Name", "Fred");
            }

            [Fact]
            public void ChangesShouldContainTheChange()
            {
                var change = this.objectDelta.Changes.Single();
                Assert.Equal("Name", change.Key);
                Assert.Equal("Fred", change.Value);
            }
        }

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
    }
}