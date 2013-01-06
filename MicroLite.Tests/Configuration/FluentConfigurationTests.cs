﻿namespace MicroLite.Tests.Configuration
{
    using System;
    using MicroLite.Configuration;
    using MicroLite.FrameworkExtensions;
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
                Configure.SessionFactories.Clear();

                var fluentConfiguration = new FluentConfiguration();

                this.sessionFactory = fluentConfiguration.ForConnection("SqlConnection").CreateSessionFactory();
            }

            public void Dispose()
            {
                Configure.SessionFactories.Clear();
            }

            [Fact]
            public void TheSessionFactoryShouldBeAddedToTheSessionFactoriesProperty()
            {
                Assert.Contains(this.sessionFactory, Configure.SessionFactories);
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

                this.sessionFactory1 = fluentConfiguration.ForConnection("SqlConnection").CreateSessionFactory();
                this.sessionFactory2 = fluentConfiguration.ForConnection("SqlConnection").CreateSessionFactory();
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

                var exception = Assert.Throws<MicroLiteException>(() => fluentConfiguration.ForConnection("TestDB"));

                Assert.Equal(Messages.FluentConfiguration_ConnectionNotFound.FormatWith("TestDB"), exception.Message);
            }
        }

        public class WhenCallingForConnectionAndTheConnectionNameIsNull
        {
            [Fact]
            public void AnArgumentNullExceptionShouldBeThrown()
            {
                var fluentConfiguration = new FluentConfiguration();

                var exception = Assert.Throws<ArgumentNullException>(() => fluentConfiguration.ForConnection(null));

                Assert.Equal(exception.ParamName, "connectionName");
            }
        }

        public class WhenCallingForConnectionAndTheProviderInTheAppConfigIsNotSupported
        {
            [Fact]
            public void AMicroLiteExceptionShouldBeThrown()
            {
                var fluentConfiguration = new FluentConfiguration();

                var exception = Assert.Throws<NotSupportedException>(
                    () => fluentConfiguration.ForConnection("SqlConnection", "MicroLite.Dialect.DB2"));

                Assert.Equal(Messages.SqlDialectFactory_DialectNotSupported.FormatWith("MicroLite.Dialect.DB2"), exception.Message);
            }
        }

        public class WhenCallingForConnectionAndTheSqlDialectIsNotSupported
        {
            [Fact]
            public void AMicroLiteExceptionShouldBeThrown()
            {
                var fluentConfiguration = new FluentConfiguration();

                var exception = Assert.Throws<MicroLiteException>(
                    () => fluentConfiguration.ForConnection("ConnectionWithInvalidProviderName"));
            }
        }
    }
}