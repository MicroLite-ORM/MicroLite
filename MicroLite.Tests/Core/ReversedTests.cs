using MicroLite.Core;
using Xunit;

namespace MicroLite.Tests.Core
{
    public class ReversedTests
    {
        [Fact]
        public void ArrayIsReversedByConstructorAndRestoredByDispose()
        {
            var values = new[] { 1, 2, 3, 4, 5 };

            using (new Reversed<int>(values))
            {
                Assert.Equal(5, values[0]);
                Assert.Equal(4, values[1]);
                Assert.Equal(3, values[2]);
                Assert.Equal(2, values[3]);
                Assert.Equal(1, values[4]);
            }

            Assert.Equal(1, values[0]);
            Assert.Equal(2, values[1]);
            Assert.Equal(3, values[2]);
            Assert.Equal(4, values[3]);
            Assert.Equal(5, values[4]);
        }
    }
}