namespace MicroLite.Tests.Core
{
    using System;
    using System.Linq;
    using MicroLite.Core;
    using MicroLite.Logging;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="ObjectInfo"/> class.
    /// </summary>
    [TestFixture]
    public class ObjectInfoTests
    {
        private enum CustomerStatus
        {
            Inactive = 0,
            Active = 1
        }

        [Test]
        public void DefaultIdentiferValueIsSetCorrectlyForGuid()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithGuidIdentifier));

            Assert.AreEqual(Guid.Empty, objectInfo.DefaultIdentiferValue);
        }

        [Test]
        public void DefaultIdentiferValueIsSetCorrectlyForInteger()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            Assert.AreEqual(0, objectInfo.DefaultIdentiferValue);
        }

        [Test]
        public void ForReturnsSameInstanceOnEachCall()
        {
            var objectInfo1 = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));
            var objectInfo2 = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            Assert.AreSame(objectInfo1, objectInfo2);
        }

        [Test]
        public void ForThrowsArgumentNullExceptonForNullForType()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => ObjectInfo.For(null));

            Assert.AreEqual("forType", exception.ParamName);
        }

        [Test]
        public void ForThrowsMicroLiteExceptionIfAbstractClass()
        {
            var exception = Assert.Throws<MicroLiteException>(
                () => ObjectInfo.For(typeof(AbstractCustomer)));

            Assert.AreEqual(
                LogMessages.TypeMustNotBeAbstract.FormatWith(typeof(AbstractCustomer).Name),
                exception.Message);
        }

        [Test]
        public void ForThrowsMicroLiteExceptionIfNoDefaultConstructor()
        {
            var exception = Assert.Throws<MicroLiteException>(
                () => ObjectInfo.For(typeof(CustomerWithNoDefaultConstructor)));

            Assert.AreEqual(
                LogMessages.TypeMustHaveDefaultConstructor.FormatWith(typeof(CustomerWithNoDefaultConstructor).Name),
                exception.Message);
        }

        [Test]
        public void ForThrowsMicroLiteExceptionIfNoIdentifierPropertyFound()
        {
            var exception = Assert.Throws<MicroLiteException>(
                () => ObjectInfo.For(typeof(CustomerWithNoIdentifier)));

            Assert.AreEqual(
                LogMessages.NoIdentifierFoundForType.FormatWith(typeof(CustomerWithNoIdentifier).Name),
                exception.Message);
        }

        [Test]
        public void ForThrowsMicroLiteExceptionIfNotClass()
        {
            var exception = Assert.Throws<MicroLiteException>(
                () => ObjectInfo.For(typeof(CustomerStruct)));

            Assert.AreEqual(
                LogMessages.TypeMustBeClass.FormatWith(typeof(CustomerStruct).Name),
                exception.Message);
        }

        [Test]
        public void ForTypeReturnsTypePassedToConstructor()
        {
            var forType = typeof(CustomerWithIntegerIdentifier);

            var objectInfo = ObjectInfo.For(forType);

            Assert.AreEqual(forType, objectInfo.ForType);
        }

        [Test]
        public void GetPropertyInfoForColumnReturnsCorrectPropertyInfo()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            Assert.AreEqual("Id", objectInfo.GetPropertyInfoForColumn("CustomerId").Name);
            Assert.AreEqual("Name", objectInfo.GetPropertyInfoForColumn("Name").Name);
            Assert.AreEqual("DateOfBirth", objectInfo.GetPropertyInfoForColumn("DoB").Name);
            Assert.AreEqual("Status", objectInfo.GetPropertyInfoForColumn("StatusId").Name);
        }

        [Test]
        public void GetPropertyInfoForColumnReturnsNullForUnmappedColumns()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            Assert.IsNull(objectInfo.GetPropertyInfoForColumn("AgeInYears"));
            Assert.IsNull(objectInfo.GetPropertyInfoForColumn("TempraryNotes"));
        }

        [Test]
        public void HasDefaultGuidValue()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithGuidIdentifier));

            var customer = new CustomerWithGuidIdentifier();

            customer.Id = Guid.Empty;
            Assert.IsTrue(objectInfo.HasDefaultIdentifierValue(customer));

            customer.Id = Guid.NewGuid();
            Assert.IsFalse(objectInfo.HasDefaultIdentifierValue(customer));
        }

        [Test]
        public void HasDefaultIntegerValue()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var customer = new CustomerWithIntegerIdentifier();

            customer.Id = 0;
            Assert.IsTrue(objectInfo.HasDefaultIdentifierValue(customer));

            customer.Id = 123;
            Assert.IsFalse(objectInfo.HasDefaultIdentifierValue(customer));
        }

        [Test]
        public void TableInfoColumnInfoAreCapturedCorrectly()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var columns = objectInfo.TableInfo.Columns.ToArray();

            Assert.AreEqual(4, columns.Length);

            Assert.AreEqual("DoB", columns[0]); // From Column attribute.
            Assert.AreEqual("CustomerId", columns[1]); // From Column attribute.
            Assert.AreEqual("Name", columns[2]); // From property name.
            Assert.AreEqual("StatusId", columns[3]); // From Column attribute.
        }

        [Test]
        public void TableInfoIdentifierColumnIsSetCorrectly()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            Assert.AreEqual("CustomerId", objectInfo.TableInfo.IdentifierColumn);
        }

        [Test]
        public void TableInfoIdentifierColumnIsSetCorrectlyForPocoWithClassNameIdProperty()
        {
            var objectInfo = ObjectInfo.For(typeof(PocoCustomer2));

            Assert.AreEqual("PocoCustomer2Id", objectInfo.TableInfo.IdentifierColumn);
            Assert.AreEqual(IdentifierStrategy.DbGenerated, objectInfo.TableInfo.IdentifierStrategy);
        }

        [Test]
        public void TableInfoIdentifierColumnIsSetCorrectlyForPocoWithIdProperty()
        {
            var objectInfo = ObjectInfo.For(typeof(PocoCustomer1));

            Assert.AreEqual("Id", objectInfo.TableInfo.IdentifierColumn);
            Assert.AreEqual(IdentifierStrategy.DbGenerated, objectInfo.TableInfo.IdentifierStrategy);
        }

        [Test]
        public void TableInfoIdentifierStrategyIsSetCorrectly()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            Assert.AreEqual(IdentifierStrategy.Assigned, objectInfo.TableInfo.IdentifierStrategy);
        }

        [Test]
        public void TableInfoTableNameIsSetCorrectly()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            Assert.AreEqual("Customers", objectInfo.TableInfo.Name);
        }

        [Test]
        public void TableInfoTableSchemaIsSetCorrectly()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            Assert.AreEqual("Sales", objectInfo.TableInfo.Schema);
        }

        private struct CustomerStruct
        {
        }

        private abstract class AbstractCustomer
        {
        }

        private class CustomerWithGuidIdentifier
        {
            [MicroLite.Identifier(IdentifierStrategy.Assigned)] // Don't use default or we can't prove we read it correctly.
            public Guid Id
            {
                get;
                set;
            }
        }

        [MicroLite.Table("Sales", "Customers")]
        private class CustomerWithIntegerIdentifier
        {
            public CustomerWithIntegerIdentifier()
            {
            }

            public int AgeInYears
            {
                get
                {
                    return DateTime.Today.Year - this.DateOfBirth.Year;
                }
            }

            [MicroLite.Column("DoB")]
            public DateTime DateOfBirth
            {
                get;
                set;
            }

            [MicroLite.Column("CustomerId")]
            [MicroLite.Identifier(IdentifierStrategy.Assigned)] // Don't use default or we can't prove we read it correctly.
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

            [MicroLite.Column("StatusId")]
            public CustomerStatus Status
            {
                get;
                set;
            }

            [MicroLite.Ignore]
            public string TempraryNotes
            {
                get;
                set;
            }
        }

        private class CustomerWithNoDefaultConstructor
        {
            public CustomerWithNoDefaultConstructor(string foo)
            {
            }
        }

        private class CustomerWithNoIdentifier
        {
        }

        /// <summary>
        /// Used to check that we can auto determine the identifier if there is a property called Id.
        /// </summary>
        private class PocoCustomer1
        {
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