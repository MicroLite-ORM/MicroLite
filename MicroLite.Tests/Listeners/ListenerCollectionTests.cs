namespace MicroLite.Tests.Listeners
{
    using System.Linq;
    using MicroLite.Listeners;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ListenerCollection"/> class.
    /// </summary>
    public class ListenerCollectionTests
    {
        public class WhenCallingAdd
        {
            private readonly ListenerCollection collection = new ListenerCollection();
            private readonly TestListener listener = new TestListener();

            public WhenCallingAdd()
            {
                this.collection.Add(this.listener);
            }

            [Fact]
            public void TheListenerShouldBeAddedAtTheTopOfTheList()
            {
                Assert.IsType<TestListener>(this.collection[0]);
            }
        }

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
            public void ThereShouldBe2RegisteredListeners()
            {
                Assert.Equal(2, this.collection.Count);
            }
        }

        private class TestListener : Listener
        {
        }
    }
}