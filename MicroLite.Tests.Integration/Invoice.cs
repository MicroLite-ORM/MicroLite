namespace MicroLite.Tests.Integration
{
    using System;

    public class Invoice
    {
        public DateTime Created
        {
            get;
            set;
        }

        public string CreatedBy
        {
            get;
            set;
        }

        public int CustomerId
        {
            get;
            set;
        }

        public int InvoiceId
        {
            get;
            set;
        }

        public int Number
        {
            get;
            set;
        }

        public DateTime? PaymentProcessed
        {
            get;
            set;
        }

        public DateTime? PaymentReceived
        {
            get;
            set;
        }

        public InvoiceStatus Status
        {
            get;
            set;
        }

        public decimal Total
        {
            get;
            set;
        }
    }
}