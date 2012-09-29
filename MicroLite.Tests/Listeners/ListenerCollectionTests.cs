namespace MicroLite.Tests.Core
{
    using System.Linq;
    using MicroLite.Listeners;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="ListenerCollection"/> class.
    /// </summary>
    [TestFixture]
    public class ListenerCollectionTests
    {
        [Test]
        public void AddOnlyAddsTypeOnce()
        {
            var listenerCollection = new ListenerCollection();
            listenerCollection.Clear();

            listenerCollection.Add<TestListener>();
            listenerCollection.Add<TestListener>();

            Assert.AreEqual(1, listenerCollection.Count());
        }

        [Test]
        public void ConstructorRegistersAssignedListener()
        {
            var listenerCollection = new ListenerCollection();

            var listener = listenerCollection.SingleOrDefault(x => x.GetType() == typeof(AssignedListener));

            Assert.NotNull(listener);
        }

        [Test]
        public void ConstructorRegistersGuidCombListener()
        {
            var listenerCollection = new ListenerCollection();

            var listener = listenerCollection.SingleOrDefault(x => x.GetType() == typeof(GuidCombListener));

            Assert.NotNull(listener);
        }

        [Test]
        public void ConstructorRegistersGuidListener()
        {
            var listenerCollection = new ListenerCollection();

            var listener = listenerCollection.SingleOrDefault(x => x.GetType() == typeof(GuidListener));

            Assert.NotNull(listener);
        }

        [Test]
        public void ConstructorRegistersIdentityListener()
        {
            var listenerCollection = new ListenerCollection();

            var listener = listenerCollection.SingleOrDefault(x => x.GetType() == typeof(IdentityListener));

            Assert.NotNull(listener);
        }

        [Test]
        public void EnumerationReturnsNewInstanceOfEachTypeOnEachCall()
        {
            var listenerCollection = new ListenerCollection();
            listenerCollection.Clear();

            listenerCollection.Add<TestListener>();

            var listener1 = listenerCollection.Single();
            var listener2 = listenerCollection.Single();

            Assert.AreNotSame(listener1, listener2);
        }

        private class TestListener : Listener
        {
        }
    }
}