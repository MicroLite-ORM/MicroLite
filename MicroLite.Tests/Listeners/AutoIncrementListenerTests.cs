namespace MicroLite.Tests.Core
{
    using System;
    using MicroLite.Listeners;
    using NUnit.Framework;

    /// <summary>
    /// Unit tests for the <see cref="AutoIncrementListener"/> class.
    /// </summary>
    [TestFixture]
    public class AutoIncrementListenerTests
    {
        [Test]
        public void AfterInsertSetsIdentifierValue()
        {
            var customer = new Customer();
            decimal scalarResult = 4354;

            var listener = new AutoIncrementListener();
            listener.AfterInsert(customer, scalarResult);

            Assert.AreEqual(Convert.ToInt32(scalarResult), customer.Id);
        }

        [Test]
        public void AfterInsertThrowsArgumentNullExceptionForNullExecuteScalarResult()
        {
            var listener = new AutoIncrementListener();

            var exception = Assert.Throws<ArgumentNullException>(() => listener.AfterInsert(new Customer(), null));

            Assert.AreEqual("executeScalarResult", exception.ParamName);
        }

        [Test]
        public void AfterInsertThrowsArgumentNullExceptionForNullInstance()
        {
            var listener = new AutoIncrementListener();

            var exception = Assert.Throws<ArgumentNullException>(() => listener.AfterInsert(null, 1));

            Assert.AreEqual("instance", exception.ParamName);
        }

        [Test]
        public void BeforeDeleteDoesNotThrowIfIdentifierSet()
        {
            var customer = new Customer
            {
                Id = 1242534
            };

            var listener = new AutoIncrementListener();

            listener.BeforeDelete(customer);
        }

        [Test]
        public void BeforeDeleteThrowsArgumentNullExceptionForNullInstance()
        {
            var listener = new AutoIncrementListener();

            var exception = Assert.Throws<ArgumentNullException>(() => listener.BeforeDelete(null));

            Assert.AreEqual("instance", exception.ParamName);
        }

        [Test]
        public void BeforeDeleteThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = 0
            };

            var listener = new AutoIncrementListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeDelete(customer));

            Assert.AreEqual(Messages.IListener_IdentifierNotSetForDelete, exception.Message);
        }

        [Test]
        public void BeforeInsertDoesNotThrowIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = 0
            };

            var listener = new AutoIncrementListener();

            listener.BeforeInsert(customer);
        }

        [Test]
        public void BeforeInsertThrowsArgumentNullExceptionForNullInstance()
        {
            var listener = new AutoIncrementListener();

            var exception = Assert.Throws<ArgumentNullException>(() => listener.BeforeInsert(null));

            Assert.AreEqual("instance", exception.ParamName);
        }

        [Test]
        public void BeforeInsertThrowsMicroLiteExceptionIfIdentifierAlreadySet()
        {
            var customer = new Customer
            {
                Id = 1242534
            };

            var listener = new AutoIncrementListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeInsert(customer));

            Assert.AreEqual(Messages.IListener_IdentifierSetForInsert, exception.Message);
        }

        [Test]
        public void BeforeUpdateDoesNotThrowIfIdentifierSet()
        {
            var customer = new Customer
            {
                Id = 1242534
            };

            var listener = new AutoIncrementListener();

            listener.BeforeUpdate(customer);
        }

        [Test]
        public void BeforeUpdateThrowsArgumentNullExceptionForNullInstance()
        {
            var listener = new AutoIncrementListener();

            var exception = Assert.Throws<ArgumentNullException>(() => listener.BeforeUpdate(null));

            Assert.AreEqual("instance", exception.ParamName);
        }

        [Test]
        public void BeforeUpdateThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = 0
            };

            var listener = new AutoIncrementListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeUpdate(customer));

            Assert.AreEqual(Messages.IListener_IdentifierNotSetForUpdate, exception.Message);
        }

        [MicroLite.Mapping.Table("Sales", "Customers")]
        private class Customer
        {
            [MicroLite.Mapping.Column("CustomerId")]
            [MicroLite.Mapping.Identifier(MicroLite.Mapping.IdentifierStrategy.AutoIncrement)]
            public int Id
            {
                get;
                set;
            }
        }

        [MicroLite.Mapping.Table("Sales", "Customers")]
        private class CustomerWithAssigned
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