namespace MicroLite.Tests.Listeners
{
    using System;
    using MicroLite.Listeners;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="DbGeneratedListener"/> class.
    /// </summary>
    public class DbGeneratedListenerTests : UnitTest
    {
        [Fact]
        public void AfterInsertSetsIdentifierValue()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer();
            int scalarResult = 4354;

            var listener = new DbGeneratedListener();
            listener.AfterInsert(customer, scalarResult);

            Assert.Equal(scalarResult, customer.Id);
        }

        [Fact]
        public void AfterInsertSetsIdentifierValueConvertingItToThePropertyType()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer();
            decimal scalarResult = 4354;

            var listener = new DbGeneratedListener();
            listener.AfterInsert(customer, scalarResult);

            Assert.Equal(Convert.ToInt32(scalarResult), customer.Id);
        }

        [Fact]
        public void AfterInsertThrowsArgumentNullExceptionForNullExecuteScalarResult()
        {
            var listener = new DbGeneratedListener();

            var exception = Assert.Throws<ArgumentNullException>(() => listener.AfterInsert(new Customer(), null));

            Assert.Equal("executeScalarResult", exception.ParamName);
        }

        [Fact]
        public void AfterInsertThrowsArgumentNullExceptionForNullInstance()
        {
            var listener = new DbGeneratedListener();

            var exception = Assert.Throws<ArgumentNullException>(() => listener.AfterInsert(null, 1));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void BeforeDeleteDoesNotThrowIfIdentifierSet()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer
            {
                Id = 1242534
            };

            var listener = new DbGeneratedListener();

            listener.BeforeDelete(customer);
        }

        [Fact]
        public void BeforeDeleteDoesNotThrowMicroLiteExceptionIfIdentifierNotSetAndIdentifierStrategyIsAssigned()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var customer = new Customer
            {
                Id = 0
            };

            var listener = new DbGeneratedListener();

            Assert.DoesNotThrow(() => listener.BeforeDelete(customer));
        }

        [Fact]
        public void BeforeDeleteThrowsArgumentNullExceptionForNullInstance()
        {
            var listener = new DbGeneratedListener();

            var exception = Assert.Throws<ArgumentNullException>(() => listener.BeforeDelete(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void BeforeDeleteThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer
            {
                Id = 0
            };

            var listener = new DbGeneratedListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeDelete(customer));

            Assert.Equal(Messages.IListener_IdentifierNotSetForDelete, exception.Message);
        }

        [Fact]
        public void BeforeInsertDoesNotThrowIfIdentifierNotSet()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer
            {
                Id = 0
            };

            var listener = new DbGeneratedListener();

            listener.BeforeInsert(customer);
        }

        [Fact]
        public void BeforeInsertThrowsArgumentNullExceptionForNullInstance()
        {
            var listener = new DbGeneratedListener();

            var exception = Assert.Throws<ArgumentNullException>(() => listener.BeforeInsert(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void BeforeInsertThrowsMicroLiteExceptionIfIdentifierAlreadySet()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer
            {
                Id = 1242534
            };

            var listener = new DbGeneratedListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeInsert(customer));

            Assert.Equal(Messages.IListener_IdentifierSetForInsert, exception.Message);
        }

        [Fact]
        public void BeforeUpdateDoesNotThrowIfIdentifierSet()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer
            {
                Id = 1242534
            };

            var listener = new DbGeneratedListener();

            listener.BeforeUpdate(customer);
        }

        [Fact]
        public void BeforeUpdateDoesNotThrowMicroLiteExceptionIfIdentifierNotSetAndIdentifierStrategyIsAssigned()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var customer = new Customer
            {
                Id = 0
            };

            var listener = new DbGeneratedListener();

            Assert.DoesNotThrow(() => listener.BeforeUpdate(customer));
        }

        [Fact]
        public void BeforeUpdateThrowsArgumentNullExceptionForNullInstance()
        {
            var listener = new DbGeneratedListener();

            var exception = Assert.Throws<ArgumentNullException>(() => listener.BeforeUpdate(null));

            Assert.Equal("instance", exception.ParamName);
        }

        [Fact]
        public void BeforeUpdateThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer
            {
                Id = 0
            };

            var listener = new DbGeneratedListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeUpdate(customer));

            Assert.Equal(Messages.IListener_IdentifierNotSetForUpdate, exception.Message);
        }
    }
}