namespace MicroLite
{
    /// <summary>
    /// The interface which specifies the factory options for creating <see cref="ISession"/>s.
    /// </summary>
    public interface ISessionFactory : IHideObjectMembers
    {
        /// <summary>
        /// Gets the connection string used by the session factory.
        /// </summary>
        string ConnectionString
        {
            get;
        }

        /// <summary>
        /// Opens a new session to the database.
        /// </summary>
        /// <returns>A new session instance.</returns>
        ISession OpenSession();
    }
}