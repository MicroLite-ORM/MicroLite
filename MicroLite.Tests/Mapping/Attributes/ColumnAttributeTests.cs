namespace MicroLite.Tests.Mapping.Attributes
{
    using MicroLite.Mapping.Attributes;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ColumnAttribute" /> class.
    /// </summary>
    public class ColumnAttributeTests
    {
        [Fact]
        public void ConstructorSetsAllowInsert()
        {
            var columnAttribute = new ColumnAttribute("Foo", allowInsert: true, allowUpdate: false);

            Assert.Equal("Foo", columnAttribute.Name);
            Assert.True(columnAttribute.AllowInsert);
            Assert.False(columnAttribute.AllowUpdate);
        }

        [Fact]
        public void ConstructorSetsAllowUpdate()
        {
            var columnAttribute = new ColumnAttribute("Foo", allowInsert: false, allowUpdate: true);

            Assert.Equal("Foo", columnAttribute.Name);
            Assert.False(columnAttribute.AllowInsert);
            Assert.True(columnAttribute.AllowUpdate);
        }

        [Fact]
        public void ConstructorSetsNameDbTypeToNullAndAllowInsertAndAllowUpdateToTrue()
        {
            var columnAttribute = new ColumnAttribute("ObjectID");

            Assert.Equal("ObjectID", columnAttribute.Name);
            Assert.True(columnAttribute.AllowInsert);
            Assert.True(columnAttribute.AllowUpdate);
        }
    }
}