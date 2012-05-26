namespace MicroLite.Configuration
{
    using System.Data.Common;

    /// <summary>
    /// The class used to hold all the configurable options during configuration.
    /// </summary>
    internal sealed class Options
    {
        internal string ConnectionString
        {
            get;
            set;
        }

        internal DbProviderFactory ProviderFactory
        {
            get;
            set;
        }
    }
}