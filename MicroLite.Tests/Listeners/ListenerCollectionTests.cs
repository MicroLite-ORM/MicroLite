namespace MicroLite.Tests.Core
{
    using System.Linq;
    using MicroLite.Listeners;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ListenerCollection"/> class.
    /// </summary>
    public class ListenerCollectionTests
    {
        public class WhenCallingTheConstructor
        {
            private readonly ListenerCollection collection = new ListenerCollection();

            [Fact]
            public void ConstructorRegistersAssignedListener()
            {
                var listener = this.collection.OfType<AssignedListener>().SingleOrDefault();

                Assert.NotNull(listener);
            }

            [Fact]
            public void ConstructorRegistersDbGeneratedListener()
            {
                var listener = this.collection.OfType<DbGeneratedListener>().SingleOrDefault();

                Assert.NotNull(listener);
            }

            [Fact]
            public void ConstructorRegistersGuidCombListener()
            {
                var listener = this.collection.OfType<GuidCombListener>().SingleOrDefault();

                Assert.NotNull(listener);
            }

            [Fact]
            public void ConstructorRegistersGuidListener()
            {
                var listener = this.collection.OfType<GuidListener>().SingleOrDefault();

                Assert.NotNull(listener);
            }

            [Fact]
            public void ThereShouldBe4RegisteredListeners()
            {
                Assert.Equal(4, this.collection.Count);
            }
        }

        public class WhenEnumerating
        {
            private readonly ListenerCollection collection = new ListenerCollection();
            private readonly IListener listener1;
            private readonly IListener listener2;

            public WhenEnumerating()
            {
                collection.Clear();

                collection.Add(new TestListener());

                listener1 = collection.Single();
                listener2 = collection.Single();
            }

            [Fact]
            public void TheSameInstanceShouldBeReturned()
            {
                Assert.Same(listener1, listener2);
            }
        }

        private class TestListener : Listener
        {
        }
    }
}