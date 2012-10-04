namespace MicroLite.Tests.Mapping
{
    using MicroLite.Mapping;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="TableAttribute"/> class.
    /// </summary>
    [TestFixture]
    public class TableAttributeTests
    {
        [Test]
        public void ConstructorSetsName()
        {
            var name = "Customers";

            var tableAttribute = new TableAttribute(name);

            Assert.AreEqual(name, tableAttribute.Name);
            Assert.IsNull(tableAttribute.Schema);
        }

        [Test]
        public void ConstructorSetsSchemaAndName()
        {
            var schema = "dbo";
            var name = "Customers";

            var tableAttribute = new TableAttribute(schema, name);

            Assert.AreEqual(name, tableAttribute.Name);
            Assert.AreEqual(schema, tableAttribute.Schema);
        }
    }
}