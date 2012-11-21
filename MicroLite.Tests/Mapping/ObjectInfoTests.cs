namespace MicroLite.Tests.Mapping
{
    using System;
    using MicroLite.FrameworkExtensions;
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
        public void ConstructorThrowsArgumentNullExceptionForNullForTableInfo()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new ObjectInfo(typeof(CustomerWithIntegerIdentifier), null));

            Assert.AreEqual("tableInfo", exception.ParamName);
        }

        [Test]
        public void ConstructorThrowsArgumentNullExceptionForNullForType()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new ObjectInfo(null, null));

            Assert.AreEqual("forType", exception.ParamName);
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
                Messages.ObjectInfo_TypeMustNotBeAbstract.FormatWith(typeof(AbstractCustomer).Name),
                exception.Message);
        }

        [Test]
        public void ForThrowsMicroLiteExceptionIfNoDefaultConstructor()
        {
            var exception = Assert.Throws<MicroLiteException>(
                () => ObjectInfo.For(typeof(CustomerWithNoDefaultConstructor)));

            Assert.AreEqual(
                Messages.ObjectInfo_TypeMustHaveDefaultConstructor.FormatWith(typeof(CustomerWithNoDefaultConstructor).Name),
                exception.Message);
        }

        [Test]
        public void ForThrowsMicroLiteExceptionIfNotClass()
        {
            var exception = Assert.Throws<MicroLiteException>(
                () => ObjectInfo.For(typeof(CustomerStruct)));

            Assert.AreEqual(
                Messages.ObjectInfo_TypeMustBeClass.FormatWith(typeof(CustomerStruct).Name),
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
        public void GetPropertyValueForColumnReturnsIntValueIfPropertyIsEnum()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var customer = new CustomerWithIntegerIdentifier
            {
                Status = CustomerStatus.Active
            };

            var value = (int)objectInfo.GetPropertyValueForColumn(customer, "StatusId");

            Assert.AreEqual(1, value);
        }

        [Test]
        public void GetPropertyValueForColumnReturnsNullForNullablePropertyValue()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var customer = new CustomerWithIntegerIdentifier
            {
                CreditLimit = null
            };

            var value = (Decimal?)objectInfo.GetPropertyValueForColumn(customer, "CreditLimit");

            Assert.IsNull(value);
        }

        [Test]
        public void GetPropertyValueForColumnReturnsPropertyValue()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var customer = new CustomerWithIntegerIdentifier
            {
                Name = "Trev"
            };

            var value = (string)objectInfo.GetPropertyValueForColumn(customer, "Name");

            Assert.AreEqual(customer.Name, value);
        }

        [Test]
        public void GetPropertyValueForColumnReturnsValueForNullablePropertyWithValue()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var customer = new CustomerWithIntegerIdentifier
            {
                CreditLimit = 10250M
            };

            var value = (Decimal)objectInfo.GetPropertyValueForColumn(customer, "CreditLimit");

            Assert.AreEqual(customer.CreditLimit.Value, value);
        }

        [Test]
        public void GetPropertyValueForColumnThrowsArgumentNullExceptionForNullInstance()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<ArgumentNullException>(() => objectInfo.GetPropertyValueForColumn(null, "Name"));

            Assert.AreEqual("instance", exception.ParamName);
        }

        [Test]
        public void GetPropertyValueForColumnThrowsMicroLiteExceptionIfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<MicroLiteException>(
                () => objectInfo.GetPropertyValueForColumn(new CustomerWithGuidIdentifier(), "Name"));

            Assert.AreEqual(
                string.Format(Messages.ObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        [Test]
        public void GetPropertyValueForColumnThrowsMicroLiteExceptionIfInvalidColumnName()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<MicroLiteException>(
                () => objectInfo.GetPropertyValueForColumn(new CustomerWithIntegerIdentifier(), "UnknownColumn"));

            Assert.AreEqual(
                string.Format(Messages.ObjectInfo_ColumnNotMapped, "UnknownColumn", objectInfo.ForType.Name),
                exception.Message);
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

        [Test]
        public void SetPropertyValueForColumnIgnoresUnmappedPropertyWithoutThrowingAnException()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var instance = new CustomerWithIntegerIdentifier();

            objectInfo.SetPropertyValueForColumn(instance, "UnknownColumn", 1);
        }

        [Test]
        public void SetPropertyValueForColumnSetsEnumValue()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var instance = new CustomerWithIntegerIdentifier();

            objectInfo.SetPropertyValueForColumn(instance, "StatusId", 1);

            Assert.AreEqual(instance.Status, CustomerStatus.Active);
        }

        /// <summary>
        /// SQLite stores all integers as a long (64bit integer), enums are 32bit integers so the value should be down cast.
        /// </summary>
        [Test]
        public void SetPropertyValueForColumnSetsEnumValueFromLong()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var instance = new CustomerWithIntegerIdentifier();

            objectInfo.SetPropertyValueForColumn(instance, "StatusId", (long)1);

            Assert.AreEqual(instance.Status, CustomerStatus.Active);
        }

        [Test]
        public void SetPropertyValueForColumnSetsNullablePropertyToNull()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var instance = new CustomerWithIntegerIdentifier();

            objectInfo.SetPropertyValueForColumn(instance, "CreditLimit", DBNull.Value);

            Assert.IsNull(instance.CreditLimit);
        }

        [Test]
        public void SetPropertyValueForColumnSetsNullablePropertyToValue()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var instance = new CustomerWithIntegerIdentifier();

            objectInfo.SetPropertyValueForColumn(instance, "CreditLimit", 2500M);

            Assert.AreEqual(2500M, instance.CreditLimit.Value);
        }

        [Test]
        public void SetPropertyValueForColumnSetsPropertyValue()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var instance = new CustomerWithIntegerIdentifier();

            objectInfo.SetPropertyValueForColumn(instance, "Name", "Trev");

            Assert.AreEqual("Trev", instance.Name);
        }

        [Test]
        public void SetPropertyValueForColumnThrowsArgumentNullExceptionForNullInstance()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<ArgumentNullException>(() => objectInfo.SetPropertyValueForColumn(null, "StatusId", 1));

            Assert.AreEqual("instance", exception.ParamName);
        }

        [Test]
        public void SetPropertyValueForColumnThrowsMicroLiteExceptionIfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var instance = new CustomerWithGuidIdentifier();

            var exception = Assert.Throws<MicroLiteException>(
                () => objectInfo.SetPropertyValueForColumn(instance, "StatusId", 1));

            Assert.AreEqual(
                string.Format(Messages.ObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        private struct CustomerStruct
        {
        }

        private abstract class AbstractCustomer
        {
        }

        [MicroLite.Mapping.Table("Sales", "Customers")]
        private class CustomerWithGuidIdentifier
        {
            [MicroLite.Mapping.Column("CustomerId")]
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

            [MicroLite.Mapping.Column("CreditLimit")]
            public Decimal? CreditLimit
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

        [MicroLite.Mapping.Table("Sales", "Customers")]
        private class CustomerWithStringIdentifier
        {
            [MicroLite.Mapping.Column("CustomerId")]
            [MicroLite.Mapping.Identifier(IdentifierStrategy.Assigned)]
            public string Id
            {
                get;
                set;
            }
        }
    }
}