namespace MicroLite.Tests.Mapping
{
    using System;
    using System.Data;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ObjectInfo" /> class.
    /// </summary>
    public class ObjectInfoTests : IDisposable
    {
        public ObjectInfoTests()
        {
            ObjectInfo.MappingConvention = new AttributeMappingConvention();
        }

        private enum CustomerStatus
        {
            Inactive = 0,
            Active = 1
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfForTypeIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new ObjectInfo(null, null));

            Assert.Equal("forType", exception.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfTableInfoIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new ObjectInfo(typeof(CustomerWithIntegerIdentifier), null));

            Assert.Equal("tableInfo", exception.ParamName);
        }

        [Fact]
        public void CreateInstance_ReturnsAnInstanceOfTheTypeTheObjectInfoIsFor()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var instance = objectInfo.CreateInstance<CustomerWithIntegerIdentifier>();

            Assert.IsType<CustomerWithIntegerIdentifier>(instance);
        }

        [Fact]
        public void CreateInstance_ThrowsMappingException_IfIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<MappingException>(
                () => objectInfo.CreateInstance<CustomerWithGuidIdentifier>());

            Assert.Equal(
                string.Format(Messages.ObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        public void Dispose()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(ConventionMappingSettings.Default);
        }

        [Fact]
        public void For_ReturnsExpandoObjectInfo_ForTypeOfDynamic()
        {
            var objectInfo = ObjectInfoHelper<dynamic>();

            Assert.IsType<ExpandoObjectInfo>(objectInfo);
        }

        [Fact]
        public void For_ReturnsSameObjectInfo_ForSameType()
        {
            var forType = typeof(CustomerWithIntegerIdentifier);

            var objectInfo1 = ObjectInfo.For(forType);
            var objectInfo2 = ObjectInfo.For(forType);

            Assert.Same(objectInfo1, objectInfo2);
        }

        [Fact]
        public void For_ThrowsArgumentNullExceptonForNullForType()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => ObjectInfo.For(null));

            Assert.Equal("forType", exception.ParamName);
        }

        [Fact]
        public void For_ThrowsMappingException_IfAbstractClass()
        {
            var exception = Assert.Throws<MappingException>(
                () => ObjectInfo.For(typeof(AbstractCustomer)));

            Assert.Equal(
                Messages.ObjectInfo_TypeMustNotBeAbstract.FormatWith(typeof(AbstractCustomer).Name),
                exception.Message);
        }

        [Fact]
        public void For_ThrowsMappingException_IfNoDefaultConstructor()
        {
            var exception = Assert.Throws<MappingException>(
                () => ObjectInfo.For(typeof(CustomerWithNoDefaultConstructor)));

            Assert.Equal(
                Messages.ObjectInfo_TypeMustHaveDefaultConstructor.FormatWith(typeof(CustomerWithNoDefaultConstructor).Name),
                exception.Message);
        }

        [Fact]
        public void For_ThrowsMicroLiteException_IfNotClass()
        {
            var exception = Assert.Throws<MappingException>(
                () => ObjectInfo.For(typeof(CustomerStruct)));

            Assert.Equal(
                Messages.ObjectInfo_TypeMustBeClass.FormatWith(typeof(CustomerStruct).Name),
                exception.Message);
        }

        [Fact]
        public void ForType_ReturnsTypePassedToConstructor()
        {
            var forType = typeof(CustomerWithIntegerIdentifier);

            var objectInfo = ObjectInfo.For(forType);

            Assert.Equal(forType, objectInfo.ForType);
        }

        [Fact]
        public void GetColumnInfo_ReturnsColumnInfo_IfColumnMapped()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));
            var columnInfo = objectInfo.GetColumnInfo("Name");

            Assert.NotNull(columnInfo);
            Assert.Equal("Name", columnInfo.ColumnName);
        }

        [Fact]
        public void GetColumnInfo_ReturnsNull_IfColumnNotMapped()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            Assert.Null(objectInfo.GetColumnInfo("Wibble"));
        }

        [Fact]
        public void GetIdentifierValue_ReturnsPropertyValueOfIdentifierProperty()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var customer = new CustomerWithIntegerIdentifier
            {
                Id = 122323
            };

            var identifierValue = (int)objectInfo.GetIdentifierValue(customer);

            Assert.Equal(customer.Id, identifierValue);
        }

        [Fact]
        public void GetIdentifierValue_ThrowsArgumentNullException_IfInstanceIsNull()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<ArgumentNullException>(
                () => objectInfo.GetIdentifierValue(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void GetIdentifierValue_ThrowsMappingException_IfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<MappingException>(
                () => objectInfo.GetIdentifierValue(new CustomerWithGuidIdentifier()));

            Assert.Equal(
                string.Format(Messages.ObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        [Fact]
        public void GetInsertValues_ReturnsPropertyValues()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var customer = new CustomerWithIntegerIdentifier
            {
                CreditLimit = 8822.22M,
                DateOfBirth = new DateTime(1972, 12, 23),
                Id = 122323,
                Name = "Fred Flintstone",
                Status = CustomerStatus.Active
            };

            var values = objectInfo.GetInsertValues(customer);

            Assert.Equal(5, values.Length);
            Assert.Equal(customer.CreditLimit, values[0]);
            Assert.Equal(customer.DateOfBirth, values[1]);
            Assert.Equal(customer.Id, values[2]); // Id should be included because Id is Assigned.
            Assert.Equal(customer.Name, values[3]);
            Assert.Equal((int)customer.Status, values[4]);
        }

        [Fact]
        public void GetInsertValues_ThrowsMappingException_IfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<MappingException>(
                () => objectInfo.GetInsertValues(new CustomerWithGuidIdentifier()));

            Assert.Equal(
                string.Format(Messages.ObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        [Fact]
        public void GetUpdateValues_ReturnsPropertyValues()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var customer = new CustomerWithIntegerIdentifier
            {
                CreditLimit = 8822.22M,
                DateOfBirth = new DateTime(1972, 12, 23),
                Id = 122323,
                Name = "Fred Flintstone",
                Status = CustomerStatus.Active
            };

            var values = objectInfo.GetUpdateValues(customer);

            Assert.Equal(5, values.Length);
            Assert.Equal(customer.CreditLimit, values[0]);
            Assert.Equal(customer.DateOfBirth, values[1]);
            Assert.Equal(customer.Name, values[2]);
            Assert.Equal((int)customer.Status, values[3]);
            Assert.Equal(customer.Id, values[4]);
        }

        [Fact]
        public void GetUpdateValues_ThrowsMappingException_IfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<MappingException>(
                () => objectInfo.GetUpdateValues(new CustomerWithGuidIdentifier()));

            Assert.Equal(
                string.Format(Messages.ObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        [Fact]
        public void HasDefaultIdentifierValue_ThrowsArgumentNullException_IfInstanceIsNull()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<ArgumentNullException>(
                () => objectInfo.HasDefaultIdentifierValue(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void HasDefaultIdentifierValue_ThrowsMappingException_IfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var instance = new CustomerWithGuidIdentifier();

            var exception = Assert.Throws<MappingException>(
                () => objectInfo.HasDefaultIdentifierValue(instance));

            Assert.Equal(
                string.Format(Messages.ObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        [Fact]
        public void HasDefaultIdentifierValue_WhenIdentifierIsGuid()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithGuidIdentifier));

            var customer = new CustomerWithGuidIdentifier();

            customer.Id = Guid.Empty;
            Assert.True(objectInfo.HasDefaultIdentifierValue(customer));

            customer.Id = Guid.NewGuid();
            Assert.False(objectInfo.HasDefaultIdentifierValue(customer));
        }

        [Fact]
        public void HasDefaultIdentifierValue_WhenIdentifierIsInteger()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var customer = new CustomerWithIntegerIdentifier();

            customer.Id = 0;
            Assert.True(objectInfo.HasDefaultIdentifierValue(customer));

            customer.Id = 123;
            Assert.False(objectInfo.HasDefaultIdentifierValue(customer));
        }

        [Fact]
        public void HasDefaultIdentifierValue_WhenIdentifierIsString()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithStringIdentifier));

            var customer = new CustomerWithStringIdentifier();

            customer.Id = null;
            Assert.True(objectInfo.HasDefaultIdentifierValue(customer));

            customer.Id = "AFIK";
            Assert.False(objectInfo.HasDefaultIdentifierValue(customer));
        }

        [Fact]
        public void SetIdentifierValue_SetsPropertyValue()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var customer = new CustomerWithIntegerIdentifier();

            objectInfo.SetIdentifierValue(customer, 122323);

            Assert.Equal(122323, customer.Id);
        }

        [Fact]
        public void SetPropertyValues_SetsPropertyValues()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var instance = objectInfo.CreateInstance<CustomerWithIntegerIdentifier>();

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(2);
            mockReader.Setup(x => x.IsDBNull(It.IsAny<int>())).Returns(false);

            mockReader.Setup(x => x.GetName(0)).Returns("CustomerId");
            mockReader.Setup(x => x.GetName(1)).Returns("StatusId");

            mockReader.Setup(x => x.GetInt32(0)).Returns(12345);
            mockReader.Setup(x => x.GetValue(1)).Returns(1);

            objectInfo.SetPropertyValues(instance, mockReader.Object);

            Assert.Equal(12345, instance.Id);
            Assert.Equal(CustomerStatus.Active, instance.Status);
        }

        /// <summary>
        /// A helper method required because you can't do typeof(dynamic).
        /// </summary>
        private static IObjectInfo ObjectInfoHelper<T>()
        {
            var objectInfo = ObjectInfo.For(typeof(T));

            return objectInfo;
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