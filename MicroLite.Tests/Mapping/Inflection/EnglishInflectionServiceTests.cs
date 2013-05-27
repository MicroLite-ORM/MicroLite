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
            var inflectionService = new EnglishInflectionService();
            Assert.Equal("Elves", inflectionService.ToPlural("Elf"));
        }

        [Fact]
        public void ChangesWordEndingManToEndMen()
        {
            var inflectionService = new EnglishInflectionService();
            Assert.Equal("Women", inflectionService.ToPlural("Woman"));
        }

        [Fact]
        public void CorrectlyChangesSpecialCases()
        {
            var inflectionService = new EnglishInflectionService();
            Assert.Equal("People", inflectionService.ToPlural("Person"));
            Assert.Equal("Children", inflectionService.ToPlural("Child"));
            Assert.Equal("Mice", inflectionService.ToPlural("Mouse"));
            Assert.Equal("Slices", inflectionService.ToPlural("Slice"));
            Assert.Equal("Viri", inflectionService.ToPlural("Virus"));
        }

        [Fact]
        public void CorrectlyChangesStandardWordsByAppendingAnS()
        {
            var inflectionService = new EnglishInflectionService();
            Assert.Equal("Customers", inflectionService.ToPlural("Customer"));
            Assert.Equal("Invoices", inflectionService.ToPlural("Invoice"));
        }

        [Fact]
        public void CorrectlyChangesWordsEndingYToEndIes()
        {
            var inflectionService = new EnglishInflectionService();
            Assert.Equal("Stories", inflectionService.ToPlural("Story"));
        }

        [Fact]
        public void DoesNotTryToPluralizeWordsWithNoPluralVersion()
        {
            var inflectionService = new EnglishInflectionService();
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
                Assert.Equal(word, inflectionService.ToPlural(word));
            }
        }

        [Fact]
        public void EmptyStringIsNotModified()
        {
            var inflectionService = new EnglishInflectionService();
            Assert.Equal(string.Empty, inflectionService.ToPlural(string.Empty));
        }

        [Fact]
        public void WordsAddedUsingAddInvariantWordAreNotPluralized()
        {
            var inflectionService = new EnglishInflectionService();
            inflectionService.AddInvariantWord("Test");

            Assert.Equal("Test", inflectionService.ToPlural("Test"));
        }

        [Fact]
        public void WordsCoveredByAddedRulesArePluralizedAccordingToThoseRules()
        {
            var inflectionService = new EnglishInflectionService();
            inflectionService.AddRule("(.+)", @"$1zzz");

            Assert.Equal("Customerzzz", inflectionService.ToPlural("Customer"));
        }
    }
}