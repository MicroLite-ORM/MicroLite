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
            public void TheCollectionShouldContainTheAddedInstance()
            {
                var typeConverter = this.collection.SingleOrDefault(t => t == this.typeConverter);

                Assert.NotNull(typeConverter);
            }
        }

        public class WhenCallingClear
        {
            private readonly TypeConverterCollection collection = new TypeConverterCollection();

            public WhenCallingClear()
            {
                this.collection.Clear();
            }

            [Fact]
            public void TheCollectionShouldBeEmpty()
            {
                Assert.Equal(0, this.collection.Count);
            }
        }

        public class WhenCallingCopyTo
        {
            private readonly ITypeConverter[] array;
            private readonly TypeConverterCollection collection = new TypeConverterCollection();

            public WhenCallingCopyTo()
            {
                this.array = new ITypeConverter[collection.Count];
                collection.CopyTo(this.array, 0);
            }

            [Fact]
            public void TheItemsInTheArrayShouldMatchTheItemsInTheCollection()
            {
                for (int i = 0; i < collection.Count; i++)
                {
                    Assert.Same(this.array[i], this.collection.Skip(i).First());
                }
            }
        }

        public class WhenCallingRemove
        {
            private readonly TypeConverterCollection collection = new TypeConverterCollection();
            private ITypeConverter typeConverterToRemove;

            public WhenCallingRemove()
            {
                typeConverterToRemove = this.collection.OfType<ObjectTypeConverter>().Single();
                this.collection.Remove(typeConverterToRemove);
            }

            [Fact]
            public void TheTypeConverterShouldBeRemoved()
            {
                Assert.False(this.collection.Contains(typeConverterToRemove));
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

            [Fact]
            public void TheCollectionShouldNotBeReadOnly()
            {
                Assert.False(this.collection.IsReadOnly);
            }

            [Fact]
            public void ThereShouldBe3RegisteredTypeConverters()
            {
                Assert.Equal(3, this.collection.Count);
            }
        }

        public class WhenEnumerating
        {
            private readonly TypeConverterCollection collection = new TypeConverterCollection();
            private readonly ITypeConverter typeConverter1 = new TestTypeConverter();
            private readonly ITypeConverter typeConverter2 = new TestTypeConverter();

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