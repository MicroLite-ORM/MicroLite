namespace MicroLite.Tests.Mapping
{
    using System;
    using System.Data;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ObjectInfo" /> class.
    /// </summary>
    public class ObjectInfoTests : UnitTest
    {
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
                () => new ObjectInfo(typeof(Customer), null));

            Assert.Equal("tableInfo", exception.ParamName);
        }

        [Fact]
        public void CreateInstance()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(2);
            mockReader.Setup(x => x.IsDBNull(It.IsAny<int>())).Returns(false);

            mockReader.Setup(x => x.GetName(0)).Returns("Id");
            mockReader.Setup(x => x.GetName(1)).Returns("CustomerStatusId");

            mockReader.Setup(x => x.GetInt32(0)).Returns(12345);
            mockReader.Setup(x => x.GetValue(1)).Returns(1);

            var instance = objectInfo.CreateInstance(mockReader.Object) as Customer;

            Assert.NotNull(instance);
            Assert.IsType<Customer>(instance);
            Assert.Equal(12345, instance.Id);
            Assert.Equal(CustomerStatus.Active, instance.Status);
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
            var forType = typeof(Customer);

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
            var forType = typeof(Customer);

            var objectInfo = ObjectInfo.For(forType);

            Assert.Equal(forType, objectInfo.ForType);
        }

        [Fact]
        public void GetColumnInfo_ReturnsColumnInfo_IfColumnMapped()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));
            var columnInfo = objectInfo.GetColumnInfo("Name");

            Assert.NotNull(columnInfo);
            Assert.Equal("Name", columnInfo.ColumnName);
        }

        [Fact]
        public void GetColumnInfo_ReturnsNull_IfColumnNotMapped()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            Assert.Null(objectInfo.GetColumnInfo("Wibble"));
        }

        [Fact]
        public void GetIdentifierValue_ReturnsPropertyValueOfIdentifierProperty()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var customer = new Customer
            {
                Id = 122323
            };

            var identifierValue = (int)objectInfo.GetIdentifierValue(customer);

            Assert.Equal(customer.Id, identifierValue);
        }

        [Fact]
        public void GetIdentifierValue_ThrowsArgumentNullException_IfInstanceIsNull()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var exception = Assert.Throws<ArgumentNullException>(
                () => objectInfo.GetIdentifierValue(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void GetIdentifierValue_ThrowsMappingException_IfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var exception = Assert.Throws<MappingException>(
                () => objectInfo.GetIdentifierValue(new CustomerWithGuidIdentifier()));

            Assert.Equal(
                string.Format(Messages.ObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        [Fact]
        public void GetInsertValues_ReturnsPropertyValues_WhenUsingAssigned()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var objectInfo = ObjectInfo.For(typeof(Customer));

            var customer = new Customer
            {
                Created = new DateTime(2011, 12, 24),
                CreditLimit = 10500.00M,
                DateOfBirth = new System.DateTime(1975, 9, 18),
                Id = 134875,
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now,
                Website = new Uri("http://microliteorm.wordpress.com")
            };

            var values = objectInfo.GetInsertValues(customer);

            Assert.Equal(7, values.Length);
            Assert.Equal(customer.Created, values[0]);
            Assert.Equal(customer.CreditLimit, values[1]);
            Assert.Equal(customer.DateOfBirth, values[2]);
            Assert.Equal(customer.Id, values[3]); // Id should be included as it's assigned.
            Assert.Equal(customer.Name, values[4]);
            Assert.Equal((int)customer.Status, values[5]);
            Assert.Equal(customer.Website.ToString(), values[6]);
        }

        [Fact]
        public void GetInsertValues_ReturnsPropertyValues_WhenUsingDbGenerated()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var objectInfo = ObjectInfo.For(typeof(Customer));

            var customer = new Customer
            {
                Created = new DateTime(2011, 12, 24),
                CreditLimit = 10500.00M,
                DateOfBirth = new System.DateTime(1975, 9, 18),
                Id = 134875,
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now,
                Website = new Uri("http://microliteorm.wordpress.com")
            };

            var values = objectInfo.GetInsertValues(customer);

            Assert.Equal(6, values.Length);
            Assert.Equal(customer.Created, values[0]);
            Assert.Equal(customer.CreditLimit, values[1]);
            Assert.Equal(customer.DateOfBirth, values[2]);
            Assert.Equal(customer.Name, values[3]);
            Assert.Equal((int)customer.Status, values[4]);
            Assert.Equal(customer.Website.ToString(), values[5]);
        }

        [Fact]
        public void GetInsertValues_ThrowsMappingException_IfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var exception = Assert.Throws<MappingException>(
                () => objectInfo.GetInsertValues(new CustomerWithGuidIdentifier()));

            Assert.Equal(
                string.Format(Messages.ObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        [Fact]
        public void GetUpdateValues_ReturnsPropertyValues_WhenUsingAssigned()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var objectInfo = ObjectInfo.For(typeof(Customer));

            var customer = new Customer
            {
                Created = new DateTime(2011, 12, 24),
                CreditLimit = 10500.00M,
                DateOfBirth = new System.DateTime(1975, 9, 18),
                Id = 134875,
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now,
                Website = new Uri("http://microliteorm.wordpress.com")
            };

            var values = objectInfo.GetUpdateValues(customer);

            Assert.Equal(7, values.Length);
            Assert.Equal(customer.CreditLimit, values[0]);
            Assert.Equal(customer.DateOfBirth, values[1]);
            Assert.Equal(customer.Name, values[2]);
            Assert.Equal((int)customer.Status, values[3]);
            Assert.Equal(customer.Updated, values[4]);
            Assert.Equal(customer.Website.ToString(), values[5]);
            Assert.Equal(customer.Id, values[6]);
        }

        [Fact]
        public void GetUpdateValues_ReturnsPropertyValues_WhenUsingDbGenerated()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var objectInfo = ObjectInfo.For(typeof(Customer));

            var customer = new Customer
            {
                Created = new DateTime(2011, 12, 24),
                CreditLimit = 10500.00M,
                DateOfBirth = new System.DateTime(1975, 9, 18),
                Id = 134875,
                Name = "Joe Bloggs",
                Status = CustomerStatus.Active,
                Updated = DateTime.Now,
                Website = new Uri("http://microliteorm.wordpress.com")
            };

            var values = objectInfo.GetUpdateValues(customer);

            Assert.Equal(7, values.Length);
            Assert.Equal(customer.CreditLimit, values[0]);
            Assert.Equal(customer.DateOfBirth, values[1]);
            Assert.Equal(customer.Name, values[2]);
            Assert.Equal((int)customer.Status, values[3]);
            Assert.Equal(customer.Updated, values[4]);
            Assert.Equal(customer.Website.ToString(), values[5]);
            Assert.Equal(customer.Id, values[6]);
        }

        [Fact]
        public void GetUpdateValues_ThrowsMappingException_IfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var exception = Assert.Throws<MappingException>(
                () => objectInfo.GetUpdateValues(new CustomerWithGuidIdentifier()));

            Assert.Equal(
                string.Format(Messages.ObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        [Fact]
        public void HasDefaultIdentifierValue_ThrowsArgumentNullException_IfInstanceIsNull()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var exception = Assert.Throws<ArgumentNullException>(
                () => objectInfo.HasDefaultIdentifierValue(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void HasDefaultIdentifierValue_ThrowsMappingException_IfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

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
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var customer = new Customer();

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
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var customer = new Customer();

            objectInfo.SetIdentifierValue(customer, 122323);

            Assert.Equal(122323, customer.Id);
        }

        /// <summary>
        /// A helper method required because you can't do typeof(dynamic).
        /// </summary>
        private static IObjectInfo ObjectInfoHelper<T>()
        {
            var objectInfo = ObjectInfo.For(typeof(T));

            return objectInfo;
        }

        public struct CustomerStruct
        {
        }

        public abstract class AbstractCustomer
        {
        }

        public class CustomerWithGuidIdentifier
        {
            public Guid Id
            {
                get;
                set;
            }
        }

        public class CustomerWithNoDefaultConstructor
        {
            public CustomerWithNoDefaultConstructor(string foo)
            {
            }
        }

        public class CustomerWithNoIdentifier
        {
        }

        public class CustomerWithStringIdentifier
        {
            public string Id
            {
                get;
                set;
            }
        }
    }
}