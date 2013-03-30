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
        [Fact]
        public void AddOnlyAddsTypeOnce()
        {
            var listenerCollection = new ListenerCollection();
            listenerCollection.Clear();

            listenerCollection.Add<TestListener>();
            listenerCollection.Add<TestListener>();

            Assert.Equal(1, listenerCollection.Count());
        }

        [Fact]
        public void ConstructorRegistersAssignedListener()
        {
            var collection = new ListenerCollection();

            var listener = collection.OfType<AssignedListener>().SingleOrDefault();

            Assert.NotNull(listener);
        }

        [Fact]
        public void ConstructorRegistersDbGeneratedListener()
        {
            var collection = new ListenerCollection();

            var listener = collection.OfType<DbGeneratedListener>().SingleOrDefault();

            Assert.NotNull(listener);
        }

        [Fact]
        public void ConstructorRegistersGuidCombListener()
        {
            var collection = new ListenerCollection();

            var listener = collection.OfType<GuidCombListener>().SingleOrDefault();

            Assert.NotNull(listener);
        }

        [Fact]
        public void ConstructorRegistersGuidListener()
        {
            var collection = new ListenerCollection();

            var listener = collection.OfType<GuidListener>().SingleOrDefault();

            Assert.NotNull(listener);
        }

        [Fact]
        public void EnumerationReturnsNewInstanceOfEachTypeOnEachCall()
        {
            var collection = new ListenerCollection();
            collection.Clear();

            collection.Add<TestListener>();

            var listener1 = collection.Single();
            var listener2 = collection.Single();

            Assert.NotSame(listener1, listener2);
        }

        private class TestListener : Listener
        {
        }
    }
}