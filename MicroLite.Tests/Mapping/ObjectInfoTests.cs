namespace MicroLite.Tests.Mapping
{
    using System;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="ObjectInfo"/> class.
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
        public void ConstructorThrowsArgumentNullExceptionForNullForTableInfo()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new ObjectInfo(typeof(CustomerWithIntegerIdentifier), null));

            Assert.Equal("tableInfo", exception.ParamName);
        }

        [Fact]
        public void ConstructorThrowsArgumentNullExceptionForNullForType()
        {
            var exception = Assert.Throws<ArgumentNullException>(
                () => new ObjectInfo(null, null));

            Assert.Equal("forType", exception.ParamName);
        }

        [Fact]
        public void CreateInstance()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var instance = objectInfo.CreateInstance();

            Assert.IsType<CustomerWithIntegerIdentifier>(instance);
        }

        [Fact]
        public void DefaultIdentifierValueIsSetCorrectlyForGuid()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithGuidIdentifier));

            Assert.Equal(Guid.Empty, objectInfo.DefaultIdentifierValue);
        }

        [Fact]
        public void DefaultIdentifierValueIsSetCorrectlyForInteger()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            Assert.Equal(0, objectInfo.DefaultIdentifierValue);
        }

        [Fact]
        public void DefaultIdentifierValueIsSetCorrectlyForString()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithStringIdentifier));

            Assert.Null(objectInfo.DefaultIdentifierValue);
        }

        public void Dispose()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(ConventionMappingSettings.Default);
        }

        [Fact]
        public void ForReturnsExpandoObjectInfoForTypeOfDynamic()
        {
            var objectInfo = ObjectInfoHelper<dynamic>();

            Assert.IsType<ExpandoObjectInfo>(objectInfo);
        }

        [Fact]
        public void ForReturnsSameObjectInfoForSameType()
        {
            var forType = typeof(CustomerWithIntegerIdentifier);

            var objectInfo1 = ObjectInfo.For(forType);
            var objectInfo2 = ObjectInfo.For(forType);

            Assert.Same(objectInfo1, objectInfo2);
        }

        [Fact]
        public void ForThrowsArgumentNullExceptonForNullForType()
        {
            var exception = Assert.Throws<ArgumentNullException>(() => ObjectInfo.For(null));

            Assert.Equal("forType", exception.ParamName);
        }

        [Fact]
        public void ForThrowsMicroLiteExceptionIfAbstractClass()
        {
            var exception = Assert.Throws<MicroLiteException>(
                () => ObjectInfo.For(typeof(AbstractCustomer)));

            Assert.Equal(
                Messages.ObjectInfo_TypeMustNotBeAbstract.FormatWith(typeof(AbstractCustomer).Name),
                exception.Message);
        }

        [Fact]
        public void ForThrowsMicroLiteExceptionIfNoDefaultConstructor()
        {
            var exception = Assert.Throws<MicroLiteException>(
                () => ObjectInfo.For(typeof(CustomerWithNoDefaultConstructor)));

            Assert.Equal(
                Messages.ObjectInfo_TypeMustHaveDefaultConstructor.FormatWith(typeof(CustomerWithNoDefaultConstructor).Name),
                exception.Message);
        }

        [Fact]
        public void ForThrowsMicroLiteExceptionIfNotClass()
        {
            var exception = Assert.Throws<MicroLiteException>(
                () => ObjectInfo.For(typeof(CustomerStruct)));

            Assert.Equal(
                Messages.ObjectInfo_TypeMustBeClass.FormatWith(typeof(CustomerStruct).Name),
                exception.Message);
        }

        [Fact]
        public void ForTypeReturnsTypePassedToConstructor()
        {
            var forType = typeof(CustomerWithIntegerIdentifier);

            var objectInfo = ObjectInfo.For(forType);

            Assert.Equal(forType, objectInfo.ForType);
        }

        [Fact]
        public void GetIdentifierValueReturnsPropertyValue()
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
        public void GetIdentifierValueThrowsArgumentNullExceptionForNullInstance()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<ArgumentNullException>(() => objectInfo.GetIdentifierValue(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void GetIdentifierValueThrowsMicroLiteExceptionIfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<MicroLiteException>(
                () => objectInfo.GetIdentifierValue(new CustomerWithGuidIdentifier()));

            Assert.Equal(
                string.Format(Messages.ObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        [Fact]
        public void GetPropertyValueForColumnConvertsPropertyValueToDbValue()
        {
            var instance = new CustomerWithIntegerIdentifier
            {
                Status = CustomerStatus.Active
            };

            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var value = objectInfo.GetPropertyValueForColumn(instance, "StatusId");

            Assert.Equal(1, value);
        }

        [Fact]
        public void GetPropertyValueForColumnThrowsArgumentNullExceptionForNullInstance()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<ArgumentNullException>(() => objectInfo.GetPropertyValueForColumn(null, "Name"));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void GetPropertyValueForColumnThrowsMicroLiteExceptionIfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<MicroLiteException>(
                () => objectInfo.GetPropertyValueForColumn(new CustomerWithGuidIdentifier(), "Name"));

            Assert.Equal(
                string.Format(Messages.ObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        [Fact]
        public void GetPropertyValueForColumnThrowsMicroLiteExceptionIfInvalidColumnName()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<MicroLiteException>(
                () => objectInfo.GetPropertyValueForColumn(new CustomerWithIntegerIdentifier(), "UnknownColumn"));

            Assert.Equal(
                string.Format(Messages.ObjectInfo_ColumnNotMapped, "UnknownColumn", objectInfo.ForType.Name),
                exception.Message);
        }

        [Fact]
        public void GetPropertyValueReturnsPropertyValue()
        {
            var instance = new CustomerWithIntegerIdentifier
            {
                Status = CustomerStatus.Active
            };

            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var value = objectInfo.GetPropertyValue(instance, "Status");

            Assert.Equal(CustomerStatus.Active, value);
        }

        [Fact]
        public void GetPropertyValueThrowsArgumentNullExceptionForNullInstance()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<ArgumentNullException>(() => objectInfo.GetPropertyValue(null, "Name"));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void GetPropertyValueThrowsMicroLiteExceptionIfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<MicroLiteException>(
                () => objectInfo.GetPropertyValue(new CustomerWithGuidIdentifier(), "Name"));

            Assert.Equal(
                string.Format(Messages.ObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        [Fact]
        public void GetPropertyValueThrowsMicroLiteExceptionIfInvalidPropertyName()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<MicroLiteException>(
                () => objectInfo.GetPropertyValue(new CustomerWithIntegerIdentifier(), "UnknownProperty"));

            Assert.Equal(
                string.Format(Messages.ObjectInfo_UnknownProperty, objectInfo.ForType.Name, "UnknownProperty"),
                exception.Message);
        }

        [Fact]
        public void HasDefaultIdentifierValueThrowsArgumentNullExceptionForNullInstance()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<ArgumentNullException>(() => objectInfo.HasDefaultIdentifierValue(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void HasDefaultIdentifierValueThrowsMicroLiteExceptionIfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var instance = new CustomerWithGuidIdentifier();

            var exception = Assert.Throws<MicroLiteException>(
                () => objectInfo.HasDefaultIdentifierValue(instance));

            Assert.Equal(
                string.Format(Messages.ObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        [Fact]
        public void HasDefaultIdentifierValueWhenIdentifierIsGuid()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithGuidIdentifier));

            var customer = new CustomerWithGuidIdentifier();

            customer.Id = Guid.Empty;
            Assert.True(objectInfo.HasDefaultIdentifierValue(customer));

            customer.Id = Guid.NewGuid();
            Assert.False(objectInfo.HasDefaultIdentifierValue(customer));
        }

        [Fact]
        public void HasDefaultIdentifierValueWhenIdentifierIsInteger()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var customer = new CustomerWithIntegerIdentifier();

            customer.Id = 0;
            Assert.True(objectInfo.HasDefaultIdentifierValue(customer));

            customer.Id = 123;
            Assert.False(objectInfo.HasDefaultIdentifierValue(customer));
        }

        [Fact]
        public void HasDefaultIdentifierValueWhenIdentifierIsString()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithStringIdentifier));

            var customer = new CustomerWithStringIdentifier();

            customer.Id = null;
            Assert.True(objectInfo.HasDefaultIdentifierValue(customer));

            customer.Id = "AFIK";
            Assert.False(objectInfo.HasDefaultIdentifierValue(customer));
        }

        [Fact]
        public void SetPropertyValueForColumnSetsPropertyValue()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var instance = new CustomerWithIntegerIdentifier();

            objectInfo.SetPropertyValueForColumn(instance, "StatusId", 1);

            Assert.Equal(CustomerStatus.Active, instance.Status);
        }

        [Fact]
        public void SetPropertyValueForColumnThrowsAnExceptionForUnknownColumn()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var instance = new CustomerWithIntegerIdentifier();

            var exception = Assert.Throws<MicroLiteException>(() => objectInfo.SetPropertyValueForColumn(instance, "UnknownColumn", 1));

            Assert.Equal(
                string.Format(Messages.ObjectInfo_UnknownColumn, objectInfo.ForType.Name, "UnknownColumn"),
                exception.Message);
        }

        [Fact]
        public void SetPropertyValueForColumnThrowsArgumentNullExceptionForNullInstance()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<ArgumentNullException>(() => objectInfo.SetPropertyValueForColumn(null, "StatusId", 1));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void SetPropertyValueForColumnThrowsMicroLiteExceptionIfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var instance = new CustomerWithGuidIdentifier();

            var exception = Assert.Throws<MicroLiteException>(
                () => objectInfo.SetPropertyValueForColumn(instance, "StatusId", 1));

            Assert.Equal(
                string.Format(Messages.ObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
                exception.Message);
        }

        [Fact]
        public void SetPropertyValueSetsPropertyValue()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var instance = new CustomerWithIntegerIdentifier();

            objectInfo.SetPropertyValue(instance, "Status", CustomerStatus.Active);

            Assert.Equal(CustomerStatus.Active, instance.Status);
        }

        [Fact]
        public void SetPropertyValueThrowsAnExceptionForUnknownProperty()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var instance = new CustomerWithIntegerIdentifier();

            var exception = Assert.Throws<MicroLiteException>(() => objectInfo.SetPropertyValue(instance, "UnknownProperty", CustomerStatus.Active));

            Assert.Equal(
                string.Format(Messages.ObjectInfo_UnknownProperty, objectInfo.ForType.Name, "UnknownProperty"),
                exception.Message);
        }

        [Fact]
        public void SetPropertyValueThrowsArgumentNullExceptionForNullInstance()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var exception = Assert.Throws<ArgumentNullException>(() => objectInfo.SetPropertyValue(null, "Status", CustomerStatus.Active));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void SetPropertyValueThrowsMicroLiteExceptionIfInstanceIsIncorrectType()
        {
            var objectInfo = ObjectInfo.For(typeof(CustomerWithIntegerIdentifier));

            var instance = new CustomerWithGuidIdentifier();

            var exception = Assert.Throws<MicroLiteException>(
                () => objectInfo.SetPropertyValue(instance, "Status", CustomerStatus.Active));

            Assert.Equal(
                string.Format(Messages.ObjectInfo_TypeMismatch, typeof(CustomerWithGuidIdentifier).Name, objectInfo.ForType.Name),
                exception.Message);
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