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
        public void CreateInstanceThrowsArgumentNullExceptionForNullReader()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var exception = Assert.Throws<ArgumentNullException>(
                () => objectInfo.CreateInstance(null));

            Assert.Equal("reader", exception.ParamName);
        }

        [Fact]
        public void CreateInstanceWithNullValues()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(8);
            mockReader.Setup(x => x.IsDBNull(It.IsAny<int>())).Returns((int i) =>
            {
                // CreditLimit, Updated and Website are null
                return i == 2 || i == 6 || i == 7;
            });

            mockReader.Setup(x => x.GetName(0)).Returns("Id");
            mockReader.Setup(x => x.GetInt32(0)).Returns(12345);

            mockReader.Setup(x => x.GetName(1)).Returns("Created");
            mockReader.Setup(x => x.GetDateTime(1)).Returns(new DateTime(2009, 4, 20));

            mockReader.Setup(x => x.GetName(2)).Returns("CreditLimit"); // null

            mockReader.Setup(x => x.GetName(3)).Returns("DateOfBirth");
            mockReader.Setup(x => x.GetDateTime(3)).Returns(new DateTime(1972, 8, 14));

            mockReader.Setup(x => x.GetName(4)).Returns("Name");
            mockReader.Setup(x => x.GetString(4)).Returns("John Smith");

            mockReader.Setup(x => x.GetName(5)).Returns("CustomerStatusId");
            mockReader.Setup(x => x.GetInt32(5)).Returns(1);

            mockReader.Setup(x => x.GetName(6)).Returns("Updated"); // null

            mockReader.Setup(x => x.GetName(7)).Returns("Website"); // null

            var instance = objectInfo.CreateInstance(mockReader.Object) as Customer;

            Assert.NotNull(instance);
            Assert.IsType<Customer>(instance);

            Assert.Equal(12345, instance.Id);
            Assert.Equal(new DateTime(2009, 4, 20), instance.Created);
            Assert.Null(instance.CreditLimit);
            Assert.Equal(new DateTime(1972, 8, 14), instance.DateOfBirth);
            Assert.Equal("John Smith", instance.Name);
            Assert.Equal(CustomerStatus.Active, instance.Status);
            Assert.Null(instance.Updated);
            Assert.Null(instance.Website);
        }

        [Fact]
        public void CreateInstanceWithoutNullValues()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(8);
            mockReader.Setup(x => x.IsDBNull(It.IsAny<int>())).Returns(false);

            mockReader.Setup(x => x.GetName(0)).Returns("Id");
            mockReader.Setup(x => x.GetInt32(0)).Returns(12345);

            mockReader.Setup(x => x.GetName(1)).Returns("Created");
            mockReader.Setup(x => x.GetDateTime(1)).Returns(new DateTime(2009, 4, 20));

            mockReader.Setup(x => x.GetName(2)).Returns("CreditLimit");
            mockReader.Setup(x => x.GetDecimal(2)).Returns(10250.00M);

            mockReader.Setup(x => x.GetName(3)).Returns("DateOfBirth");
            mockReader.Setup(x => x.GetDateTime(3)).Returns(new DateTime(1972, 8, 14));

            mockReader.Setup(x => x.GetName(4)).Returns("Name");
            mockReader.Setup(x => x.GetString(4)).Returns("John Smith");

            mockReader.Setup(x => x.GetName(5)).Returns("CustomerStatusId");
            mockReader.Setup(x => x.GetInt32(5)).Returns(1);

            mockReader.Setup(x => x.GetName(6)).Returns("Updated");
            mockReader.Setup(x => x.GetDateTime(6)).Returns(new DateTime(2014, 3, 27));

            mockReader.Setup(x => x.GetName(7)).Returns("Website");
            mockReader.Setup(x => x.GetString(7)).Returns("http://microliteorm.wordpress.com");

            var instance = objectInfo.CreateInstance(mockReader.Object) as Customer;

            Assert.NotNull(instance);
            Assert.IsType<Customer>(instance);

            Assert.Equal(new DateTime(2009, 4, 20), instance.Created);
            Assert.Equal(10250.00M, instance.CreditLimit);
            Assert.Equal(new DateTime(1972, 8, 14), instance.DateOfBirth);
            Assert.Equal(12345, instance.Id);
            Assert.Equal("John Smith", instance.Name);
            Assert.Equal(CustomerStatus.Active, instance.Status);
            Assert.Equal(new DateTime(2014, 3, 27), instance.Updated);
            Assert.Equal(new Uri("http://microliteorm.wordpress.com"), instance.Website);
        }

#if !NET_3_5

        [Fact]
        public void For_ReturnsExpandoObjectInfo_ForTypeOfDynamic()
        {
            var objectInfo = ObjectInfoHelper<dynamic>();

            Assert.IsType<ExpandoObjectInfo>(objectInfo);
        }

