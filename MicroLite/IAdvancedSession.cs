namespace MicroLite
{
    /// <summary>
    /// The interface which provides access to advanced operations.
    /// </summary>
    /// <remarks>
    /// These operations allow for more advanded use and have been moved to a separate property to avoid
    /// cluttering the ISession API.
    /// </remarks>
    public interface IAdvancedSession : IHideObjectMembers
    {
        /// <summary>
        /// Executes the specified SQL query.
        /// </summary>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <returns>The number of rows affected by the sql query.</returns>
        int Execute(SqlQuery sqlQuery);

        /// <summary>
        /// Executes the supplied SQL query as a scalar command.
        /// </summary>
        /// <typeparam name="T">The type of result to be returned.</typeparam>
        /// <param name="sqlQuery">The SQL query to execute.</param>
        /// <returns>The result of the scalar query.</returns>
        T ExecuteScalar<T>(SqlQuery sqlQuery);
    }
}