namespace MicroLite.Tests.Integration.Delete
{
    using System;
    using Xunit;

    public class WhenDeletingAKnownInstance : IntegrationTest
    {
        private readonly Customer customer;

        public WhenDeletingAKnownInstance()
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

            this.customer = customer;
        }

        [Fact]
        public void DeleteShouldReturnTrue()
        {
            bool deleted;

            using (var transaction = this.Session.BeginTransaction())
            {
                deleted = this.Session.Delete(this.customer);

                transaction.Commit();
            }

            Assert.True(deleted);
        }
    }
}