namespace MicroLite.Tests
{
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="TableAttribute"/> class.
    /// </summary>
    public class TableAttributeTests
    {
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