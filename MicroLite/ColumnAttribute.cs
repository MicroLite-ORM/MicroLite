namespace MicroLite
{
    using System;

    /// <summary>
    /// An attribute which can be applied to a property to specify the column name if it is different to the
    /// property name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ColumnAttribute : Attribute
    {
        private readonly string name;

        /// <summary>
        /// Initialises a new instance of the <see cref="ColumnAttribute"/> class.
        /// </summary>
        /// <param name="name">The name of the column in the database table.</param>
        public ColumnAttribute(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Gets the name of the column in the database table.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }
        }
    }
}