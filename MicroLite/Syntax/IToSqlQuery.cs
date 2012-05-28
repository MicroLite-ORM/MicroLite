namespace MicroLite.Syntax
{
    /// <summary>
    /// The interface to end the fluent build syntax.
    /// </summary>
    /// <remarks>
    /// It's a bit of a verbose hack, need to see if I can use cast operators instead somehow...
    /// </remarks>
    public interface IToSqlQuery : IHideObjectMembers
    {
        /// <summary>
        /// Creates a <see cref="SqlQuery"/> from the values specified.
        /// </summary>
        /// <returns>The created <see cref="SqlQuery"/>.</returns>
        SqlQuery ToSqlQuery();
    }
}