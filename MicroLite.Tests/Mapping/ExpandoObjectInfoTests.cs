namespace MicroLite.Tests.Mapping
{
    using System;
    using System.Data;
    using System.Dynamic;
    using MicroLite.Mapping;
    using Moq;
    using Xunit;

    public class ExpandoObjectInfoTests
    {
        [Fact]
        public void CreateInstance()
        {
            var objectInfo = new ExpandoObjectInfo();

            var instance = objectInfo.CreateInstance<ExpandoObject>();

            Assert.IsType<ExpandoObject>(instance);
        }

        [Fact]
        public void ForTypeReturnsExpandoObject()
        {
            var objectInfo = new ExpandoObjectInfo();

            Assert.Equal(typeof(ExpandoObject), objectInfo.ForType);
        }

        [Fact]
        public void GetColumnInfoThrowsNotSupportedException()
        {
            var objectInfo = new ExpandoObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(
                () => objectInfo.GetColumnInfo("Name"));

            Assert.Equal(exception.Message, Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        [Fact]
        public void GetIdentifierValueThrowsNotSupportedException()
        {
            var objectInfo = new ExpandoObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(
                () => objectInfo.GetIdentifierValue(new ExpandoObject()));

            Assert.Equal(exception.Message, Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        [Fact]
        public void GetInsertValuesThrowsNotSupportedException()
        {
            var objectInfo = new ExpandoObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(
                () => objectInfo.GetInsertValues(new ExpandoObject()));

            Assert.Equal(exception.Message, Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        [Fact]
        public void GetUpdateValuesThrowsNotSupportedException()
        {
            var objectInfo = new ExpandoObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(
                () => objectInfo.GetUpdateValues(new ExpandoObject()));

            Assert.Equal(exception.Message, Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        [Fact]
        public void HasDefaultIdentifierValueThrowsNotSupportedException()
        {
            var objectInfo = new ExpandoObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(
                () => objectInfo.HasDefaultIdentifierValue(new ExpandoObject()));

            Assert.Equal(exception.Message, Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        [Fact]
        public void SetIdentifierValueThrowsNotSupportedException()
        {
            var objectInfo = new ExpandoObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(
                () => objectInfo.SetIdentifierValue(new ExpandoObject(), 1));

            Assert.Equal(exception.Message, Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        [Fact]
        public void SetPropertyValuesSetsPropertyValues()
        {
            var objectInfo = new ExpandoObjectInfo();

            var instance = (dynamic)objectInfo.CreateInstance<ExpandoObject>();

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(2);
            mockReader.Setup(x => x.IsDBNull(It.IsAny<int>())).Returns(false);

            mockReader.Setup(x => x.GetName(0)).Returns("Id");
            mockReader.Setup(x => x.GetName(1)).Returns("Name");

            mockReader.Setup(x => x.GetValue(0)).Returns(12345);
            mockReader.Setup(x => x.GetValue(1)).Returns("Fred Flintstone");

            objectInfo.SetPropertyValues(instance, mockReader.Object);

            Assert.Equal(12345, instance.Id);
            Assert.Equal("Fred Flintstone", instance.Name);
        }

        [Fact]
        public void SetPropertyValuesSetsPropertyValueToNullIfPassedDBNull()
        {
            var objectInfo = new ExpandoObjectInfo();

            var instance = (dynamic)objectInfo.CreateInstance<ExpandoObject>();

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(1);
            mockReader.Setup(x => x.IsDBNull(It.IsAny<int>())).Returns(true);

            mockReader.Setup(x => x.GetName(0)).Returns("Name");

            objectInfo.SetPropertyValues(instance, mockReader.Object);

            Assert.Null(instance.Name);
        }

        [Fact]
        public void TableInfoThrowsNotSupportedException()
        {
            var objectInfo = new ExpandoObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(
                () => objectInfo.TableInfo);

            Assert.Equal(exception.Message, Messages.ExpandoObjectInfo_NotSupportedReason);
        }
    }
}