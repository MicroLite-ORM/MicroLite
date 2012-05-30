namespace MicroLite.Tests.Core
{
    using System.Linq;
    using MicroLite.Listeners;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="ListenerManager"/> class.
    /// </summary>
    [TestFixture]
    public class ListenerManagerTests
    {
        [Test]
        public void AddOnlyAddsTypeOnce()
        {
            ListenerManager.Add<TestListener>();
            ListenerManager.Add<TestListener>();

            Assert.AreEqual(1, ListenerManager.Create().Count());
        }

        [Test]
        public void CreateReturnsNewInstanceOfEachTypeOnEachCall()
        {
            ListenerManager.Add<TestListener>();

            var listener1 = ListenerManager.Create().Single();
            var listener2 = ListenerManager.Create().Single();

            Assert.AreNotSame(listener1, listener2);
        }

        [SetUp]
        public void SetUp()
        {
            ListenerManager.Clear();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            ListenerManager.Clear();
        }

        private class TestListener : Listener
        {
        }
    }
}