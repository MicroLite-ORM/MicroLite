namespace MicroLite.Tests.Core
{
    using System;
    using MicroLite.Listeners;
    using NUnit.Framework;

    /// <summary>
    /// Unit tests for the <see cref="IdentityListener"/> class.
    /// </summary>
    [TestFixture]
    public class IdentityListenerTests
    {
        [Test]
        public void AfterInsertSetsIdentifierValue()
        {
            var customer = new Customer();
            decimal scalarResult = 4354;

            var listener = new IdentityListener();
            listener.AfterInsert(customer, scalarResult);

            Assert.AreEqual(Convert.ToInt32(scalarResult), customer.Id);
        }

        [Test]
        public void BeforeInsertAppendsSelectScopeIdentityToCommandText()
        {
            var sqlQuery = new SqlQuery(string.Empty);

            var listener = new IdentityListener();
            listener.BeforeInsert(new Customer(), sqlQuery);

            Assert.AreEqual(";SELECT SCOPE_IDENTITY()", sqlQuery.CommandText);
            CollectionAssert.IsEmpty(sqlQuery.Arguments);
        }

        [Test]
        public void BeforeInsertDoesNotChangeCommandText()
        {
            var sqlQuery = new SqlQuery(string.Empty);

            var listener = new IdentityListener();
            listener.BeforeInsert(new CustomerWithAssigned(), sqlQuery);

            Assert.AreEqual(string.Empty, sqlQuery.CommandText);
            CollectionAssert.IsEmpty(sqlQuery.Arguments);
        }

        [Test]
        public void BeforeInsertDoesNotThrowIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = 0
            };

            var listener = new IdentityListener();

            listener.BeforeInsert(customer);
        }

        [Test]
        public void BeforeInsertThrowsMicroLiteExceptionIfIdentifierAlreadySet()
        {
            var customer = new Customer
            {
                Id = 1242534
            };

            var listener = new IdentityListener();

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

            var listener = new IdentityListener();

            listener.BeforeUpdate(customer);
        }

        [Test]
        public void BeforeUpdateThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = 0
            };

            var listener = new IdentityListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeUpdate(customer));

            Assert.AreEqual(Messages.IListener_IdentifierNotSetForUpdate, exception.Message);
        }

        [MicroLite.Mapping.Table("Sales", "Customers")]
        private class Customer
        {
            [MicroLite.Mapping.Column("CustomerId")]
            [MicroLite.Mapping.Identifier(MicroLite.Mapping.IdentifierStrategy.Identity)]
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