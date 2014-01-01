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
            private readonly TestTypeConverter typeConverter = new TestTypeConverter();

            public WhenCallingAdd()
            {
                this.collection.Add(this.typeConverter);
            }

            [Fact]
            public void TheTypeConverterShouldBeAddedAtTheTopOfTheList()
            {
                Assert.IsType<TestTypeConverter>(this.collection[0]);
            }
        }

        public class WhenCallingTheConstructor
        {
            private readonly TypeConverterCollection collection = new TypeConverterCollection();

            [Fact]
            public void TheEnumTypeConverterShouldBePositionOne()
            {
                Assert.IsType<EnumTypeConverter>(this.collection[1]);
            }

            [Fact]
            public void TheObjectTypeConverterShouldBePositionTwo()
            {
                Assert.IsType<ObjectTypeConverter>(this.collection[2]);
            }

            [Fact]
            public void ThereShouldBe3RegisteredTypeConverters()
            {
                Assert.Equal(3, this.collection.Count);
            }

            [Fact]
            public void TheXDocumentTypeConverterShouldBePositionZero()
            {
                Assert.IsType<XDocumentTypeConverter>(this.collection[0]);
            }
        }

        public class WhenEnumerating
        {
            private readonly TypeConverterCollection collection = new TypeConverterCollection();
            private readonly ITypeConverter typeConverter1;
            private readonly ITypeConverter typeConverter2;

            public WhenEnumerating()
            {
                collection.Clear();

                collection.Add(new TestTypeConverter());

                typeConverter1 = collection.Single();
                typeConverter2 = collection.Single();
            }

            [Fact]
            public void TheSameInstanceShouldBeReturned()
            {
                Assert.Same(typeConverter1, typeConverter2);
            }
        }

        private class TestTypeConverter : ITypeConverter
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
}