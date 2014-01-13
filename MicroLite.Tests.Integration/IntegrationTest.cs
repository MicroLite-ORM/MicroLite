namespace MicroLite.Tests.Integration
{
    using System;
    using MicroLite.Configuration;
    using MicroLite.Mapping;

    public abstract class IntegrationTest : IDisposable
    {
        private static readonly ISessionFactory sessionFactory;
        private ISession session;

        static IntegrationTest()
        {
            Configure
                .Extensions()
                .WithConventionBasedMapping(ConventionMappingSettings.Default);

            sessionFactory = Configure
                .Fluently()
                .ForSQLiteConnection("SQLiteInMemory")
                .CreateSessionFactory();
        }

        protected ISession Session
        {
            get
            {
                if (this.session == null)
                {
                    this.session = sessionFactory.OpenSession(ConnectionScope.PerSession);

                    this.CreateDatabase();
                }

                return this.session;
            }
        }

        public void Dispose()
        {
            if (this.session != null)
            {
                this.session.Dispose();
                this.session = null;
            }
        }

        private void CreateDatabase()
        {
            this.session.Advanced.Execute(new SqlQuery(@"CREATE TABLE Customers (
CustomerId       INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
DateOfBirth      TEXT    NOT NULL,
EmailAddress     TEXT,
Forename         TEXT    NOT NULL,
CustomerStatusId INTEGER NOT NULL,
Surname          TEXT    NOT NULL
);"));

            this.session.Advanced.Execute(new SqlQuery(@"CREATE TABLE Invoices (
InvoiceId        INTEGER  PRIMARY KEY AUTOINCREMENT NOT NULL,
Created          DATETIME NOT NULL,
CreatedBy        TEXT     NOT NULL,
CustomerId       INTEGER  NOT NULL,
Number           INTEGER  NOT NULL,
PaymentProcessed DATETIME,
PaymentReceived  DATETIME,
InvoiceStatusId  INTEGER  NOT NULL,
Total            REAL     NOT NULL
);"));
        }
    }
}