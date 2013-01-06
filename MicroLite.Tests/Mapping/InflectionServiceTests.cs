namespace MicroLite.Tests.Mapping
{
    using MicroLite.Mapping;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="InflectionService"/> class.
    /// </summary>
    public class InflectionServiceTests
    {
        [Fact]
        public void ChangesWordEndingLfToEndVes()
        {
            Assert.Equal("Elves", InflectionService.ToPlural("Elf"));
        }

        [Fact]
        public void ChangesWordEndingManToEndMen()
        {
            Assert.Equal("Women", InflectionService.ToPlural("Woman"));
        }

        [Fact]
        public void CorrectlyChangesSpecialCases()
        {
            Assert.Equal("People", InflectionService.ToPlural("Person"));
            Assert.Equal("Children", InflectionService.ToPlural("Child"));
            Assert.Equal("Mice", InflectionService.ToPlural("Mouse"));
            Assert.Equal("Slices", InflectionService.ToPlural("Slice"));
            Assert.Equal("Viri", InflectionService.ToPlural("Virus"));
        }

        [Fact]
        public void CorrectlyChangesStandardWordsByAppendingAnS()
        {
            Assert.Equal("Customers", InflectionService.ToPlural("Customer"));
            Assert.Equal("Invoices", InflectionService.ToPlural("Invoice"));
        }

        [Fact]
        public void CorrectlyChangesWordsEndingYToEndIes()
        {
            Assert.Equal("Stories", InflectionService.ToPlural("Story"));
        }

        [Fact]
        public void DoesNotTryToPluralizeWordsWithNoPluralVersion()
        {
            var unpluralizableWords = new[]
            {
                "Equipment",
                "Information",
                "Money",
                "Species",
                "Series"
            };

            foreach (var word in unpluralizableWords)
            {
                Assert.Equal(word, InflectionService.ToPlural(word));
            }
        }

        [Fact]
        public void EmptyStringIsNotModified()
        {
            Assert.Equal(string.Empty, InflectionService.ToPlural(string.Empty));
        }
    }
}