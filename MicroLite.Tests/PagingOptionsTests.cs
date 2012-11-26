namespace MicroLite.Tests
{
    using System;
    using NUnit.Framework;

    /// <summary>
    /// Unit Tests for the <see cref="PagingOptions"/> struct.
    /// </summary>
    public class PagingOptionsTests
    {
        [TestFixture]
        public class PagingOptionsNone
        {
            private readonly PagingOptions none = PagingOptions.None;

            [Test]
            public void ShouldHaveZeroCount()
            {
                Assert.AreEqual(0, none.Count);
            }

            [Test]
            public void ShouldHaveZeroOffset()
            {
                Assert.AreEqual(0, none.Offset);
            }
        }

        [TestFixture]
        public class WhenCallingEqualsAndTheCountAndOffsetMatch
        {
            [Test]
            public void TrueShouldBeReturned()
            {
                var pagingOptions1 = PagingOptions.SkipTake(10, 25);
                var pagingOptions2 = PagingOptions.SkipTake(10, 25);

                Assert.IsTrue(pagingOptions1 == pagingOptions2);
            }
        }

        [TestFixture]
        public class WhenCallingEqualsAndTheCountDiffers
        {
            [Test]
            public void FalseShouldBeReturned()
            {
                var pagingOptions1 = PagingOptions.SkipTake(10, 25);
                var pagingOptions2 = PagingOptions.SkipTake(20, 25);

                Assert.IsFalse(pagingOptions1 == pagingOptions2);
            }
        }

        [TestFixture]
        public class WhenCallingEqualsAndTheOffsetDiffers
        {
            [Test]
            public void FalseShouldBeReturned()
            {
                var pagingOptions1 = PagingOptions.SkipTake(10, 25);
                var pagingOptions2 = PagingOptions.SkipTake(10, 50);

                Assert.IsFalse(pagingOptions1 == pagingOptions2);
            }
        }

        [TestFixture]
        public class WhenCallingEqualsAndTheOtherObjectIsABoxedInstanceOfPagingOptions
        {
            [Test]
            public void TrueShouldBeReturned()
            {
                var pagingOptions1 = PagingOptions.SkipTake(10, 25);
                var pagingOptions2 = (object)PagingOptions.SkipTake(10, 25);

                Assert.IsTrue(pagingOptions1.Equals(pagingOptions2));
            }
        }

        [TestFixture]
        public class WhenCallingEqualsAndTheOtherObjectIsNotAnInstanceOfPagingOptions
        {
            [Test]
            public void FalseShouldBeReturned()
            {
                var pagingOptions1 = PagingOptions.SkipTake(10, 25);
                var pagingOptions2 = new object();

                Assert.IsFalse(pagingOptions1.Equals(pagingOptions2));
            }
        }

        [TestFixture]
        public class WhenCallingForPageAndPageIsBelowOne
        {
            [Test]
            public void AnArgumentOutOfRangeExceptionShouldBeThrown()
            {
                var exception = Assert.Throws<ArgumentOutOfRangeException>(() => PagingOptions.ForPage(0, 10));

                Assert.AreEqual("page", exception.ParamName);
                Assert.IsTrue(exception.Message.StartsWith(Messages.PagingOptions_PagesMustBeAtleastOne));
            }
        }

        [TestFixture]
        public class WhenCallingForPageAndResultsPerPageIsBelowOne
        {
            [Test]
            public void AnArgumentOutOfRangeExceptionShouldBeThrown()
            {
                var exception = Assert.Throws<ArgumentOutOfRangeException>(() => PagingOptions.ForPage(5, 0));

                Assert.AreEqual("resultsPerPage", exception.ParamName);
                Assert.IsTrue(exception.Message.StartsWith(Messages.PagingOptions_ResultsPerPageMustBeAtLeast1));
            }
        }

        [TestFixture]
        public class WhenCallingForPageForTheFirstPage
        {
            private readonly int page = 1;
            private readonly PagingOptions pagingOptions;
            private readonly int resultsPerPage = 50;

            public WhenCallingForPageForTheFirstPage()
            {
                pagingOptions = PagingOptions.ForPage(this.page, this.resultsPerPage);
            }

            [Test]
            public void TheCountShouldEqualTheResultsPerPage()
            {
                Assert.AreEqual(this.resultsPerPage, this.pagingOptions.Count);
            }

            [Test]
            public void TheOffsetShouldEqualZero()
            {
                Assert.AreEqual(0, this.pagingOptions.Offset);
            }
        }

        [TestFixture]
        public class WhenCallingForPageForTheSecondPage
        {
            private readonly int page = 2;
            private readonly PagingOptions pagingOptions;
            private readonly int resultsPerPage = 50;

            public WhenCallingForPageForTheSecondPage()
            {
                pagingOptions = PagingOptions.ForPage(this.page, this.resultsPerPage);
            }

            [Test]
            public void TheCountShouldEqualTheResultsPerPage()
            {
                Assert.AreEqual(this.resultsPerPage, this.pagingOptions.Count);
            }

            [Test]
            public void TheOffsetShouldEqualThePageNumberLessOneMultipliedByTheResultsPerPage()
            {
                Assert.AreEqual(50, this.pagingOptions.Offset);
            }
        }

        [TestFixture]
        public class WhenCallingGetHashCode
        {
            private readonly int count = 50;
            private readonly int offset = 100;
            private readonly PagingOptions pagingOptions;

            public WhenCallingGetHashCode()
            {
                this.pagingOptions = PagingOptions.SkipTake(this.offset, this.count);
            }

            [Test]
            public void TheHashCodeOfTheCountShiftedByTheOffsetShouldBeReturned()
            {
                Assert.AreEqual(this.count.GetHashCode() ^ this.offset.GetHashCode(), this.pagingOptions.GetHashCode());
            }
        }

        [TestFixture]
        public class WhenCallingNotEqualsAndTheCountAndOffsetMatch
        {
            [Test]
            public void FalseShouldBeReturned()
            {
                var pagingOptions1 = PagingOptions.ForPage(page: 10, resultsPerPage: 25);
                var pagingOptions2 = PagingOptions.ForPage(page: 10, resultsPerPage: 25);

                Assert.IsFalse(pagingOptions1 != pagingOptions2);
            }
        }

        [TestFixture]
        public class WhenCallingNotEqualsAndTheCountDiffers
        {
            [Test]
            public void TrueShouldBeReturned()
            {
                var pagingOptions1 = PagingOptions.ForPage(page: 10, resultsPerPage: 25);
                var pagingOptions2 = PagingOptions.ForPage(page: 10, resultsPerPage: 50);

                Assert.IsTrue(pagingOptions1 != pagingOptions2);
            }
        }

        [TestFixture]
        public class WhenCallingNotEqualsAndTheOffsetDiffers
        {
            [Test]
            public void TrueShouldBeReturned()
            {
                var pagingOptions1 = PagingOptions.ForPage(page: 10, resultsPerPage: 25);
                var pagingOptions2 = PagingOptions.ForPage(page: 10, resultsPerPage: 50);

                Assert.IsTrue(pagingOptions1 != pagingOptions2);
            }
        }

        [TestFixture]
        public class WhenCallingSkipTake
        {
            private readonly PagingOptions pagingOptions;
            private readonly int skip = 100;
            private readonly int take = 50;

            public WhenCallingSkipTake()
            {
                pagingOptions = PagingOptions.SkipTake(this.skip, this.take);
            }

            [Test]
            public void TheCountShouldEqualTheTake()
            {
                Assert.AreEqual(this.take, this.pagingOptions.Count);
            }

            [Test]
            public void TheOffsetShouldEqualTheSkip()
            {
                Assert.AreEqual(this.skip, this.pagingOptions.Offset);
            }
        }

        [TestFixture]
        public class WhenCallingSkipTakeAndSkipIsBelowZero
        {
            [Test]
            public void AnArgumentOutOfRangeExceptionShouldBeThrown()
            {
                var exception = Assert.Throws<ArgumentOutOfRangeException>(() => PagingOptions.SkipTake(-1, 10));

                Assert.AreEqual("skip", exception.ParamName);
                Assert.IsTrue(exception.Message.StartsWith(Messages.PagingOptions_SkipMustBeZeroOrAbove));
            }
        }

        [TestFixture]
        public class WhenCallingSkipTakeAndTakeIsBelowOne
        {
            [Test]
            public void AnArgumentOutOfRangeExceptionShouldBeThrown()
            {
                var exception = Assert.Throws<ArgumentOutOfRangeException>(() => PagingOptions.SkipTake(10, 0));

                Assert.AreEqual("take", exception.ParamName);
                Assert.IsTrue(exception.Message.StartsWith(Messages.PagingOptions_TakeMustBeZeroOrAbove));
            }
        }
    }
}