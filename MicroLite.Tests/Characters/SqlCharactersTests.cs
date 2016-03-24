namespace MicroLite.Tests.Characters
{
    using System;
    using System.Threading;
    using MicroLite.Characters;
    using Moq;
    using Xunit;

    public class SqlCharactersTests : UnitTest
    {
#if !NET35

        [Fact]
        public void CurrentIsVisibleAccrossTasks()
        {
            var sqlCharacters = new TestSqlCharacters();
            SqlCharacters.Current = sqlCharacters;

            SqlCharacters actual = null;

            System.Threading.Tasks.Task.Factory.StartNew(
                () => actual = SqlCharacters.Current,
                System.Threading.Tasks.TaskCreationOptions.LongRunning).Wait();

            Assert.Same(sqlCharacters, actual);
        }

#endif

        [Fact]
        public void CurrentIsVisibleAccrossThreadPools()
        {
            var sqlCharacters = new TestSqlCharacters();
            SqlCharacters.Current = sqlCharacters;

            SqlCharacters actual = null;
            var handle = new ManualResetEvent(false);

            ThreadPool.QueueUserWorkItem(_ =>
            {
                actual = SqlCharacters.Current;
                handle.Set();
            });

            handle.WaitOne();

            Assert.Same(sqlCharacters, actual);
        }

        /// <summary>
        /// Issue #370 - SqlCharacters.Current is not visible on new threads
        /// </summary>
        [Fact]
        public void CurrentIsVisibleAccrossThreads()
        {
            var sqlCharacters = new TestSqlCharacters();

            SqlCharacters actual = null;
            var handle = new AutoResetEvent(false);

            var thread1 = new Thread(() =>
            {
                SqlCharacters.Current = sqlCharacters;
                handle.Set();
            });

            var thread2 = new Thread(() =>
            {
                actual = SqlCharacters.Current;
                handle.Set();
            });

            thread1.Start();
            handle.WaitOne();

            thread2.Start();
            handle.WaitOne();

            thread1.Abort();
            thread2.Abort();

            Assert.Same(sqlCharacters, actual);
        }

        [Fact]
        public void CurrentReturnsEmptySqlCharactersIfNotSet()
        {
            Assert.IsType<SqlCharacters>(SqlCharacters.Current);
        }

        [Fact]
        public void CurrentReturnsSpecifiedSqlCharactersIfSet()
        {
            var sqlCharacters = new TestSqlCharacters();

            SqlCharacters.Current = sqlCharacters;

            Assert.Same(sqlCharacters, SqlCharacters.Current);
        }

        [Fact]
        public void DefaultPropertyValues()
        {
            var mockSqlCharacters = new Mock<SqlCharacters>();
            mockSqlCharacters.CallBase = true;

            var sqlCharacters = mockSqlCharacters.Object;

            Assert.Equal(string.Empty, sqlCharacters.LeftDelimiter);
            Assert.Equal("%", sqlCharacters.LikeWildcard);
            Assert.Equal(string.Empty, sqlCharacters.RightDelimiter);
            Assert.Equal("*", sqlCharacters.SelectWildcard);
            Assert.Equal("?", sqlCharacters.SqlParameter);
            Assert.Equal(";", sqlCharacters.StatementSeparator);
            Assert.Equal(string.Empty, sqlCharacters.StoredProcedureInvocationCommand);
            Assert.Equal(false, sqlCharacters.SupportsNamedParameters);
        }

        [Fact]
        public void EmptyEscapeSqlThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => SqlCharacters.Empty.EscapeSql(null));
        }

        [Fact]
        public void EmptyReturnsDefaultSqlCharacters()
        {
            Assert.IsType<SqlCharacters>(SqlCharacters.Empty);
        }

        [Fact]
        public void EscapeSqlDoesNotExcapeValueIfAlreadyEscaped()
        {
            var sqlCharacters = new TestSqlCharacters();

            Assert.Equal("~Name~", sqlCharacters.EscapeSql("~Name~"));
        }

        [Fact]
        public void EscapeSqlExcapesQualifiedValueIfNotAlreadyEscaped()
        {
            var sqlCharacters = new TestSqlCharacters();

            Assert.Equal("~Table~.~Name~", sqlCharacters.EscapeSql("Table.Name"));
        }

        [Fact]
        public void EscapeSqlExcapesValueIfNotAlreadyEscaped()
        {
            var sqlCharacters = new TestSqlCharacters();

            Assert.Equal("~Name~", sqlCharacters.EscapeSql("Name"));
        }

        [Fact]
        public void GetParameterNameReturnsCorrectValue()
        {
            var sqlCharacters = new TestSqlCharacters();

            Assert.Equal("#p0", sqlCharacters.GetParameterName(0));
        }

        [Fact]
        public void GetParameterNameReturnsCorrectValueForDefaultSqlCharacters()
        {
            Assert.Equal("?", SqlCharacters.Empty.GetParameterName(0));
        }

        [Fact]
        public void IsEscapedReturnsFalseIfEmpty()
        {
            var sqlCharacters = new TestSqlCharacters();

            Assert.False(sqlCharacters.IsEscaped(string.Empty));
        }

        [Fact]
        public void IsEscapedReturnsFalseIfNotEscapedWithCorrectDelimiters()
        {
            var sqlCharacters = new TestSqlCharacters();

            Assert.False(sqlCharacters.IsEscaped("Name"));
        }

        [Fact]
        public void IsEscapedReturnsFalseIfNull()
        {
            var sqlCharacters = new TestSqlCharacters();

            Assert.False(sqlCharacters.IsEscaped(null));
        }

        [Fact]
        public void IsEscapedReturnsTrueIfEscapedWithCorrectDelimiters()
        {
            var sqlCharacters = new TestSqlCharacters();

            Assert.True(sqlCharacters.IsEscaped("~Name~"));
        }

        /// <summary>
        /// Overrides the base properties with non standard values for testing.
        /// </summary>
        private sealed class TestSqlCharacters : SqlCharacters
        {
            public override string LeftDelimiter
            {
                get
                {
                    return "~";
                }
            }

            public override string RightDelimiter
            {
                get
                {
                    return "~";
                }
            }

            public override string SqlParameter
            {
                get
                {
                    return "#";
                }
            }

            public override bool SupportsNamedParameters
            {
                get
                {
                    return true;
                }
            }
        }
    }
}