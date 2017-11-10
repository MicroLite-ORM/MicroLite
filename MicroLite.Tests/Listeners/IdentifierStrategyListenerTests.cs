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
            UnitTest.SetConventionMapping(IdentifierStrategy.DbGenerated);

            var listener = new IdentifierStrategyListener();
            var customer = new Customer();

            listener.AfterInsert(customer, null);

            Assert.Equal(0, customer.Id);
        }

        [Fact]
        public void AfterInsertDoesNotSetIdentifierIfExecuteScalarResultIsNullAndIdentifierStrategySequence()
        {
            UnitTest.SetConventionMapping(IdentifierStrategy.Sequence);

            var listener = new IdentifierStrategyListener();
            var customer = new Customer();

            listener.AfterInsert(customer, null);

            Assert.Equal(0, customer.Id);
        }

        [Fact]
        public void AfterInsertDoesNotThrowArgumentNullExceptionForNullExecuteScalarResultIfIdentifierStrategyAssigned()
        {
            UnitTest.SetConventionMapping(IdentifierStrategy.Assigned);

            var listener = new IdentifierStrategyListener();

            listener.AfterInsert(new Customer(), null);
        }

        [Fact]
        public void AfterInsertSetsIdentifierValueConvertingItToThePropertyType()
        {
            UnitTest.SetConventionMapping(IdentifierStrategy.DbGenerated);

            var customer = new Customer();
            decimal scalarResult = 4354;

            var listener = new IdentifierStrategyListener();
            listener.AfterInsert(customer, scalarResult);

            Assert.Equal(Convert.ToInt32(scalarResult), customer.Id);
        }

        [Fact]
        public void AfterInsertSetsIdentifierValueForIdentifierStrategyDbGenerated()
        {
            UnitTest.SetConventionMapping(IdentifierStrategy.DbGenerated);

            var customer = new Customer();
            int scalarResult = 4354;

            var listener = new IdentifierStrategyListener();
            listener.AfterInsert(customer, scalarResult);

            Assert.Equal(scalarResult, customer.Id);
        }

        [Fact]
        public void AfterInsertSetsIdentifierValueForIdentifierStrategySequence()
        {
            UnitTest.SetConventionMapping(IdentifierStrategy.Sequence);

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