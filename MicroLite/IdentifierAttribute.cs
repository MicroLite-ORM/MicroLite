namespace MicroLite
{
    using System;

    /// <summary>
    /// An attribute which can be applied to a property to specify that it maps to the row identifier (primary key)
    /// in the table and also defines the <see cref="IdentifierStrategy"/> used to manage the identifier's value.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class IdentifierAttribute : Attribute
    {
        private readonly IdentifierStrategy identifierStrategy;

        /// <summary>
        /// Initialises a new instance of the <see cref="IdentifierAttribute"/> class.
        /// </summary>
        /// <param name="identifierStrategy">The identifier strategy used to manage the identifier's value.</param>
        public IdentifierAttribute(IdentifierStrategy identifierStrategy)
        {
            this.identifierStrategy = identifierStrategy;
        }

        /// <summary>
        /// Gets the identifier strategy used to manage the identifier's value.
        /// </summary>
        public IdentifierStrategy IdentifierStrategy
        {
            get
            {
                return this.identifierStrategy;
            }
        }
    }
}