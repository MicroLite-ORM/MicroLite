namespace MicroLite.Tests
{
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SqlString"/> class.
    /// </summary>
    public class SqlStringTests
    {
        public class WhenParsingCommandText_ContainingAllClauses_SpecifyingAllClauses
        {
            private readonly SqlString sqlString;

            public WhenParsingCommandText_ContainingAllClauses_SpecifyingAllClauses()
            {
                this.sqlString = SqlString.Parse(
                    "SELECT Column1, Column2, Column3 FROM Table WHERE Column1 = @p0 AND Column2 > @p1 GROUP BY Column3 ORDER BY Column2 DESC",
                    Clauses.Select | Clauses.From | Clauses.Where | Clauses.GroupBy | Clauses.OrderBy);
            }

            [Fact]
            public void TheFromClauseShouldBeSet()
            {
                Assert.Equal("Table", this.sqlString.From);
            }

            [Fact]
            public void TheGroupByClauseShouldBeSet()
            {
                Assert.Equal("Column3", this.sqlString.GroupBy);
            }

            [Fact]
            public void TheOrderByClauseShouldBeSet()
            {
                Assert.Equal("Column2 DESC", this.sqlString.OrderBy);
            }

            [Fact]
            public void TheSelectClauseShouldBeSet()
            {
                Assert.Equal("Column1, Column2, Column3", this.sqlString.Select);
            }

            [Fact]
            public void TheWhereClauseShouldBeSet()
            {
                Assert.Equal("Column1 = @p0 AND Column2 > @p1", this.sqlString.Where);
            }
        }

        public class WhenParsingCommandText_ContainingAllClauses_SpecifyingClausesFrom
        {
            private readonly SqlString sqlString;

            public WhenParsingCommandText_ContainingAllClauses_SpecifyingClausesFrom()
            {
                this.sqlString = SqlString.Parse(
                    "SELECT Column1, Column2, Column3 FROM Table WHERE Column1 = @p0 AND Column2 > @p1 GROUP BY Column3 ORDER BY Column2 DESC",
                    Clauses.From);
            }

            [Fact]
            public void TheFromClauseShouldBeSet()
            {
                Assert.Equal("Table", this.sqlString.From);
            }

            [Fact]
            public void TheGroupByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.GroupBy);
            }

            [Fact]
            public void TheOrderByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.OrderBy);
            }

            [Fact]
            public void TheSelectClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Select);
            }

            [Fact]
            public void TheWhereClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Where);
            }
        }

        public class WhenParsingCommandText_ContainingAllClauses_SpecifyingClausesGroupBy
        {
            private readonly SqlString sqlString;

            public WhenParsingCommandText_ContainingAllClauses_SpecifyingClausesGroupBy()
            {
                this.sqlString = SqlString.Parse(
                    "SELECT Column1, Column2, Column3 FROM Table WHERE Column1 = @p0 AND Column2 > @p1 GROUP BY Column3 ORDER BY Column2 DESC",
                    Clauses.GroupBy);
            }

            [Fact]
            public void TheFromClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.From);
            }

            [Fact]
            public void TheGroupByClauseShouldBeSet()
            {
                Assert.Equal("Column3", this.sqlString.GroupBy);
            }

            [Fact]
            public void TheOrderByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.OrderBy);
            }

            [Fact]
            public void TheSelectClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Select);
            }

            [Fact]
            public void TheWhereClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Where);
            }
        }

        public class WhenParsingCommandText_ContainingAllClauses_SpecifyingClausesOrderBy
        {
            private readonly SqlString sqlString;

            public WhenParsingCommandText_ContainingAllClauses_SpecifyingClausesOrderBy()
            {
                this.sqlString = SqlString.Parse(
                    "SELECT Column1, Column2, Column3 FROM Table WHERE Column1 = @p0 AND Column2 > @p1 GROUP BY Column3 ORDER BY Column2 DESC",
                    Clauses.OrderBy);
            }

            [Fact]
            public void TheFromClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.From);
            }

            [Fact]
            public void TheGroupByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.GroupBy);
            }

            [Fact]
            public void TheOrderByClauseShouldBeSet()
            {
                Assert.Equal("Column2 DESC", this.sqlString.OrderBy);
            }

            [Fact]
            public void TheSelectClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Select);
            }

            [Fact]
            public void TheWhereClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Where);
            }
        }

        public class WhenParsingCommandText_ContainingAllClauses_SpecifyingClausesSelect
        {
            private readonly SqlString sqlString;

            public WhenParsingCommandText_ContainingAllClauses_SpecifyingClausesSelect()
            {
                this.sqlString = SqlString.Parse(
                    "SELECT Column1, Column2, Column3 FROM Table WHERE Column1 = @p0 AND Column2 > @p1 GROUP BY Column3 ORDER BY Column2 DESC",
                    Clauses.Select);
            }

            [Fact]
            public void TheFromClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.From);
            }

            [Fact]
            public void TheGroupByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.GroupBy);
            }

            [Fact]
            public void TheOrderByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.OrderBy);
            }

            [Fact]
            public void TheSelectClauseShouldBeSet()
            {
                Assert.Equal("Column1, Column2, Column3", this.sqlString.Select);
            }

            [Fact]
            public void TheWhereClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Where);
            }
        }

        public class WhenParsingCommandText_ContainingAllClauses_SpecifyingClausesWhere
        {
            private readonly SqlString sqlString;

            public WhenParsingCommandText_ContainingAllClauses_SpecifyingClausesWhere()
            {
                this.sqlString = SqlString.Parse(
                    "SELECT Column1, Column2, Column3 FROM Table WHERE Column1 = @p0 AND Column2 > @p1 GROUP BY Column3 ORDER BY Column2 DESC",
                    Clauses.Where);
            }

            [Fact]
            public void TheFromClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.From);
            }

            [Fact]
            public void TheGroupByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.GroupBy);
            }

            [Fact]
            public void TheOrderByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.OrderBy);
            }

            [Fact]
            public void TheSelectClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Select);
            }

            [Fact]
            public void TheWhereClauseShouldBeSet()
            {
                Assert.Equal("Column1 = @p0 AND Column2 > @p1", this.sqlString.Where);
            }
        }

        public class WhenParsingCommandText_ContainingGroupByWithoutWhereOrOrderBy_SpecifyingClausesFrom
        {
            private readonly SqlString sqlString;

            public WhenParsingCommandText_ContainingGroupByWithoutWhereOrOrderBy_SpecifyingClausesFrom()
            {
                this.sqlString = SqlString.Parse(
                    "SELECT Column1, Column2, Column3 FROM Table GROUP BY Column3",
                    Clauses.From);
            }

            [Fact]
            public void TheFromClauseShouldBeSet()
            {
                Assert.Equal("Table", this.sqlString.From);
            }

            [Fact]
            public void TheGroupByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.GroupBy);
            }

            [Fact]
            public void TheOrderByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.OrderBy);
            }

            [Fact]
            public void TheSelectClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Select);
            }

            [Fact]
            public void TheWhereClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Where);
            }
        }

        public class WhenParsingCommandText_ContainingOrderByWithoutWhereOrGroupBy_SpecifyingClausesFrom
        {
            private readonly SqlString sqlString;

            public WhenParsingCommandText_ContainingOrderByWithoutWhereOrGroupBy_SpecifyingClausesFrom()
            {
                this.sqlString = SqlString.Parse(
                    "SELECT Column1, Column2, Column3 FROM Table ORDER BY Column2 DESC",
                    Clauses.From);
            }

            [Fact]
            public void TheFromClauseShouldBeSet()
            {
                Assert.Equal("Table", this.sqlString.From);
            }

            [Fact]
            public void TheGroupByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.GroupBy);
            }

            [Fact]
            public void TheOrderByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.OrderBy);
            }

            [Fact]
            public void TheSelectClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Select);
            }

            [Fact]
            public void TheWhereClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Where);
            }
        }

        public class WhenParsingCommandText_ContainingWhereAndGroupByWithoutOrderBy_SpecifyingClausesWhere
        {
            private readonly SqlString sqlString;

            public WhenParsingCommandText_ContainingWhereAndGroupByWithoutOrderBy_SpecifyingClausesWhere()
            {
                this.sqlString = SqlString.Parse(
                    "SELECT Column1, Column2, Column3 FROM Table WHERE Column1 = @p0 AND Column2 > @p1 GROUP BY Column3",
                    Clauses.Where);
            }

            [Fact]
            public void TheFromClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.From);
            }

            [Fact]
            public void TheGroupByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.GroupBy);
            }

            [Fact]
            public void TheOrderByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.OrderBy);
            }

            [Fact]
            public void TheSelectClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Select);
            }

            [Fact]
            public void TheWhereClauseShouldBeSet()
            {
                Assert.Equal("Column1 = @p0 AND Column2 > @p1", this.sqlString.Where);
            }
        }

        public class WhenParsingCommandText_ContainingWhereAndOrderByWithoutGroupBy_SpecifyingClausesWhere
        {
            private readonly SqlString sqlString;

            public WhenParsingCommandText_ContainingWhereAndOrderByWithoutGroupBy_SpecifyingClausesWhere()
            {
                this.sqlString = SqlString.Parse(
                    "SELECT Column1, Column2, Column3 FROM Table WHERE Column1 = @p0 AND Column2 > @p1 ORDER BY Column2 DESC",
                    Clauses.Where);
            }

            [Fact]
            public void TheFromClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.From);
            }

            [Fact]
            public void TheGroupByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.GroupBy);
            }

            [Fact]
            public void TheOrderByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.OrderBy);
            }

            [Fact]
            public void TheSelectClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Select);
            }

            [Fact]
            public void TheWhereClauseShouldBeSet()
            {
                Assert.Equal("Column1 = @p0 AND Column2 > @p1", this.sqlString.Where);
            }
        }

        public class WhenParsingCommandText_ContainingWhereWithGroupByWithoutOrderBy_SpecifyingClausesFrom
        {
            private readonly SqlString sqlString;

            public WhenParsingCommandText_ContainingWhereWithGroupByWithoutOrderBy_SpecifyingClausesFrom()
            {
                this.sqlString = SqlString.Parse(
                    "SELECT Column1, Column2, Column3 FROM Table WHERE Column1 = @p0 AND Column2 > @p1 GROUP BY Column3",
                    Clauses.From);
            }

            [Fact]
            public void TheFromClauseShouldBeSet()
            {
                Assert.Equal("Table", this.sqlString.From);
            }

            [Fact]
            public void TheGroupByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.GroupBy);
            }

            [Fact]
            public void TheOrderByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.OrderBy);
            }

            [Fact]
            public void TheSelectClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Select);
            }

            [Fact]
            public void TheWhereClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Where);
            }
        }

        public class WhenParsingCommandText_ContainingWhereWithOrderByWithoutGroupBy_SpecifyingClausesFrom
        {
            private readonly SqlString sqlString;

            public WhenParsingCommandText_ContainingWhereWithOrderByWithoutGroupBy_SpecifyingClausesFrom()
            {
                this.sqlString = SqlString.Parse(
                    "SELECT Column1, Column2, Column3 FROM Table WHERE Column1 = @p0 AND Column2 > @p1 ORDER BY Column2 DESC",
                    Clauses.From);
            }

            [Fact]
            public void TheFromClauseShouldBeSet()
            {
                Assert.Equal("Table", this.sqlString.From);
            }

            [Fact]
            public void TheGroupByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.GroupBy);
            }

            [Fact]
            public void TheOrderByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.OrderBy);
            }

            [Fact]
            public void TheSelectClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Select);
            }

            [Fact]
            public void TheWhereClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Where);
            }
        }

        public class WhenParsingCommandText_ContainingWhereWithoutGroupByOrOrderBy_SpecifyingClausesFrom
        {
            private readonly SqlString sqlString;

            public WhenParsingCommandText_ContainingWhereWithoutGroupByOrOrderBy_SpecifyingClausesFrom()
            {
                this.sqlString = SqlString.Parse(
                    "SELECT Column1, Column2, Column3 FROM Table WHERE Column1 = @p0 AND Column2 > @p1",
                    Clauses.From);
            }

            [Fact]
            public void TheFromClauseShouldBeSet()
            {
                Assert.Equal("Table", this.sqlString.From);
            }

            [Fact]
            public void TheGroupByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.GroupBy);
            }

            [Fact]
            public void TheOrderByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.OrderBy);
            }

            [Fact]
            public void TheSelectClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Select);
            }

            [Fact]
            public void TheWhereClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Where);
            }
        }

        public class WhenParsingCommandText_ContainingWhereWithoutGroupByOrOrderBy_SpecifyingClausesWhere
        {
            private readonly SqlString sqlString;

            public WhenParsingCommandText_ContainingWhereWithoutGroupByOrOrderBy_SpecifyingClausesWhere()
            {
                this.sqlString = SqlString.Parse(
                    "SELECT Column1, Column2, Column3 FROM Table WHERE Column1 = @p0 AND Column2 > @p1",
                    Clauses.Where);
            }

            [Fact]
            public void TheFromClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.From);
            }

            [Fact]
            public void TheGroupByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.GroupBy);
            }

            [Fact]
            public void TheOrderByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.OrderBy);
            }

            [Fact]
            public void TheSelectClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Select);
            }

            [Fact]
            public void TheWhereClauseShouldBeSet()
            {
                Assert.Equal("Column1 = @p0 AND Column2 > @p1", this.sqlString.Where);
            }
        }

        public class WhenParsingEmptyCommandText
        {
            private readonly SqlString sqlString;

            public WhenParsingEmptyCommandText()
            {
                this.sqlString = SqlString.Parse(
                    string.Empty,
                    Clauses.Select | Clauses.From | Clauses.Where | Clauses.GroupBy | Clauses.OrderBy);
            }

            [Fact]
            public void TheFromClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.From);
            }

            [Fact]
            public void TheGroupByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.GroupBy);
            }

            [Fact]
            public void TheOrderByClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.OrderBy);
            }

            [Fact]
            public void TheSelectClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Select);
            }

            [Fact]
            public void TheWhereClauseShouldBeEmpty()
            {
                Assert.Equal(string.Empty, this.sqlString.Where);
            }
        }

        public class WhenParsingMultiLineCommandText_ContainingAllClauses_SpecifyingAllClauses
        {
            private readonly SqlString sqlString;

            public WhenParsingMultiLineCommandText_ContainingAllClauses_SpecifyingAllClauses()
            {
                this.sqlString = SqlString.Parse(
                    @"SELECT Column1, Column2, Column3
FROM Table
WHERE Column1 = @p0 AND Column2 > @p1
GROUP BY Column3
ORDER BY Column2 DESC",
                    Clauses.Select | Clauses.From | Clauses.Where | Clauses.GroupBy | Clauses.OrderBy);
            }

            [Fact]
            public void TheFromClauseShouldBeSet()
            {
                Assert.Equal("Table", this.sqlString.From);
            }

            [Fact]
            public void TheGroupByClauseShouldBeSet()
            {
                Assert.Equal("Column3", this.sqlString.GroupBy);
            }

            [Fact]
            public void TheOrderByClauseShouldBeSet()
            {
                Assert.Equal("Column2 DESC", this.sqlString.OrderBy);
            }

            [Fact]
            public void TheSelectClauseShouldBeSet()
            {
                Assert.Equal("Column1, Column2, Column3", this.sqlString.Select);
            }

            [Fact]
            public void TheWhereClauseShouldBeSet()
            {
                Assert.Equal("Column1 = @p0 AND Column2 > @p1", this.sqlString.Where);
            }
        }
    }
}