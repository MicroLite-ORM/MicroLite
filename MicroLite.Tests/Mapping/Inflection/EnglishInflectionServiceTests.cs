namespace MicroLite.Tests.Mapping
{
    using MicroLite.Mapping.Inflection;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="EnglishInflectionService"/> class.
    /// </summary>
    public class EnglishInflectionServiceTests
    {
        [Fact]
        public void ChangesWordEndingLfToEndVes()
        {
            Assert.Equal("Elves", InflectionService.English.ToPlural("Elf"));
        }

        [Fact]
        public void ChangesWordEndingManToEndMen()
        {
            Assert.Equal("Women", InflectionService.English.ToPlural("Woman"));
        }

        [Fact]
        public void CorrectlyChangesSpecialCases()
        {
            Assert.Equal("People", InflectionService.English.ToPlural("Person"));
            Assert.Equal("Children", InflectionService.English.ToPlural("Child"));
            Assert.Equal("Mice", InflectionService.English.ToPlural("Mouse"));
            Assert.Equal("Slices", InflectionService.English.ToPlural("Slice"));
            Assert.Equal("Viri", InflectionService.English.ToPlural("Virus"));
        }

        [Fact]
        public void CorrectlyChangesStandardWordsByAppendingAnS()
        {
            Assert.Equal("Customers", InflectionService.English.ToPlural("Customer"));
            Assert.Equal("Invoices", InflectionService.English.ToPlural("Invoice"));
        }

        [Fact]
        public void CorrectlyChangesWordsEndingYToEndIes()
        {
            Assert.Equal("Stories", InflectionService.English.ToPlural("Story"));
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
                Assert.Equal(word, InflectionService.English.ToPlural(word));
            }
        }

        [Fact]
        public void EmptyStringIsNotModified()
        {
            Assert.Equal(string.Empty, InflectionService.English.ToPlural(string.Empty));
        }
    }
}