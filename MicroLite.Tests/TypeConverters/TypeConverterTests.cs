namespace MicroLite.Tests.TypeConverters
{
    using System;
    using System.Data;
    using System.Xml.Linq;
    using MicroLite.Tests.TestEntities;
    using MicroLite.TypeConverters;
    using Xunit;

    public class TypeConverterTests
    {
        [Fact]
        public void ResolveDbType()
        {
            Assert.Equal(DbType.Byte, TypeConverter.ResolveDbType(typeof(byte)));
            Assert.Equal(DbType.Byte, TypeConverter.ResolveDbType(typeof(byte?)));
            Assert.Equal(DbType.SByte, TypeConverter.ResolveDbType(typeof(sbyte)));
            Assert.Equal(DbType.SByte, TypeConverter.ResolveDbType(typeof(sbyte?)));
            Assert.Equal(DbType.Int16, TypeConverter.ResolveDbType(typeof(short)));
            Assert.Equal(DbType.Int16, TypeConverter.ResolveDbType(typeof(short?)));
            Assert.Equal(DbType.UInt16, TypeConverter.ResolveDbType(typeof(ushort)));
            Assert.Equal(DbType.UInt16, TypeConverter.ResolveDbType(typeof(ushort?)));
            Assert.Equal(DbType.Int32, TypeConverter.ResolveDbType(typeof(int)));
            Assert.Equal(DbType.Int32, TypeConverter.ResolveDbType(typeof(int?)));
            Assert.Equal(DbType.UInt32, TypeConverter.ResolveDbType(typeof(uint)));
            Assert.Equal(DbType.UInt32, TypeConverter.ResolveDbType(typeof(uint?)));
            Assert.Equal(DbType.Int64, TypeConverter.ResolveDbType(typeof(long)));
            Assert.Equal(DbType.Int64, TypeConverter.ResolveDbType(typeof(long?)));
            Assert.Equal(DbType.UInt64, TypeConverter.ResolveDbType(typeof(ulong)));
            Assert.Equal(DbType.UInt64, TypeConverter.ResolveDbType(typeof(ulong?)));
            Assert.Equal(DbType.Single, TypeConverter.ResolveDbType(typeof(float)));
            Assert.Equal(DbType.Single, TypeConverter.ResolveDbType(typeof(float?)));
            Assert.Equal(DbType.Decimal, TypeConverter.ResolveDbType(typeof(decimal)));
            Assert.Equal(DbType.Decimal, TypeConverter.ResolveDbType(typeof(decimal?)));
            Assert.Equal(DbType.Double, TypeConverter.ResolveDbType(typeof(double)));
            Assert.Equal(DbType.Double, TypeConverter.ResolveDbType(typeof(double?)));
            Assert.Equal(DbType.Boolean, TypeConverter.ResolveDbType(typeof(bool)));
            Assert.Equal(DbType.Boolean, TypeConverter.ResolveDbType(typeof(bool?)));
            Assert.Equal(DbType.StringFixedLength, TypeConverter.ResolveDbType(typeof(char)));
            Assert.Equal(DbType.StringFixedLength, TypeConverter.ResolveDbType(typeof(char?)));
            Assert.Equal(DbType.String, TypeConverter.ResolveDbType(typeof(string)));
            Assert.Equal(DbType.Binary, TypeConverter.ResolveDbType(typeof(byte[])));
            Assert.Equal(DbType.DateTime, TypeConverter.ResolveDbType(typeof(DateTime)));
            Assert.Equal(DbType.DateTime, TypeConverter.ResolveDbType(typeof(DateTime?)));
            Assert.Equal(DbType.DateTimeOffset, TypeConverter.ResolveDbType(typeof(DateTimeOffset)));
            Assert.Equal(DbType.DateTimeOffset, TypeConverter.ResolveDbType(typeof(DateTimeOffset?)));
            Assert.Equal(DbType.Guid, TypeConverter.ResolveDbType(typeof(Guid)));
            Assert.Equal(DbType.Guid, TypeConverter.ResolveDbType(typeof(Guid?)));
            Assert.Equal(DbType.Time, TypeConverter.ResolveDbType(typeof(TimeSpan)));
            Assert.Equal(DbType.Time, TypeConverter.ResolveDbType(typeof(TimeSpan?)));

            Assert.Equal(DbType.Int32, TypeConverter.ResolveDbType(typeof(CustomerStatus)));
            Assert.Equal(DbType.String, TypeConverter.ResolveDbType(typeof(DbEncryptedString)));
            Assert.Equal(DbType.String, TypeConverter.ResolveDbType(typeof(Uri)));
            Assert.Equal(DbType.String, TypeConverter.ResolveDbType(typeof(XDocument)));
        }

        [Fact]
        public void ResolveDbTypeThrowsArgumentNullExceptionForNullType()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => TypeConverter.ResolveDbType(null));

            Assert.Equal("type", exception.ParamName);
        }

        [Fact]
        public void ResolveDbTypeThrowsNotSupportedExceptionIfTypeNotMappedToDbType()
        {
            var exception = Assert.Throws<NotSupportedException>(
                () => TypeConverter.ResolveDbType(typeof(Customer)));

            Assert.Equal(typeof(Customer).FullName, exception.Message);
        }

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