namespace MicroLite.Tests.Listeners
{
    using System;
    using MicroLite.Listeners;
    using MicroLite.Mapping;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="GuidCombListener"/> class.
    /// </summary>
    public class GuidCombListenerTests : IDisposable
    {
        public GuidCombListenerTests()
        {
            // The tests in this suite all use attribute mapping for the test.
            ObjectInfo.MappingConvention = new AttributeMappingConvention();
        }

        [Fact]
        public void BeforeDeleteDoesNotThrowIfIdentifierSet()
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid()
            };

            var listener = new GuidCombListener();

            listener.BeforeDelete(customer);
        }

        [Fact]
        public void BeforeDeleteThrowsArgumentNullExceptionForNullInstance()
        {
            var listener = new GuidCombListener();

            var exception = Assert.Throws<ArgumentNullException>(() => listener.BeforeDelete(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void BeforeDeleteThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = Guid.Empty
            };

            var listener = new GuidCombListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeDelete(customer));

            Assert.Equal(Messages.IListener_IdentifierNotSetForDelete, exception.Message);
        }

        [Fact]
        public void BeforeInsertSetsIdentifierValueToNewGuidIfIdIsEmptyGuid()
        {
            var customer = new Customer
            {
                Id = Guid.Empty
            };

            var listener = new GuidCombListener();

            listener.BeforeInsert(customer);

            Assert.NotEqual(Guid.Empty, customer.Id);
        }

        [Fact]
        public void BeforeInsertThrowsArgumentNullExceptionForNullInstance()
        {
            var listener = new GuidCombListener();

            var exception = Assert.Throws<ArgumentNullException>(() => listener.BeforeInsert(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void BeforeInsertThrowsMicroLiteExceptionIfIdentifierSet()
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid()
            };

            var listener = new GuidCombListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeInsert(customer));

            Assert.Equal(Messages.IListener_IdentifierSetForInsert, exception.Message);
        }

        [Fact]
        public void BeforeUpdateDoesNotThrowIfIdentifierSet()
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid()
            };

            var listener = new GuidCombListener();

            listener.BeforeUpdate(customer);
        }

        [Fact]
        public void BeforeUpdateThrowsArgumentNullExceptionForNullInstance()
        {
            var listener = new GuidCombListener();

            var exception = Assert.Throws<ArgumentNullException>(() => listener.BeforeUpdate(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void BeforeUpdateThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = Guid.Empty
            };

            var listener = new GuidCombListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeUpdate(customer));

            Assert.Equal(Messages.IListener_IdentifierNotSetForUpdate, exception.Message);
        }

        public void Dispose()
        {
            // Reset the mapping convention after tests have run.
            ObjectInfo.MappingConvention = new ConventionMappingConvention(ConventionMappingSettings.Default);
        }

        [MicroLite.Mapping.Table("Sales", "Customers")]
        private class Customer
        {
            [MicroLite.Mapping.Column("CustomerId")]
            [MicroLite.Mapping.Identifier(MicroLite.Mapping.IdentifierStrategy.GuidComb)]
            public Guid Id
            {
                get;
                set;
            }
        }
    }
}