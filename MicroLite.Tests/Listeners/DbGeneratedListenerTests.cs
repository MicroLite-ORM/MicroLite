namespace MicroLite.Tests.Core
{
    using System;
    using MicroLite.Listeners;
    using NUnit.Framework;

    /// <summary>
    /// Unit tests for the <see cref="DbGeneratedListener"/> class.
    /// </summary>
    [TestFixture]
    public class DbGeneratedListenerTests
    {
        [Test]
        public void AfterInsertSetsIdentifierValue()
        {
            var customer = new Customer();
            decimal scalarResult = 4354;

            var listener = new DbGeneratedListener();
            listener.AfterInsert(customer, scalarResult);

            Assert.AreEqual(Convert.ToInt32(scalarResult), customer.Id);
        }

        [Test]
        public void BeforeInsertAppendsSelectScopeIdentityToCommandText()
        {
            var sqlQuery = new SqlQuery(string.Empty);

            var listener = new DbGeneratedListener();
            listener.BeforeInsert(typeof(Customer), sqlQuery);

            Assert.AreEqual(";SELECT SCOPE_IDENTITY()", sqlQuery.CommandText);
            CollectionAssert.IsEmpty(sqlQuery.Arguments);
        }

        [Test]
        public void BeforeInsertDoesNotChangeCommandText()
        {
            var sqlQuery = new SqlQuery(string.Empty);

            var listener = new DbGeneratedListener();
            listener.BeforeInsert(typeof(CustomerWithAssigned), sqlQuery);

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

            var listener = new DbGeneratedListener();

            listener.BeforeInsert(customer);
        }

        [Test]
        public void BeforeInsertThrowsMicroLiteExceptionIfIdentifierAlreadySet()
        {
            var customer = new Customer
            {
                Id = 1242534
            };

            var listener = new DbGeneratedListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeInsert(customer));

            Assert.AreEqual(Messages.DbGenerated_IdentifierSetForInsert, exception.Message);
        }

        [Test]
        public void BeforeUpdateDoesNotThrowIfIdentifierSet()
        {
            var customer = new Customer
            {
                Id = 1242534
            };

            var listener = new DbGeneratedListener();

            listener.BeforeUpdate(customer);
        }

        [Test]
        public void BeforeUpdateThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = 0
            };

            var listener = new DbGeneratedListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeUpdate(customer));

            Assert.AreEqual(Messages.DbGenerated_IdentifierNotSetForUpdate, exception.Message);
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