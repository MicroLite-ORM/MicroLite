namespace MicroLite.Tests.Core
{
    using System;
    using System.Data;
    using MicroLite.Core;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="ObjectBuilder"/> class.
    /// </summary>
    [TestFixture]
    public class ObjectBuilderTests
    {
        private enum CustomerStatus
        {
            Inactive = 0,
            Active = 1
        }

        [Test]
        public void BuildNewInstanceIgnoresUnknownColumnWithoutThrowingException()
        {
            var mockDataReader = new Mock<IDataReader>();
            mockDataReader.Setup(x => x.FieldCount).Returns(1);

            mockDataReader.Setup(x => x.GetName(0)).Returns("FooBarInvalid");

            var objectBuilder = new ObjectBuilder();
            objectBuilder.BuildNewInstance<Customer>(mockDataReader.Object);
        }

        [Test]
        public void BuildNewInstanceThrowsMicroLiteExceptionIfUnableToSetProperty()
        {
            var mockDataReader = new Mock<IDataReader>();
            mockDataReader.Setup(x => x.FieldCount).Returns(1);

            mockDataReader.Setup(x => x.GetName(0)).Returns("StatusId");

            mockDataReader.Setup(x => x[0]).Returns((decimal)123242.23234);

            var objectBuilder = new ObjectBuilder();

            var exception = Assert.Throws<MicroLiteException>(
                () => objectBuilder.BuildNewInstance<Customer>(mockDataReader.Object));

            Assert.NotNull(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
        }

        /// <summary>
        /// Issue #8 - ObjectBuilder throws exception converting DBNull to nullable ValueType.
        /// </summary>
        [Test]
        public void PropertyValueIsSetToNullForNullableInt()
        {
            var mockDataReader = new Mock<IDataReader>();
            mockDataReader.Setup(x => x.FieldCount).Returns(1);

            mockDataReader.Setup(x => x.GetName(0)).Returns("ReferredById");

            mockDataReader.Setup(x => x[0]).Returns(DBNull.Value);

            var objectBuilder = new ObjectBuilder();

            var customer = objectBuilder.BuildNewInstance<Customer>(mockDataReader.Object);

            Assert.IsNull(customer.ReferredById);
        }

        /// <summary>
        /// Issue #19 - Null strings in a column result in empty strings in the property
        /// </summary>
        [Test]
        public void PropertyValueIsSetToNullIdReaderValueIsDBNull()
        {
            var mockDataReader = new Mock<IDataReader>();
            mockDataReader.Setup(x => x.FieldCount).Returns(1);

            mockDataReader.Setup(x => x.GetName(0)).Returns("Name");

            mockDataReader.Setup(x => x[0]).Returns(DBNull.Value);

            var objectBuilder = new ObjectBuilder();

            var customer = objectBuilder.BuildNewInstance<Customer>(mockDataReader.Object);

            Assert.IsNull(customer.Name);
        }

        /// <summary>
        /// Issue #7 - ObjectBuilder throws exception converting int to nullable int.
        /// </summary>
        [Test]
        public void PropertyValueIsSetToValueForNullableInt()
        {
            var mockDataReader = new Mock<IDataReader>();
            mockDataReader.Setup(x => x.FieldCount).Returns(1);

            mockDataReader.Setup(x => x.GetName(0)).Returns("ReferredById");

            mockDataReader.Setup(x => x[0]).Returns((int?)1235);

            var objectBuilder = new ObjectBuilder();

            var customer = objectBuilder.BuildNewInstance<Customer>(mockDataReader.Object);

            Assert.AreEqual(1235, customer.ReferredById);
        }

        [Test]
        public void PropertyValuesAreSetCorrectly()
        {
            var mockDataReader = new Mock<IDataReader>();
            mockDataReader.Setup(x => x.FieldCount).Returns(4);

            mockDataReader.Setup(x => x.GetName(0)).Returns("CustomerId");
            mockDataReader.Setup(x => x.GetName(1)).Returns("Name");
            mockDataReader.Setup(x => x.GetName(2)).Returns("DoB");
            mockDataReader.Setup(x => x.GetName(3)).Returns("StatusId");

            mockDataReader.Setup(x => x[0]).Returns(123242);
            mockDataReader.Setup(x => x[1]).Returns("Trevor Pilley");
            mockDataReader.Setup(x => x[2]).Returns(new DateTime(1982, 11, 27));
            mockDataReader.Setup(x => x[3]).Returns(1);

            var objectBuilder = new ObjectBuilder();

            var customer = objectBuilder.BuildNewInstance<Customer>(mockDataReader.Object);

            Assert.AreEqual(new DateTime(1982, 11, 27), customer.DateOfBirth);
            Assert.AreEqual(123242, customer.Id);
            Assert.AreEqual("Trevor Pilley", customer.Name);
            Assert.AreEqual(CustomerStatus.Active, customer.Status);
        }

        [MicroLite.Mapping.Table("Sales", "Customers")]
        private class Customer
        {
            public Customer()
            {
            }

            [MicroLite.Mapping.Column("DoB")]
            public DateTime DateOfBirth
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("CustomerId")]
            [MicroLite.Mapping.Identifier(MicroLite.Mapping.IdentifierStrategy.Identity)]
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

            [MicroLite.Mapping.Column("ReferredById")]
            public int? ReferredById
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
    }
}