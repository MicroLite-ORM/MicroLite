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
        public void BeforeInsertAppendsSelectScopeIdentityToCommandText()
        {
            var sqlQuery = new SqlQuery(string.Empty);

            var listener = new AutoIncrementListener();
            listener.BeforeInsert(new Customer(), sqlQuery);

            Assert.AreEqual(";SELECT last_insert_rowid()", sqlQuery.CommandText);
            CollectionAssert.IsEmpty(sqlQuery.Arguments);
        }

        [Test]
        public void BeforeInsertDoesNotChangeCommandText()
        {
            var sqlQuery = new SqlQuery(string.Empty);

            var listener = new AutoIncrementListener();
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

            var listener = new AutoIncrementListener();

            listener.BeforeInsert(customer);
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