namespace MicroLite.Tests.Core
{
    using System;
    using MicroLite.Listeners;
    using MicroLite.Mapping;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="AssignedListener"/> class.
    /// </summary>
    public class AssignedListenerTests : IDisposable
    {
        public AssignedListenerTests()
        {
            // The tests in this suite all use attribute mapping for the test.
            ObjectInfo.MappingConvention = new AttributeMappingConvention();
        }

        [Fact]
        public void BeforeDeleteDoesNotThrowIfIdentifierSet()
        {
            var customer = new Customer
            {
                Id = 1242534
            };

            var listener = new AssignedListener();

            listener.BeforeDelete(customer);
        }

        [Fact]
        public void BeforeDeleteThrowsArgumentNullExceptionForNullInstance()
        {
            var listener = new AssignedListener();

            var exception = Assert.Throws<ArgumentNullException>(() => listener.BeforeDelete(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void BeforeDeleteThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = 0
            };

            var listener = new AssignedListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeDelete(customer));

            Assert.Equal(Messages.IListener_IdentifierNotSetForDelete, exception.Message);
        }

        [Fact]
        public void BeforeInsertDoesNotThrowIfIdentifierSet()
        {
            var customer = new Customer
            {
                Id = 1234
            };

            var listener = new AssignedListener();

            listener.BeforeInsert(customer);
        }

        [Fact]
        public void BeforeInsertThrowsArgumentNullExceptionForNullInstance()
        {
            var listener = new AssignedListener();

            var exception = Assert.Throws<ArgumentNullException>(() => listener.BeforeInsert(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void BeforeInsertThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = 0
            };

            var listener = new AssignedListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeInsert(customer));

            Assert.Equal(Messages.AssignedListener_IdentifierNotSetForInsert, exception.Message);
        }

        [Fact]
        public void BeforeUpdateDoesNotThrowIfIdentifierSet()
        {
            var customer = new Customer
            {
                Id = 1242534
            };

            var listener = new AssignedListener();

            listener.BeforeUpdate(customer);
        }

        [Fact]
        public void BeforeUpdateThrowsArgumentNullExceptionForNullInstance()
        {
            var listener = new AssignedListener();

            var exception = Assert.Throws<ArgumentNullException>(() => listener.BeforeUpdate(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void BeforeUpdateThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = 0
            };

            var listener = new AssignedListener();

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
            [MicroLite.Mapping.Identifier(MicroLite.Mapping.IdentifierStrategy.Assigned)]
            public int Id
            {
                get;
                set;
            }
        }
    }
}