namespace MicroLite.Tests.TypeConverters
{
    using System.Linq;
    using MicroLite.TypeConverters;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="TypeConverterCollection"/> class.
    /// </summary>
    public class TypeConverterCollectionTests
    {
        [Fact]
        public void ConstructorRegistersEnumTypeConverter()
        {
            var collection = new TypeConverterCollection();

            var typeConverter = collection.OfType<EnumTypeConverter>().SingleOrDefault();

            Assert.NotNull(typeConverter);
        }

        [Fact]
        public void ConstructorRegistersObjectTypeConverter()
        {
            var collection = new TypeConverterCollection();

            var typeConverter = collection.OfType<ObjectTypeConverter>().SingleOrDefault();

            Assert.NotNull(typeConverter);
        }

        [Fact]
        public void ConstructorRegistersXDocumentTypeConverter()
        {
            var collection = new TypeConverterCollection();

            var typeConverter = collection.OfType<XDocumentTypeConverter>().SingleOrDefault();

            Assert.NotNull(typeConverter);
        }
    }
}