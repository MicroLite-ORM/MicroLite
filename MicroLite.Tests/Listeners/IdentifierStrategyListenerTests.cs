namespace MicroLite.Tests.Listeners
{
    using System;
    using MicroLite.Listeners;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Xunit;

    /// <summary>
    /// Unit tests for the <see cref="IdentifierStrategyListener"/> class.
    /// </summary>
    public class IdentifierStrategyListenerTests : UnitTest
    {
        [Fact]
        public void AfterInsertDoesNotSetIdentifierIfExecuteScalarResultIsNullAndIdentifierDbGenerated()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var listener = new IdentifierStrategyListener();
            var customer = new Customer();

            Assert.DoesNotThrow(() => listener.AfterInsert(customer, null));

            Assert.Equal(0, customer.Id);
        }

        [Fact]
        public void AfterInsertDoesNotSetIdentifierIfExecuteScalarResultIsNullAndIdentifierStrategySequence()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Sequence));

            var listener = new IdentifierStrategyListener();
            var customer = new Customer();

            Assert.DoesNotThrow(() => listener.AfterInsert(customer, null));

            Assert.Equal(0, customer.Id);
        }

        [Fact]
        public void AfterInsertDoesNotThrowArgumentNullExceptionForNullExecuteScalarResultIfIdentifierStrategyAssigned()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Assigned));

            var listener = new IdentifierStrategyListener();

            Assert.DoesNotThrow(() => listener.AfterInsert(new Customer(), null));
        }

        [Fact]
        public void AfterInsertSetsIdentifierValueConvertingItToThePropertyType()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer();
            decimal scalarResult = 4354;

            var listener = new IdentifierStrategyListener();
            listener.AfterInsert(customer, scalarResult);

            Assert.Equal(Convert.ToInt32(scalarResult), customer.Id);
        }

        [Fact]
        public void AfterInsertSetsIdentifierValueForIdentifierStrategyDbGenerated()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));

            var customer = new Customer();
            int scalarResult = 4354;

            var listener = new IdentifierStrategyListener();
            listener.AfterInsert(customer, scalarResult);

            Assert.Equal(scalarResult, customer.Id);
        }

        [Fact]
        public void AfterInsertSetsIdentifierValueForIdentifierStrategySequence()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.Sequence));

            var customer = new Customer();
            int scalarResult = 4354;

            var listener = new IdentifierStrategyListener();
            listener.AfterInsert(customer, scalarResult);

            Assert.Equal(scalarResult, customer.Id);
        }

        [Fact]
        public void AfterInsertThrowsArgumentNullExceptionForNullInstance()
        {
            var listener = new IdentifierStrategyListener();

            var exception = Assert.Throws<ArgumentNullException>(() => listener.AfterInsert(null, 1));

            Assert.Equal("instance", exception.ParamName);
        }
    }
}