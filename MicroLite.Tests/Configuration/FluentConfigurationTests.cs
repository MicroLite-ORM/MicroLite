namespace MicroLite.Tests.Configuration
{
    using System;
    using MicroLite.Configuration;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Query;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="FluentConfiguration"/> class.
    /// </summary>
    public class FluentConfigurationTests
    {
        public class WhenCallingCreateSessionFactory : IDisposable
        {
            private readonly ISessionFactory sessionFactory;

            public WhenCallingCreateSessionFactory()
            {
                this.ResetExternalDependencies();

                var fluentConfiguration = new FluentConfiguration();

                this.sessionFactory = fluentConfiguration.ForConnection("SqlConnection", "MicroLite.Dialect.MsSqlDialect").CreateSessionFactory();
            }

            public void Dispose()
            {
                this.ResetExternalDependencies();
            }

            [Fact]
            public void TheSessionFactoryShouldBeAddedToTheSessionFactoriesProperty()
            {
                Assert.Contains(this.sessionFactory, Configure.SessionFactories);
            }

            [Fact]
            public void TheSqlCharactersPropertyOnSqlBuilderShouldBeSet()
            {
                Assert.Equal(this.sessionFactory.SqlDialect.SqlCharacters, SqlBuilder.SqlCharacters);
            }

            private void ResetExternalDependencies()
            {
                Configure.SessionFactories.Clear();
                SqlBuilder.SqlCharacters = null;
            }
        }

        public class WhenCallingCreateSessionFactoryMultipleTimesForTheSameConnection : IDisposable
        {
            private readonly ISessionFactory sessionFactory1;
            private readonly ISessionFactory sessionFactory2;

            public WhenCallingCreateSessionFactoryMultipleTimesForTheSameConnection()
            {
                Configure.SessionFactories.Clear();

                var fluentConfiguration = new FluentConfiguration();

                this.sessionFactory1 = fluentConfiguration.ForConnection("SqlConnection", "MicroLite.Dialect.MsSqlDialect").CreateSessionFactory();
                this.sessionFactory2 = fluentConfiguration.ForConnection("SqlConnection", "MicroLite.Dialect.MsSqlDialect").CreateSessionFactory();
            }

            public void Dispose()
            {
                Configure.SessionFactories.Clear();
            }

            [Fact]
            public void TheSameSessionFactoryShouldBeReturned()
            {
                Assert.Same(this.sessionFactory1, this.sessionFactory2);
            }
        }

        public class WhenCallingForConnectionAndTheConnectionNameDoesNotExistInTheAppConfig
        {
            [Fact]
            public void AMicroLiteExceptionShouldBeThrown()
            {
                var fluentConfiguration = new FluentConfiguration();

                var exception = Assert.Throws<MicroLiteException>(() => fluentConfiguration.ForConnection("TestDB", "MicroLite.Dialect.MsSqlDialect"));

                Assert.Equal(Messages.FluentConfiguration_ConnectionNotFound.FormatWith("TestDB"), exception.Message);
            }
        }

        public class WhenCallingForConnectionAndTheConnectionNameIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var fluentConfiguration = new FluentConfiguration();

                var exception = Assert.Throws<ArgumentNullException>(() => fluentConfiguration.ForConnection(null, "MicroLite.Dialect.MsSqlDialect"));

                Assert.Equal(exception.ParamName, "connectionName");
            }
        }

        public class WhenCallingForConnectionAndTheProviderInTheAppConfigIsNotSupported
        {
            [Fact]
            public void AMicroLiteExceptionShouldBeThrown()
            {
                var fluentConfiguration = new FluentConfiguration();

                Assert.Throws<MicroLiteException>(
                    () => fluentConfiguration.ForConnection("ConnectionWithInvalidProviderName", "MicroLite.Dialect.MsSqlDialect"));
            }
        }

        public class WhenCallingForConnectionAndTheSqlDialectDoesNotImplementISqlDialect
        {
            [Fact]
            public void AMicroLiteExceptionShouldBeThrown()
            {
                var fluentConfiguration = new FluentConfiguration();

                var exception = Assert.Throws<NotSupportedException>(
                    () => fluentConfiguration.ForConnection("SqlConnection", "MicroLite.SqlQuery"));

                Assert.Equal(Messages.FluentConfiguration_DialectMustImplementISqlDialect.FormatWith("MicroLite.SqlQuery"), exception.Message);
            }
        }

        public class WhenCallingForConnectionAndTheSqlDialectIsNotSupported
        {
            [Fact]
            public void AMicroLiteExceptionShouldBeThrown()
            {
                var fluentConfiguration = new FluentConfiguration();

                var exception = Assert.Throws<NotSupportedException>(
                    () => fluentConfiguration.ForConnection("SqlConnection", "MicroLite.Dialect.DB2"));

                Assert.Equal(Messages.FluentConfiguration_DialectNotSupported.FormatWith("MicroLite.Dialect.DB2"), exception.Message);
            }
        }
    }
}