#endif

        [Fact]
        public void For_ReturnsSameObjectInfo_ForSameType()
        {
            var forType = typeof(Customer);

            var objectInfo1 = ObjectInfo.For(forType);
            var objectInfo2 = ObjectInfo.For(forType);

            Assert.Same(objectInfo1, objectInfo2);
        }

#if !NET_3_5

        [Fact]
        public void For_ReturnsTupleObjectInfo_ForTypeOfTupleT1()
        {
            var objectInfo = ObjectInfo.For(typeof(Tuple<int>));

            Assert.IsType<TupleObjectInfo>(objectInfo);
        }

        [Fact]
        public void For_ReturnsTupleObjectInfo_ForTypeOfTupleT2()
        {
            var objectInfo = ObjectInfo.For(typeof(Tuple<int, string>));

            Assert.IsType<TupleObjectInfo>(objectInfo);
        }

        [Fact]
        public void For_ReturnsTupleObjectInfo_ForTypeOfTupleT3()
        {
            var objectInfo = ObjectInfo.For(typeof(Tuple<int, string, DateTime>));

            Assert.IsType<TupleObjectInfo>(objectInfo);
        }

        [Fact]
        public void For_ReturnsTupleObjectInfo_ForTypeOfTupleT4()
        {
            var objectInfo = ObjectInfo.For(typeof(Tuple<int, string, DateTime, bool>));

            Assert.IsType<TupleObjectInfo>(objectInfo);
        }

        [Fact]
        public void For_ReturnsTupleObjectInfo_ForTypeOfTupleT5()
        {
            var objectInfo = ObjectInfo.For(typeof(Tuple<int, string, DateTime, bool, decimal>));

            Assert.IsType<TupleObjectInfo>(objectInfo);
        }

        [Fact]
        public void For_ReturnsTupleObjectInfo_ForTypeOfTupleT6()
        {
            var objectInfo = ObjectInfo.For(typeof(Tuple<int, string, DateTime, bool, decimal, double>));

            Assert.IsType<TupleObjectInfo>(objectInfo);
        }

        [Fact]
        public void For_ReturnsTupleObjectInfo_ForTypeOfTupleT7()
        {
            var objectInfo = ObjectInfo.For(typeof(Tuple<int, string, DateTime, bool, decimal, double, Guid>));

            Assert.IsType<TupleObjectInfo>(objectInfo);
        }

