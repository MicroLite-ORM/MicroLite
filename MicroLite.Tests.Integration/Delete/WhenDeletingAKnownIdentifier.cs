namespace MicroLite.Tests.Integration.Delete
{
    using System;
    using Xunit;

    public class WhenDeletingAKnownIdentifier : IntegrationTest
    {
        private readonly int customerId;

        public WhenDeletingAKnownIdentifier()
        {
            var customer = new Customer
            {
                DateOfBirth = DateTime.Today,
                EmailAddress = "joe.bloggs@email.com",
                Forename = "Joe",
                Surname = "Bloggs",
                Status = CustomerStatus.Active
            };

            using (var transaction = this.Session.BeginTransaction())
            {
                this.Session.Insert(customer);

                transaction.Commit();
            }

            this.customerId = customer.CustomerId;
        }

        [Fact]
        public void DeleteShouldReturnTrue()
        {
            bool deleted;

            using (var transaction = this.Session.BeginTransaction())
            {
                deleted = this.Session.Advanced.Delete(typeof(Customer), customerId);

                transaction.Commit();
            }

            Assert.True(deleted);
        }
    }
}