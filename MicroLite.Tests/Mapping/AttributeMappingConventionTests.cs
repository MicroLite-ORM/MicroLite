namespace MicroLite.Tests.Mapping
{
    using System;
    using System.Linq;
    using MicroLite.Mapping;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="AttributeMappingConvention"/> class.
    /// </summary>
    [TestFixture]
    public class AttributeMappingConventionTests
    {
        private enum CustomerStatus
        {
            Inactive = 0,
            Active = 1
        }

        [Test]
        public void CreateObjectInfoThrowsMicroLiteExceptionIfNoTableAttribute()
        {
            var mappingConvention = new AttributeMappingConvention();

            var exception = Assert.Throws<MicroLiteException>(
                () => mappingConvention.CreateObjectInfo(typeof(CustomerWithNoTableAttribute)));

            Assert.AreEqual(string.Format(Messages.StrictAttributeMappingConvention_NoTableAttribute, typeof(CustomerWithNoTableAttribute).FullName), exception.Message);
        }

        [Test]
        public void TableInfoColumnsAreMappedCorrectly()
        {
            var mappingConvention = new AttributeMappingConvention();
            var objectInfo = mappingConvention.CreateObjectInfo(typeof(Customer));

            var columns = objectInfo.TableInfo.Columns.ToArray();

            Assert.AreEqual(5, columns.Length);

            Assert.AreEqual("Created", columns[0].ColumnName);
            Assert.IsFalse(columns[0].AllowUpdate);
            Assert.IsFalse(columns[0].IsIdentifier);
            Assert.AreEqual(typeof(Customer).GetProperty("Created"), columns[0].PropertyInfo);

            Assert.AreEqual("DoB", columns[1].ColumnName);
            Assert.IsTrue(columns[1].AllowUpdate);
            Assert.IsFalse(columns[1].IsIdentifier);
            Assert.AreEqual(typeof(Customer).GetProperty("DateOfBirth"), columns[1].PropertyInfo);

            Assert.AreEqual("CustomerId", columns[2].ColumnName);
            Assert.IsTrue(columns[2].AllowUpdate);
            Assert.IsTrue(columns[2].IsIdentifier);
            Assert.AreEqual(typeof(Customer).GetProperty("Id"), columns[2].PropertyInfo);

            Assert.AreEqual("Name", columns[3].ColumnName);
            Assert.IsTrue(columns[3].AllowUpdate);
            Assert.IsFalse(columns[3].IsIdentifier);
            Assert.AreEqual(typeof(Customer).GetProperty("Name"), columns[3].PropertyInfo);

            Assert.AreEqual("StatusId", columns[4].ColumnName);
            Assert.IsTrue(columns[4].AllowUpdate);
            Assert.IsFalse(columns[4].IsIdentifier);
            Assert.AreEqual(typeof(Customer).GetProperty("Status"), columns[4].PropertyInfo);
        }

        [Test]
        public void TableInfoHasCorrectProperties()
        {
            var mappingConvention = new AttributeMappingConvention();
            var objectInfo = mappingConvention.CreateObjectInfo(typeof(Customer));

            Assert.AreEqual("CustomerId", objectInfo.TableInfo.IdentifierColumn);
            Assert.AreEqual(MicroLite.Mapping.IdentifierStrategy.Assigned, objectInfo.TableInfo.IdentifierStrategy);
            Assert.AreEqual("Customers", objectInfo.TableInfo.Name);
            Assert.AreEqual("Sales", objectInfo.TableInfo.Schema);
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

            [MicroLite.Mapping.Column("Created", allowUpdate: false)]
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