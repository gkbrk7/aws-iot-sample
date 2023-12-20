using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using SampleIoTApp.Application.Utilities;
using Xunit;

namespace HandheldAlertHandler.Tests.Utilities
{
    public class DistanceUtilitiesTest
    {
        [Fact]
        public void DistanceUtilities_WithValidCoordinates_ReturnsHaversineDistanceInMeters()
        {
            // Arrange
            var lon1 = 30.635689089966434;
            var lat1 = 36.867422295188234;

            var lon2 = 30.63530285186829;
            var lat2 = 36.86537943130328;

            // Act
            var result = DistanceUtilities.CalculateHaversineDistanceInMeters(lat1, lon1, lat2, lon2);

            // Asert
            result.Should().BePositive();
            result.Should().BeGreaterThan(0);
            result.Should().BeApproximately(230, 1);
        }


        [Fact]
        public void DistanceUtilities_WithOneInvalidLatitudeCoordinates_ReturnsZero()
        {
            // Arrange
            var lon1 = 30.635689089966434;
            var lat1 = 36.867422295188234;

            var lon2 = 0;
            var lat2 = 270;

            // Act
            var result = DistanceUtilities.CalculateHaversineDistanceInMeters(lat1, lon1, lat2, lon2);

            // Asert
            result.Should().Be(0);
        }

        [Fact]
        public void DistanceUtilities_WithOneInvalidLongitudeCoordinates_ReturnsZero()
        {
            // Arrange
            var lon1 = 30.635689089966434;
            var lat1 = 36.867422295188234;

            var lon2 = 270;
            var lat2 = 36.86537943130328;

            // Act
            var result = DistanceUtilities.CalculateHaversineDistanceInMeters(lat1, lon1, lat2, lon2);

            // Asert
            result.Should().Be(0);
        }

        [Fact]
        public void DistanceUtilities_WithTwoInvalidCoordinates_ReturnsZero()
        {
            // Arrange
            var lon1 = 30.635689089966434;
            var lat1 = 500;

            var lon2 = 270;
            var lat2 = 36.86537943130328;

            // Act
            var result = DistanceUtilities.CalculateHaversineDistanceInMeters(lat1, lon1, lat2, lon2);

            // Asert
            result.Should().Be(0);
        }

        [Fact]
        public void DistanceUtilities_WithTwoEdgeCoordinates_ReturnsZero()
        {
            // Arrange
            var lon1 = 180;
            var lat1 = 90;

            var lon2 = -180;
            var lat2 = -90;

            // Act
            var result = DistanceUtilities.CalculateHaversineDistanceInMeters(lat1, lon1, lat2, lon2);

            // Asert
            result.Should().BePositive();
        }
    }
}