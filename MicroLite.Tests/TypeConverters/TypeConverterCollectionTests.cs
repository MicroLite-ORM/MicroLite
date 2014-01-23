namespace MicroLite.Tests.TypeConverters
{
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
            public void TheEnumTypeConverterShouldBePositionZero()
            {
                Assert.IsType<EnumTypeConverter>(this.collection[0]);
            }

            [Fact]
            public void TheObjectTypeConverterShouldBePositionThree()
            {
                Assert.IsType<ObjectTypeConverter>(this.collection[3]);
            }

            [Fact]
            public void ThereShouldBe4RegisteredTypeConverters()
            {
                Assert.Equal(4, this.collection.Count);
            }

            [Fact]
            public void TheUriTypeConverterShouldBePositionOne()
            {
                Assert.IsType<UriTypeConverter>(this.collection[1]);
            }

            [Fact]
            public void TheXDocumentTypeConverterShouldBePositionTwo()
            {
                Assert.IsType<XDocumentTypeConverter>(this.collection[2]);
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