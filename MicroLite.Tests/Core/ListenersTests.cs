namespace MicroLite.Tests.Core
{
    using System.Linq;
    using MicroLite.Core;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="Listeners"/> class.
    /// </summary>
    [TestFixture]
    public class ListenersTests
    {
        [Test]
        public void AddOnlyAddsTypeOnce()
        {
            Listeners.Add<TestListener>();
            Listeners.Add<TestListener>();

            Assert.AreEqual(1, Listeners.Create().Count());
        }

        [Test]
        public void CreateReturnsNewInstanceOfEachTypeOnEachCall()
        {
            Listeners.Add<TestListener>();

            var listener1 = Listeners.Create().Single();
            var listener2 = Listeners.Create().Single();

            Assert.AreNotSame(listener1, listener2);
        }

        [SetUp]
        public void SetUp()
        {
            Listeners.Clear();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            Listeners.Clear();
        }

        private class TestListener : Listener
        {
        }
    }
}