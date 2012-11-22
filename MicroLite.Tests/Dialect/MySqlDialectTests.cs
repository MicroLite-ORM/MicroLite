namespace MicroLite.Tests.Dialect
{
    using System;
    using System.Data;
    using MicroLite.Dialect;
    using MicroLite.Mapping;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="MySqlDialect"/> class.
    /// </summary>
    [TestFixture]
    public class MySqlDialectTests
    {
        private enum CustomerStatus
        {
            Inactive = 0,
            Active = 1
        }

        [Test]
        public void InsertQueryForAutoIncrementInstance()
        {
            var customer = new Customer
            {
                Created = DateTime.Now,
                DateOfBirth = new System.DateTime(1982, 11, 27),
                Name = "Trevor Pilley",
                Status = CustomerStatus.Active
            };

            var sqlDialect = new MySqlDialect();

            var sqlQuery = sqlDialect.CreateQuery(StatementType.Insert, customer);

            Assert.AreEqual("INSERT INTO `Customers` (`Created`, `DoB`, `Name`, `StatusId`) VALUES (?, ?, ?, ?);SELECT LAST_INSERT_ID()", sqlQuery.CommandText);
            Assert.AreEqual(customer.Created, sqlQuery.Arguments[0]);
            Assert.AreEqual(customer.DateOfBirth, sqlQuery.Arguments[1]);
            Assert.AreEqual(customer.Name, sqlQuery.Arguments[2]);
            Assert.AreEqual((int)customer.Status, sqlQuery.Arguments[3]);
        }

        [MicroLite.Mapping.Table("Customers")]
        private class Customer
        {
            public Customer()
            {
            }

            [MicroLite.Mapping.Column("Created", allowInsert: true, allowUpdate: false)]
            public DateTime Created
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("DoB")]
            public DateTime DateOfBirth
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("CustomerId")]
            [MicroLite.Mapping.Identifier(IdentifierStrategy.AutoIncrement)]
            public int Id
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("Name")]
            public string Name
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("StatusId")]
            public CustomerStatus Status
            {
                get;
                set;
            }

            [MicroLite.Mapping.Column("Updated", allowInsert: false, allowUpdate: true)]
            public DateTime? Updated
            {
                get;
                set;
            }
        }
    }
}