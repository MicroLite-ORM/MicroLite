namespace MicroLite
{
    using System;

    /// <summary>
    /// An attribute which can be applied to a property to specify that it should be ignored by the
    /// MicroLite ORM framework.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class IgnoreAttribute : Attribute
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="IgnoreAttribute"/> class.
        /// </summary>
        public IgnoreAttribute()
        {
        }
    }
}