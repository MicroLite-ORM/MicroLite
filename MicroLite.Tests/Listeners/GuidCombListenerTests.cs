namespace MicroLite.Tests.Listeners
{
    using System;
    using MicroLite.Listeners;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="GuidCombListener"/> class.
    /// </summary>
    [TestFixture]
    public class GuidCombListenerTests
    {
        [Test]
        public void BeforeDeleteDoesNotThrowIfIdentifierSet()
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid()
            };

            var listener = new GuidCombListener();

            listener.BeforeDelete(customer);
        }

        [Test]
        public void BeforeDeleteThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = Guid.Empty
            };

            var listener = new GuidCombListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeDelete(customer));

            Assert.AreEqual(Messages.IListener_IdentifierNotSetForDelete, exception.Message);
        }

        [Test]
        public void BeforeInsertSetsIdentifierValueToNewGuidIfIdIsEmptyGuid()
        {
            var customer = new Customer
            {
                Id = Guid.Empty
            };

            var listener = new GuidCombListener();

            listener.BeforeInsert(customer);

            Assert.AreNotEqual(Guid.Empty, customer.Id);
        }

        [Test]
        public void BeforeInsertThrowsMicroLiteExceptionIfIdentifierSet()
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid()
            };

            var listener = new GuidCombListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeInsert(customer));

            Assert.AreEqual(Messages.IListener_IdentifierSetForInsert, exception.Message);
        }

        [Test]
        public void BeforeUpdateDoesNotThrowIfIdentifierSet()
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid()
            };

            var listener = new GuidCombListener();

            listener.BeforeUpdate(customer);
        }

        [Test]
        public void BeforeUpdateThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = Guid.Empty
            };

            var listener = new GuidCombListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeUpdate(customer));

            Assert.AreEqual(Messages.IListener_IdentifierNotSetForUpdate, exception.Message);
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