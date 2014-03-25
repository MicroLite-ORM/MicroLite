namespace MicroLite.Tests.TypeConverters
{
    using System;
    using System.Xml.Linq;
    using MicroLite.Tests.TestEntities;
    using MicroLite.TypeConverters;
    using Xunit;

    public class TypeConverterTests
    {
        public class WhenCallingDefault
        {
            [Fact]
            public void AnObjectTypeConverterIsReturned()
            {
                var typeConverter = TypeConverter.Default;
                Assert.IsType<ObjectTypeConverter>(typeConverter);
            }

            [Fact]
            public void TheSameInstanceIsReturnedEachTime()
            {
                var typeConverter1 = TypeConverter.Default;
                var typeConverter2 = TypeConverter.Default;

                Assert.Same(typeConverter1, typeConverter2);
            }
        }

        public class WhenCallingFor_WithATypeOfEnum
        {
            [Fact]
            public void TheEnumTypeConverterIsReturned()
            {
                var typeConverter = TypeConverter.For(typeof(CustomerStatus));
                Assert.IsType<EnumTypeConverter>(typeConverter);
            }
        }

        public class WhenCallingFor_WithATypeOfInt
        {
            [Fact]
            public void NoTypeConverterIsReturned()
            {
                var typeConverter = TypeConverter.For(typeof(int));
                Assert.Null(typeConverter);
            }
        }

        public class WhenCallingForWith_ATypeOfUri
        {
            [Fact]
            public void TheUriTypeConverterConverterIsReturned()
            {
                var typeConverter = TypeConverter.For(typeof(Uri));
                Assert.IsType<MicroLite.TypeConverters.UriTypeConverter>(typeConverter);
            }
        }

        public class WhenCallingForWith_ATypeOfXDocument
        {
            [Fact]
            public void TheXDocumentTypeConverterIsReturned()
            {
                var typeConverter = TypeConverter.For(typeof(XDocument));
                Assert.IsType<XDocumentTypeConverter>(typeConverter);
            }
        }

        public class WhenCallingIsNotEntityAndConvertible
        {
            [Fact]
            public void FalseIsReturnedForAnyOtherType()
            {
                Assert.False(TypeConverter.IsNotEntityAndConvertible(typeof(Customer)));
            }

            [Fact]
            public void TrueIsReturnedForRegisteredTypeConverters()
            {
                Assert.True(TypeConverter.IsNotEntityAndConvertible(typeof(CustomerStatus)));
                Assert.True(TypeConverter.IsNotEntityAndConvertible(typeof(XDocument)));
                Assert.True(TypeConverter.IsNotEntityAndConvertible(typeof(Uri)));
            }

            [Fact]
            public void TrueIsReturnedForValueTypes()
            {
                Assert.True(TypeConverter.IsNotEntityAndConvertible(typeof(int)));
                Assert.True(TypeConverter.IsNotEntityAndConvertible(typeof(DateTime)));
                Assert.True(TypeConverter.IsNotEntityAndConvertible(typeof(Guid)));
            }
        }

        public class WhenCallingIsNotEntityAndConvertible_AndTheTypeIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => TypeConverter.IsNotEntityAndConvertible(null));

                Assert.Equal("type", exception.ParamName);
            }
        }

        public class WhenCallingResolveActualType_AndTheTypeIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                var exception = Assert.Throws<ArgumentNullException>(
                    () => TypeConverter.ResolveActualType(null));

                Assert.Equal("type", exception.ParamName);
            }
        }

        public class WhenCallingResolveActualTypeWithANonNullableType
        {
            [Fact]
            public void TheSameTypeIsReturned()
            {
                Assert.Equal(typeof(string), TypeConverter.ResolveActualType(typeof(string)));
            }
        }

        public class WhenCallingResolveActualTypeWithANullableType
        {
            [Fact]
            public void TheInnerTypeIsReturned()
            {
                Assert.Equal(typeof(int), TypeConverter.ResolveActualType(typeof(int?)));
            }
        }
    }
}