namespace MicroLite.Tests.Mapping
{
    using MicroLite.Mapping;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="IdentifierAttribute"/> class.
    /// </summary>
    public class IdentifierAttributeTests
    {
        [Fact]
        public void ConstructorSetsIdentifierStrategy()
        {
            var identifierStrategy = IdentifierStrategy.Assigned;

            var identifierAttribute = new IdentifierAttribute(identifierStrategy);

            Assert.Equal(identifierStrategy, identifierAttribute.IdentifierStrategy);
        }
    }
}