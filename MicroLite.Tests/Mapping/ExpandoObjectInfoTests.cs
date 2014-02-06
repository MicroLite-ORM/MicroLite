namespace MicroLite.Tests.Mapping
{
    using System;
    using System.Dynamic;
    using MicroLite.Mapping;
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
        public void SetPropertyValueForColumnSetsPropertyValue()
        {
            var objectInfo = new ExpandoObjectInfo();

            var instance = (dynamic)objectInfo.CreateInstance<ExpandoObject>();

            objectInfo.SetPropertyValueForColumn(instance, "Id", 12345);
            objectInfo.SetPropertyValueForColumn(instance, "Name", "Fred Flintstone");

            Assert.Equal(12345, instance.Id);
            Assert.Equal("Fred Flintstone", instance.Name);
        }

        [Fact]
        public void SetPropertyValueForColumnSetsPropertyValueToNullIfPassedDBNull()
        {
            var objectInfo = new ExpandoObjectInfo();

            var instance = (dynamic)objectInfo.CreateInstance<ExpandoObject>();

            objectInfo.SetPropertyValueForColumn(instance, "Name", DBNull.Value);

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