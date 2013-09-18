using System;
using System.Xml.Linq;
using MicroLite.TypeConverters;
using Xunit;

namespace MicroLite.Tests.TypeConverters
{
    public class TypeConverterTests
    {
        private enum Status
        {
            Default = 0,
            New = 1
        }

        public class WhenCallingForWithATypeOfEnum
        {
            [Fact]
            public void TheEnumTypeConverterIsReturned()
            {
                var typeConverter = TypeConverter.For(typeof(Status));
                Assert.IsType<EnumTypeConverter>(typeConverter);
            }
        }

        public class WhenCallingForWithATypeOfInt
        {
            [Fact]
            public void TheObjectTypeConverterIsReturned()
            {
                var typeConverter = TypeConverter.For(typeof(int));
                Assert.IsType<ObjectTypeConverter>(typeConverter);
            }
        }

        public class WhenCallingForWithATypeOfXDocument
        {
            [Fact]
            public void TheXDocumentTypeConverterIsReturned()
            {
                var typeConverter = TypeConverter.For(typeof(XDocument));
                Assert.IsType<XDocumentTypeConverter>(typeConverter);
            }
        }

        public class WhenCallingResolveActualTypeWithNullType
        {
            [Fact]
            public void AnArgumentNullExceptionIsThrown()
            {
                Assert.Throws<ArgumentNullException>(() => TypeConverter.ResolveActualType(null));
            }
        }
    }
}