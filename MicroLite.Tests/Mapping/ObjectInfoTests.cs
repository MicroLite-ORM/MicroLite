namespace MicroLite.Tests.Mapping
{
    using System;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ObjectInfo" /> class.
    /// </summary>
    public class ObjectInfoTests : UnitTest
    {
        [Fact]
        public void For_ReturnsExpandoObjectInfo_ForTypeOfDynamic()
        {
            var objectInfo = ObjectInfoHelper<dynamic>();

            Assert.IsType<ExpandoObjectInfo>(objectInfo);
        }

        [Fact]
        public void For_ReturnsPocoObjectInfo_ForPocoType()
        {
            var forType = typeof(Customer);

            var objectInfo1 = ObjectInfo.For(forType);

            Assert.IsType<PocoObjectInfo>(objectInfo1);
        }

        [Fact]
        public void For_ReturnsSameObjectInfo_ForSameType()
        {
            var forType = typeof(Customer);

            var objectInfo1 = ObjectInfo.For(forType);
            var objectInfo2 = ObjectInfo.For(forType);

            Assert.Same(objectInfo1, objectInfo2);
        }

        [Fact]
        public void For_ReturnsTupleObjectInfo_ForTypeOfTupleT1()
        {
            var objectInfo = ObjectInfo.For(typeof(Tuple<int>));

            Assert.IsType<TupleObjectInfo>(objectInfo);
        }

        [Fact]
        public void For_ReturnsTupleObjectInfo_ForTypeOfTupleT2()
        {
            var objectInfo = ObjectInfo.For(typeof(Tuple<int, string>));

            Assert.IsType<TupleObjectInfo>(objectInfo);
        }

        [Fact]
        public void For_ReturnsTupleObjectInfo_ForTypeOfTupleT3()
        {
            var objectInfo = ObjectInfo.For(typeof(Tuple<int, string, DateTime>));

            Assert.IsType<TupleObjectInfo>(objectInfo);
        }

        [Fact]
        public void For_ReturnsTupleObjectInfo_ForTypeOfTupleT4()
        {
            var objectInfo = ObjectInfo.For(typeof(Tuple<int, string, DateTime, bool>));

            Assert.IsType<TupleObjectInfo>(objectInfo);
        }

        [Fact]
        public void For_ReturnsTupleObjectInfo_ForTypeOfTupleT5()
        {
            var objectInfo = ObjectInfo.For(typeof(Tuple<int, string, DateTime, bool, decimal>));

            Assert.IsType<TupleObjectInfo>(objectInfo);
        }

        [Fact]
        public void For_ReturnsTupleObjectInfo_ForTypeOfTupleT6()
        {
            var objectInfo = ObjectInfo.For(typeof(Tuple<int, string, DateTime, bool, decimal, double>));

            Assert.IsType<TupleObjectInfo>(objectInfo);
        }

        [Fact]
        public void For_ReturnsTupleObjectInfo_ForTypeOfTupleT7()
        {
            var objectInfo = ObjectInfo.For(typeof(Tuple<int, string, DateTime, bool, decimal, double, Guid>));

            Assert.IsType<TupleObjectInfo>(objectInfo);
        }

        [Fact]
        public void For_ThrowsArgumentNullExceptonForNullForType()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => ObjectInfo.For(null));

            Assert.Equal("forType", exception.ParamName);
        }

        [Fact]
        public void For_ThrowsMappingException_IfAbstractClass()
        {
            var exception = Assert.Throws<MappingException>(
                () => ObjectInfo.For(typeof(AbstractCustomer)));

            Assert.Equal(
                ExceptionMessages.ObjectInfo_TypeMustNotBeAbstract.FormatWith(typeof(AbstractCustomer).Name),
                exception.Message);
        }

        [Fact]
        public void For_ThrowsMappingException_IfNoDefaultConstructor()
        {
            var exception = Assert.Throws<MappingException>(
                () => ObjectInfo.For(typeof(CustomerWithNoDefaultConstructor)));

            Assert.Equal(
                ExceptionMessages.ObjectInfo_TypeMustHaveDefaultConstructor.FormatWith(typeof(CustomerWithNoDefaultConstructor).Name),
                exception.Message);
        }

        [Fact]
        public void For_ThrowsMappingException_IfNonPublicClass()
        {
            var exception = Assert.Throws<MappingException>(
                () => ObjectInfo.For(typeof(InternalCustomer)));

            Assert.Equal(
                ExceptionMessages.ObjectInfo_TypeMustBePublic.FormatWith(typeof(InternalCustomer).Name),
                exception.Message);
        }

        [Fact]
        public void For_ThrowsMicroLiteException_IfNotClass()
        {
            var exception = Assert.Throws<MappingException>(
                () => ObjectInfo.For(typeof(CustomerStruct)));

            Assert.Equal(
                ExceptionMessages.ObjectInfo_TypeMustBeClass.FormatWith(typeof(CustomerStruct).Name),
                exception.Message);
        }

        /// <summary>
        /// A helper method required because you can't do typeof(dynamic).
        /// </summary>
        private static IObjectInfo ObjectInfoHelper<T>()
        {
            var objectInfo = ObjectInfo.For(typeof(T));

            return objectInfo;
        }

        public struct CustomerStruct
        {
        }

        public abstract class AbstractCustomer
        {
        }

        public class CustomerWithNoDefaultConstructor
        {
            public CustomerWithNoDefaultConstructor(string foo)
            {
            }
        }

        internal class InternalCustomer
        {
            public int Id
            {
                get;
                set;
            }
        }
    }
}