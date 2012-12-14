namespace MicroLite.Tests.Mapping
{
    using System;
    using System.Linq;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="AttributeMappingConvention"/> class.
    /// </summary>
    public class AttributeMappingConventionTests
    {
        private enum CustomerStatus
        {
            Inactive = 0,
            Active = 1
        }

        [Fact]
        public void PropertyWithoutColumnAttributeIsIgnored()
        {
            var mappingConvention = new AttributeMappingConvention();
            var objectInfo = mappingConvention.CreateObjectInfo(typeof(Customer));

            Assert.False(objectInfo.TableInfo.Columns.Any(c => c.ColumnName == "UnMappedProperty"));
        }

        [Fact]
        public void TableInfoColumnsAreMappedCorrectly()
        {
            var mappingConvention = new AttributeMappingConvention();
            var objectInfo = mappingConvention.CreateObjectInfo(typeof(Customer));

            var columns = objectInfo.TableInfo.Columns.ToArray();

            Assert.Equal(6, columns.Length);

            Assert.Equal("Created", columns[0].ColumnName);
            Assert.True(columns[0].AllowInsert);
            Assert.False(columns[0].AllowUpdate);
            Assert.False(columns[0].IsIdentifier);
            Assert.Equal(typeof(Customer).GetProperty("Created"), columns[0].PropertyInfo);

            Assert.Equal("DoB", columns[1].ColumnName);
            Assert.True(columns[1].AllowInsert);
            Assert.True(columns[1].AllowUpdate);
            Assert.False(columns[1].IsIdentifier);
            Assert.Equal(typeof(Customer).GetProperty("DateOfBirth"), columns[1].PropertyInfo);

            Assert.Equal("CustomerId", columns[2].ColumnName);
            Assert.True(columns[2].AllowInsert);
            Assert.True(columns[2].AllowUpdate);
            Assert.True(columns[2].IsIdentifier);
            Assert.Equal(typeof(Customer).GetProperty("Id"), columns[2].PropertyInfo);

            Assert.Equal("Name", columns[3].ColumnName);
            Assert.True(columns[3].AllowInsert);
            Assert.True(columns[3].AllowUpdate);
            Assert.False(columns[3].IsIdentifier);
            Assert.Equal(typeof(Customer).GetProperty("Name"), columns[3].PropertyInfo);

            Assert.Equal("StatusId", columns[4].ColumnName);
            Assert.True(columns[4].AllowInsert);
            Assert.True(columns[4].AllowUpdate);
            Assert.False(columns[4].IsIdentifier);
            Assert.Equal(typeof(Customer).GetProperty("Status"), columns[4].PropertyInfo);

            Assert.Equal("Updated", columns[5].ColumnName);
            Assert.False(columns[5].AllowInsert);
            Assert.True(columns[5].AllowUpdate);
            Assert.False(columns[5].IsIdentifier);
            Assert.Equal(typeof(Customer).GetProperty("Updated"), columns[5].PropertyInfo);
        }

        [Fact]
        public void TableInfoHasCorrectProperties()
        {
            var mappingConvention = new AttributeMappingConvention();
            var objectInfo = mappingConvention.CreateObjectInfo(typeof(Customer));

            Assert.Equal("CustomerId", objectInfo.TableInfo.IdentifierColumn);
            Assert.Equal(MicroLite.Mapping.IdentifierStrategy.Assigned, objectInfo.TableInfo.IdentifierStrategy);
            Assert.Equal("Customers", objectInfo.TableInfo.Name);
            Assert.Equal("Sales", objectInfo.TableInfo.Schema);
        }

        public class WhenCallingCreateObjectInfoAndTheTypeHasNoTableAttribute
        {
            [Fact]
            public void AMicroLiteExceptionIsThrown()
            {
                var mappingConvention = new AttributeMappingConvention();

                var exception = Assert.Throws<MicroLiteException>(
                    () => mappingConvention.CreateObjectInfo(typeof(CustomerWithNoTableAttribute)));

                Assert.Equal(Messages.AttributeMappingConvention_NoTableAttribute.FormatWith(typeof(CustomerWithNoTableAttribute).FullName), exception.Message);
            }
        }

        public class WhenCallingCreateObjectInfoAndTypeIsNull
        {
            [Fact]
            public void CreateObjectInfoThrowsArgumentNullExceptionForNullType()
            {
                var mappingConvention = new AttributeMappingConvention();

                var exception = Assert.Throws<ArgumentNullException>(
                    () => mappingConvention.CreateObjectInfo(null));

                Assert.Equal("forType", exception.ParamName);
            }
        }

        [MicroLite.Mapping.Table("Sales", "Customers")]
        private class Customer
        {
            public Customer()
            {
            }

            public int AgeInYears
            {
                get
                {
                    return DateTime.Today.Year - this.DateOfBirth.Year;
                }
            }

            [MicroLite.Mapping.Column("Created", allowInsert: true, allowUpdate: false)]
            public DateTime Created
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("DoB")]
            public DateTime DateOfBirth
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("CustomerId")]
            [MicroLite.Mapping.Identifier(MicroLite.Mapping.IdentifierStrategy.Assigned)] // Don't use default or we can't prove we read it correctly.
            public int Id
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("Name")]
            public string Name
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("StatusId")]
            public CustomerStatus Status
            {
                get;
                set;
            }

            public string UnMappedProperty
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("Updated", allowInsert: false, allowUpdate: true)]
            public DateTime? Updated
            {
                get;
                set;
            }
        }

        [MicroLite.Mapping.Table("Sales", "Customers")]
        private class CustomerWithNoIdentifierAttribute
        {
        }

        private class CustomerWithNoTableAttribute
        {
        }
    }
}