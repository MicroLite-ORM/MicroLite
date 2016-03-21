﻿namespace MicroLite.Tests.TestEntities
{
    using System;

    /// <summary>
    /// An object which can be used when testing classes which need to use an ObjectInfo.
    /// </summary>
    public class Customer
    {
        public Customer()
        {
        }

        public DateTime Created
        {
            get;
            set;
        }

        public Decimal? CreditLimit
        {
            get;
            set;
        }

        public DateTime DateOfBirth
        {
            get;
            set;
        }

        public int Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public CustomerStatus Status
        {
            get;
            set;
        }

        public DateTime? Updated
        {
            get;
            set;
        }

        public Uri Website
        {
            get;
            set;
        }
    }

    /// <summary>
    /// An object which can be used when testing classes which need to use an ObjectInfo.
    /// </summary>
    public class CustomerWithVersion
    {
        public CustomerWithVersion()
        {
        }

        public DateTime Created
        {
            get;
            set;
        }

        public Decimal? CreditLimit
        {
            get;
            set;
        }

        public DateTime DateOfBirth
        {
            get;
            set;
        }

        public int Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public CustomerStatus Status
        {
            get;
            set;
        }

        public DateTime? Updated
        {
            get;
            set;
        }

        public Uri Website
        {
            get;
            set;
        }

        public int Version
        {
            get;
            set;
        }
    }

    /// <summary>
    /// An object which can be used when testing classes which need to use an ObjectInfo.
    /// </summary>
    public class CustomerWithVersion<T>
    {
        public CustomerWithVersion()
        {
        }

        public DateTime Created
        {
            get;
            set;
        }

        public Decimal? CreditLimit
        {
            get;
            set;
        }

        public DateTime DateOfBirth
        {
            get;
            set;
        }

        public int Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public CustomerStatus Status
        {
            get;
            set;
        }

        public DateTime? Updated
        {
            get;
            set;
        }

        public Uri Website
        {
            get;
            set;
        }

        public T Version
        {
            get;
            set;
        }
    }

    /// <summary>
    /// An object which can be used when testing classes which need to use an ObjectInfo.
    /// </summary>
    public class CustomerWithCustomVersion
    {
        public CustomerWithCustomVersion()
        {
        }

        public DateTime Created
        {
            get;
            set;
        }

        public Decimal? CreditLimit
        {
            get;
            set;
        }

        public DateTime DateOfBirth
        {
            get;
            set;
        }

        public int Id
        {
            get;
            set;
        }

        public string Name
        {
            get;
            set;
        }

        public CustomerStatus Status
        {
            get;
            set;
        }

        public DateTime? Updated
        {
            get;
            set;
        }

        public Uri Website
        {
            get;
            set;
        }

        public int RowId
        {
            get;
            set;
        }
    }
}