namespace MicroLite.Tests.Dialect
{
    using System;
    using MicroLite.Dialect;
    using MicroLite.FrameworkExtensions;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SqlDialectFactory"/> class.
    /// </summary>
    public class SqlDialectFactoryTests
    {
        [Fact]
        public void AddAddsDialect()
        {
            SqlDialectFactory.Add("MyDialect", typeof(EmptySqlDialect));

            Assert.NotNull(SqlDialectFactory.GetDialect("MyDialect"));
        }

        [Fact]
        public void AddThrowsArgumentNullExceptionForNullDialectName()
        {
            Assert.Throws<ArgumentNullException>(() => SqlDialectFactory.Add(null, typeof(ISqlDialect)));
        }

        [Fact]
        public void AddThrowsArgumentNullExceptionForNullDialectType()
        {
            Assert.Throws<ArgumentNullException>(() => SqlDialectFactory.Add("MyDialect", null));
        }

        [Fact]
        public void AddThrowsMicroLiteExceptionIfDialectNameAlreadyUsed()
        {
            var exception = Assert.Throws<MicroLiteException>(() => SqlDialectFactory.Add("MicroLite.Dialect.MsSqlDialect", typeof(ISqlDialect)));

            Assert.Equal(Messages.SqlDialectFactory_DialectNameAlreadyUsed.FormatWith("MicroLite.Dialect.MsSqlDialect"), exception.Message);
        }

        [Fact]
        public void AddThrowsMicroLiteExceptionIfDialectTypeIsNotISqlDialect()
        {
            var exception = Assert.Throws<MicroLiteException>(() => SqlDialectFactory.Add("MyDialect", typeof(string)));

            Assert.Equal(Messages.SqlDialectFactory_DialectMustImplementISqlDialect.FormatWith("String"), exception.Message);
        }
        [Fact]
        public void GetDialectReturnsMsSqlDialect()
        {
            var sqlDialect = SqlDialectFactory.GetDialect("MicroLite.Dialect.MsSqlDialect");

            Assert.IsType<MsSqlDialect>(sqlDialect);
        }

        [Fact]
        public void GetDialectReturnsMySqlDialect()
        {
            var sqlDialect = SqlDialectFactory.GetDialect("MicroLite.Dialect.MySqlDialect");

            Assert.IsType<MySqlDialect>(sqlDialect);
        }

        [Fact]
        public void GetDialectReturnsNewInstanceEachCall()
        {
            var sqlDialect1 = SqlDialectFactory.GetDialect("MicroLite.Dialect.MsSqlDialect");
            var sqlDialect2 = SqlDialectFactory.GetDialect("MicroLite.Dialect.MsSqlDialect");

            Assert.NotSame(sqlDialect1, sqlDialect2);
        }

        [Fact]
        public void GetDialectReturnsPostgreSqlDialect()
        {
            var sqlDialect = SqlDialectFactory.GetDialect("MicroLite.Dialect.PostgreSqlDialect");

            Assert.IsType<PostgreSqlDialect>(sqlDialect);
        }

        [Fact]
        public void GetDialectReturnsSQLiteDialect()
        {
            var sqlDialect = SqlDialectFactory.GetDialect("MicroLite.Dialect.SQLiteDialect");

            Assert.IsType<SQLiteDialect>(sqlDialect);
        }

        [Fact]
        public void GetDialectThrowsNotSupportedException()
        {
            var dialectName = "MicroLite.Dialect.DB2";

            var exception = Assert.Throws<NotSupportedException>(() => SqlDialectFactory.GetDialect(dialectName));

            Assert.Equal(Messages.SqlDialectFactory_DialectNotSupported.FormatWith(dialectName), exception.Message);
        }

        [Fact]
        public void VerifyDialectDoesNotThrowNotSupportedExceptionForSupportedDialect()
        {
            SqlDialectFactory.VerifyDialect("MicroLite.Dialect.MsSqlDialect");
        }

        [Fact]
        public void VerifyDialectThrowsNotSupportedException()
        {
            var dialectName = "MicroLite.Dialect.DB2";

            var exception = Assert.Throws<NotSupportedException>(() => SqlDialectFactory.VerifyDialect(dialectName));

            Assert.Equal(Messages.SqlDialectFactory_DialectNotSupported.FormatWith(dialectName), exception.Message);
        }

        private class EmptySqlDialect : ISqlDialect
        {
            public bool SupportsBatchedQueries
            {
                get
                {
                    throw new NotImplementedException();
                }
            }

            public void BuildCommand(System.Data.IDbCommand command, SqlQuery sqlQuery)
            {
                throw new NotImplementedException();
            }

            public SqlQuery Combine(System.Collections.Generic.IEnumerable<SqlQuery> sqlQueries)
            {
                throw new NotImplementedException();
            }

            public SqlQuery CountQuery(SqlQuery sqlQuery)
            {
                throw new NotImplementedException();
            }

            public SqlQuery CreateQuery(System.Data.StatementType statementType, object instance)
            {
                throw new NotImplementedException();
            }

            public SqlQuery CreateQuery(System.Data.StatementType statementType, Type forType, object identifier)
            {
                throw new NotImplementedException();
            }

            public SqlQuery PageQuery(SqlQuery sqlQuery, PagingOptions pagingOptions)
            {
                throw new NotImplementedException();
            }
        }
    }
}