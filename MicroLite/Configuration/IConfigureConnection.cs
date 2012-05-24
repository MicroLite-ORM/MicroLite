namespace MicroLite.Configuration
{
    /// <summary>
    /// The interface which specifies the options for configuring the connection in the fluent configuration
    /// of the MicroLite ORM framework.
    /// </summary>
    public interface IConfigureConnection : IHideObjectMembers
    {
        /// <summary>
        /// Specifies the named connection string in the app config to be used.
        /// </summary>
        /// <param name="connectionName">The name of the connection string in the app config.</param>
        /// <returns>The next step in the fluent configuration.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionName is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the connection is not found in the app config.</exception>
        /// <exception cref="System.NotSupportedException">Thrown if the provider name is not supported.</exception>
        ICreateSessionFactory ForConnection(string connectionName);

        /// <summary>
        /// Specifies the connection string and provider name to be used.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="providerName">The provider name.</param>
        /// <returns>The next step in the fluent configuration.</returns>
        /// <exception cref="System.ArgumentNullException">Thrown if connectionString or providerName is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the provider name is not installed.</exception>
        /// <exception cref="System.NotSupportedException">Thrown if the provider name is not supported.</exception>
        ICreateSessionFactory ForConnection(string connectionString, string providerName);
    }
}