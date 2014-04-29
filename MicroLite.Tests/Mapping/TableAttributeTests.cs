namespace MicroLite.Tests.Mapping
{
    using MicroLite.Mapping;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="TableAttribute" /> class.
    /// </summary>
    public class TableAttributeTests
    {
        [Fact]
        public void ConstructorSetsName()
        {
            var name = "Customers";

            var tableAttribute = new TableAttribute(name);

            Assert.Equal(name, tableAttribute.Name);
            Assert.Null(tableAttribute.Schema);
        }

        [Fact]
        public void ConstructorSetsSchemaAndName()
        {
            var schema = "dbo";
            var name = "Customers";

            var tableAttribute = new TableAttribute(schema, name);

            Assert.Equal(name, tableAttribute.Name);
            Assert.Equal(schema, tableAttribute.Schema);
        }
    }
}