namespace MicroLite.Tests
{
    using System.Linq;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="ExtensionManager"/> class.
    /// </summary>
    [TestFixture]
    public class ExtensionManagerTests
    {
        [Test]
        public void CreateListenersCallsFactoryCreatedByRegisterListenerT()
        {
            ExtensionManager.RegisterListener<TestListener>();

            var listener = ExtensionManager.CreateListeners().Single();

            Assert.IsInstanceOf<TestListener>(listener);
        }

        [Test]
        public void RegisterListenerOnlyRegistersTypeOnce()
        {
            ExtensionManager.RegisterListener<TestListener>();
            ExtensionManager.RegisterListener<TestListener>();

            Assert.AreEqual(1, ExtensionManager.CreateListeners().Count());
        }

        [SetUp]
        public void SetUp()
        {
            ExtensionManager.ClearListeners();
        }

        [TestFixtureTearDown]
        public void TearDown()
        {
            ExtensionManager.ClearListeners();
        }

        private class TestListener : Listener
        {
        }
    }
}