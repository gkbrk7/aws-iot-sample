using FluentAssertions;
using SampleIoTApp.Application.Extensions;
using Xunit;

namespace HandheldAlertHandler.Tests.Extensions
{
    public class DateTimeExtensionsTest
    {
        [Fact]
        public void DateTime_WithSameAndValidEndDate_ReturnsTimeDifferenceInSeconds()
        {
            // Arrange
            var dateTimeComparable = new DateTime(2023, 12, 20, 10, 0, 0);
            var dateTimeComparator = new DateTime(2023, 12, 20, 10, 0, 0);


            // Act
            var result = dateTimeComparator.GetTimeDifferenceInSeconds(dateTimeComparable);

            // Assert
            result.Should().Be(0);
            result.Should().NotBeNull();
        }

        [Fact]
        public void PastDateTime_WithDifferentEndDate_ReturnsTimeDifferenceInSeconds()
        {
            // Arrange
            var dateTimeComparable = new DateTime(2023, 12, 20, 10, 0, 0);
            var dateTimeComparator = new DateTime(2023, 12, 20, 11, 0, 0);


            // Act
            var result = dateTimeComparator.GetTimeDifferenceInSeconds(dateTimeComparable);

            // Assert
            result.Should().Be(3600);
            result.Should().NotBeNull();
        }

        [Fact]
        public void DateTime_WithDifferentAndPastEndDate_ReturnsTimeDifferenceInSeconds()
        {
            // Arrange
            var dateTimeComparable = new DateTime(2023, 12, 20, 10, 0, 0);
            var dateTimeComparator = new DateTime(2023, 12, 20, 9, 0, 0);


            // Act
            var result = dateTimeComparator.GetTimeDifferenceInSeconds(dateTimeComparable);

            // Assert
            result.Should().Be(3600);
            result.Should().NotBeNull();
        }

        [Fact]
        public void DateTime_NullEndDate_ReturnsTimeDifferenceInSeconds()
        {
            // Arrange
            var dateTimeComparator = new DateTime(2023, 12, 20, 9, 0, 0);


            // Act
            var result = dateTimeComparator.GetTimeDifferenceInSeconds(null);

            // Assert
            result.Should().BeNull();
        }

    }
}