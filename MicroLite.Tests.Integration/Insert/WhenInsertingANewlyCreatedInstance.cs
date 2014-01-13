namespace MicroLite.Tests.Integration.Insert
{
    using System;
    using Xunit;

    public class WhenInsertingANewlyCreatedInstance : IntegrationTest
    {
        private readonly Customer customer;

        public WhenInsertingANewlyCreatedInstance()
        {
            this.customer = new Customer
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
        }

        [Fact]
        public void TheIdentifierShouldBeSet()
        {
            Assert.NotEqual(0, this.customer.CustomerId);
        }
    }
}