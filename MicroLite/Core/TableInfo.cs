namespace MicroLite.Core
{
    using System.Collections.Generic;

    [System.Diagnostics.DebuggerDisplay("{Schema}.{Name}")]
    internal sealed class TableInfo
    {
        private readonly ICollection<string> columns = new List<string>();

        internal ICollection<string> Columns
        {
            get
            {
                return this.columns;
            }
        }

        internal string IdentifierColumn
        {
            get;
            set;
        }

        internal IdentifierStrategy IdentifierStrategy
        {
            get;
            set;
        }

        internal string Name
        {
            get;
            set;
        }

        internal string Schema
        {
            get;
            set;
        }
    }
}