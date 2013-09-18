namespace MicroLite.Tests.Integration.Delete
{
    using Xunit;

    public class WhenDeletingAnUnknownIdentifier : IntegrationTest
    {
        private readonly bool deleted;

        public WhenDeletingAnUnknownIdentifier()
        {
            this.deleted = this.Session.Advanced.Delete(typeof(Customer), 1);
        }

        [Fact]
        public void DeleteShouldReturnFalse()
        {
            Assert.False(this.deleted);
        }
    }
}