#endif

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
                ExceptionMessages.ObjectInfo_TypeMustNotBeAbstract.FormatWith(typeof(AbstractCustomer).Name),
                exception.Message);
        }

        [Fact]
        public void For_ThrowsMappingException_IfNoDefaultConstructor()
        {
            var exception = Assert.Throws<MappingException>(
                () => ObjectInfo.For(typeof(CustomerWithNoDefaultConstructor)));

            Assert.Equal(
                ExceptionMessages.ObjectInfo_TypeMustHaveDefaultConstructor.FormatWith(typeof(CustomerWithNoDefaultConstructor).Name),
                exception.Message);
        }

        [Fact]
        public void For_ThrowsMappingException_IfNonPublicClass()
        {
            var exception = Assert.Throws<MappingException>(
                () => ObjectInfo.For(typeof(InternalCustomer)));

            Assert.Equal(
                ExceptionMessages.ObjectInfo_TypeMustBePublic.FormatWith(typeof(InternalCustomer).Name),
                exception.Message);
        }

        [Fact]
        public void For_ThrowsMicroLiteException_IfNotClass()
        {
            var exception = Assert.Throws<MappingException>(
                () => ObjectInfo.For(typeof(CustomerStruct)));

            Assert.Equal(
                ExceptionMessages.ObjectInfo_TypeMustBeClass.FormatWith(typeof(CustomerStruct).Name),
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
                string.Format(ExceptionMessages.ObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        [Fact]
        public void GetIdentifierValueThrowsMicroLiteException_WhenNoIdentifierMapped()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new CustomerWithNoIdentifier();

            var objectInfo = ObjectInfo.For(typeof(CustomerWithNoIdentifier));

            var exception = Assert.Throws<MicroLiteException>(() => objectInfo.GetIdentifierValue(customer));

            Assert.Equal(ExceptionMessages.ObjectInfo_NoIdentifierColumn.FormatWith("Sales", "CustomerWithNoIdentivesier"), exception.Message);
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
                string.Format(ExceptionMessages.ObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        [Fact]
        public void GetInsertValuesThrowsMicroLiteException_WhenNoIdentifierMapped()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new CustomerWithNoIdentifier();

            var objectInfo = ObjectInfo.For(typeof(CustomerWithNoIdentifier));

            var exception = Assert.Throws<MicroLiteException>(() => objectInfo.GetInsertValues(customer));

            Assert.Equal(ExceptionMessages.ObjectInfo_NoIdentifierColumn.FormatWith("Sales", "CustomerWithNoIdentivesier"), exception.Message);
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
                string.Format(ExceptionMessages.ObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        [Fact]
        public void GetUpdateValuesThrowsMicroLiteException_WhenNoIdentifierMapped()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new CustomerWithNoIdentifier();

            var objectInfo = ObjectInfo.For(typeof(CustomerWithNoIdentifier));

            var exception = Assert.Throws<MicroLiteException>(() => objectInfo.GetUpdateValues(customer));

            Assert.Equal(ExceptionMessages.ObjectInfo_NoIdentifierColumn.FormatWith("Sales", "CustomerWithNoIdentivesier"), exception.Message);
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
                string.Format(ExceptionMessages.ObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
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
        public void HasDefaultIdentifierValueThrowsMicroLiteException_WhenNoIdentifierMapped()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new CustomerWithNoIdentifier();

            var objectInfo = ObjectInfo.For(typeof(CustomerWithNoIdentifier));

            var exception = Assert.Throws<MicroLiteException>(() => objectInfo.HasDefaultIdentifierValue(customer));

            Assert.Equal(ExceptionMessages.ObjectInfo_NoIdentifierColumn.FormatWith("Sales", "CustomerWithNoIdentivesier"), exception.Message);
        }

        [Fact]
        public void IsDefaultIdentifier_WhenIdentifierIsGuid()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithGuidIdentifier));

            Assert.True(objectInfo.IsDefaultIdentifier(Guid.Empty));

            Assert.False(objectInfo.IsDefaultIdentifier(Guid.NewGuid()));
        }

        [Fact]
        public void IsDefaultIdentifier_WhenIdentifierIsInteger()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            Assert.True(objectInfo.IsDefaultIdentifier(0));

            Assert.False(objectInfo.IsDefaultIdentifier(18734));
        }

        [Fact]
        public void IsDefaultIdentifier_WhenIdentifierIsString()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithStringIdentifier));

            Assert.True(objectInfo.IsDefaultIdentifier(null));

            Assert.False(objectInfo.IsDefaultIdentifier("AFIK"));
        }

        [Fact]
        public void SetIdentifierValue_SetsPropertyValue()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var customer = new Customer();

            objectInfo.SetIdentifierValue(customer, 122323);

            Assert.Equal(122323, customer.Id);
        }

        [Fact]
        public void VerifyInstanceForInsertDoesNotThrowMicroLiteException_WhenIdentifierStrategyAssigned_AndIdentifierSet()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var customer = new Customer
            {
                Id = 147843
            };

            var objectInfo = ObjectInfo.For(typeof(Customer));

            Assert.DoesNotThrow(() => objectInfo.VerifyInstanceForInsert(customer));
        }

        [Fact]
        public void VerifyInstanceForInsertDoesNotThrowMicroLiteException_WhenIdentifierStrategyDbGenerated_AndIdentifierNotSet()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer
            {
                Id = 0
            };

            var objectInfo = ObjectInfo.For(typeof(Customer));

            Assert.DoesNotThrow(() => objectInfo.VerifyInstanceForInsert(customer));
        }

        [Fact]
        public void VerifyInstanceForInsertThrowsMicroLiteException_WhenIdentifierStrategyAssigned_AndIdentifierNotSet()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var customer = new Customer
            {
                Id = 0
            };

            var objectInfo = ObjectInfo.For(typeof(Customer));

            var exception = Assert.Throws<MicroLiteException>(() => objectInfo.VerifyInstanceForInsert(customer));

            Assert.Equal(ExceptionMessages.ObjectInfo_IdentifierNotSetForInsert, exception.Message);
        }

        [Fact]
        public void VerifyInstanceForInsertThrowsMicroLiteException_WhenIdentifierStrategyDbGenerated_AndIdentifierSet()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer
            {
                Id = 147843
            };

            var objectInfo = ObjectInfo.For(typeof(Customer));

            var exception = Assert.Throws<MicroLiteException>(() => objectInfo.VerifyInstanceForInsert(customer));

            Assert.Equal(ExceptionMessages.ObjectInfo_IdentifierSetForInsert, exception.Message);
        }

        [Fact]
        public void VerifyInstanceForInsertThrowsMicroLiteException_WhenNoIdentifierMapped()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new CustomerWithNoIdentifier();

            var objectInfo = ObjectInfo.For(typeof(CustomerWithNoIdentifier));

            var exception = Assert.Throws<MicroLiteException>(() => objectInfo.VerifyInstanceForInsert(customer));

            Assert.Equal(ExceptionMessages.ObjectInfo_NoIdentifierColumn.FormatWith("Sales", "CustomerWithNoIdentivesier"), exception.Message);
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

        internal class InternalCustomer
        {
            public int Id
            {
                get;
                set;
            }
        }
    }
}