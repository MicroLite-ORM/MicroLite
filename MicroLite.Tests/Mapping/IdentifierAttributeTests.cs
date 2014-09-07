namespace MicroLite.Tests.Mapping
{
    using MicroLite.Mapping;
    using Xunit;

    /// <summary>
    /// Unit Tests for the <see cref="IdentifierAttribute" /> class.
    /// </summary>
    public class IdentifierAttributeTests
    {
        public class WhenConstructedWithIdentifierStrategy
        {
            private readonly IdentifierAttribute identifierAttribute;
            private readonly IdentifierStrategy identifierStrategy;

            public WhenConstructedWithIdentifierStrategy()
            {
                this.identifierStrategy = IdentifierStrategy.Assigned;
                this.identifierAttribute = new IdentifierAttribute(this.identifierStrategy);
            }

            [Fact]
            public void TheIdentifierStrategyIsSet()
            {
                Assert.Equal(this.identifierStrategy, this.identifierAttribute.IdentifierStrategy);
            }

            [Fact]
            public void TheSequenceNameIsNull()
            {
                Assert.Null(this.identifierAttribute.SequenceName);
            }
        }

        public class WhenConstructedWithIdentifierStrategyAndSequenceName
        {
            private readonly IdentifierAttribute identifierAttribute;
            private readonly IdentifierStrategy identifierStrategy;
            private readonly string sequenceName;

            public WhenConstructedWithIdentifierStrategyAndSequenceName()
            {
                this.identifierStrategy = IdentifierStrategy.Assigned;
                this.sequenceName = "CustomerIdSequence";
                this.identifierAttribute = new IdentifierAttribute(this.identifierStrategy, this.sequenceName);
            }

            [Fact]
            public void TheIdentifierStrategyIsSet()
            {
                Assert.Equal(this.identifierStrategy, this.identifierAttribute.IdentifierStrategy);
            }

            [Fact]
            public void TheSequenceNameIsSet()
            {
                Assert.Equal(this.sequenceName, this.identifierAttribute.SequenceName);
            }
        }
    }
}