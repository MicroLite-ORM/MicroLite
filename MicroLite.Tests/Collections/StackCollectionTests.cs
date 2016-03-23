namespace MicroLite.Tests.Collections
{
    using MicroLite.Collections;
    using Xunit;

    public class StackCollectionTests
    {
        public class WhenAddingItems
        {
            private readonly StackCollection<string> collection = new StackCollection<string>();

            [Fact]
            public void ItemsAreAddedToTheTopOfTheCollection()
            {
                collection.Add("Added First");
                Assert.Equal(1, collection.Count);
                Assert.Equal("Added First", collection[0]);

                collection.Add("Added Second");
                Assert.Equal(2, collection.Count);
                Assert.Equal("Added Second", collection[0]);
                Assert.Equal("Added First", collection[1]);
            }
        }

        public class WhenConstructed
        {
            private readonly StackCollection<string> collection = new StackCollection<string>();

            [Fact]
            public void TheCollectionIsEmpty()
            {
                Assert.Empty(collection);
            }
        }
    }
}