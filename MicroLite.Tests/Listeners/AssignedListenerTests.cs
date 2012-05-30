namespace MicroLite.Tests.Core
{
    using MicroLite.Listeners;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="AssignedListener"/> class.
    /// </summary>
    [TestFixture]
    public class AssignedListenerTests
    {
        [Test]
        public void BeforeInsertDoesNotThrowIfIdentifierSet()
        {
            var customer = new Customer
            {
                Id = 1234
            };

            var listener = new AssignedListener();

            listener.BeforeInsert(customer);
        }

        [Test]
        public void BeforeInsertThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = 0
            };

            var listener = new AssignedListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeInsert(customer));

            Assert.AreEqual(Messages.Assigned_IdentifierNotSetForInsert, exception.Message);
        }

        [Test]
        public void BeforeUpdateDoesNotThrowIfIdentifierSet()
        {
            var customer = new Customer
            {
                Id = 1242534
            };

            var listener = new AssignedListener();

            listener.BeforeUpdate(customer);
        }

        [Test]
        public void BeforeUpdateThrowsMicroLiteExceptionIfIdentifierNotSet()
        {
            var customer = new Customer
            {
                Id = 0
            };

            var listener = new AssignedListener();

            var exception = Assert.Throws<MicroLiteException>(() => listener.BeforeUpdate(customer));

            Assert.AreEqual(Messages.Assigned_IdentifierNotSetForUpdate, exception.Message);
        }

        private class Customer
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