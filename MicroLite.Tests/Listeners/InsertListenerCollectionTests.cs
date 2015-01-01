namespace MicroLite.Tests.Listeners
{
    using System.Linq;
    using MicroLite.Listeners;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="InsertListenerCollection"/> class.
    /// </summary>
    public class InsertListenerCollectionTests
    {
        public class WhenCallingAdd
        {
            private readonly InsertListenerCollection collection = new InsertListenerCollection();
            private readonly TestListener listener = new TestListener();

            public WhenCallingAdd()
            {
                this.collection.Add(this.listener);
            }

            [Fact]
            public void TheListenerShouldBeAddedAtTheTopOfTheList()
            {
                // The second listener should be added at 0.
                Assert.Same(this.listener, this.collection[0]);
            }
        }

        public class WhenCallingTheConstructor
        {
            private readonly InsertListenerCollection collection = new InsertListenerCollection();

            [Fact]
            public void ConstructorRegistersIdentifierStrategyListener()
            {
                var listener = this.collection.OfType<IdentifierStrategyListener>().SingleOrDefault();

                Assert.NotNull(listener);
            }

            [Fact]
            public void ThereShouldBe1RegisteredListener()
            {
                Assert.Equal(1, this.collection.Count);
            }
        }

        private class TestListener : IInsertListener
        {
            public void AfterInsert(object instance, object executeScalarResult)
            {
            }

            public void BeforeInsert(object instance)
            {
            }
        }
    }
}