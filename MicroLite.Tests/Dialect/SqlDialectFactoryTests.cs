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
        public void GetDialectReturnsMySqlDialect()
        {
            var sqlDialect = SqlDialectFactory.GetDialect("MicroLite.Dialect.MySqlDialect");

            Assert.IsInstanceOf<MySqlDialect>(sqlDialect);
        }

        [Test]
        public void GetDialectReturnsNewInstanceEachCall()
        {
            var sqlDialect1 = SqlDialectFactory.GetDialect("MicroLite.Dialect.MsSqlDialect");
            var sqlDialect2 = SqlDialectFactory.GetDialect("MicroLite.Dialect.MsSqlDialect");

            Assert.AreNotSame(sqlDialect1, sqlDialect2);
        }

        [Test]
        public void GetDialectReturnsPostgreSqlDialect()
        {
            var sqlDialect = SqlDialectFactory.GetDialect("MicroLite.Dialect.PostgreSqlDialect");

            Assert.IsInstanceOf<PostgreSqlDialect>(sqlDialect);
        }

        [Test]
        public void GetDialectReturnsSQLiteDialect()
        {
            var sqlDialect = SqlDialectFactory.GetDialect("MicroLite.Dialect.SQLiteDialect");

            Assert.IsInstanceOf<SQLiteDialect>(sqlDialect);
        }

        [Test]
        public void GetDialectThrowsNotSupportedException()
        {
            var dialectName = "MicroLite.Dialect.DB2";

            var exception = Assert.Throws<NotSupportedException>(() => SqlDialectFactory.GetDialect(dialectName));

            Assert.AreEqual(Messages.SqlDialectFactory_DialectNotSupported.FormatWith(dialectName), exception.Message);
        }

        [Test]
        public void VerifyDialectDoesNotThrowNotSupportedExceptionForSupportedDialect()
        {
            SqlDialectFactory.VerifyDialect("MicroLite.Dialect.MsSqlDialect");
        }

        [Test]
        public void VerifyDialectThrowsNotSupportedException()
        {
            var dialectName = "MicroLite.Dialect.DB2";

            var exception = Assert.Throws<NotSupportedException>(() => SqlDialectFactory.VerifyDialect(dialectName));

            Assert.AreEqual(Messages.SqlDialectFactory_DialectNotSupported.FormatWith(dialectName), exception.Message);
        }
    }
}