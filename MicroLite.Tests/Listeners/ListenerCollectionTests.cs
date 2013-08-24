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
        public class WhenCallingAdd
        {
            private readonly ListenerCollection collection = new ListenerCollection();
            private readonly TestListener listener = new TestListener();

            public WhenCallingAdd()
            {
                this.collection.Add(this.listener);
            }

            [Fact]
            public void TheCollectionShouldContainTheAddedInstance()
            {
                var typeConverter = this.collection.SingleOrDefault(t => t == this.listener);

                Assert.NotNull(typeConverter);
            }
        }

        public class WhenCallingClear
        {
            private readonly ListenerCollection collection = new ListenerCollection();

            public WhenCallingClear()
            {
                this.collection.Clear();
            }

            [Fact]
            public void TheCollectionShouldBeEmpty()
            {
                Assert.Equal(0, this.collection.Count);
            }
        }

        public class WhenCallingCopyTo
        {
            private readonly IListener[] array;
            private readonly ListenerCollection collection = new ListenerCollection();

            public WhenCallingCopyTo()
            {
                this.array = new IListener[collection.Count];
                collection.CopyTo(this.array, 0);
            }

            [Fact]
            public void TheItemsInTheArrayShouldMatchTheItemsInTheCollection()
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    Assert.Same(this.array[i], this.collection.Skip(i).First());
                }
            }
        }

        public class WhenCallingRemove
        {
            private readonly ListenerCollection collection = new ListenerCollection();
            private IListener listenerToRemove;

            public WhenCallingRemove()
            {
                listenerToRemove = this.collection.OfType<DbGeneratedListener>().Single();
                this.collection.Remove(listenerToRemove);
            }

            [Fact]
            public void TheListenerShouldBeRemoved()
            {
                Assert.False(this.collection.Contains(listenerToRemove));
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
            public void TheCollectionShouldNotBeReadOnly()
            {
                Assert.False(this.collection.IsReadOnly);
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
            private readonly IListener listener1 = new TestListener();
            private readonly IListener listener2 = new TestListener();

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