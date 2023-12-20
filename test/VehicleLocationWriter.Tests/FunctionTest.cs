using Xunit;
using SampleIoTApp.Application.Interfaces;
using Moq;
using SampleIoTApp.Domain;
using FluentAssertions;
using SampleIoTApp.Application.Commands;
using SampleIoTApp.Application.Services;
using SampleIoTApp.Application.Utilities;

namespace VehicleLocationWriter.Tests;

public class FunctionTest
{
    private readonly Mock<IVehicleLocationRepository> mockDynamoVehicleLocationRepo;
    private readonly Mock<ILoggerService> mockLoggerService;

    public FunctionTest()
    {
        this.mockDynamoVehicleLocationRepo = new();
        this.mockLoggerService = new();
    }

    [Fact]
    public async void ShouldReturnHandheldId()
    {
        // Arrange
        var handheldAlertCommand = new VehicleLocationCommand
        {
            Latitude = 53.236545,
            Longitude = 5.693921,
            Timestamp = DateTime.Parse("2022-10-10T16:45:39Z"),
            VehicleId = "VV:AA:AA:AA:01"
        };


        var vehicleLocation = new VehicleLocation
        {
            Latitude = 53.236545,
            Longitude = 5.693435,
            Timestamp = DateTime.Parse("2022-10-10T16:45:39Z"),
            VehicleId = "VV:AA:AA:AA:01"
        };

        mockDynamoVehicleLocationRepo.Setup(v => v.Save(It.IsAny<VehicleLocation>())).Verifiable();

        var vehicleLocationCommandHandler = new VehicleLocationCommandHandler(mockDynamoVehicleLocationRepo.Object, mockLoggerService.Object);

        // Act
        var response = await vehicleLocationCommandHandler.Handle(handheldAlertCommand, CancellationToken.None);

        // Assert
        response.Success.Should().BeTrue();
        response.Should().BeOfType<ApiResponse>();
    }

    [Fact]
    public async void ShouldReturnFalseForEmptyVehicleId()
    {
        // Arrange
        var handheldAlertCommand = new VehicleLocationCommand
        {
            Latitude = 53.236545,
            Longitude = 5.693921,
            Timestamp = DateTime.Parse("2022-10-10T16:45:39Z"),
            VehicleId = ""
        };

        var vehicleHandheldPair = new VehicleHandheldPair
        {
            HandheldMacAddress = "HH:BB:BB:BB:01",
            VehicleMacAddress = "VV:AA:AA:AA:01"
        };

        var vehicleLocation = new VehicleLocation
        {
            Latitude = 53.236545,
            Longitude = 5.693435,
            Timestamp = DateTime.Parse("2022-10-10T16:45:39Z"),
            VehicleId = "VV:AA:AA:AA:01"
        };

        mockDynamoVehicleLocationRepo.Setup(v => v.Save(It.IsAny<VehicleLocation>())).Verifiable();

        var vehicleLocationCommandHandler = new VehicleLocationCommandHandler(mockDynamoVehicleLocationRepo.Object, mockLoggerService.Object);

        // Act
        var response = await vehicleLocationCommandHandler.Handle(handheldAlertCommand, CancellationToken.None);

        // Assert
        response.Success.Should().BeFalse();
        response.Should().BeOfType<ApiResponse>();
        response.Message.Should().Be("VehicleId Not Found!");
    }
}
