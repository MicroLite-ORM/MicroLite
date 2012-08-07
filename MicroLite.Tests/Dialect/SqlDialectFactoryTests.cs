namespace MicroLite.Tests.Dialect
{
    using System;
    using MicroLite.Dialect;
    using MicroLite.FrameworkExtensions;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="SqlDialectFactory"/> class.
    /// </summary>
    [TestFixture]
    public class SqlDialectFactoryTests
    {
        [Test]
        public void GetDialectReturnsMsSqlDialect()
        {
            var sqlDialect = SqlDialectFactory.GetDialect("MicroLite.Dialect.MsSqlDialect");

            Assert.IsInstanceOf<MsSqlDialect>(sqlDialect);
        }

        [Test]
        public void GetDialectThrowsNotSupportedException()
        {
            var dialectName = "MicroLite.Dialect.DB2";

            var exception = Assert.Throws<NotSupportedException>(() => SqlDialectFactory.GetDialect(dialectName));

            Assert.AreEqual(Messages.SqlDialectFactory_DialectNotSupported.FormatWith(dialectName), exception.Message);
        }
    }
}