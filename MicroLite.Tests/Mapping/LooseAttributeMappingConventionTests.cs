namespace MicroLite.Tests.Mapping
{
    using System;
    using System.Linq;
    using MicroLite.Mapping;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="LooseAttributeMappingConvention"/> class.
    /// </summary>
    [TestFixture]
    public class LooseAttributeMappingConventionTests
    {
        private enum CustomerStatus
        {
            Inactive = 0,
            Active = 1
        }

        [Test]
        public void TableInfoColumnsAreMappedCorrectlyForTypeWithAttributes()
        {
            var mappingConvention = new LooseAttributeMappingConvention();
            var objectInfo = mappingConvention.CreateObjectInfo(typeof(CustomerWithAttributes));

            var columns = objectInfo.TableInfo.Columns.ToArray();

            Assert.AreEqual(4, columns.Length);

            Assert.AreEqual("DoB", columns[0].ColumnName);
            Assert.IsFalse(columns[0].IsIdentifier);
            Assert.AreEqual(typeof(CustomerWithAttributes).GetProperty("DateOfBirth"), columns[0].PropertyInfo);

            Assert.AreEqual("CustomerId", columns[1].ColumnName);
            Assert.IsTrue(columns[1].IsIdentifier);
            Assert.AreEqual(typeof(CustomerWithAttributes).GetProperty("Id"), columns[1].PropertyInfo);

            Assert.AreEqual("Name", columns[2].ColumnName);
            Assert.IsFalse(columns[2].IsIdentifier);
            Assert.AreEqual(typeof(CustomerWithAttributes).GetProperty("Name"), columns[2].PropertyInfo);

            Assert.AreEqual("StatusId", columns[3].ColumnName);
            Assert.IsFalse(columns[3].IsIdentifier);
            Assert.AreEqual(typeof(CustomerWithAttributes).GetProperty("Status"), columns[3].PropertyInfo);
        }

        [Test]
        public void TableInfoColumnsAreMappedCorrectlyForTypeWithoutAttributes()
        {
            var mappingConvention = new LooseAttributeMappingConvention();
            var objectInfo = mappingConvention.CreateObjectInfo(typeof(CustomerWithoutAttributes));

            var columns = objectInfo.TableInfo.Columns.ToArray();

            Assert.AreEqual(4, columns.Length);

            Assert.AreEqual("DateOfBirth", columns[0].ColumnName);
            Assert.IsFalse(columns[0].IsIdentifier);
            Assert.AreEqual(typeof(CustomerWithoutAttributes).GetProperty("DateOfBirth"), columns[0].PropertyInfo);

            Assert.AreEqual("Id", columns[1].ColumnName);
            Assert.IsTrue(columns[1].IsIdentifier);
            Assert.AreEqual(typeof(CustomerWithoutAttributes).GetProperty("Id"), columns[1].PropertyInfo);

            Assert.AreEqual("Name", columns[2].ColumnName);
            Assert.IsFalse(columns[2].IsIdentifier);
            Assert.AreEqual(typeof(CustomerWithoutAttributes).GetProperty("Name"), columns[2].PropertyInfo);

            Assert.AreEqual("Status", columns[3].ColumnName);
            Assert.IsFalse(columns[3].IsIdentifier);
            Assert.AreEqual(typeof(CustomerWithoutAttributes).GetProperty("Status"), columns[3].PropertyInfo);
        }

        [Test]
        public void TableInfoHasCorrectPropertiesForTypeWithAttributes()
        {
            var mappingConvention = new LooseAttributeMappingConvention();
            var objectInfo = mappingConvention.CreateObjectInfo(typeof(CustomerWithAttributes));

            Assert.AreEqual("CustomerId", objectInfo.TableInfo.IdentifierColumn);
            Assert.AreEqual(IdentifierStrategy.Assigned, objectInfo.TableInfo.IdentifierStrategy);
            Assert.AreEqual("Customers", objectInfo.TableInfo.Name);
            Assert.AreEqual("Sales", objectInfo.TableInfo.Schema);
        }

        [Test]
        public void TableInfoHasCorrectPropertiesForTypeWithoutAttributes()
        {
            var mappingConvention = new LooseAttributeMappingConvention();
            var objectInfo = mappingConvention.CreateObjectInfo(typeof(CustomerWithoutAttributes));

            Assert.AreEqual("Id", objectInfo.TableInfo.IdentifierColumn);
            Assert.AreEqual(IdentifierStrategy.Identity, objectInfo.TableInfo.IdentifierStrategy);
            Assert.AreEqual(typeof(CustomerWithoutAttributes).Name, objectInfo.TableInfo.Name);
            Assert.IsEmpty(objectInfo.TableInfo.Schema);
        }

        [Test]
        public void TableInfoIdentifierColumnIsSetCorrectlyForPocoWithClassNameIdProperty()
        {
            var mappingConvention = new LooseAttributeMappingConvention();
            var objectInfo = mappingConvention.CreateObjectInfo(typeof(PocoCustomer2));

            Assert.AreEqual("PocoCustomer2Id", objectInfo.TableInfo.IdentifierColumn);
            Assert.AreEqual(IdentifierStrategy.Identity, objectInfo.TableInfo.IdentifierStrategy);
        }

        [Test]
        public void TableInfoIdentifierColumnIsSetCorrectlyForPocoWithIdProperty()
        {
            var mappingConvention = new LooseAttributeMappingConvention();
            var objectInfo = mappingConvention.CreateObjectInfo(typeof(PocoCustomer1));

            Assert.AreEqual("CustomerId", objectInfo.TableInfo.IdentifierColumn);
            Assert.AreEqual(IdentifierStrategy.Identity, objectInfo.TableInfo.IdentifierStrategy);
        }

        [MicroLite.Mapping.Table("Sales", "Customers")]
        private class CustomerWithAttributes
        {
            public CustomerWithAttributes()
            {
            }

            public int AgeInYears
            {
                get
                {
                    return DateTime.Today.Year - this.DateOfBirth.Year;
                }
            }

            [MicroLite.Mapping.Column("DoB")]
            public DateTime DateOfBirth
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("CustomerId")]
            [MicroLite.Mapping.Identifier(IdentifierStrategy.Assigned)] // Don't use default or we can't prove we read it correctly.
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

            [MicroLite.Mapping.Ignore]
            public string TempraryNotes
            {
                get;
                set;
            }
        }

        private class CustomerWithoutAttributes
        {
            public CustomerWithoutAttributes()
            {
            }

            public int AgeInYears
            {
                get
                {
                    return DateTime.Today.Year - this.DateOfBirth.Year;
                }
            }

            public DateTime DateOfBirth
            {
                get;
                set;
            }

            public int Id
            {
                get;
                set;
            }

            public string Name
            {
                get;
                set;
            }

            public CustomerStatus Status
            {
                get;
                set;
            }

            [MicroLite.Mapping.Ignore]
            public string TempraryNotes
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Used to check that we can auto determine the identifier if there is a property called Id.
        /// </summary>
        private class PocoCustomer1
        {
            [MicroLite.Mapping.Column("CustomerId")]
            public int Id
            {
                get;
                set;
            }
        }

        /// <summary>
        /// Used to check that we can auto determine the identifier if there is a property called {ClassName}Id.
        /// </summary>
        private class PocoCustomer2
        {
            public int PocoCustomer2Id
            {
                get;
                set;
            }
        }
    }
}