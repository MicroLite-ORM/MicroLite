namespace MicroLite.Tests.Builder
{
    using System;
    using System.Data;
    using MicroLite.Builder;
    using MicroLite.Characters;
    using MicroLite.FrameworkExtensions;
    using MicroLite.Mapping;
    using MicroLite.Tests.TestEntities;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="SelectSqlBuilder"/> class.
    /// </summary>
    public class SelectSqlBuilderTests : UnitTest
    {
        public SelectSqlBuilderTests()
        {
            ObjectInfo.MappingConvention = new ConventionMappingConvention(
                UnitTest.GetConventionMappingSettings(IdentifierStrategy.DbGenerated));
        }

        [Fact]
        public void AndWhereThrowsArgumentNullExceptionForEmptyArgs()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.AndWhere("x = ?", null));

            Assert.Equal("args", exception.ParamName);
        }

        [Fact]
        public void AndWhereThrowsArgumentExceptionForEmptyColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.AndWhere(""));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"), exception.Message);
        }

        [Fact]
        public void AndWhereThrowsArgumentExceptionForEmptyPredicate()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.AndWhere("", new object()));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("predicate"), exception.Message);
        }

        [Fact]
        public void AndWhereThrowsArgumentExceptionForNullColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.AndWhere(null));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"), exception.Message);
        }

        [Fact]
        public void AndWhereThrowsArgumentExceptionForNullPredicate()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.AndWhere(null, new object()));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("predicate"), exception.Message);
        }

        [Fact]
        public void AverageThrowsArgumentExceptionForNullColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.Average(null));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("columnName"), exception.Message);
        }

        [Fact]
        public void AverageThrowsArgumentExceptionForNullColumnAlias()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.Average("Column", null));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("columnAlias"), exception.Message);
        }

        /// <summary>
        /// Issue #223 - SqlBuilder Between is not appending AND or OR
        /// </summary>
        [Fact]
        public void BetweenShouldAppendOperandIfItIsAnAdditionalPredicate()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2").In("Opt1", "Opt2")
                .AndWhere("Column3").Between(1, 10)
                .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column2 IN (?,?)) AND (Column3 BETWEEN ? AND ?)", sqlQuery.CommandText);
            Assert.Equal(4, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Opt1", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[1].DbType);
            Assert.Equal("Opt2", sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(1, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal(10, sqlQuery.Arguments[3].Value);
        }

        [Fact]
        public void BetweenThrowsArgumentExceptionForNullLower()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.Between(null, 10));

            Assert.Equal("lower", exception.ParamName);
        }

        [Fact]
        public void BetweenThrowsArgumentExceptionForNullUpper()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.Between(1, null));

            Assert.Equal("upper", exception.ParamName);
        }

        [Fact]
        public void CountThrowsArgumentExceptionForNullColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.Count(null));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("columnName"), exception.Message);
        }

        [Fact]
        public void CountThrowsArgumentExceptionForNullColumnAlias()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.Count("Column", null));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("columnAlias"), exception.Message);
        }

        [Fact]
        public void DistinctThrowsArgumentExceptionForNullColumnName()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.Distinct((string)null));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"), exception.Message);
        }

        [Fact]
        public void DistinctThrowsArgumentExceptionForNullColumns()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.Distinct((string[])null));

            Assert.Equal("columns", exception.ParamName);
        }

        [Fact]
        public void ExistsThrowArgumentNullExceptionForNullSqlQuery()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.From("Customer").Where().Exists(null));

            Assert.Equal("subQuery", exception.ParamName);
        }

        [Fact]
        public void FromThrowsArgumentExceptionForEmptyTableName()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.From(""));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("table"), exception.Message);
        }

        [Fact]
        public void GroupByAppendsColumnsCorrectlyForMultipleColumns()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            var sqlQuery = sqlBuilder.From("Customer")
                .GroupBy("CustomerId", "Created")
                .ToSqlQuery();

            Assert.Equal("SELECT CustomerId FROM Customer GROUP BY CustomerId,Created", sqlQuery.CommandText);
        }

        [Fact]
        public void GroupByAppendsColumnsCorrectlyForSingleColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            var sqlQuery = sqlBuilder.From("Customer")
                .GroupBy("CustomerId")
                .ToSqlQuery();

            Assert.Equal("SELECT CustomerId FROM Customer GROUP BY CustomerId", sqlQuery.CommandText);
        }

        [Fact]
        public void GroupByThrowsArgumentNullException()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            Assert.Throws<ArgumentException>(() => sqlBuilder.From("Customer").GroupBy((string)null));
            Assert.Throws<ArgumentNullException>(() => sqlBuilder.From("Customer").GroupBy((string[])null));
        }

        [Fact]
        public void HavingThrowsArgumentExceptionForEmptyPredicate()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.Having("", new object()));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("predicate"), exception.Message);
        }

        [Fact]
        public void InThrowArgumentNullExceptionForNullArgs()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.From("Customer").Where("Column").In((object[])null));

            Assert.Equal("args", exception.ParamName);
        }

        [Fact]
        public void InThrowArgumentNullExceptionForNullSqlQueries()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.From("Customer").Where("Column").In((SqlQuery[])null));

            Assert.Equal("subQueries", exception.ParamName);
        }

        [Fact]
        public void InThrowArgumentNullExceptionForNullSqlQuery()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.From("Customer").Where("Column").In((SqlQuery)null));

            Assert.Equal("subQuery", exception.ParamName);
        }

        [Fact]
        public void MaxThrowsArgumentExceptionForNullColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.Max(null));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("columnName"), exception.Message);
        }

        [Fact]
        public void MaxThrowsArgumentExceptionForNullColumnAlias()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.Max("Column", null));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("columnAlias"), exception.Message);
        }

        [Fact]
        public void MinThrowsArgumentExceptionForNullColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.Min(null));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("columnName"), exception.Message);
        }

        [Fact]
        public void MinThrowsArgumentExceptionForNullColumnAlias()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.Min("Column", null));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("columnAlias"), exception.Message);
        }

        [Fact]
        public void NotBetweenThrowsArgumentExceptionForNullLower()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.NotBetween(null, 10));

            Assert.Equal("lower", exception.ParamName);
        }

        [Fact]
        public void NotBetweenThrowsArgumentExceptionForNullUpper()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.NotBetween(1, null));

            Assert.Equal("upper", exception.ParamName);
        }

        [Fact]
        public void NotExistsThrowArgumentNullExceptionForNullSqlQuery()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.From("Customer").Where().NotExists(null));

            Assert.Equal("subQuery", exception.ParamName);
        }

        [Fact]
        public void NotInThrowsArgumentNullExceptionForNullArgs()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.From("Customer").Where("Column").NotIn((object[])null));
        }

        [Fact]
        public void NotInThrowsArgumentNullExceptionForNullSqlQueries()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.From("Customer").Where("Column").NotIn((SqlQuery[])null));
        }

        [Fact]
        public void NotInThrowsArgumentNullExceptionForNullSqlQuery()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.From("Customer").Where("Column").NotIn((SqlQuery)null));
        }

        [Fact]
        public void OrderByAscendingAppendsColumnsCorrectlyForMultipleColumns()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            var sqlQuery = sqlBuilder.From("Customer")
                .OrderByAscending("FirstName", "LastName")
                .ToSqlQuery();

            Assert.Equal("SELECT CustomerId FROM Customer ORDER BY FirstName ASC,LastName ASC", sqlQuery.CommandText);
        }

        [Fact]
        public void OrderByAscendingAppendsColumnsCorrectlyForSingleColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            var sqlQuery = sqlBuilder.From("Customer")
                .OrderByAscending("CustomerId")
                .ToSqlQuery();

            Assert.Equal("SELECT CustomerId FROM Customer ORDER BY CustomerId ASC", sqlQuery.CommandText);
        }

        [Fact]
        public void OrderByAscendingThrowsArgumentNullException()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            Assert.Throws<ArgumentException>(
                () => sqlBuilder.From("Customer").OrderByAscending((string)null));

            Assert.Throws<ArgumentException>(
                () => sqlBuilder.From("Customer").OrderByAscending(""));

            Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.From("Customer").OrderByAscending((string[])null));
        }

        [Fact]
        public void OrderByDescendingAppendsColumnsCorrectlyForMultipleColumns()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            var sqlQuery = sqlBuilder.From("Customer")
                .OrderByDescending("FirstName", "LastName")
                .ToSqlQuery();

            Assert.Equal("SELECT CustomerId FROM Customer ORDER BY FirstName DESC,LastName DESC", sqlQuery.CommandText);
        }

        [Fact]
        public void OrderByDescendingAppendsColumnsCorrectlyForSingleColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            var sqlQuery = sqlBuilder.From("Customer")
                .OrderByDescending("CustomerId")
                .ToSqlQuery();

            Assert.Equal("SELECT CustomerId FROM Customer ORDER BY CustomerId DESC", sqlQuery.CommandText);
        }

        [Fact]
        public void OrderByDescendingThrowsArgumentNullException()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            Assert.Throws<ArgumentException>(
                () => sqlBuilder.From("Customer").OrderByDescending((string)null));

            Assert.Throws<ArgumentException>(
                () => sqlBuilder.From("Customer").OrderByDescending(string.Empty));

            Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.From("Customer").OrderByDescending((string[])null));
        }

        [Fact]
        public void OrWhereThrowsArgumentNullExceptionForEmptyArgs()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentNullException>(
                () => sqlBuilder.OrWhere("x = ?", null));

            Assert.Equal("args", exception.ParamName);
        }

        [Fact]
        public void OrWhereThrowsArgumentExceptionForEmptyColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.OrWhere(""));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"), exception.Message);
        }

        [Fact]
        public void OrWhereThrowsArgumentExceptionForEmptyPredicate()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.OrWhere("", new object()));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("predicate"), exception.Message);
        }

        [Fact]
        public void OrWhereThrowsArgumentExceptionForNullColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.OrWhere(null));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"), exception.Message);
        }

        [Fact]
        public void OrWhereThrowsArgumentExceptionForNullPredicate()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.OrWhere(null, new object()));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("predicate"), exception.Message);
        }

        [Fact]
        public void SelectAverage()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, (string)null);

            var sqlQuery = sqlBuilder
                .Average("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal("SELECT AVG(CreditLimit) AS CreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectAverageWithAlias()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, (string)null);

            var sqlQuery = sqlBuilder
                .Average("CreditLimit", columnAlias: "AverageCreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal("SELECT AVG(CreditLimit) AS AverageCreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectAverageWithOtherColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Id");

            var sqlQuery = sqlBuilder
                .Average("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .GroupBy("CustomerStatusId")
                .ToSqlQuery();

            Assert.Equal("SELECT Id,AVG(CreditLimit) AS CreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?) GROUP BY CustomerStatusId", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectAverageWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, (string)null);

            var sqlQuery = sqlBuilder
                .Average("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal("SELECT AVG([CreditLimit]) AS CreditLimit FROM [Sales].[Customers] WHERE ([CustomerStatusId] = @p0)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0].Value);
        }

        /// <summary>
        /// Issue #221 - SqlBuilder does not allow chaining multiple function calls.
        /// </summary>
        [Fact]
        public void SelectChainingMultipleFunctions()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .Count("Column2", "Col2")
                .Max("Column3", "Col3")
                .From("Table")
                .ToSqlQuery();

            Assert.Equal("SELECT Column1,COUNT(Column2) AS Col2,MAX(Column3) AS Col3 FROM Table", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void SelectCount()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, (string)null);

            var sqlQuery = sqlBuilder
                .Count("Id")
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Equal("SELECT COUNT(Id) AS Id FROM Sales.Customers", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void SelectCountWithAlias()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, (string)null);

            var sqlQuery = sqlBuilder
                .Count("Id", columnAlias: "CustomerCount")
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Equal("SELECT COUNT(Id) AS CustomerCount FROM Sales.Customers", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void SelectCountWithOtherColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerStatusId");

            var sqlQuery = sqlBuilder
                .Count("Id")
                .From(typeof(Customer))
                .GroupBy("CustomerStatusId")
                .ToSqlQuery();

            Assert.Equal("SELECT CustomerStatusId,COUNT(Id) AS Id FROM Sales.Customers GROUP BY CustomerStatusId", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void SelectCountWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, (string)null);

            var sqlQuery = sqlBuilder
                .Count("Id")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal("SELECT COUNT([Id]) AS Id FROM [Sales].[Customers] WHERE ([CustomerStatusId] = @p0)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectDistinctColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, (string)null);

            var sqlQuery = sqlBuilder
                .Distinct("CreditLimit")
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Equal("SELECT DISTINCT CreditLimit FROM Sales.Customers", sqlQuery.CommandText);
            Assert.Equal(0, sqlQuery.Arguments.Count);
        }

        [Fact]
        public void SelectDistinctColumnsWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, (string)null);

            var sqlQuery = sqlBuilder
                .Distinct("CreditLimit", "DateOfBirth")
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Equal("SELECT DISTINCT [CreditLimit],[DateOfBirth] FROM [Sales].[Customers]", sqlQuery.CommandText);
            Assert.Equal(0, sqlQuery.Arguments.Count);
        }

        [Fact]
        public void SelectDistinctColumnWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, (string)null);

            var sqlQuery = sqlBuilder
                .Distinct("CreditLimit")
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Equal("SELECT DISTINCT [CreditLimit] FROM [Sales].[Customers]", sqlQuery.CommandText);
            Assert.Equal(0, sqlQuery.Arguments.Count);
        }

        [Fact]
        public void SelectFromOrderByAscending()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .OrderByAscending("Column1", "Column2")
                .ToSqlQuery();

            Assert.Equal("SELECT Column1,Column2 FROM Table ORDER BY Column1 ASC,Column2 ASC", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        /// <summary>
        /// Issue #93 - SqlBuilder produces invalid Sql if OrderBy is called multiple times.
        /// </summary>
        [Fact]
        public void SelectFromOrderByAscendingThenDescending()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .OrderByAscending("Column1")
                .OrderByDescending("Column2")
                .ToSqlQuery();

            Assert.Equal("SELECT Column1,Column2 FROM Table ORDER BY Column1 ASC,Column2 DESC", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void SelectFromOrderByAscendingWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .OrderByAscending("Column1", "Column2")
                .ToSqlQuery();

            Assert.Equal("SELECT [Column1],[Column2] FROM [Table] ORDER BY [Column1] ASC,[Column2] ASC", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void SelectFromOrderByDescending()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .OrderByDescending("Column1", "Column2")
                .ToSqlQuery();

            Assert.Equal("SELECT Column1,Column2 FROM Table ORDER BY Column1 DESC,Column2 DESC", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        /// <summary>
        /// Issue #93 - SqlBuilder produces invalid Sql if OrderBy is called multiple times.
        /// </summary>
        [Fact]
        public void SelectFromOrderByDescendingThenAscending()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .OrderByDescending("Column1")
                .OrderByAscending("Column2")
                .ToSqlQuery();

            Assert.Equal("SELECT Column1,Column2 FROM Table ORDER BY Column1 DESC,Column2 ASC", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void SelectFromOrderByDescendingWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .OrderByDescending("Column1", "Column2")
                .ToSqlQuery();

            Assert.Equal("SELECT [Column1],[Column2] FROM [Table] ORDER BY [Column1] DESC,[Column2] DESC", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void SelectFromSpecifyingColumnsAndTableName()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .ToSqlQuery();

            Assert.Equal("SELECT Column1,Column2 FROM Table", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void SelectFromSpecifyingColumnsAndTableNameWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .ToSqlQuery();

            Assert.Equal("SELECT [Column1],[Column2] FROM [Table]", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void SelectFromSpecifyingColumnsAndType()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Name", "DateOfBirth");

            var sqlQuery = sqlBuilder
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Equal("SELECT Name,DateOfBirth FROM Sales.Customers", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void SelectFromSpecifyingColumnsAndTypeWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Name", "DateOfBirth");

            var sqlQuery = sqlBuilder
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Equal("SELECT [Name],[DateOfBirth] FROM [Sales].[Customers]", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void SelectFromType()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var sqlQuery = sqlBuilder
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Equal("SELECT Created,CreditLimit,DateOfBirth,Id,Name,CustomerStatusId,Updated,Website FROM Sales.Customers", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void SelectFromTypeWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance);

            var sqlQuery = sqlBuilder
                .From(typeof(Customer))
                .ToSqlQuery();

            Assert.Equal("SELECT [Created],[CreditLimit],[DateOfBirth],[Id],[Name],[CustomerStatusId],[Updated],[Website] FROM [Sales].[Customers]", sqlQuery.CommandText);
            Assert.Empty(sqlQuery.Arguments);
        }

        [Fact]
        public void SelectFromWhere()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1 = @p0", "Foo")
                .ToSqlQuery();

            Assert.Equal("SELECT Column1,Column2 FROM Table WHERE (Column1 = @p0)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectFromWhereAnd()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").IsEqualTo("Foo")
                .AndWhere("Column2").IsEqualTo("Bar")
                .ToSqlQuery();

            Assert.Equal("SELECT Column1,Column2 FROM Table WHERE (Column1 = ?) AND (Column2 = ?)", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[1].DbType);
            Assert.Equal("Bar", sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void SelectFromWhereComplex()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1", "Column2", "Column3");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1 = @p0 OR @p0 IS NULL", "Foo")
                .AndWhere("Column2 BETWEEN @p0 AND @p1", DateTime.Today.AddDays(-1), DateTime.Today)
                .OrWhere("Column3 IN (@p0, @p1, @p2, @p3)", 1, 2, 3, 4)
                .ToSqlQuery();

            Assert.Equal("SELECT Column1,Column2,Column3 FROM Table WHERE (Column1 = @p0 OR @p0 IS NULL) AND (Column2 BETWEEN @p1 AND @p2) OR (Column3 IN (@p3, @p4, @p5, @p6))", sqlQuery.CommandText);

            Assert.Equal(7, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.DateTime, sqlQuery.Arguments[1].DbType);
            Assert.Equal(DateTime.Today.AddDays(-1), sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.DateTime, sqlQuery.Arguments[2].DbType);
            Assert.Equal(DateTime.Today, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal(1, sqlQuery.Arguments[3].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[4].DbType);
            Assert.Equal(2, sqlQuery.Arguments[4].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[5].DbType);
            Assert.Equal(3, sqlQuery.Arguments[5].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[6].DbType);
            Assert.Equal(4, sqlQuery.Arguments[6].Value);
        }

        [Fact]
        public void SelectFromWhereOr()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1", "Column2");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").IsEqualTo("Foo")
                .OrWhere("Column2").IsEqualTo("Bar")
                .ToSqlQuery();

            Assert.Equal("SELECT Column1,Column2 FROM Table WHERE (Column1 = ?) OR (Column2 = ?)", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("Foo", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[1].DbType);
            Assert.Equal("Bar", sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void SelectMax()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, (string)null);

            var sqlQuery = sqlBuilder
                .Max("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal("SELECT MAX(CreditLimit) AS CreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectMaxWithAlias()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, (string)null);

            var sqlQuery = sqlBuilder
                .Max("CreditLimit", columnAlias: "MaxCreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal("SELECT MAX(CreditLimit) AS MaxCreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectMaxWithOtherColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Id");

            var sqlQuery = sqlBuilder
                .Max("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal("SELECT Id,MAX(CreditLimit) AS CreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectMaxWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, (string)null);

            var sqlQuery = sqlBuilder
                .Max("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal("SELECT MAX([CreditLimit]) AS CreditLimit FROM [Sales].[Customers] WHERE ([CustomerStatusId] = @p0)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectMin()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, (string)null);

            var sqlQuery = sqlBuilder
                .Min("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal("SELECT MIN(CreditLimit) AS CreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectMinWithAlias()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, (string)null);

            var sqlQuery = sqlBuilder
                .Min("CreditLimit", columnAlias: "MinCreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal("SELECT MIN(CreditLimit) AS MinCreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectMinWithOtherColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Id");

            var sqlQuery = sqlBuilder
                .Min("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal("SELECT Id,MIN(CreditLimit) AS CreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectMinWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, (string)null);

            var sqlQuery = sqlBuilder
                .Min("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal("SELECT MIN([CreditLimit]) AS CreditLimit FROM [Sales].[Customers] WHERE ([CustomerStatusId] = @p0)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectSum()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, (string)null);

            var sqlQuery = sqlBuilder
                .Sum("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal("SELECT SUM(CreditLimit) AS CreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectSumWithAlias()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, (string)null);

            var sqlQuery = sqlBuilder
                .Sum("CreditLimit", columnAlias: "SumCreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal("SELECT SUM(CreditLimit) AS SumCreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectSumWithOtherColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Id");

            var sqlQuery = sqlBuilder
                .Sum("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal("SELECT Id,SUM(CreditLimit) AS CreditLimit FROM Sales.Customers WHERE (CustomerStatusId = ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectSumWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, (string)null);

            var sqlQuery = sqlBuilder
                .Sum("CreditLimit")
                .From(typeof(Customer))
                .Where("CustomerStatusId").IsEqualTo(CustomerStatus.Active)
                .ToSqlQuery();

            Assert.Equal("SELECT SUM([CreditLimit]) AS CreditLimit FROM [Sales].[Customers] WHERE ([CustomerStatusId] = @p0)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);
            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(CustomerStatus.Active, sqlQuery.Arguments[0].Value);
        }

        /// <summary>
        /// Issue #360 - SqlBuilder is using always using the first argument
        /// </summary>
        [Fact]
        public void SelectWhere()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1 = ? AND Column2 = ?", "FOO", "Bar")
                .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 = ? AND Column2 = ?)", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.String, sqlQuery.Arguments[1].DbType);
            Assert.Equal("Bar", sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void SelectWhereAndWhereInArgs()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2").IsEqualTo("FOO")
                .AndWhere("Column1")
                .In(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column2 = ?) AND (Column1 IN (?,?,?))", sqlQuery.CommandText);

            Assert.Equal(4, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(1, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(2, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal(3, sqlQuery.Arguments[3].Value);
        }

        [Fact]
        public void SelectWhereAndWhereInArgsWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2").IsEqualTo("FOO")
                .AndWhere("Column1").In(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column2] = @p0) AND ([Column1] IN (@p1,@p2,@p3))", sqlQuery.CommandText);

            Assert.Equal(4, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(1, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(2, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal(3, sqlQuery.Arguments[3].Value);
        }

        [Fact]
        public void SelectWhereAndWhereInMultipleSqlQueries()
        {
            var subQuery1 = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);
            var subQuery2 = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 2048);

            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2 = ?", "FOO")
                .AndWhere("Column1").In(subQuery1, subQuery2)
                .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column2 = ?) AND (Column1 IN ((SELECT Id FROM Table WHERE Column = ?), (SELECT Id FROM Table WHERE Column = ?)))", sqlQuery.CommandText);

            Assert.Equal(3, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(2048, sqlQuery.Arguments[2].Value);
        }

        [Fact]
        public void SelectWhereAndWhereInSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);

            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2 = ?", "FOO")
                .AndWhere("Column1").In(subQuery)
                .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column2 = ?) AND (Column1 IN (SELECT Id FROM Table WHERE Column = ?))", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void SelectWhereAndWhereInSqlQueryWithSqlCharacters()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = @p0", 1024);

            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2 = @p0", "FOO")
                .AndWhere("Column1").In(subQuery)
                .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE (Column2 = @p0) AND ([Column1] IN (SELECT Id FROM Table WHERE Column = @p1))", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void SelectWhereAndWhereNotInArgsWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2").IsEqualTo("FOO")
                .AndWhere("Column1").NotIn(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column2] = @p0) AND ([Column1] NOT IN (@p1,@p2,@p3))", sqlQuery.CommandText);

            Assert.Equal(4, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(1, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(2, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal(3, sqlQuery.Arguments[3].Value);
        }

        [Fact]
        public void SelectWhereAndWhereNotInMultipleSqlQueries()
        {
            var subQuery1 = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);
            var subQuery2 = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 2048);

            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2 = ?", "FOO")
                .AndWhere("Column1").NotIn(subQuery1, subQuery2)
                .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column2 = ?) AND (Column1 NOT IN ((SELECT Id FROM Table WHERE Column = ?), (SELECT Id FROM Table WHERE Column = ?)))", sqlQuery.CommandText);

            Assert.Equal(3, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(2048, sqlQuery.Arguments[2].Value);
        }

        [Fact]
        public void SelectWhereAndWhereNotInSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);

            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2 = ?", "FOO")
                .AndWhere("Column1").NotIn(subQuery)
                .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column2 = ?) AND (Column1 NOT IN (SELECT Id FROM Table WHERE Column = ?))", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void SelectWhereBetween()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .Between(1, 10)
                   .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 BETWEEN ? AND ?)", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(10, sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void SelectWhereBetweenWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .Between(1, 10)
                   .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] BETWEEN @p0 AND @p1)", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(10, sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void SelectWhereColumnIsEqualToObjectValue()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsEqualTo("FOO")
                   .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 = ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereColumnIsEqualToObjectValueWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsEqualTo("FOO")
                   .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] = @p0)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereColumnIsEqualToSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Column2 FROM Table2 WHERE Column3 = ?", "FOO");

            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsEqualTo(subQuery)
                   .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 = (SELECT Column2 FROM Table2 WHERE Column3 = ?))", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereColumnIsEqualToWithSqlCharacters()
        {
            var subQuery = new SqlQuery("SELECT Column2 FROM Table2 WHERE Column3 = @p0", "FOO");

            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsEqualTo(subQuery)
                   .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] = (SELECT Column2 FROM Table2 WHERE Column3 = @p0))", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereColumnIsGreaterThan()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsGreaterThan("FOO")
                   .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 > ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereColumnIsGreaterThanOrEqualTo()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsGreaterThanOrEqualTo("FOO")
                   .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 >= ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereColumnIsGreaterThanOrEqualToWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsGreaterThanOrEqualTo("FOO")
                   .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] >= @p0)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereColumnIsGreaterThanWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsGreaterThan("FOO")
                   .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] > @p0)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereColumnIsLessThan()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsLessThan("FOO")
                   .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 < ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereColumnIsLessThanOrEqualTo()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsLessThanOrEqualTo("FOO")
                   .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 <= ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereColumnIsLessThanOrEqualToWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsLessThanOrEqualTo("FOO")
                   .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] <= @p0)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereColumnIsLessThanWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsLessThan("FOO")
                   .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] < @p0)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereColumnIsLike()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsLike("FOO%")
                   .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 LIKE ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO%", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereColumnIsLikeWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsLike("FOO%")
                   .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] LIKE @p0)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO%", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereColumnIsNotEqualToObjectValue()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsNotEqualTo("FOO")
                   .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 <> ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereColumnIsNotEqualToObjectValueWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsNotEqualTo("FOO")
                   .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] <> @p0)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereColumnIsNotEqualToSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Column2 FROM Table2 WHERE Column3 = ?", "FOO");

            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsNotEqualTo(subQuery)
                   .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 <> (SELECT Column2 FROM Table2 WHERE Column3 = ?))", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereColumnIsNotEqualToWithSqlCharacters()
        {
            var subQuery = new SqlQuery("SELECT Column2 FROM Table2 WHERE Column3 = @p0", "FOO");

            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsNotEqualTo(subQuery)
                   .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] <> (SELECT Column2 FROM Table2 WHERE Column3 = @p0))", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereColumnIsNotLike()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsNotLike("FOO%")
                   .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 NOT LIKE ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO%", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereColumnIsNotLikeWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsNotLike("FOO%")
                   .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] NOT LIKE @p0)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO%", sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereColumnIsNotNull()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsNotNull()
                   .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 IS NOT NULL)", sqlQuery.CommandText);

            Assert.Equal(0, sqlQuery.Arguments.Count);
        }

        [Fact]
        public void SelectWhereColumnIsNotNullWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsNotNull()
                   .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] IS NOT NULL)", sqlQuery.CommandText);

            Assert.Equal(0, sqlQuery.Arguments.Count);
        }

        [Fact]
        public void SelectWhereColumnIsNull()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsNull()
                   .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 IS NULL)", sqlQuery.CommandText);

            Assert.Equal(0, sqlQuery.Arguments.Count);
        }

        [Fact]
        public void SelectWhereColumnIsNullWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .IsNull()
                   .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] IS NULL)", sqlQuery.CommandText);

            Assert.Equal(0, sqlQuery.Arguments.Count);
        }

        [Fact]
        public void SelectWhereExistsSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);

            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where()
                .Exists(subQuery)
                .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE EXISTS (SELECT Id FROM Table WHERE Column = ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereGroupByHavingOrderBy()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            var sqlQuery = sqlBuilder
                .Sum("Total")
                .From("Invoices")
                .Where("OrderDate").IsGreaterThan(new DateTime(2000, 1, 1))
                .GroupBy("Total")
                .Having("SUM(Total) > ?", 10000M)
                .OrderByDescending("OrderDate")
                .ToSqlQuery();

            Assert.Equal("SELECT CustomerId,SUM(Total) AS Total FROM Invoices WHERE (OrderDate > ?) GROUP BY Total HAVING SUM(Total) > ? ORDER BY OrderDate DESC", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.DateTime, sqlQuery.Arguments[0].DbType);
            Assert.Equal(new DateTime(2000, 1, 1), sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Decimal, sqlQuery.Arguments[1].DbType);
            Assert.Equal(10000M, sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void SelectWhereGroupByOrderBy()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "CustomerId");

            var sqlQuery = sqlBuilder
                .Sum("Total")
                .From("Invoices")
                .Where("OrderDate").IsGreaterThan(new DateTime(2000, 1, 1))
                .GroupBy("Total")
                .OrderByDescending("OrderDate")
                .ToSqlQuery();

            Assert.Equal("SELECT CustomerId,SUM(Total) AS Total FROM Invoices WHERE (OrderDate > ?) GROUP BY Total ORDER BY OrderDate DESC", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.DateTime, sqlQuery.Arguments[0].DbType);
            Assert.Equal(new DateTime(2000, 1, 1), sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereGroupByOrderByWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "CustomerId");

            var sqlQuery = sqlBuilder
                .Sum("Total")
                .From("Invoices")
                .Where("OrderDate").IsGreaterThan(new DateTime(2000, 1, 1))
                .GroupBy("Total")
                .OrderByDescending("OrderDate")
                .ToSqlQuery();

            Assert.Equal("SELECT [CustomerId],SUM([Total]) AS Total FROM [Invoices] WHERE ([OrderDate] > @p0) GROUP BY [Total] ORDER BY [OrderDate] DESC", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.DateTime, sqlQuery.Arguments[0].DbType);
            Assert.Equal(new DateTime(2000, 1, 1), sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereInArgs()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1")
                .In(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 IN (?,?,?))", sqlQuery.CommandText);

            Assert.Equal(3, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(2, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(3, sqlQuery.Arguments[2].Value);
        }

        [Fact]
        public void SelectWhereInArgsWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1")
                .In(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] IN (@p0,@p1,@p2))", sqlQuery.CommandText);

            Assert.Equal(3, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(2, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(3, sqlQuery.Arguments[2].Value);
        }

        [Fact]
        public void SelectWhereInMultipleSqlQueries()
        {
            var subQuery1 = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);
            var subQuery2 = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 2048);

            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").In(subQuery1, subQuery2)
                .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 IN ((SELECT Id FROM Table WHERE Column = ?), (SELECT Id FROM Table WHERE Column = ?)))", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(2048, sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void SelectWhereInSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);

            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1")
                .In(subQuery)
                .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 IN (SELECT Id FROM Table WHERE Column = ?))", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereNotBetween()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .NotBetween(1, 10)
                   .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 NOT BETWEEN ? AND ?)", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(10, sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void SelectWhereNotBetweenWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                   .From("Table")
                   .Where("Column1")
                   .NotBetween(1, 10)
                   .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] NOT BETWEEN @p0 AND @p1)", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(10, sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void SelectWhereNotExistsSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);

            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where()
                .NotExists(subQuery)
                .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE NOT EXISTS (SELECT Id FROM Table WHERE Column = ?)", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereNotInArgs()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1")
                .NotIn(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 NOT IN (?,?,?))", sqlQuery.CommandText);

            Assert.Equal(3, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(2, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(3, sqlQuery.Arguments[2].Value);
        }

        [Fact]
        public void SelectWhereNotInArgsWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").NotIn(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column1] NOT IN (@p0,@p1,@p2))", sqlQuery.CommandText);

            Assert.Equal(3, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(2, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(3, sqlQuery.Arguments[2].Value);
        }

        [Fact]
        public void SelectWhereNotInMultipleSqlQueries()
        {
            var subQuery1 = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);
            var subQuery2 = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 2048);

            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").NotIn(subQuery1, subQuery2)
                .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 NOT IN ((SELECT Id FROM Table WHERE Column = ?), (SELECT Id FROM Table WHERE Column = ?)))", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(2048, sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void SelectWhereNotInSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);

            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column1").NotIn(subQuery)
                .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column1 NOT IN (SELECT Id FROM Table WHERE Column = ?))", sqlQuery.CommandText);

            Assert.Equal(1, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[0].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[0].Value);
        }

        [Fact]
        public void SelectWhereOrWhereInArgs()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2").IsEqualTo("FOO")
                .OrWhere("Column1")
                .In(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column2 = ?) OR (Column1 IN (?,?,?))", sqlQuery.CommandText);

            Assert.Equal(4, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(1, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(2, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal(3, sqlQuery.Arguments[3].Value);
        }

        [Fact]
        public void SelectWhereOrWhereInArgsWithSqlCharacters()
        {
            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2").IsEqualTo("FOO")
                .OrWhere("Column1").In(1, 2, 3)
                .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE ([Column2] = @p0) OR ([Column1] IN (@p1,@p2,@p3))", sqlQuery.CommandText);

            Assert.Equal(4, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(1, sqlQuery.Arguments[1].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[2].DbType);
            Assert.Equal(2, sqlQuery.Arguments[2].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[3].DbType);
            Assert.Equal(3, sqlQuery.Arguments[3].Value);
        }

        [Fact]
        public void SelectWhereOrWhereInSqlQuery()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = ?", 1024);

            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2 = ?", "FOO")
                .OrWhere("Column1")
                .In(subQuery)
                .ToSqlQuery();

            Assert.Equal("SELECT Column1 FROM Table WHERE (Column2 = ?) OR (Column1 IN (SELECT Id FROM Table WHERE Column = ?))", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[1].Value);
        }

        [Fact]
        public void SelectWhereOrWhereInSqlQueryWithSqlCharacters()
        {
            var subQuery = new SqlQuery("SELECT Id FROM Table WHERE Column = @p0", 1024);

            var sqlBuilder = new SelectSqlBuilder(MsSqlCharacters.Instance, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2 = @p0", "FOO")
                .OrWhere("Column1")
                .In(subQuery)
                .ToSqlQuery();

            Assert.Equal("SELECT [Column1] FROM [Table] WHERE (Column2 = @p0) OR ([Column1] IN (SELECT Id FROM Table WHERE Column = @p1))", sqlQuery.CommandText);

            Assert.Equal(2, sqlQuery.Arguments.Count);

            Assert.Equal(DbType.String, sqlQuery.Arguments[0].DbType);
            Assert.Equal("FOO", sqlQuery.Arguments[0].Value);

            Assert.Equal(DbType.Int32, sqlQuery.Arguments[1].DbType);
            Assert.Equal(1024, sqlQuery.Arguments[1].Value);
        }

        /// <summary>
        /// Issue #224 - SqlBuilder SingleColumn predicates not appending AND or OR
        /// </summary>
        [Fact]
        public void SingleColumnPredicatesShouldAppendOperand()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty, "Column1");

            var sqlQuery = sqlBuilder
                .From("Table")
                .Where("Column2").In("Opt1", "Opt2")
                .AndWhere("Column3").IsEqualTo(1)
                .AndWhere("Column4").IsGreaterThan(2)
                .AndWhere("Column5").IsGreaterThanOrEqualTo(3)
                .AndWhere("Column6").IsLessThan(4)
                .AndWhere("Column7").IsLessThanOrEqualTo(5)
                .AndWhere("Column8").IsLike("%J")
                .AndWhere("Column9").IsNotEqualTo(6)
                .AndWhere("Column10").IsNotNull()
                .AndWhere("Column11").IsNull()
                .ToSqlQuery();

            Assert.Equal(@"SELECT Column1 FROM Table WHERE (Column2 IN (?,?)) AND (Column3 = ?) AND (Column4 > ?) AND (Column5 >= ?) AND (Column6 < ?) AND (Column7 <= ?) AND (Column8 LIKE ?) AND (Column9 <> ?) AND (Column10 IS NOT NULL) AND (Column11 IS NULL)", sqlQuery.CommandText);
        }

        [Fact]
        public void SumThrowsArgumentExceptionForNullColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.Sum(null));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("columnName"), exception.Message);
        }

        [Fact]
        public void SumThrowsArgumentExceptionForNullColumnAlias()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.Sum("Column", null));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("columnAlias"), exception.Message);
        }

        [Fact]
        public void WhenCreatedWithoutSpecifyingAnyColumnsTheCommandTextIsSelectWildcard()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            Assert.Equal("SELECT *", sqlBuilder.ToSqlQuery().CommandText);
        }

        [Fact]
        public void WhereThrowsArgumentExceptionForNullColumn()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.Where(null));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("column"), exception.Message);
        }

        [Fact]
        public void WhereThrowsArgumentExceptionForNullPredicate()
        {
            var sqlBuilder = new SelectSqlBuilder(SqlCharacters.Empty);

            var exception = Assert.Throws<ArgumentException>(
                () => sqlBuilder.Where(null, new object[0]));

            Assert.Equal(ExceptionMessages.ArgumentNullOrEmpty.FormatWith("predicate"), exception.Message);
        }
    }
}