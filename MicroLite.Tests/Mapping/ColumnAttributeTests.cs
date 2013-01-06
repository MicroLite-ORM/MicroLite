namespace MicroLite.Tests.Mapping
{
    using MicroLite.Mapping;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ColumnAttribute"/> class.
    /// </summary>
    public class ColumnAttributeTests
    {
        [Fact]
        public void ConstructorSetsAllowInsert()
        {
            var columnAttribute = new ColumnAttribute("Foo", allowInsert: true, allowUpdate: false);

            Assert.True(columnAttribute.AllowInsert);
        }

        [Fact]
        public void ConstructorSetsAllowUpdate()
        {
            var columnAttribute = new ColumnAttribute("Foo", allowInsert: false, allowUpdate: true);

            Assert.True(columnAttribute.AllowUpdate);
        }

        [Fact]
        public void ConstructorSetsName()
        {
            var columnName = "ObjectID";

            var columnAttribute = new ColumnAttribute(columnName);

            Assert.Equal(columnName, columnAttribute.Name);
        }
    }
}