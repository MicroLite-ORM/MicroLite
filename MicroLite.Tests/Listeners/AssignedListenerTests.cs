namespace MicroLite.Tests.Listeners
{
    using System;
    using MicroLite.Listeners;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="AssignedListener"/> class.
    /// </summary>
    public class AssignedListenerTests : UnitTest
    {
        [Fact]
        public void BeforeDeleteDoesNotThrowIfIdentifierSet()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var customer = new Customer
            {
                Id = 1242534
            };

            var listener = new AssignedListener();

            listener.BeforeDelete(customer);
        }

        [Fact]
        public void BeforeDeleteDoesNotThrowMicroLiteExceptionIfIdentifierNotSetAndIdentifierStrategyIsDbGenerated()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer
            {
                Id = 0
            };

            var listener = new AssignedListener();

            Assert.DoesNotThrow(() => listener.BeforeDelete(customer));
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
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

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
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

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
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

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
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var customer = new Customer
            {
                Id = 1242534
            };

            var listener = new AssignedListener();

            listener.BeforeUpdate(customer);
        }

        [Fact]
        public void BeforeUpdateDoesNotThrowMicroLiteExceptionIfIdentifierNotSetAndIdentifierStrategyIsDbGenerated()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer
            {
                Id = 0
            };

            var listener = new AssignedListener();

            Assert.DoesNotThrow(() => listener.BeforeUpdate(customer));
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
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var customer = new Customer
            {
                Id = 0
            };

            var listener = new AssignedListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeUpdate(customer));

            Assert.Equal(Messages.IListener_IdentifierNotSetForUpdate, exception.Message);
        }
    }
}