namespace MicroLite.Tests
{
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="IdentifierAttribute"/> class.
    /// </summary>
    public class IdentifierAttributeTests
    {
        [Test]
        public void ConstructorSetsIdentifierStrategy()
        {
            var identifierStrategy = IdentifierStrategy.Assigned;

            var identifierAttribute = new IdentifierAttribute(identifierStrategy);

            Assert.AreEqual(identifierStrategy, identifierAttribute.IdentifierStrategy);
        }
    }
}