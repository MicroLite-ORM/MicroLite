namespace MicroLite.Tests
{
    using System;
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
            public void ChangeCountShouldReturnTheCorrectNumberOfChanges()
            {
                Assert.Equal(1, this.objectDelta.ChangeCount);
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
            public void ChangeCountShouldReturnZero()
            {
                Assert.Equal(0, this.objectDelta.ChangeCount);
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

        public class WhenConstructedWithNullIdentifier
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new ObjectDelta(typeof(Customer), null));

                Assert.Equal("identifier", exception.ParamName);
            }
        }

        public class WhenConstructedWithNullType
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(() => new ObjectDelta(null, 1332));

                Assert.Equal("forType", exception.ParamName);
            }
        }
    }
}