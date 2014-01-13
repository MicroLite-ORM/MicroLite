namespace MicroLite.Tests.Integration.Update
{
    using System;
    using Xunit;

    public class WhenUpdatingAnExistingInstance : IntegrationTest
    {
        private readonly Customer customer;

        public WhenUpdatingAnExistingInstance()
        {
            using (var transaction = this.Session.BeginTransaction())
            {
                this.Session.Insert(new Customer
                {
                    DateOfBirth = DateTime.Today,
                    EmailAddress = "joe.bloggs@email.com",
                    Forename = "Joe",
                    Status = CustomerStatus.Active,
                    Surname = "Bloggs"
                });

                transaction.Commit();
            }

            using (var transaction = this.Session.BeginTransaction())
            {
                this.Session.Update(new Customer
                {
                    CustomerId = 1,
                    DateOfBirth = new DateTime(2000, 6, 20),
                    EmailAddress = "john.smith@email.com",
                    Forename = "John",
                    Status = CustomerStatus.Suspended,
                    Surname = "Smith"
                });

                transaction.Commit();
            }

            using (var transaction = this.Session.BeginTransaction())
            {
                this.customer = this.Session.Single<Customer>(1);

                transaction.Commit();
            }
        }

        [Fact]
        public void TheUpdatedValuesShouldBeReturned()
        {
            Assert.Equal(new DateTime(2000, 6, 20), this.customer.DateOfBirth);
            Assert.Equal("john.smith@email.com", this.customer.EmailAddress);
            Assert.Equal("John", this.customer.Forename);
            Assert.Equal(CustomerStatus.Suspended, this.customer.Status);
            Assert.Equal("Smith", this.customer.Surname);
        }
    }
}