namespace MicroLite.Tests.Mapping.Attributes
{
    using System;
    using System.Data;
    using System.Linq;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;
    using MicroLite.Mapping.Attributes;
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

            Assert.Equal(7, columns.Length);

            Assert.Equal("Created", columns[0].ColumnName);
            Assert.True(columns[0].AllowInsert);
            Assert.False(columns[0].AllowUpdate);
            Assert.Equal(DbType.DateTime, columns[0].DbType);
            Assert.False(columns[0].IsIdentifier);
            Assert.Equal(typeof(AssignedCustomer).GetProperty("Created"), columns[0].PropertyInfo);
            Assert.Null(columns[0].SequenceName);
            Assert.False(columns[0].IsVersion);

            Assert.Equal("DoB", columns[1].ColumnName);
            Assert.True(columns[1].AllowInsert);
            Assert.True(columns[1].AllowUpdate);
            Assert.Equal(DbType.DateTime, columns[1].DbType);
            Assert.False(columns[1].IsIdentifier);
            Assert.Equal(typeof(AssignedCustomer).GetProperty("DateOfBirth"), columns[1].PropertyInfo);
            Assert.Null(columns[1].SequenceName);
            Assert.False(columns[1].IsVersion);

            Assert.Equal("CustomerId", columns[2].ColumnName);
            Assert.True(columns[2].AllowInsert);
            Assert.False(columns[2].AllowUpdate);
            Assert.Equal(DbType.Int32, columns[2].DbType);
            Assert.True(columns[2].IsIdentifier);
            Assert.Equal(typeof(AssignedCustomer).GetProperty("Id"), columns[2].PropertyInfo);
            Assert.Null(columns[2].SequenceName);
            Assert.False(columns[2].IsVersion);

            Assert.Equal("Name", columns[3].ColumnName);
            Assert.True(columns[3].AllowInsert);
            Assert.True(columns[3].AllowUpdate);
            Assert.Equal(DbType.String, columns[3].DbType);
            Assert.False(columns[3].IsIdentifier);
            Assert.Equal(typeof(AssignedCustomer).GetProperty("Name"), columns[3].PropertyInfo);
            Assert.Null(columns[3].SequenceName);
            Assert.False(columns[3].IsVersion);

            Assert.Equal("StatusId", columns[4].ColumnName);
            Assert.True(columns[4].AllowInsert);
            Assert.True(columns[4].AllowUpdate);
            Assert.Equal(DbType.Int32, columns[4].DbType);
            Assert.False(columns[4].IsIdentifier);
            Assert.Equal(typeof(AssignedCustomer).GetProperty("Status"), columns[4].PropertyInfo);
            Assert.Null(columns[4].SequenceName);
            Assert.False(columns[4].IsVersion);

            Assert.Equal("Updated", columns[5].ColumnName);
            Assert.False(columns[5].AllowInsert);
            Assert.True(columns[5].AllowUpdate);
            Assert.Equal(DbType.DateTime, columns[5].DbType);
            Assert.False(columns[5].IsIdentifier);
            Assert.Equal(typeof(AssignedCustomer).GetProperty("Updated"), columns[5].PropertyInfo);
            Assert.Null(columns[5].SequenceName);
            Assert.False(columns[5].IsVersion);

