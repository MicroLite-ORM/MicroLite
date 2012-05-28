namespace MicroLite.Syntax
{
    /// <summary>
    /// The interface which specifies the and/or methods to extend the where clause in the fluent sql builder syntax.
    /// </summary>
    public interface IAndOrOrderBy : IOrderBy, IToSqlQuery, IHideObjectMembers
    {
        /// <summary>
        /// Adds a predicate as an AND to the where clause of the query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The args.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOrOrderBy AndWhere(string predicate, params object[] args);

        /// <summary>
        /// Adds a predicate as an OR to the where clause of the query.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <param name="args">The args.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IAndOrOrderBy OrWhere(string predicate, params object[] args);
    }
}