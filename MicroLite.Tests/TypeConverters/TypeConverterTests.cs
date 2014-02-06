namespace MicroLite.Tests.TypeConverters
{
    using System;
    using System.Xml.Linq;
    using MicroLite.TypeConverters;
    using Xunit;

    public class TypeConverterTests
    {
        private enum Status
        {
            Default = 0,
            New = 1
        }

        public class WhenCallingFor_WithATypeOfEnum
        {
            [Fact]
            public void TheEnumTypeConverterIsReturned()
            {
                var typeConverter = TypeConverter.For(typeof(Status));
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
    }
}