            Assert.Equal("Version", columns[6].ColumnName);
            Assert.True(columns[6].AllowInsert);
            Assert.True(columns[6].AllowUpdate);
            Assert.Equal(DbType.Int32, columns[6].DbType);
            Assert.False(columns[6].IsIdentifier);
            Assert.Equal(typeof(AssignedCustomer).GetProperty("Version"), columns[6].PropertyInfo);
            Assert.Null(columns[6].SequenceName);
            Assert.True(columns[6].IsVersion);
        }

        [Fact]
        public void TableInfoColumnsAreMappedCorrectlyForDbGeneratedIdentifier()
        {
            var mappingConvention = new AttributeMappingConvention();
            var objectInfo = mappingConvention.CreateObjectInfo(typeof(DbGeneratedCustomer));

            var columns = objectInfo.TableInfo.Columns.ToArray();

            Assert.Equal(7, columns.Length);

            Assert.Equal("Created", columns[0].ColumnName);
            Assert.True(columns[0].AllowInsert);
            Assert.False(columns[0].AllowUpdate);
            Assert.Equal(DbType.DateTime, columns[0].DbType);
            Assert.False(columns[0].IsIdentifier);
            Assert.Equal(typeof(DbGeneratedCustomer).GetProperty("Created"), columns[0].PropertyInfo);
            Assert.Null(columns[0].SequenceName);
            Assert.False(columns[0].IsVersion);

            Assert.Equal("DoB", columns[1].ColumnName);
            Assert.True(columns[1].AllowInsert);
            Assert.True(columns[1].AllowUpdate);
            Assert.Equal(DbType.DateTime, columns[1].DbType);
            Assert.False(columns[1].IsIdentifier);
            Assert.Equal(typeof(DbGeneratedCustomer).GetProperty("DateOfBirth"), columns[1].PropertyInfo);
            Assert.Null(columns[1].SequenceName);
            Assert.False(columns[1].IsVersion);

            Assert.Equal("CustomerId", columns[2].ColumnName);
            Assert.False(columns[2].AllowInsert);
            Assert.False(columns[2].AllowUpdate);
            Assert.Equal(DbType.Int32, columns[2].DbType);
            Assert.True(columns[2].IsIdentifier);
            Assert.Equal(typeof(DbGeneratedCustomer).GetProperty("Id"), columns[2].PropertyInfo);
            Assert.Null(columns[2].SequenceName);
            Assert.False(columns[2].IsVersion);

            Assert.Equal("Name", columns[3].ColumnName);
            Assert.True(columns[3].AllowInsert);
            Assert.True(columns[3].AllowUpdate);
            Assert.Equal(DbType.String, columns[3].DbType);
            Assert.False(columns[3].IsIdentifier);
            Assert.Equal(typeof(DbGeneratedCustomer).GetProperty("Name"), columns[3].PropertyInfo);
            Assert.Null(columns[3].SequenceName);
            Assert.False(columns[3].IsVersion);

            Assert.Equal("StatusId", columns[4].ColumnName);
            Assert.True(columns[4].AllowInsert);
            Assert.True(columns[4].AllowUpdate);
            Assert.Equal(DbType.Int32, columns[4].DbType);
            Assert.False(columns[4].IsIdentifier);
            Assert.Equal(typeof(DbGeneratedCustomer).GetProperty("Status"), columns[4].PropertyInfo);
            Assert.Null(columns[4].SequenceName);
            Assert.False(columns[4].IsVersion);

            Assert.Equal("Updated", columns[5].ColumnName);
            Assert.False(columns[5].AllowInsert);
            Assert.True(columns[5].AllowUpdate);
            Assert.Equal(DbType.DateTime, columns[5].DbType);
            Assert.False(columns[5].IsIdentifier);
            Assert.Equal(typeof(DbGeneratedCustomer).GetProperty("Updated"), columns[5].PropertyInfo);
            Assert.Null(columns[5].SequenceName);
            Assert.False(columns[5].IsVersion);

            Assert.Equal("Version", columns[6].ColumnName);
            Assert.True(columns[6].AllowInsert);
            Assert.True(columns[6].AllowUpdate);
            Assert.Equal(DbType.Int32, columns[6].DbType);
            Assert.False(columns[6].IsIdentifier);
            Assert.Equal(typeof(DbGeneratedCustomer).GetProperty("Version"), columns[6].PropertyInfo);
            Assert.Null(columns[6].SequenceName);
            Assert.True(columns[6].IsVersion);
        }

        [Fact]
        public void TableInfoColumnsAreMappedCorrectlyForDbGeneratedSequence()
        {
            var mappingConvention = new AttributeMappingConvention();
            var objectInfo = mappingConvention.CreateObjectInfo(typeof(SequenceCustomer));

            var columns = objectInfo.TableInfo.Columns.ToArray();

            Assert.Equal(7, columns.Length);

            Assert.Equal("Created", columns[0].ColumnName);
            Assert.True(columns[0].AllowInsert);
            Assert.False(columns[0].AllowUpdate);
            Assert.Equal(DbType.DateTime, columns[0].DbType);
            Assert.False(columns[0].IsIdentifier);
            Assert.Equal(typeof(SequenceCustomer).GetProperty("Created"), columns[0].PropertyInfo);
            Assert.Null(columns[0].SequenceName);
            Assert.False(columns[0].IsVersion);

            Assert.Equal("DoB", columns[1].ColumnName);
            Assert.True(columns[1].AllowInsert);
            Assert.True(columns[1].AllowUpdate);
            Assert.Equal(DbType.DateTime, columns[1].DbType);
            Assert.False(columns[1].IsIdentifier);
            Assert.Equal(typeof(SequenceCustomer).GetProperty("DateOfBirth"), columns[1].PropertyInfo);
            Assert.Null(columns[1].SequenceName);
            Assert.False(columns[1].IsVersion);

            Assert.Equal("CustomerId", columns[2].ColumnName);
            Assert.False(columns[2].AllowInsert);
            Assert.False(columns[2].AllowUpdate);
            Assert.Equal(DbType.Int32, columns[2].DbType);
            Assert.True(columns[2].IsIdentifier);
            Assert.Equal(typeof(SequenceCustomer).GetProperty("Id"), columns[2].PropertyInfo);
            Assert.Equal("CustomerIdSequence", columns[2].SequenceName);
            Assert.False(columns[2].IsVersion);

            Assert.Equal("Name", columns[3].ColumnName);
            Assert.True(columns[3].AllowInsert);
            Assert.True(columns[3].AllowUpdate);
            Assert.Equal(DbType.String, columns[3].DbType);
            Assert.False(columns[3].IsIdentifier);
            Assert.Equal(typeof(SequenceCustomer).GetProperty("Name"), columns[3].PropertyInfo);
            Assert.Null(columns[3].SequenceName);
            Assert.False(columns[3].IsVersion);

            Assert.Equal("StatusId", columns[4].ColumnName);
            Assert.True(columns[4].AllowInsert);
            Assert.True(columns[4].AllowUpdate);
            Assert.Equal(DbType.Int32, columns[4].DbType);
            Assert.False(columns[4].IsIdentifier);
            Assert.Equal(typeof(SequenceCustomer).GetProperty("Status"), columns[4].PropertyInfo);
            Assert.Null(columns[4].SequenceName);
            Assert.False(columns[4].IsVersion);

            Assert.Equal("Updated", columns[5].ColumnName);
            Assert.False(columns[5].AllowInsert);
            Assert.True(columns[5].AllowUpdate);
            Assert.Equal(DbType.DateTime, columns[5].DbType);
            Assert.False(columns[5].IsIdentifier);
            Assert.Equal(typeof(SequenceCustomer).GetProperty("Updated"), columns[5].PropertyInfo);
            Assert.Null(columns[5].SequenceName);
            Assert.False(columns[5].IsVersion);

            Assert.Equal("Version", columns[6].ColumnName);
            Assert.True(columns[6].AllowInsert);
            Assert.True(columns[6].AllowUpdate);
            Assert.Equal(DbType.Int32, columns[6].DbType);
            Assert.False(columns[6].IsIdentifier);
            Assert.Equal(typeof(SequenceCustomer).GetProperty("Version"), columns[6].PropertyInfo);
            Assert.Null(columns[6].SequenceName);
            Assert.True(columns[6].IsVersion);
        }

        [Fact]
        public void TableInfoHasCorrectPropertiesForAssigned()
        {
            var mappingConvention = new AttributeMappingConvention();
            var objectInfo = mappingConvention.CreateObjectInfo(typeof(AssignedCustomer));

            Assert.Equal("CustomerId", objectInfo.TableInfo.IdentifierColumn.ColumnName);
            Assert.Equal(IdentifierStrategy.Assigned, objectInfo.TableInfo.IdentifierStrategy);
            Assert.Equal("Customers", objectInfo.TableInfo.Name);
            Assert.Equal("Sales", objectInfo.TableInfo.Schema);
        }

        [Fact]
        public void TableInfoHasCorrectPropertiesForDbGenerated()
        {
            var mappingConvention = new AttributeMappingConvention();
            var objectInfo = mappingConvention.CreateObjectInfo(typeof(DbGeneratedCustomer));

            Assert.Equal("CustomerId", objectInfo.TableInfo.IdentifierColumn.ColumnName);
            Assert.Equal(IdentifierStrategy.DbGenerated, objectInfo.TableInfo.IdentifierStrategy);
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

                Assert.Equal(ExceptionMessages.AttributeMappingConvention_NoTableAttribute.FormatWith(typeof(CustomerWithNoTableAttribute).FullName), exception.Message);
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

            [Column("Version")]
            [Version]
            public int Version
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

            [Column("Version")]
            [Version]
            public int Version
            {
                get;
                set;
            }
        }

        [Table("Sales", "Customers")]
        private class SequenceCustomer
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
            [Identifier(IdentifierStrategy.Sequence, "CustomerIdSequence")]
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

            [Column("Version")]
            [Version]
            public int Version
            {
                get;
                set;
            }
        }
    }
}