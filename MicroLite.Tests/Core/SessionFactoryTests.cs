namespace MicroLite.Tests.Core
{
    using System.Data.Common;
    using MicroLite.Core;
    using Moq;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="SessionFactory"/> class.
    /// </summary>
    [TestFixture]
    public class SessionFactoryTests
    {
        [Test]
        public void OpenSessionCreatesConnectionAndSetsConnectionString()
        {
            var connectionString = "Data Source=localhost;Initial Catalog=TestDB;";

            var mockConnection = new Mock<DbConnection>();
            mockConnection.SetupProperty(x => x.ConnectionString);

            var mockFactory = new Mock<DbProviderFactory>();
            mockFactory.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

            var sessionFactory = new SessionFactory(connectionString, mockFactory.Object);
            var session = sessionFactory.OpenSession();

            mockFactory.VerifyAll();
            mockConnection.VerifySet(x => x.ConnectionString = connectionString);
        }

        [Test]
        public void OpenSessionReturnsNewInstanceOnEachCall()
        {
            var mockConnection = new Mock<DbConnection>();
            mockConnection.SetupProperty(x => x.ConnectionString);

            var mockFactory = new Mock<DbProviderFactory>();
            mockFactory.Setup(x => x.CreateConnection()).Returns(mockConnection.Object);

            var sessionFactory = new SessionFactory("Data Source=localhost;", mockFactory.Object);

            var session1 = sessionFactory.OpenSession();
            var session2 = sessionFactory.OpenSession();

            Assert.AreNotSame(session1, session2);
        }
    }
}