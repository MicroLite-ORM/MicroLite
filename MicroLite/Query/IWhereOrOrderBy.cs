namespace MicroLite.Query
{
    /// <summary>
    /// The interface which specifies the where method in the fluent sql builder syntax.
    /// </summary>
    public interface IWhereOrOrderBy : IOrderBy, IToSqlQuery, IHideObjectMembers
    {
        /// <summary>
        /// Specifies the where clause for the query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The args.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOrOrderBy Where(string predicate, params object[] args);
    }
}