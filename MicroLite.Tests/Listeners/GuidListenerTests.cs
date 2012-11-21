namespace MicroLite.Tests.Listeners
{
    using System;
    using MicroLite.Listeners;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="GuidListener"/> class.
    /// </summary>
    [TestFixture]
    public class GuidListenerTests
    {
        [Test]
        public void BeforeDeleteDoesNotThrowIfIdentifierSet()
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid()
            };

            var listener = new GuidListener();

            listener.BeforeDelete(customer);
        }

        [Test]
        public void BeforeDeleteThrowsArgumentNullExceptionForNullInstance()
        {
            var listener = new GuidListener();

            var exception = Assert.Throws<ArgumentNullException>(() => listener.BeforeDelete(null));

            Assert.AreEqual("instance", exception.ParamName);
        }

        [Test]
        public void BeforeDeleteThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = Guid.Empty
            };

            var listener = new GuidListener();

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

            var listener = new GuidListener();

            listener.BeforeInsert(customer);

            Assert.AreNotEqual(Guid.Empty, customer.Id);
        }

        [Test]
        public void BeforeInsertThrowsArgumentNullExceptionForNullInstance()
        {
            var listener = new GuidListener();

            var exception = Assert.Throws<ArgumentNullException>(() => listener.BeforeInsert(null));

            Assert.AreEqual("instance", exception.ParamName);
        }

        [Test]
        public void BeforeInsertThrowsMicroLiteExceptionIfIdentifierSet()
        {
            var customer = new Customer
            {
                Id = Guid.NewGuid()
            };

            var listener = new GuidListener();

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

            var listener = new GuidListener();

            listener.BeforeUpdate(customer);
        }

        [Test]
        public void BeforeUpdateThrowsArgumentNullExceptionForNullInstance()
        {
            var listener = new GuidListener();

            var exception = Assert.Throws<ArgumentNullException>(() => listener.BeforeUpdate(null));

            Assert.AreEqual("instance", exception.ParamName);
        }

        [Test]
        public void BeforeUpdateThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = Guid.Empty
            };

            var listener = new GuidListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeUpdate(customer));

            Assert.AreEqual(Messages.IListener_IdentifierNotSetForUpdate, exception.Message);
        }

        [MicroLite.Mapping.Table("Sales", "Customers")]
        private class Customer
        {
            [MicroLite.Mapping.Column("CustomerId")]
            [MicroLite.Mapping.Identifier(MicroLite.Mapping.IdentifierStrategy.Guid)]
            public Guid Id
            {
                get;
                set;
            }
        }
    }
}