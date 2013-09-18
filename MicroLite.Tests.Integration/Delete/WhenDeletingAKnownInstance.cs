namespace MicroLite.Tests.Integration.Delete
{
    using System;
    using Xunit;

    public class WhenDeletingAKnownInstance : IntegrationTest
    {
        private readonly bool deleted;

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

            this.Session.Insert(customer);

            this.deleted = this.Session.Delete(customer);
        }

        [Fact]
        public void DeleteShouldReturnTrue()
        {
            Assert.True(this.deleted);
        }
    }
}