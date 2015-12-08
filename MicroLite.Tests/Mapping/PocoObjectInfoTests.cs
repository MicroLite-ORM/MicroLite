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
    /// Unit Tests for the <see cref="PocoObjectInfo" /> class.
    /// </summary>
    public class PocoObjectInfoTests : UnitTest
    {
        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfForTypeIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new PocoObjectInfo(null, null));

            Assert.Equal("forType", exception.ParamName);
        }

        [Fact]
        public void Constructor_ThrowsArgumentNullException_IfTableInfoIsNull()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new PocoObjectInfo(typeof(Customer), null));

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
            mockReader.Setup(x => x.FieldCount).Returns(9);
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

            mockReader.Setup(x => x.GetName(8)).Returns("Version");
            mockReader.Setup(x => x.GetInt32(8)).Returns(1);

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
            Assert.Equal(1, instance.Version);
        }

        [Fact]
        public void CreateInstanceWithoutNullValues()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var mockReader = new Mock<IDataReader>();
            mockReader.Setup(x => x.FieldCount).Returns(9);
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

            mockReader.Setup(x => x.GetName(8)).Returns("Version");
            mockReader.Setup(x => x.GetInt32(8)).Returns(1);

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
            Assert.Equal(1, instance.Version);
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
                string.Format(ExceptionMessages.PocoObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
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

            Assert.Equal(ExceptionMessages.PocoObjectInfo_NoIdentifierColumn.FormatWith("Sales", "CustomerWithNoIdentifiers"), exception.Message);
        }

        [Fact]
        public void GetVersionValue_ReturnsPropertyValueOfVersionProperty()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var customer = new Customer
            {
                Version = 233
            };

            var identifierValue = (int)objectInfo.GetVersionValue(customer);

            Assert.Equal(customer.Id, identifierValue);
        }

        [Fact]
        public void GetVersionValue_ThrowsArgumentNullException_IfInstanceIsNull()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var exception = Assert.Throws<ArgumentNullException>(
                () => objectInfo.GetVersionValue(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void GetVersionValue_ThrowsMappingException_IfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var exception = Assert.Throws<MappingException>(
                () => objectInfo.GetVersionValue(new CustomerWithNoVersion()));

            Assert.Equal(
                string.Format(ExceptionMessages.PocoObjectInfo_TypeMismatch, typeof(CustomerWithNoVersion).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        [Fact]
        public void GetVersionValueThrowsMicroLiteException_WhenNoVersionMapped()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new CustomerWithNoVersion();

            var objectInfo = ObjectInfo.For(typeof(CustomerWithNoVersion));

            var exception = Assert.Throws<MicroLiteException>(() => objectInfo.GetVersionValue(customer));

            Assert.Equal(ExceptionMessages.PocoObjectInfo_NoVersionColumn.FormatWith("Sales", "CustomerWithNoVersions"), exception.Message);
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
                Website = new Uri("http://microliteorm.wordpress.com"),
                Version = 233
            };

            var values = objectInfo.GetInsertValues(customer);

            Assert.Equal(8, values.Length);

            Assert.Equal(DbType.DateTime, values[0].DbType);
            Assert.Equal(customer.Created, values[0].Value);

            Assert.Equal(DbType.Decimal, values[1].DbType);
            Assert.Equal(customer.CreditLimit, values[1].Value);

            Assert.Equal(DbType.DateTime, values[2].DbType);
            Assert.Equal(customer.DateOfBirth, values[2].Value);

            Assert.Equal(DbType.Int32, values[3].DbType);
            Assert.Equal(customer.Id, values[3].Value); // Id should be included as it's assigned.

            Assert.Equal(DbType.String, values[4].DbType);
            Assert.Equal(customer.Name, values[4].Value);

            Assert.Equal(DbType.Int32, values[5].DbType);
            Assert.Equal((int)customer.Status, values[5].Value);

            Assert.Equal(DbType.Int32, values[6].DbType);
            Assert.Equal(customer.Version, values[6].Value);

            Assert.Equal(DbType.String, values[7].DbType);
            Assert.Equal(customer.Website.ToString(), values[7].Value);
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
                Website = new Uri("http://microliteorm.wordpress.com"),
                Version = 1
            };

            var values = objectInfo.GetInsertValues(customer);

            Assert.Equal(7, values.Length);

            Assert.Equal(DbType.DateTime, values[0].DbType);
            Assert.Equal(customer.Created, values[0].Value);

            Assert.Equal(DbType.Decimal, values[1].DbType);
            Assert.Equal(customer.CreditLimit, values[1].Value);

            Assert.Equal(DbType.DateTime, values[2].DbType);
            Assert.Equal(customer.DateOfBirth, values[2].Value);

            Assert.Equal(DbType.String, values[3].DbType);
            Assert.Equal(customer.Name, values[3].Value);

            Assert.Equal(DbType.Int32, values[4].DbType);
            Assert.Equal((int)customer.Status, values[4].Value);

            Assert.Equal(DbType.Int32, values[5].DbType);
            Assert.Equal(customer.Version, values[5].Value);

            Assert.Equal(DbType.String, values[6].DbType);
            Assert.Equal(customer.Website.ToString(), values[6].Value);
        }

        [Fact]
        public void GetInsertValues_ThrowsMappingException_IfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var exception = Assert.Throws<MappingException>(
                () => objectInfo.GetInsertValues(new CustomerWithGuidIdentifier()));

            Assert.Equal(
                string.Format(ExceptionMessages.PocoObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
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

            Assert.Equal(ExceptionMessages.PocoObjectInfo_NoIdentifierColumn.FormatWith("Sales", "CustomerWithNoIdentifiers"), exception.Message);
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
                Website = new Uri("http://microliteorm.wordpress.com"),
                Version = 233
            };

            var values = objectInfo.GetUpdateValues(customer);

            Assert.Equal(9, values.Length);

            Assert.Equal(DbType.Decimal, values[0].DbType);
            Assert.Equal(customer.CreditLimit, values[0].Value);

            Assert.Equal(DbType.DateTime, values[1].DbType);
            Assert.Equal(customer.DateOfBirth, values[1].Value);

            Assert.Equal(DbType.String, values[2].DbType);
            Assert.Equal(customer.Name, values[2].Value);

            Assert.Equal(DbType.Int32, values[3].DbType);
            Assert.Equal((int)customer.Status, values[3].Value);

            Assert.Equal(DbType.DateTime, values[4].DbType);
            Assert.Equal(customer.Updated, values[4].Value);

            Assert.Equal(DbType.Int32, values[5].DbType);
            Assert.Equal(customer.Version + 1, values[5].Value);

            Assert.Equal(DbType.String, values[6].DbType);
            Assert.Equal(customer.Website.ToString(), values[6].Value);

            Assert.Equal(DbType.Int32, values[7].DbType);
            Assert.Equal(customer.Version, values[7].Value);

            Assert.Equal(DbType.Int32, values[8].DbType);
            Assert.Equal(customer.Id, values[8].Value);
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
                Website = new Uri("http://microliteorm.wordpress.com"),
                Version = 233
            };

            var values = objectInfo.GetUpdateValues(customer);

            Assert.Equal(9, values.Length);

            Assert.Equal(DbType.Decimal, values[0].DbType);
            Assert.Equal(customer.CreditLimit, values[0].Value);

            Assert.Equal(DbType.DateTime, values[1].DbType);
            Assert.Equal(customer.DateOfBirth, values[1].Value);

            Assert.Equal(DbType.String, values[2].DbType);
            Assert.Equal(customer.Name, values[2].Value);

            Assert.Equal(DbType.Int32, values[3].DbType);
            Assert.Equal((int)customer.Status, values[3].Value);

            Assert.Equal(DbType.DateTime, values[4].DbType);
            Assert.Equal(customer.Updated, values[4].Value);

            Assert.Equal(DbType.Int32, values[5].DbType);
            Assert.Equal(customer.Version + 1, values[5].Value);

            Assert.Equal(DbType.String, values[6].DbType);
            Assert.Equal(customer.Website.ToString(), values[6].Value);

            Assert.Equal(DbType.Int32, values[7].DbType);
            Assert.Equal(customer.Version, values[7].Value);

            Assert.Equal(DbType.Int32, values[8].DbType);
            Assert.Equal(customer.Id, values[8].Value);
        }

        [Fact]
        public void GetUpdateValues_ThrowsMappingException_IfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var exception = Assert.Throws<MappingException>(
                () => objectInfo.GetUpdateValues(new CustomerWithGuidIdentifier()));

            Assert.Equal(
                string.Format(ExceptionMessages.PocoObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
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

            Assert.Equal(ExceptionMessages.PocoObjectInfo_NoIdentifierColumn.FormatWith("Sales", "CustomerWithNoIdentifiers"), exception.Message);
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
                string.Format(ExceptionMessages.PocoObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
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

            Assert.Equal(ExceptionMessages.PocoObjectInfo_NoIdentifierColumn.FormatWith("Sales", "CustomerWithNoIdentifiers"), exception.Message);
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
        public void SetIdentifierValue_ThrowsArgumentNullException_IfInstanceIsNull()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var exception = Assert.Throws<ArgumentNullException>(
                () => objectInfo.SetIdentifierValue(null, 122323));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void SetIdentifierValue_ThrowsNullReferenceException_IfIdentifierIsNull()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            Assert.Throws<NullReferenceException>(
                () => objectInfo.SetIdentifierValue(new Customer(), null));
        }

        [Fact]
        public void SetVersionValue_SetsPropertyValue()
            var customer = new Customer();

            objectInfo.SetVersionValue(customer, 233);

            Assert.Equal(233, customer.Version);
        }

        [Fact]
        public void SetVersionValue_ThrowsArgumentNullException_IfInstanceIsNull()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var exception = Assert.Throws<ArgumentNullException>(
                () => objectInfo.SetVersionValue(null, 233));

        [Fact]
        public void SetIdentifierValue_ThrowsMappingException_IfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var exception = Assert.Throws<MappingException>(
                () => objectInfo.SetIdentifierValue(new CustomerWithGuidIdentifier(), 122323));
            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void SetVersionValue_ThrowsArgumentNullException_IfVersionIsNull()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            Assert.Equal(
                string.Format(ExceptionMessages.PocoObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        [Fact]
        public void SetIdentifierValueThrowsMicroLiteException_WhenNoIdentifierMapped()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));
            var exception = Assert.Throws<ArgumentNullException>(
                () => objectInfo.SetVersionValue(new Customer(), null));

            Assert.Equal("version", exception.ParamName);
        }

        [Fact]
        public void SetVersionValue_ThrowsMappingException_IfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(Customer));

            var customer = new CustomerWithNoIdentifier();

            var objectInfo = ObjectInfo.For(typeof(CustomerWithNoIdentifier));

            var exception = Assert.Throws<MicroLiteException>(() => objectInfo.SetIdentifierValue(customer, 122323));

            Assert.Equal(ExceptionMessages.PocoObjectInfo_NoIdentifierColumn.FormatWith("Sales", "CustomerWithNoIdentifiers"), exception.Message);
        }

        [Fact]
            var exception = Assert.Throws<MappingException>(
                () => objectInfo.SetVersionValue(new CustomerWithNoVersion(), 233));

            Assert.Equal(
                string.Format(ExceptionMessages.PocoObjectInfo_TypeMismatch, typeof(CustomerWithNoVersion).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        [Fact]
        public void SetVersionValueThrowsMicroLiteException_WhenNoVersionMapped()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new CustomerWithNoVersion();

            var objectInfo = ObjectInfo.For(typeof(CustomerWithNoVersion));

            var exception = Assert.Throws<MicroLiteException>(() => objectInfo.SetVersionValue(customer, 233));

            Assert.Equal(ExceptionMessages.PocoObjectInfo_NoVersionColumn.FormatWith("Sales", "CustomerWithNoVersions"), exception.Message);
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
        public void VerifyInstanceForInsertDoesNotThrowMicroLiteException_WhenIdentifierStrategySequence_AndIdentifierNotSet()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Sequence));

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

            Assert.Equal(ExceptionMessages.PocoObjectInfo_IdentifierNotSetForInsert, exception.Message);
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

            Assert.Equal(ExceptionMessages.PocoObjectInfo_IdentifierSetForInsert, exception.Message);
        }

        [Fact]
        public void VerifyInstanceForInsertThrowsMicroLiteException_WhenIdentifierStrategySequence_AndIdentifierSet()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Sequence));

            var customer = new Customer
            {
                Id = 147843
            };

            var objectInfo = ObjectInfo.For(typeof(Customer));

            var exception = Assert.Throws<MicroLiteException>(() => objectInfo.VerifyInstanceForInsert(customer));

            Assert.Equal(ExceptionMessages.PocoObjectInfo_IdentifierSetForInsert, exception.Message);
        }

        [Fact]
        public void VerifyInstanceForInsertThrowsMicroLiteException_WhenNoIdentifierMapped()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new CustomerWithNoIdentifier();

            var objectInfo = ObjectInfo.For(typeof(CustomerWithNoIdentifier));

            var exception = Assert.Throws<MicroLiteException>(() => objectInfo.VerifyInstanceForInsert(customer));

            Assert.Equal(ExceptionMessages.PocoObjectInfo_NoIdentifierColumn.FormatWith("Sales", "CustomerWithNoIdentifiers"), exception.Message);
        }

        public class CustomerWithGuidIdentifier
        {
            public Guid Id
            {
                get;
                set;
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

        public class CustomerWithNoVersion
        {
        }
    }
}