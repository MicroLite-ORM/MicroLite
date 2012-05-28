namespace MicroLite.Configuration
{
    using System;
    using System.Configuration;
    using System.Data.Common;
    using MicroLite.Core;
    using MicroLite.Logging;

    /// <summary>
    /// The class used to configure the MicroLite ORM framework.
    /// </summary>
    public sealed class Configure : IConfigureConnection, ICreateSessionFactory, IHideObjectMembers
    {
        private static ILog log;
        private readonly Options options = new Options();

        /// <summary>
        /// Prevents a default instance of the <see cref="Configure"/> class from being created.
        /// </summary>
        private Configure()
        {
        }

        /// <summary>
        /// The entry method to initialise the configuration process.
        /// </summary>
        /// <returns>The next step in the fluent configuration.</returns>
        public static IConfigureConnection Fluently()
        {
            Listeners.Add<AssignedListener>();
            Listeners.Add<DbGeneratedListener>();

            return new Configure();
        }

        /// <summary>
        /// Creates the session factory for the configured connection.
        /// </summary>
        /// <returns>
        /// The session factory for the specified connection.
        /// </returns>
        public ISessionFactory CreateSessionFactory()
        {
            log.TryLogInfo(LogMessages.Configure_CreatingSessionFactory, this.options.ConnectionString);
            return new SessionFactory(this.options.ConnectionString, this.options.ProviderFactory);
        }

        /// <summary>
        /// Specifies the named connection string in the app config to be used.
        /// </summary>
        /// <param name="connectionName">The name of the connection string in the app config.</param>
        /// <returns>
        /// The next step in the fluent configuration.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if connectionName is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the connection is not found in the app config.</exception>
        /// <exception cref="NotSupportedException">Thrown if the provider name is not supported.</exception>
        public ICreateSessionFactory ForConnection(string connectionName)
        {
            if (connectionName == null)
            {
                throw new ArgumentNullException("connectionName");
            }

            TryToSetLog();

            log.TryLogDebug(LogMessages.Configure_ReadingConnection, connectionName);
            var configSection = ConfigurationManager.ConnectionStrings[connectionName];

            if (configSection == null)
            {
                var message = LogMessages.Configure_ConnectionNotFound.FormatWith(connectionName);
                log.TryLogFatal(message);
                throw new MicroLiteException(message);
            }

            return this.ForConnection(configSection.ConnectionString, configSection.ProviderName);
        }

        /// <summary>
        /// Specifies the connection string and provider name to be used.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="providerName">The provider name.</param>
        /// <returns>
        /// The next step in the fluent configuration.
        /// </returns>
        /// <exception cref="ArgumentNullException">Thrown if connectionString or providerName is null.</exception>
        /// <exception cref="MicroLiteException">Thrown if the provider name is not installed.</exception>
        /// <exception cref="NotSupportedException">Thrown if the provider name is not supported.</exception>
        public ICreateSessionFactory ForConnection(string connectionString, string providerName)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException("connectionString");
            }

            if (providerName == null)
            {
                throw new ArgumentNullException("providerName");
            }

            TryToSetLog();

            if (providerName != "System.Data.SqlClient")
            {
                var message = LogMessages.Configure_ProviderNotSupported.FormatWith(providerName);
                log.TryLogFatal(message);
                throw new NotSupportedException(message);
            }

            try
            {
                this.options.ConnectionString = connectionString;
                this.options.ProviderFactory = DbProviderFactories.GetFactory(providerName);
                return this;
            }
            catch (Exception e)
            {
                throw new MicroLiteException(e.Message, e);
            }
        }

        /// <summary>
        /// Tries to set the ILog.
        /// </summary>
        /// <remarks>
        /// Unlike for all other classes, the LogManager.GetLogger function will not have been set when the type
        /// is initialised. Therefore, we cannot inline assign the log in the static field.
        /// </remarks>
        private static void TryToSetLog()
        {
            if (log == null)
            {
                log = LogManager.GetLogInstance("MicroLite.Configure");
            }
        }
    }
}