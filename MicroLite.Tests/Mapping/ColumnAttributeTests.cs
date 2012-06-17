namespace MicroLite.Tests.Mapping
{
    using MicroLite.Mapping;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="ColumnAttribute"/> class.
    /// </summary>
    public class ColumnAttributeTests
    {
        [Test]
        public void ConstructorSetsName()
        {
            var columnName = "ObjectID";

            var columnAttribute = new ColumnAttribute(columnName);

            Assert.AreEqual(columnName, columnAttribute.Name);
        }
    }
}