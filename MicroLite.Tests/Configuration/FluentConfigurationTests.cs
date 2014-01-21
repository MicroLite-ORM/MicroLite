namespace MicroLite.Tests.Configuration
{
    using System;
    using System.Data.Common;
    using MicroLite.Configuration;
    using MicroLite.Dialect;
    using MicroLite.FrameworkExtensions;
    using Moq;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="FluentConfiguration"/> class.
    /// </summary>
    public class FluentConfigurationTests
    {
        public class WhenCallingCreateSessionFactory : IDisposable
        {
            private readonly ISessionFactory sessionFactory;
            private readonly SqlCharacters sqlCharacters = new Mock<SqlCharacters>().Object;

            public WhenCallingCreateSessionFactory()
            {
                this.ResetExternalDependencies();

                var fluentConfiguration = new FluentConfiguration();

                var mockSqlDialect = new Mock<ISqlDialect>();
                mockSqlDialect.Setup(x => x.SqlCharacters).Returns(this.sqlCharacters);

                this.sessionFactory = fluentConfiguration
                    .ForConnection("SqlConnection", mockSqlDialect.Object, new Mock<DbProviderFactory>().Object)
                    .CreateSessionFactory();
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
            public void TheSqlCharactersCurrentPropertyShouldBeSetToTheSqlDialectSqlCharacters()
            {
                Assert.Equal(this.sqlCharacters, SqlCharacters.Current);
            }

            private void ResetExternalDependencies()
            {
                Configure.SessionFactories.Clear();
                SqlCharacters.Current = null;
            }
        }

        public class WhenCallingCreateSessionFactory_MultipleTimesForTheSameConnection : IDisposable
        {
            private readonly ISessionFactory sessionFactory1;
            private readonly ISessionFactory sessionFactory2;

            public WhenCallingCreateSessionFactory_MultipleTimesForTheSameConnection()
            {
                Configure.SessionFactories.Clear();

                var fluentConfiguration = new FluentConfiguration();

                this.sessionFactory1 = fluentConfiguration
                    .ForConnection("SqlConnection", new Mock<ISqlDialect>().Object, new Mock<DbProviderFactory>().Object)
                    .CreateSessionFactory();

                this.sessionFactory2 = fluentConfiguration
                    .ForConnection("SqlConnection", new Mock<ISqlDialect>().Object, new Mock<DbProviderFactory>().Object)
                    .CreateSessionFactory();
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

        public class WhenCallingForConnection_AndTheConnectionNameDoesNotExistInTheAppConfig
        {
            [Fact]
            public void AMicroLiteConfigurationExceptionShouldBeThrown()
            {
                var fluentConfiguration = new FluentConfiguration();

                var exception = Assert.Throws<ConfigurationException>(
                    () => fluentConfiguration.ForConnection("TestDB", new Mock<ISqlDialect>().Object, new Mock<DbProviderFactory>().Object));

                Assert.Equal(Messages.FluentConfiguration_ConnectionNotFound.FormatWith("TestDB"), exception.Message);
            }
        }

        public class WhenCallingForConnection_AndTheConnectionNameIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var fluentConfiguration = new FluentConfiguration();

                var exception = Assert.Throws<ArgumentNullException>(
                    () => fluentConfiguration.ForConnection(null, new Mock<ISqlDialect>().Object, new Mock<DbProviderFactory>().Object));

                Assert.Equal(exception.ParamName, "connectionName");
            }
        }

        public class WhenCallingForConnection_AndTheDbProviderFactoryIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var fluentConfiguration = new FluentConfiguration();

                var exception = Assert.Throws<ArgumentNullException>(
                    () => fluentConfiguration.ForConnection("SqlConnection", new Mock<ISqlDialect>().Object, null));

                Assert.Equal(exception.ParamName, "providerFactory");
            }
        }

        public class WhenCallingForConnection_AndTheSqlDialectIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var fluentConfiguration = new FluentConfiguration();

                var exception = Assert.Throws<ArgumentNullException>(
                    () => fluentConfiguration.ForConnection("SqlConnection", null, new Mock<DbProviderFactory>().Object));

                Assert.Equal(exception.ParamName, "sqlDialect");
            }
        }
    }
}