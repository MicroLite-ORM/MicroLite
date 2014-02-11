namespace MicroLite.Tests.Mapping
{
    using System;
    using System.Linq;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="AttributeMappingConvention" /> class.
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
            var objectInfo = mappingConvention.CreateObjectInfo(typeof(AssignedCustomer));

            Assert.False(objectInfo.TableInfo.Columns.Any(c => c.ColumnName == "UnMappedProperty"));
        }

        [Fact]
        public void TableInfoColumnsAreMappedCorrectlyForAssignedIdentifier()
        {
            var mappingConvention = new AttributeMappingConvention();
            var objectInfo = mappingConvention.CreateObjectInfo(typeof(AssignedCustomer));

            var columns = objectInfo.TableInfo.Columns.ToArray();

            Assert.Equal(6, columns.Length);

            Assert.Equal("Created", columns[0].ColumnName);
            Assert.True(columns[0].AllowInsert);
            Assert.False(columns[0].AllowUpdate);
            Assert.False(columns[0].IsIdentifier);
            Assert.Equal(typeof(AssignedCustomer).GetProperty("Created"), columns[0].PropertyInfo);

            Assert.Equal("DoB", columns[1].ColumnName);
            Assert.True(columns[1].AllowInsert);
            Assert.True(columns[1].AllowUpdate);
            Assert.False(columns[1].IsIdentifier);
            Assert.Equal(typeof(AssignedCustomer).GetProperty("DateOfBirth"), columns[1].PropertyInfo);

            Assert.Equal("CustomerId", columns[2].ColumnName);
            Assert.True(columns[2].AllowInsert);
            Assert.False(columns[2].AllowUpdate);
            Assert.True(columns[2].IsIdentifier);
            Assert.Equal(typeof(AssignedCustomer).GetProperty("Id"), columns[2].PropertyInfo);

            Assert.Equal("Name", columns[3].ColumnName);
            Assert.True(columns[3].AllowInsert);
            Assert.True(columns[3].AllowUpdate);
            Assert.False(columns[3].IsIdentifier);
            Assert.Equal(typeof(AssignedCustomer).GetProperty("Name"), columns[3].PropertyInfo);

            Assert.Equal("StatusId", columns[4].ColumnName);
            Assert.True(columns[4].AllowInsert);
            Assert.True(columns[4].AllowUpdate);
            Assert.False(columns[4].IsIdentifier);
            Assert.Equal(typeof(AssignedCustomer).GetProperty("Status"), columns[4].PropertyInfo);

            Assert.Equal("Updated", columns[5].ColumnName);
            Assert.False(columns[5].AllowInsert);
            Assert.True(columns[5].AllowUpdate);
            Assert.False(columns[5].IsIdentifier);
            Assert.Equal(typeof(AssignedCustomer).GetProperty("Updated"), columns[5].PropertyInfo);
        }

        [Fact]
        public void TableInfoColumnsAreMappedCorrectlyForDbGeneratedIdentifier()
        {
            var mappingConvention = new AttributeMappingConvention();
            var objectInfo = mappingConvention.CreateObjectInfo(typeof(DbGeneratedCustomer));

            var columns = objectInfo.TableInfo.Columns.ToArray();

            Assert.Equal(6, columns.Length);

            Assert.Equal("Created", columns[0].ColumnName);
            Assert.True(columns[0].AllowInsert);
            Assert.False(columns[0].AllowUpdate);
            Assert.False(columns[0].IsIdentifier);
            Assert.Equal(typeof(DbGeneratedCustomer).GetProperty("Created"), columns[0].PropertyInfo);

            Assert.Equal("DoB", columns[1].ColumnName);
            Assert.True(columns[1].AllowInsert);
            Assert.True(columns[1].AllowUpdate);
            Assert.False(columns[1].IsIdentifier);
            Assert.Equal(typeof(DbGeneratedCustomer).GetProperty("DateOfBirth"), columns[1].PropertyInfo);

            Assert.Equal("CustomerId", columns[2].ColumnName);
            Assert.False(columns[2].AllowInsert);
            Assert.False(columns[2].AllowUpdate);
            Assert.True(columns[2].IsIdentifier);
            Assert.Equal(typeof(DbGeneratedCustomer).GetProperty("Id"), columns[2].PropertyInfo);

            Assert.Equal("Name", columns[3].ColumnName);
            Assert.True(columns[3].AllowInsert);
            Assert.True(columns[3].AllowUpdate);
            Assert.False(columns[3].IsIdentifier);
            Assert.Equal(typeof(DbGeneratedCustomer).GetProperty("Name"), columns[3].PropertyInfo);

            Assert.Equal("StatusId", columns[4].ColumnName);
            Assert.True(columns[4].AllowInsert);
            Assert.True(columns[4].AllowUpdate);
            Assert.False(columns[4].IsIdentifier);
            Assert.Equal(typeof(DbGeneratedCustomer).GetProperty("Status"), columns[4].PropertyInfo);

            Assert.Equal("Updated", columns[5].ColumnName);
            Assert.False(columns[5].AllowInsert);
            Assert.True(columns[5].AllowUpdate);
            Assert.False(columns[5].IsIdentifier);
            Assert.Equal(typeof(DbGeneratedCustomer).GetProperty("Updated"), columns[5].PropertyInfo);
        }

        [Fact]
        public void TableInfoHasCorrectProperties()
        {
            var mappingConvention = new AttributeMappingConvention();
            var objectInfo = mappingConvention.CreateObjectInfo(typeof(AssignedCustomer));

            Assert.Equal("CustomerId", objectInfo.TableInfo.IdentifierColumn.ColumnName);
            Assert.Equal(IdentifierStrategy.Assigned, objectInfo.TableInfo.IdentifierStrategy);
            Assert.Equal("Customers", objectInfo.TableInfo.Name);
            Assert.Equal("Sales", objectInfo.TableInfo.Schema);
        }

        public class WhenCallingCreateObjectInfoAndTheTypeHasNoTableAttribute
        {
            [Fact]
            public void AMicroLiteExceptionIsThrown()
            {
                var mappingConvention = new AttributeMappingConvention();

                var exception = Assert.Throws<MappingException>(
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

        [Table("Sales", "Customers")]
        private class AssignedCustomer
        {
            public int AgeInYears
            {
                get
                {
                    return DateTime.Today.Year - this.DateOfBirth.Year;
                }
            }

            [Column("Created", allowInsert: true, allowUpdate: false)]
            public DateTime Created
            {
                get;
                set;
            }

            [Column("DoB")]
            public DateTime DateOfBirth
            {
                get;
                set;
            }

            [Column("CustomerId")]
            [Identifier(IdentifierStrategy.Assigned)]
            public int Id
            {
                get;
                set;
            }

            [Column("Name")]
            public string Name
            {
                get;
                set;
            }

            [Column("StatusId")]
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

            [Column("Updated", allowInsert: false, allowUpdate: true)]
            public DateTime? Updated
            {
                get;
                set;
            }
        }

        [Table("Sales", "Customers")]
        private class CustomerWithNoIdentifierAttribute
        {
        }

        private class CustomerWithNoTableAttribute
        {
        }

        [Table("Sales", "Customers")]
        private class DbGeneratedCustomer
        {
            public int AgeInYears
            {
                get
                {
                    return DateTime.Today.Year - this.DateOfBirth.Year;
                }
            }

            [Column("Created", allowInsert: true, allowUpdate: false)]
            public DateTime Created
            {
                get;
                set;
            }

            [Column("DoB")]
            public DateTime DateOfBirth
            {
                get;
                set;
            }

            [Column("CustomerId")]
            [Identifier(IdentifierStrategy.DbGenerated)]
            public int Id
            {
                get;
                set;
            }

            [Column("Name")]
            public string Name
            {
                get;
                set;
            }

            [Column("StatusId")]
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

            [Column("Updated", allowInsert: false, allowUpdate: true)]
            public DateTime? Updated
            {
                get;
                set;
            }
        }
    }
}