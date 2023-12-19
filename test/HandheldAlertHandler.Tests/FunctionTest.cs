using Xunit;
using Amazon.Lambda.TestUtilities;
using Moq;
using SampleIoTApp.Domain;
using SampleIoTApp.Application.Interfaces;
using SampleIoTApp.Functions.HandheldAlertHandler;
using MediatR;
using SampleIoTApp.Application.Handlers;
using FluentAssertions;

namespace HandheldAlertHandler.Tests;

public class FunctionTest
{
    private readonly Mock<IVehicleLocationRepository> mockDynamoVehicleLocationRepo;
    private readonly Mock<IVehicleHandheldPairRepository> mockDynamoVehicleHandheldPairRepo;
    private readonly Mock<IMediator> mockMediator;

    public FunctionTest()
    {
        this.mockDynamoVehicleHandheldPairRepo = new();
        this.mockDynamoVehicleLocationRepo = new();
        this.mockMediator = new();
    }

    [Fact]
    public async void ShouldReturnHandheldId()
    {
        // Arrange
        var handheldAlertCommand = new HandheldAlertCommand
        {
            Latitude = 53.236545,
            Longitude = 5.693921,
            Timestamp = DateTime.Parse("2022-10-10T16:45:39Z"),
            HandheldId = "HH:BB:BB:BB:01"
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

        mockDynamoVehicleHandheldPairRepo.Setup(p => p.Get(It.IsAny<string>())).ReturnsAsync(vehicleHandheldPair);
        mockDynamoVehicleLocationRepo.Setup(v => v.Get(It.IsAny<string>())).ReturnsAsync(vehicleLocation);

        var handheldAlertCommandHandler = new HandheldAlertCommandHandler(mockDynamoVehicleLocationRepo.Object, mockDynamoVehicleHandheldPairRepo.Object, mockMediator.Object);

        // Act
        var response = await handheldAlertCommandHandler.Handle(handheldAlertCommand, CancellationToken.None);

        // Assert
        response.Should().NotBeEmpty();
    }
}
