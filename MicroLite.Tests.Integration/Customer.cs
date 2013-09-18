namespace MicroLite.Tests.Integration
{
    using System;

    public class Customer
    {
        public int CustomerId
        {
            get;
            set;
        }

        public DateTime DateOfBirth
        {
            get;
            set;
        }

        public string EmailAddress
        {
            get;
            set;
        }

        public string Forename
        {
            get;
            set;
        }

        public CustomerStatus Status
        {
            get;
            set;
        }

        public string Surname
        {
            get;
            set;
        }
    }
}