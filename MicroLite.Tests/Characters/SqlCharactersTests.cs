namespace MicroLite.Tests.Characters
{
    using System;
    using MicroLite.Characters;
    using Moq;
    using Xunit;

    public class SqlCharactersTests : UnitTest
    {
        [Fact]
        public void CurrentReturnsDefaultSqlCharactersIfNotSet()
        {
            Assert.IsType<SqlCharacters>(SqlCharacters.Current);
        }

        [Fact]
        public void CurrentReturnsSpecifiedSqlCharactersIfSet()
        {
            SqlCharacters.Current = new TestSqlCharacters();

            Assert.IsType<TestSqlCharacters>(SqlCharacters.Current);
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