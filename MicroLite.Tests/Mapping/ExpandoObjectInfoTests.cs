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

            var instance = objectInfo.CreateInstance();

            Assert.IsType<ExpandoObject>(instance);
        }

        [Fact]
        public void DefaultIdentifierValueThrowsNotSupportedException()
        {
            var objectInfo = new ExpandoObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(() => objectInfo.DefaultIdentifierValue);

            Assert.Equal(exception.Message, Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        [Fact]
        public void ForTypeReturnsExpandoObject()
        {
            var objectInfo = new ExpandoObjectInfo();

            Assert.Equal(typeof(ExpandoObject), objectInfo.ForType);
        }

        [Fact]
        public void GetIdentifierValueThrowsNotSupportedException()
        {
            var objectInfo = new ExpandoObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(() => objectInfo.GetIdentifierValue(new ExpandoObject()));

            Assert.Equal(exception.Message, Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        [Fact]
        public void GetPropertyValueForColumnThrowsNotSupportedException()
        {
            var objectInfo = new ExpandoObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(() => objectInfo.GetPropertyValueForColumn(new ExpandoObject(), "foo"));

            Assert.Equal(exception.Message, Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        [Fact]
        public void GetPropertyValueThrowsNotSupportedException()
        {
            var objectInfo = new ExpandoObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(() => objectInfo.GetPropertyValue(new ExpandoObject(), "foo"));

            Assert.Equal(exception.Message, Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        [Fact]
        public void HasDefaultIdentifierValueThrowsNotSupportedException()
        {
            var objectInfo = new ExpandoObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(() => objectInfo.HasDefaultIdentifierValue(new ExpandoObject()));

            Assert.Equal(exception.Message, Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        [Fact]
        public void SetPropertyValueForColumnSetsPropertyValue()
        {
            var objectInfo = new ExpandoObjectInfo();

            var instance = (dynamic)objectInfo.CreateInstance();

            objectInfo.SetPropertyValueForColumn(instance, "Id", 12345);
            objectInfo.SetPropertyValueForColumn(instance, "Name", "Fred Flintstone");

            Assert.Equal(12345, instance.Id);
            Assert.Equal("Fred Flintstone", instance.Name);
        }

        [Fact]
        public void SetPropertyValueForColumnSetsPropertyValueToNullIfPassedDBNull()
        {
            var objectInfo = new ExpandoObjectInfo();

            var instance = (dynamic)objectInfo.CreateInstance();

            objectInfo.SetPropertyValueForColumn(instance, "Name", DBNull.Value);

            Assert.Null(instance.Name);
        }

        [Fact]
        public void SetPropertyValueThrowsNotSupportedException()
        {
            var objectInfo = new ExpandoObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(() => objectInfo.SetPropertyValue(new ExpandoObject(), "Name", "foo"));

            Assert.Equal(exception.Message, Messages.ExpandoObjectInfo_NotSupportedReason);
        }

        [Fact]
        public void TableInfoThrowsNotSupportedException()
        {
            var objectInfo = new ExpandoObjectInfo();

            var exception = Assert.Throws<NotSupportedException>(() => objectInfo.TableInfo);

            Assert.Equal(exception.Message, Messages.ExpandoObjectInfo_NotSupportedReason);
        }
    }
}