namespace MicroLite.Tests
{
    using System;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="PagingOptions"/> struct.
    /// </summary>
    public class PagingOptionsTests
    {
        public class PagingOptionsNone
        {
            private readonly PagingOptions none = PagingOptions.None;

            [Fact]
            public void ShouldHaveZeroCount()
            {
                Assert.Equal(0, none.Count);
            }

            [Fact]
            public void ShouldHaveZeroOffset()
            {
                Assert.Equal(0, none.Offset);
            }
        }

        public class WhenCallingEqualsAndTheCountAndOffsetMatch
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var pagingOptions1 = PagingOptions.SkipTake(10, 25);
                var pagingOptions2 = PagingOptions.SkipTake(10, 25);

                Assert.True(pagingOptions1 == pagingOptions2);
            }
        }

        public class WhenCallingEqualsAndTheCountDiffers
        {
            [Fact]
            public void FalseShouldBeReturned()
            {
                var pagingOptions1 = PagingOptions.SkipTake(10, 25);
                var pagingOptions2 = PagingOptions.SkipTake(20, 25);

                Assert.False(pagingOptions1 == pagingOptions2);
            }
        }

        public class WhenCallingEqualsAndTheOffsetDiffers
        {
            [Fact]
            public void FalseShouldBeReturned()
            {
                var pagingOptions1 = PagingOptions.SkipTake(10, 25);
                var pagingOptions2 = PagingOptions.SkipTake(10, 50);

                Assert.False(pagingOptions1 == pagingOptions2);
            }
        }

        public class WhenCallingEqualsAndTheOtherObjectIsABoxedInstanceOfPagingOptions
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var pagingOptions1 = PagingOptions.SkipTake(10, 25);
                var pagingOptions2 = (object)PagingOptions.SkipTake(10, 25);

                Assert.True(pagingOptions1.Equals(pagingOptions2));
            }
        }

        public class WhenCallingEqualsAndTheOtherObjectIsNotAnInstanceOfPagingOptions
        {
            [Fact]
            public void FalseShouldBeReturned()
            {
                var pagingOptions1 = PagingOptions.SkipTake(10, 25);
                var pagingOptions2 = new object();

                Assert.False(pagingOptions1.Equals(pagingOptions2));
            }
        }

        public class WhenCallingForPageAndPageIsBelowOne
        {
            [Fact]
            public void AnArgumentOutOfRangeExceptionShouldBeThrown()
            {
                var exception = Assert.Throws<ArgumentOutOfRangeException>(() => PagingOptions.ForPage(0, 10));

                Assert.Equal("page", exception.ParamName);
                Assert.True(exception.Message.StartsWith(Messages.PagingOptions_PagesMustBeAtleastOne));
            }
        }

        public class WhenCallingForPageAndResultsPerPageIsBelowOne
        {
            [Fact]
            public void AnArgumentOutOfRangeExceptionShouldBeThrown()
            {
                var exception = Assert.Throws<ArgumentOutOfRangeException>(() => PagingOptions.ForPage(5, 0));

                Assert.Equal("resultsPerPage", exception.ParamName);
                Assert.True(exception.Message.StartsWith(Messages.PagingOptions_ResultsPerPageMustBeAtLeast1));
            }
        }

        public class WhenCallingForPageForTheFirstPage
        {
            private readonly int page = 1;
            private readonly PagingOptions pagingOptions;
            private readonly int resultsPerPage = 50;

            public WhenCallingForPageForTheFirstPage()
            {
                pagingOptions = PagingOptions.ForPage(this.page, this.resultsPerPage);
            }

            [Fact]
            public void TheCountShouldEqualTheResultsPerPage()
            {
                Assert.Equal(this.resultsPerPage, this.pagingOptions.Count);
            }

            [Fact]
            public void TheOffsetShouldEqualZero()
            {
                Assert.Equal(0, this.pagingOptions.Offset);
            }
        }

        public class WhenCallingForPageForTheSecondPage
        {
            private readonly int page = 2;
            private readonly PagingOptions pagingOptions;
            private readonly int resultsPerPage = 50;

            public WhenCallingForPageForTheSecondPage()
            {
                pagingOptions = PagingOptions.ForPage(this.page, this.resultsPerPage);
            }

            [Fact]
            public void TheCountShouldEqualTheResultsPerPage()
            {
                Assert.Equal(this.resultsPerPage, this.pagingOptions.Count);
            }

            [Fact]
            public void TheOffsetShouldEqualThePageNumberLessOneMultipliedByTheResultsPerPage()
            {
                Assert.Equal(50, this.pagingOptions.Offset);
            }
        }

        public class WhenCallingGetHashCode
        {
            private readonly int count = 50;
            private readonly int offset = 100;
            private readonly PagingOptions pagingOptions;

            public WhenCallingGetHashCode()
            {
                this.pagingOptions = PagingOptions.SkipTake(this.offset, this.count);
            }

            [Fact]
            public void TheHashCodeOfTheCountShiftedByTheOffsetShouldBeReturned()
            {
                Assert.Equal(this.count.GetHashCode() ^ this.offset.GetHashCode(), this.pagingOptions.GetHashCode());
            }
        }

        public class WhenCallingNotEqualsAndTheCountAndOffsetMatch
        {
            [Fact]
            public void FalseShouldBeReturned()
            {
                var pagingOptions1 = PagingOptions.ForPage(page: 10, resultsPerPage: 25);
                var pagingOptions2 = PagingOptions.ForPage(page: 10, resultsPerPage: 25);

                Assert.False(pagingOptions1 != pagingOptions2);
            }
        }

        public class WhenCallingNotEqualsAndTheCountDiffers
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var pagingOptions1 = PagingOptions.ForPage(page: 10, resultsPerPage: 25);
                var pagingOptions2 = PagingOptions.ForPage(page: 10, resultsPerPage: 50);

                Assert.True(pagingOptions1 != pagingOptions2);
            }
        }

        public class WhenCallingNotEqualsAndTheOffsetDiffers
        {
            [Fact]
            public void TrueShouldBeReturned()
            {
                var pagingOptions1 = PagingOptions.ForPage(page: 10, resultsPerPage: 25);
                var pagingOptions2 = PagingOptions.ForPage(page: 10, resultsPerPage: 50);

                Assert.True(pagingOptions1 != pagingOptions2);
            }
        }

        public class WhenCallingSkipTake
        {
            private readonly PagingOptions pagingOptions;
            private readonly int skip = 100;
            private readonly int take = 50;

            public WhenCallingSkipTake()
            {
                pagingOptions = PagingOptions.SkipTake(this.skip, this.take);
            }

            [Fact]
            public void TheCountShouldEqualTheTake()
            {
                Assert.Equal(this.take, this.pagingOptions.Count);
            }

            [Fact]
            public void TheOffsetShouldEqualTheSkip()
            {
                Assert.Equal(this.skip, this.pagingOptions.Offset);
            }
        }

        public class WhenCallingSkipTakeAndSkipIsBelowZero
        {
            [Fact]
            public void AnArgumentOutOfRangeExceptionShouldBeThrown()
            {
                var exception = Assert.Throws<ArgumentOutOfRangeException>(() => PagingOptions.SkipTake(-1, 10));

                Assert.Equal("skip", exception.ParamName);
                Assert.True(exception.Message.StartsWith(Messages.PagingOptions_SkipMustBeZeroOrAbove));
            }
        }

        public class WhenCallingSkipTakeAndTakeIsBelowOne
        {
            [Fact]
            public void AnArgumentOutOfRangeExceptionShouldBeThrown()
            {
                var exception = Assert.Throws<ArgumentOutOfRangeException>(() => PagingOptions.SkipTake(10, 0));

                Assert.Equal("take", exception.ParamName);
                Assert.True(exception.Message.StartsWith(Messages.PagingOptions_TakeMustBeZeroOrAbove));
            }
        }
    }
}