namespace MicroLite.Query
{
    /// <summary>
    /// The interface which specifies the order by method in the fluent sql builder syntax.
    /// </summary>
    public interface IOrderBy : IToSqlQuery, IHideObjectMembers
    {
        /// <summary>
        /// Orders the results of the query by the specified column in ascending order.
        /// </summary>
        /// <param name="column">The column to order by.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IOrderBy OrderByAscending(string column);

        /// <summary>
        /// Orders the results of the query by the specified column in descending order.
        /// </summary>
        /// <param name="column">The column to order by.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IOrderBy OrderByDescending(string column);
    }
}