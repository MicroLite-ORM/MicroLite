namespace MicroLite.Tests.Core
{
    using System;
    using MicroLite.Core;
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
            listener.BeforeInsert(typeof(Customer), sqlQuery);

            Assert.AreEqual(";SELECT SCOPE_IDENTITY()", sqlQuery.CommandText);
            CollectionAssert.IsEmpty(sqlQuery.Parameters);
        }

        [Test]
        public void BeforeInsertDoesNotChangeCommandText()
        {
            var sqlQuery = new SqlQuery(string.Empty);

            var listener = new IdentityListener();
            listener.BeforeInsert(typeof(CustomerWithAssigned), sqlQuery);

            Assert.AreEqual(string.Empty, sqlQuery.CommandText);
            CollectionAssert.IsEmpty(sqlQuery.Parameters);
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

            Assert.AreEqual(Messages.IdentifierAlreadySet, exception.Message);
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
        public void BeforeUpdateThrowsMicroLiteExceptionIfIdentifierAlreadySet()
        {
            var customer = new Customer
            {
                Id = 0
            };

            var listener = new IdentityListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeUpdate(customer));

            Assert.AreEqual(Messages.IdentifierNotSet, exception.Message);
        }

        private class Customer
        {
            [MicroLite.Identifier(IdentifierStrategy.DbGenerated)]
            public int Id
            {
                get;
                set;
            }
        }

        private class CustomerWithAssigned
        {
            [MicroLite.Identifier(IdentifierStrategy.Assigned)]
            public int Id
            {
                get;
                set;
            }
        }
    }
}