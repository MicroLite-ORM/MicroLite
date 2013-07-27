namespace MicroLite.Tests.Core
{
    using System;
    using System.Data;
    using MicroLite.Core;
    using MicroLite.Mapping;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ObjectBuilder"/> class.
    /// </summary>
    public class ObjectBuilderTests : IDisposable
    {
        public ObjectBuilderTests()
        {
            // The tests in this suite all use attribute mapping for the test.
            ObjectInfo.MappingConvention = new AttributeMappingConvention();
        }

        private enum CustomerStatus
        {
            Inactive = 0,
            Active = 1
        }

        /// <summary>
        /// Issue #8 - ObjectBuilder throws exception converting DBNull to nullable ValueType.
        /// </summary>
        [Fact]
        public void BuildInstancePropertyValueIsSetToNullForNullableInt()
        {
            var mockDataReader = new Mock<IDataReader>();
            mockDataReader.Setup(x => x.FieldCount).Returns(1);

            mockDataReader.Setup(x => x.GetName(0)).Returns("ReferredById");

            mockDataReader.Setup(x => x[0]).Returns(DBNull.Value);

            var objectBuilder = new ObjectBuilder();

            var customer = objectBuilder.BuildInstance<Customer>(ObjectInfo.For(typeof(Customer)), mockDataReader.Object);

            Assert.Null(customer.ReferredById);
        }

        /// <summary>
        /// Issue #19 - Null strings in a column result in empty strings in the property
        /// </summary>
        [Fact]
        public void BuildInstancePropertyValueIsSetToNullIdReaderValueIsDBNull()
        {
            var mockDataReader = new Mock<IDataReader>();
            mockDataReader.Setup(x => x.FieldCount).Returns(1);

            mockDataReader.Setup(x => x.GetName(0)).Returns("Name");

            mockDataReader.Setup(x => x[0]).Returns(DBNull.Value);

            var objectBuilder = new ObjectBuilder();

            var customer = objectBuilder.BuildInstance<Customer>(ObjectInfo.For(typeof(Customer)), mockDataReader.Object);

            Assert.Null(customer.Name);
        }

        /// <summary>
        /// Issue #7 - ObjectBuilder throws exception converting int to nullable int.
        /// </summary>
        [Fact]
        public void BuildInstancePropertyValueIsSetToValueForNullableInt()
        {
            var mockDataReader = new Mock<IDataReader>();
            mockDataReader.Setup(x => x.FieldCount).Returns(1);

            mockDataReader.Setup(x => x.GetName(0)).Returns("ReferredById");

            mockDataReader.Setup(x => x[0]).Returns((int?)1235);

            var objectBuilder = new ObjectBuilder();

            var customer = objectBuilder.BuildInstance<Customer>(ObjectInfo.For(typeof(Customer)), mockDataReader.Object);

            Assert.Equal(1235, customer.ReferredById);
        }

        [Fact]
        public void BuildInstancePropertyValuesAreSetCorrectly()
        {
            var mockDataReader = new Mock<IDataReader>();
            mockDataReader.Setup(x => x.FieldCount).Returns(4);

            mockDataReader.Setup(x => x.GetName(0)).Returns("CustomerId");
            mockDataReader.Setup(x => x.GetName(1)).Returns("Name");
            mockDataReader.Setup(x => x.GetName(2)).Returns("DoB");
            mockDataReader.Setup(x => x.GetName(3)).Returns("StatusId");

            mockDataReader.Setup(x => x[0]).Returns(123242);
            mockDataReader.Setup(x => x[1]).Returns("Joe Bloggs");
            mockDataReader.Setup(x => x[2]).Returns(new DateTime(1980, 7, 13));
            mockDataReader.Setup(x => x[3]).Returns(1);

            var objectBuilder = new ObjectBuilder();

            var customer = objectBuilder.BuildInstance<Customer>(ObjectInfo.For(typeof(Customer)), mockDataReader.Object);

            Assert.Equal(new DateTime(1980, 7, 13), customer.DateOfBirth);
            Assert.Equal(123242, customer.Id);
            Assert.Equal("Joe Bloggs", customer.Name);
            Assert.Equal(CustomerStatus.Active, customer.Status);
        }

        [Fact]
        public void BuildInstanceThrowsMicroLiteExceptionIfUnableToSetProperty()
        {
            var mockDataReader = new Mock<IDataReader>();
            mockDataReader.Setup(x => x.FieldCount).Returns(1);

            mockDataReader.Setup(x => x.GetName(0)).Returns("DoB");

            mockDataReader.Setup(x => x[0]).Returns("foo");

            var objectBuilder = new ObjectBuilder();

            var exception = Assert.Throws<MicroLiteException>(
                () => objectBuilder.BuildInstance<Customer>(ObjectInfo.For(typeof(Customer)), mockDataReader.Object));

            Assert.NotNull(exception.InnerException);
            Assert.Equal(exception.InnerException.Message, exception.Message);
        }

        public void Dispose()
        {
            // Reset the mapping convention after tests have run.
            ObjectInfo.MappingConvention = new ConventionMappingConvention(ConventionMappingSettings.Default);
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
            [MicroLite.Mapping.Identifier(MicroLite.Mapping.IdentifierStrategy.DbGenerated)]
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