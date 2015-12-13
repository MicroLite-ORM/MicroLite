namespace MicroLite.Tests.Mapping
{
    using System;
    using System.Data;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ColumnInfo" /> class.
    /// </summary>
    public class ColumnInfoTests
    {
        [Fact]
        public void ConstructorSetsPropertyValues()
        {
            var columnName = "Name";
            var propertyInfo = typeof(Customer).GetProperty("Name");
            var isIdentifier = true;
            var isVersion = false;
            var allowInsert = true;
            var allowUpdate = true;
            var sequenceName = "CustomerIdSequence";
            var dbType = DbType.String;

            var columnInfo = new ColumnInfo(columnName, dbType, propertyInfo, isIdentifier, allowInsert, allowUpdate, sequenceName, isVersion);

            Assert.Equal(columnName, columnInfo.ColumnName);
            Assert.Equal(propertyInfo, columnInfo.PropertyInfo);
            Assert.Equal(isIdentifier, columnInfo.IsIdentifier);
            Assert.Equal(isVersion, columnInfo.IsVersion);
            Assert.Equal(allowInsert, columnInfo.AllowInsert);
            Assert.Equal(allowUpdate, columnInfo.AllowUpdate);
            Assert.Equal(sequenceName, columnInfo.SequenceName);
            Assert.Equal(dbType, columnInfo.DbType);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionForNullColumnName()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new ColumnInfo(null, DbType.String, typeof(Customer).GetProperty("Name"), false, true, true, "sequence", false));

            Assert.Equal("columnName", exception.ParamName);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionForNullPropertyInfo()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new ColumnInfo("Name", DbType.String, null, false, true, true, "sequence", false));

            Assert.Equal("propertyInfo", exception.ParamName);
        }
    }
}