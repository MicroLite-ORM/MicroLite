namespace MicroLite
{
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    /// <summary>
    /// A class which represents a parameterised SQL query.
    /// </summary>
    public sealed class SqlQuery
    {
        private readonly string commandText;
        private readonly ReadOnlyCollection<object> parameters;

        /// <summary>
        /// Initialises a new instance of the <see cref="SqlQuery"/> class with the supplied command text and parameter values.
        /// </summary>
        /// <param name="commandText">The SQL command text.</param>
        /// <param name="parameters">The parameter values for the query.</param>
        public SqlQuery(string commandText, params object[] parameters)
        {
            this.commandText = commandText;
            this.parameters = new ReadOnlyCollection<object>(parameters ?? new object[0]);
        }

        /// <summary>
        /// Gets the SQL statement or stored procedure to execute at the data source.
        /// </summary>
        public string CommandText
        {
            get
            {
                return this.commandText;
            }
        }

        /// <summary>
        /// Gets the parameters of the SQL statement or stored procedure.
        /// </summary>
        public IList<object> Parameters
        {
            get
            {
                return this.parameters;
            }
        }
    }
}