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
        public class WhenCallingAdd
        {
            private readonly TypeConverterCollection collection = new TypeConverterCollection();
            private readonly FakeTypeConverter typeConverter = new FakeTypeConverter();

            public WhenCallingAdd()
            {
                this.collection.Add(this.typeConverter);
            }

            [Fact]
            public void TheCollectionShouldContainTheAddedInstance()
            {
                var typeConverter = this.collection.SingleOrDefault(t => t == this.typeConverter);

                Assert.NotNull(typeConverter);
            }

            private class FakeTypeConverter : ITypeConverter
            {
                public bool CanConvert(System.Type propertyType)
                {
                    throw new System.NotImplementedException();
                }

                public object ConvertFromDbValue(object value, System.Type propertyType)
                {
                    throw new System.NotImplementedException();
                }

                public object ConvertToDbValue(object value, System.Type propertyType)
                {
                    throw new System.NotImplementedException();
                }
            }
        }

        public class WhenCallingTheConstructor
        {
            private readonly TypeConverterCollection collection = new TypeConverterCollection();

            [Fact]
            public void ConstructorRegistersEnumTypeConverter()
            {
                var typeConverter = this.collection.OfType<EnumTypeConverter>().SingleOrDefault();

                Assert.NotNull(typeConverter);
            }

            [Fact]
            public void ConstructorRegistersObjectTypeConverter()
            {
                var typeConverter = this.collection.OfType<ObjectTypeConverter>().SingleOrDefault();

                Assert.NotNull(typeConverter);
            }

            [Fact]
            public void ConstructorRegistersXDocumentTypeConverter()
            {
                var typeConverter = this.collection.OfType<XDocumentTypeConverter>().SingleOrDefault();

                Assert.NotNull(typeConverter);
            }
        }
    }
}