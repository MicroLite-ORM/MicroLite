namespace MicroLite.Tests.Mapping
{
#if !NET_3_5

    using System;
    using System.Data;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Moq;
    using Xunit;

    public class TupleObjectInfoTests : UnitTest
    {
        [Fact]
        public void CreateInstanceSetsPropertyValueToNullIfPassedDBNull()
        {
            var objectInfo = new TupleObjectInfo();

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(1);
            mockReader.Setup(x => x.IsDBNull(It.IsAny<int>())).Returns(true);

            mockReader.Setup(x => x.GetFieldType(0)).Returns(typeof(string));

            var instance = (Tuple<string>)objectInfo.CreateInstance(mockReader.Object);

            Assert.NotNull(instance);
            Assert.Null(instance.Item1);
        }

        [Fact]
        public void CreateInstanceT1()
        {
            var objectInfo = new TupleObjectInfo();

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(1);
            mockReader.Setup(x => x.IsDBNull(It.IsAny<int>())).Returns(false);

            mockReader.Setup(x => x.GetFieldType(0)).Returns(typeof(int));

            mockReader.Setup(x => x.GetValue(0)).Returns(12345);

            var instance = (Tuple<int>)objectInfo.CreateInstance(mockReader.Object);

            Assert.NotNull(instance);
            Assert.IsType<Tuple<int>>(instance);
            Assert.Equal(12345, instance.Item1);
        }

        [Fact]
        public void CreateInstanceT2()
        {
            var objectInfo = new TupleObjectInfo();

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(2);
            mockReader.Setup(x => x.IsDBNull(It.IsAny<int>())).Returns(false);

            mockReader.Setup(x => x.GetFieldType(0)).Returns(typeof(int));
            mockReader.Setup(x => x.GetFieldType(1)).Returns(typeof(string));

            mockReader.Setup(x => x.GetValue(0)).Returns(12345);
            mockReader.Setup(x => x.GetValue(1)).Returns("Fred Flintstone");

            var instance = (Tuple<int, string>)objectInfo.CreateInstance(mockReader.Object);

            Assert.NotNull(instance);
            Assert.IsType<Tuple<int, string>>(instance);
            Assert.Equal(12345, instance.Item1);
            Assert.Equal("Fred Flintstone", instance.Item2);
        }

        [Fact]
        public void CreateInstanceT3()
        {
            var objectInfo = new TupleObjectInfo();

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(3);
            mockReader.Setup(x => x.IsDBNull(It.IsAny<int>())).Returns(false);

            mockReader.Setup(x => x.GetFieldType(0)).Returns(typeof(int));
            mockReader.Setup(x => x.GetFieldType(1)).Returns(typeof(string));
            mockReader.Setup(x => x.GetFieldType(2)).Returns(typeof(decimal));

            mockReader.Setup(x => x.GetValue(0)).Returns(12345);
            mockReader.Setup(x => x.GetValue(1)).Returns("Fred Flintstone");
            mockReader.Setup(x => x.GetValue(2)).Returns(238.335M);

            var instance = (Tuple<int, string, decimal>)objectInfo.CreateInstance(mockReader.Object);

            Assert.NotNull(instance);
            Assert.IsType<Tuple<int, string, decimal>>(instance);
            Assert.Equal(12345, instance.Item1);
            Assert.Equal("Fred Flintstone", instance.Item2);
            Assert.Equal(238.335M, instance.Item3);
        }

        [Fact]
        public void CreateInstanceT4()
        {
            var objectInfo = new TupleObjectInfo();

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(4);
            mockReader.Setup(x => x.IsDBNull(It.IsAny<int>())).Returns(false);

            mockReader.Setup(x => x.GetFieldType(0)).Returns(typeof(int));
            mockReader.Setup(x => x.GetFieldType(1)).Returns(typeof(string));
            mockReader.Setup(x => x.GetFieldType(2)).Returns(typeof(decimal));
            mockReader.Setup(x => x.GetFieldType(3)).Returns(typeof(DateTime));

            mockReader.Setup(x => x.GetValue(0)).Returns(12345);
            mockReader.Setup(x => x.GetValue(1)).Returns("Fred Flintstone");
            mockReader.Setup(x => x.GetValue(2)).Returns(238.335M);
            mockReader.Setup(x => x.GetValue(3)).Returns(DateTime.Today);

            var instance = (Tuple<int, string, decimal, DateTime>)objectInfo.CreateInstance(mockReader.Object);

            Assert.NotNull(instance);
            Assert.IsType<Tuple<int, string, decimal, DateTime>>(instance);
            Assert.Equal(12345, instance.Item1);
            Assert.Equal("Fred Flintstone", instance.Item2);
            Assert.Equal(238.335M, instance.Item3);
            Assert.Equal(DateTime.Today, instance.Item4);
        }

        [Fact]
        public void CreateInstanceT5()
        {
            var objectInfo = new TupleObjectInfo();

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(5);
            mockReader.Setup(x => x.IsDBNull(It.IsAny<int>())).Returns(false);

            mockReader.Setup(x => x.GetFieldType(0)).Returns(typeof(int));
            mockReader.Setup(x => x.GetFieldType(1)).Returns(typeof(string));
            mockReader.Setup(x => x.GetFieldType(2)).Returns(typeof(decimal));
            mockReader.Setup(x => x.GetFieldType(3)).Returns(typeof(DateTime));
            mockReader.Setup(x => x.GetFieldType(4)).Returns(typeof(bool));

            mockReader.Setup(x => x.GetValue(0)).Returns(12345);
            mockReader.Setup(x => x.GetValue(1)).Returns("Fred Flintstone");
            mockReader.Setup(x => x.GetValue(2)).Returns(238.335M);
            mockReader.Setup(x => x.GetValue(3)).Returns(DateTime.Today);
            mockReader.Setup(x => x.GetValue(4)).Returns(true);

            var instance = (Tuple<int, string, decimal, DateTime, bool>)objectInfo.CreateInstance(mockReader.Object);

            Assert.NotNull(instance);
            Assert.IsType<Tuple<int, string, decimal, DateTime, bool>>(instance);
            Assert.Equal(12345, instance.Item1);
            Assert.Equal("Fred Flintstone", instance.Item2);
            Assert.Equal(238.335M, instance.Item3);
            Assert.Equal(DateTime.Today, instance.Item4);
            Assert.Equal(true, instance.Item5);
        }

        [Fact]
        public void CreateInstanceT6()
        {
            var objectInfo = new TupleObjectInfo();

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(6);
            mockReader.Setup(x => x.IsDBNull(It.IsAny<int>())).Returns(false);

            mockReader.Setup(x => x.GetFieldType(0)).Returns(typeof(int));
            mockReader.Setup(x => x.GetFieldType(1)).Returns(typeof(string));
            mockReader.Setup(x => x.GetFieldType(2)).Returns(typeof(decimal));
            mockReader.Setup(x => x.GetFieldType(3)).Returns(typeof(DateTime));
            mockReader.Setup(x => x.GetFieldType(4)).Returns(typeof(bool));
            mockReader.Setup(x => x.GetFieldType(5)).Returns(typeof(Guid));

            mockReader.Setup(x => x.GetValue(0)).Returns(12345);
            mockReader.Setup(x => x.GetValue(1)).Returns("Fred Flintstone");
            mockReader.Setup(x => x.GetValue(2)).Returns(238.335M);
            mockReader.Setup(x => x.GetValue(3)).Returns(DateTime.Today);
            mockReader.Setup(x => x.GetValue(4)).Returns(true);
            mockReader.Setup(x => x.GetValue(5)).Returns(new Guid("E7B529F9-3EAC-45C6-91F3-F05006D94BDD"));

            var instance = (Tuple<int, string, decimal, DateTime, bool, Guid>)objectInfo.CreateInstance(mockReader.Object);

            Assert.NotNull(instance);
            Assert.IsType<Tuple<int, string, decimal, DateTime, bool, Guid>>(instance);
            Assert.Equal(12345, instance.Item1);
            Assert.Equal("Fred Flintstone", instance.Item2);
            Assert.Equal(238.335M, instance.Item3);
            Assert.Equal(DateTime.Today, instance.Item4);
            Assert.Equal(true, instance.Item5);
            Assert.Equal(new Guid("E7B529F9-3EAC-45C6-91F3-F05006D94BDD"), instance.Item6);
        }

        [Fact]
        public void CreateInstanceT7()
        {
            var objectInfo = new TupleObjectInfo();

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(7);
            mockReader.Setup(x => x.IsDBNull(It.IsAny<int>())).Returns(false);

            mockReader.Setup(x => x.GetFieldType(0)).Returns(typeof(int));
            mockReader.Setup(x => x.GetFieldType(1)).Returns(typeof(string));
            mockReader.Setup(x => x.GetFieldType(2)).Returns(typeof(decimal));
            mockReader.Setup(x => x.GetFieldType(3)).Returns(typeof(DateTime));
            mockReader.Setup(x => x.GetFieldType(4)).Returns(typeof(bool));
            mockReader.Setup(x => x.GetFieldType(5)).Returns(typeof(Guid));
            mockReader.Setup(x => x.GetFieldType(6)).Returns(typeof(double));

            mockReader.Setup(x => x.GetValue(0)).Returns(12345);
            mockReader.Setup(x => x.GetValue(1)).Returns("Fred Flintstone");
            mockReader.Setup(x => x.GetValue(2)).Returns(238.335M);
            mockReader.Setup(x => x.GetValue(3)).Returns(DateTime.Today);
            mockReader.Setup(x => x.GetValue(4)).Returns(true);
            mockReader.Setup(x => x.GetValue(5)).Returns(new Guid("E7B529F9-3EAC-45C6-91F3-F05006D94BDD"));
            mockReader.Setup(x => x.GetValue(6)).Returns(986562.12455D);

            var instance = (Tuple<int, string, decimal, DateTime, bool, Guid, double>)objectInfo.CreateInstance(mockReader.Object);

            Assert.NotNull(instance);
            Assert.IsType<Tuple<int, string, decimal, DateTime, bool, Guid, double>>(instance);
            Assert.Equal(12345, instance.Item1);
            Assert.Equal("Fred Flintstone", instance.Item2);
            Assert.Equal(238.335M, instance.Item3);
            Assert.Equal(DateTime.Today, instance.Item4);
            Assert.Equal(true, instance.Item5);
            Assert.Equal(new Guid("E7B529F9-3EAC-45C6-91F3-F05006D94BDD"), instance.Item6);
            Assert.Equal(986562.12455D, instance.Item7);
        }

        [Fact]
        public void CreateInstanceThrowsArgumentNullExceptionForNullReader()
        {
            var objectInfo = new TupleObjectInfo();

            var exception = Assert.Throws<ArgumentNullException>(
                () => objectInfo.CreateInstance(null));

            Assert.Equal("reader", exception.ParamName);
        }

        [Fact]
        public void CreateInstanceTNThrowsNotSupportedException()
        {
            var objectInfo = new TupleObjectInfo();

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(8);
            mockReader.Setup(x => x.IsDBNull(It.IsAny<int>())).Returns(false);

            mockReader.Setup(x => x.GetFieldType(It.IsAny<int>())).Returns(typeof(int));

            var exception = Assert.Throws<NotSupportedException>(
                () => objectInfo.CreateInstance(mockReader.Object));

            Assert.Equal(ExceptionMessages.TupleObjectInfo_TupleNotSupported, exception.Message);
        }

        [Fact]
        public void ForTypeReturnsTuple()
        {
            var objectInfo = new TupleObjectInfo();

            Assert.Equal(typeof(Tuple), objectInfo.ForType);
        }

        [Fact]
        public void GetColumnInfoThrowsNotSupportedException()
        {
            var objectInfo = new TupleObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(
                () => objectInfo.GetColumnInfo("Name"));

            Assert.Equal(exception.Message, ExceptionMessages.TupleObjectInfo_NotSupportedReason);
        }

        [Fact]
        public void GetIdentifierValueThrowsNotSupportedException()
        {
            var objectInfo = new TupleObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(
                () => objectInfo.GetIdentifierValue(Tuple.Create(1)));

            Assert.Equal(exception.Message, ExceptionMessages.TupleObjectInfo_NotSupportedReason);
        }

        [Fact]
        public void GetInsertValuesThrowsNotSupportedException()
        {
            var objectInfo = new TupleObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(
                () => objectInfo.GetInsertValues(Tuple.Create(1)));

            Assert.Equal(exception.Message, ExceptionMessages.TupleObjectInfo_NotSupportedReason);
        }

        [Fact]
        public void GetUpdateValuesThrowsNotSupportedException()
        {
            var objectInfo = new TupleObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(
                () => objectInfo.GetUpdateValues(Tuple.Create(1)));

            Assert.Equal(exception.Message, ExceptionMessages.TupleObjectInfo_NotSupportedReason);
        }

        [Fact]
        public void HasDefaultIdentifierValueThrowsNotSupportedException()
        {
            var objectInfo = new TupleObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(
                () => objectInfo.HasDefaultIdentifierValue(Tuple.Create(1)));

            Assert.Equal(exception.Message, ExceptionMessages.TupleObjectInfo_NotSupportedReason);
        }

        [Fact]
        public void IsDefaultIdentifierThrowsNotSupportedException()
        {
            var objectInfo = new TupleObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(
                () => objectInfo.IsDefaultIdentifier(0));

            Assert.Equal(exception.Message, ExceptionMessages.TupleObjectInfo_NotSupportedReason);
        }

        [Fact]
        public void SetIdentifierValueThrowsNotSupportedException()
        {
            var objectInfo = new TupleObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(
                () => objectInfo.SetIdentifierValue(Tuple.Create(1), 1));

            Assert.Equal(exception.Message, ExceptionMessages.TupleObjectInfo_NotSupportedReason);
        }

        [Fact]
        public void TableInfoThrowsNotSupportedException()
        {
            var objectInfo = new TupleObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(
                () => objectInfo.TableInfo);

            Assert.Equal(exception.Message, ExceptionMessages.TupleObjectInfo_NotSupportedReason);
        }

        [Fact]
        public void VerifyInstanceStateThrowsNotSupportedException()
        {
            var objectInfo = new TupleObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(
                () => objectInfo.VerifyInstanceForInsert(Tuple.Create(1)));

            Assert.Equal(exception.Message, ExceptionMessages.TupleObjectInfo_NotSupportedReason);
        }
    }

#endif
}