namespace MicroLite
{
    /// <summary>
    /// The interface which specifies the from method in the fluent sql builder syntax.
    /// </summary>
    public interface IFrom : IHideObjectMembers
    {
        /// <summary>
        /// Specifies the table to perform the query against.
        /// </summary>
        /// <param name="table">The name of the table.</param>
        /// <returns>The next step in the fluent sql builder.</returns>
        IWhereOrOrderBy From(string table);
    }
}