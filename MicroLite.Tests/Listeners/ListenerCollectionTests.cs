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
            var listenerCollection = new ListenerCollection();

            var listener = listenerCollection.SingleOrDefault(x => x.GetType() == typeof(AssignedListener));

            Assert.NotNull(listener);
        }

        [Fact]
        public void ConstructorRegistersAutoIncrementListener()
        {
            var listenerCollection = new ListenerCollection();

            var listener = listenerCollection.SingleOrDefault(x => x.GetType() == typeof(AutoIncrementListener));

            Assert.NotNull(listener);
        }

        [Fact]
        public void ConstructorRegistersGuidCombListener()
        {
            var listenerCollection = new ListenerCollection();

            var listener = listenerCollection.SingleOrDefault(x => x.GetType() == typeof(GuidCombListener));

            Assert.NotNull(listener);
        }

        [Fact]
        public void ConstructorRegistersGuidListener()
        {
            var listenerCollection = new ListenerCollection();

            var listener = listenerCollection.SingleOrDefault(x => x.GetType() == typeof(GuidListener));

            Assert.NotNull(listener);
        }

        [Fact]
        public void ConstructorRegistersIdentityListener()
        {
            var listenerCollection = new ListenerCollection();

            var listener = listenerCollection.SingleOrDefault(x => x.GetType() == typeof(IdentityListener));

            Assert.NotNull(listener);
        }

        [Fact]
        public void EnumerationReturnsNewInstanceOfEachTypeOnEachCall()
        {
            var listenerCollection = new ListenerCollection();
            listenerCollection.Clear();

            listenerCollection.Add<TestListener>();

            var listener1 = listenerCollection.Single();
            var listener2 = listenerCollection.Single();

            Assert.NotSame(listener1, listener2);
        }

        private class TestListener : Listener
        {
        }
    }
}