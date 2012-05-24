namespace MicroLite.Configuration
{
    /// <summary>
    /// The interface which specifies the creation of the <see cref="ISessionFactory"/> in the fluent configuration
    /// of the MicroLite ORM framework.
    /// </summary>
    public interface ICreateSessionFactory : IHideObjectMembers
    {
        /// <summary>
        /// Creates the session factory for the configured connection.
        /// </summary>
        /// <returns>The session factory for the specified connection.</returns>
        ISessionFactory CreateSessionFactory();
    }
}