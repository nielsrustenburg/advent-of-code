using Xunit;
using AoCSharp2023;
using FluentAssertions;
using System.Linq;

namespace Tests
{
    public class Day5Tests
    {
        [Fact]
        public void WhenRangeIsInsideMapping_ShouldConvertEntireRange()
        {
            var range = new Range // 1,2,3,4,5
            {
                Start = 1,
                Length = 5,
                Type = "test"
            };

            var mapping = new AlmanacMapping // 0 - 9
            {
                Source = 0,
                Length = 10,
                Destination = 100,
                SourceType = "test",
                DestinationType = "result"
            };

            var (newRange, unmapped) = mapping.ConvertRange(range);
            newRange.Start.Should().Be(101);
            newRange.Length.Should().Be(5);
            newRange.Type.Should().Be("result");
            unmapped.Should().BeEmpty();
        }

        [Fact]
        public void WhenMappingIsInsideRange_ShouldConvertAndHaveBeforeAndAfterRemainders()
        {
            var range = new Range  // 0 - 9
            {
                Start = 0,
                Length = 10,
                Type = "test"
            };

            var mapping = new AlmanacMapping // 2-5
            {
                Source = 2,
                Length = 4,
                Destination = 100,
                SourceType = "test",
                DestinationType = "result"
            };

            var (newRange, unmapped) = mapping.ConvertRange(range);
            newRange.Start.Should().Be(100);
            newRange.Length.Should().Be(4);
            newRange.Type.Should().Be("result");
            unmapped.Should().HaveCount(2);
            unmapped.First().Start.Should().Be(0);
            unmapped.First().Length.Should().Be(2);
            unmapped.First().Type.Should().Be("test");
            unmapped.Last().Start.Should().Be(6);
            unmapped.Last().Length.Should().Be(4);
            unmapped.Last().Type.Should().Be("test");
        }

        [Fact]
        public void WhenStartOfRangeIsInsideMapping_ShouldConvertAndHaveAfter()
        {
            var range = new Range  // 3 - 9
            {
                Start = 3,
                Length = 7,
                Type = "test"
            };

            var mapping = new AlmanacMapping // 0-5
            {
                Source = 0,
                Length = 6,
                Destination = 100,
                SourceType = "test",
                DestinationType = "result"
            };

            var (newRange, unmapped) = mapping.ConvertRange(range);
            newRange.Start.Should().Be(103);
            newRange.Length.Should().Be(3);
            newRange.Type.Should().Be("result");
            unmapped.Should().ContainSingle();
            unmapped.First().Start.Should().Be(6);
            unmapped.First().Length.Should().Be(4);
            unmapped.First().Type.Should().Be("test");
        }

        [Fact]
        public void WhenEndOfRangeIsInsideMapping_ShouldConvertAndHaveBefore()
        {
            var range = new Range  // 0 - 7
            {
                Start = 0,
                Length = 8,
                Type = "test"
            };

            var mapping = new AlmanacMapping // 5-10
            {
                Source = 5,
                Length = 6,
                Destination = 100,
                SourceType = "test",
                DestinationType = "result"
            };

            var (newRange, unmapped) = mapping.ConvertRange(range);
            newRange.Start.Should().Be(100);
            newRange.Length.Should().Be(3);
            newRange.Type.Should().Be("result");
            unmapped.Should().ContainSingle();
            unmapped.First().Start.Should().Be(0);
            unmapped.First().Length.Should().Be(5);
            unmapped.First().Type.Should().Be("test");
        }
        
        [Fact]
        public void WhenEndOfRangeMatchesWithEndOfMapping_ShouldNotHaveAfter()
        {
            var range = new Range  // 4 - 7
            {
                Start = 4,
                Length = 4,
                Type = "test"
            };

            var mapping = new AlmanacMapping // 0-7
            {
                Source = 0,
                Length = 8,
                Destination = 100,
                SourceType = "test",
                DestinationType = "result"
            };

            var (newRange, unmapped) = mapping.ConvertRange(range);
            newRange.Start.Should().Be(104);
            newRange.Length.Should().Be(4);
            newRange.Type.Should().Be("result");
            unmapped.Should().BeEmpty();
        }
    }
}