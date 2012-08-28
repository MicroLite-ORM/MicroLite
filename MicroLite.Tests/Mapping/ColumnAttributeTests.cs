namespace MicroLite.Tests.Mapping
{
    using MicroLite.Mapping;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="ColumnAttribute"/> class.
    /// </summary>
    [TestFixture]
    public class ColumnAttributeTests
    {
        [Test]
        public void ConstructorSetsAllowInsert()
        {
            var columnAttribute = new ColumnAttribute("Foo", allowInsert: true, allowUpdate: false);

            Assert.IsTrue(columnAttribute.AllowInsert);
        }

        [Test]
        public void ConstructorSetsAllowUpdate()
        {
            var columnAttribute = new ColumnAttribute("Foo", allowInsert: false, allowUpdate: true);

            Assert.IsTrue(columnAttribute.AllowUpdate);
        }

        [Test]
        public void ConstructorSetsName()
        {
            var columnName = "ObjectID";

            var columnAttribute = new ColumnAttribute(columnName);

            Assert.AreEqual(columnName, columnAttribute.Name);
        }
    }
}