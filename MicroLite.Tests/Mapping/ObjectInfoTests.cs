namespace MicroLite.Tests.Mapping
{
    using System;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Logging;
    using MicroLite.Mapping;
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
        public void DefaultIdentifierValueIsSetCorrectlyForGuid()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithGuidIdentifier));

            Assert.AreEqual(Guid.Empty, objectInfo.DefaultIdentifierValue);
        }

        [Test]
        public void DefaultIdentifierValueIsSetCorrectlyForInteger()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            Assert.AreEqual(0, objectInfo.DefaultIdentifierValue);
        }

        [Test]
        public void DefaultIdentifierValueIsSetCorrectlyForString()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithStringIdentifier));

            Assert.IsNull(objectInfo.DefaultIdentifierValue);
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
        public void HasDefaultStringValue()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithStringIdentifier));

            var customer = new CustomerWithStringIdentifier();

            customer.Id = null;
            Assert.IsTrue(objectInfo.HasDefaultIdentifierValue(customer));

            customer.Id = "AFIK";
            Assert.IsFalse(objectInfo.HasDefaultIdentifierValue(customer));
        }

        private struct CustomerStruct
        {
        }

        private abstract class AbstractCustomer
        {
        }

        private class CustomerWithGuidIdentifier
        {
            [MicroLite.Mapping.Identifier(IdentifierStrategy.Assigned)] // Don't use default or we can't prove we read it correctly.
            public Guid Id
            {
                get;
                set;
            }
        }

        [MicroLite.Mapping.Table("Sales", "Customers")]
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

        private class CustomerWithNoDefaultConstructor
        {
            public CustomerWithNoDefaultConstructor(string foo)
            {
            }
        }

        private class CustomerWithNoIdentifier
        {
        }

        private class CustomerWithStringIdentifier
        {
            public string Id
            {
                get;
                set;
            }
        }
    }
}