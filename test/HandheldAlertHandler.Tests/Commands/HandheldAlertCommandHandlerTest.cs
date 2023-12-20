using FluentAssertions;
using MediatR;
using Moq;
using SampleIoTApp.Application.Events;
using SampleIoTApp.Application.Handlers;
using SampleIoTApp.Application.Interfaces;
using SampleIoTApp.Application.Services;
using SampleIoTApp.Application.Utilities;
using SampleIoTApp.Domain;
using Xunit;

namespace HandheldAlertHandler.Tests.Commands
{
    public class HandheldAlertCommandHandlerTest
    {
        private readonly Mock<IVehicleHandheldPairRepository> _vehicleHandheldPairRepository;
        private readonly Mock<IVehicleLocationRepository> _vehicleLocationRepo;
        private readonly Mock<ILoggerService> _loggerService;
        private readonly Mock<IMediator> _mediator;
        public HandheldAlertCommandHandlerTest()
        {
            this._vehicleHandheldPairRepository = new();
            this._vehicleLocationRepo = new();
            this._loggerService = new();
            this._mediator = new();
        }

        [Fact]
        public async void HandheldAlertCommandHandler_WithValidInput_ShouldReturnSuccessfullResponse()
        {
            // Arrange
            var handheldLocation = new HandheldAlertCommand
            {
                HandheldId = "HH:00:00:00:01",
                Timestamp = DateTime.Parse("2022-10-10T16:45:00Z"),
                Longitude = 30.63530285186829,
                Latitude = 36.86537943130328
            };

            var vehicleHandheldPair = new VehicleHandheldPair
            {
                HandheldMacAddress = "HH:BB:BB:BB:01",
                VehicleMacAddress = "VV:AA:AA:AA:01"
            };

            var vehicleLocation = new VehicleLocation
            {
                VehicleId = "VV:AA:AA:AA:01",
                Timestamp = DateTime.Parse("2022-10-10T16:45:15Z"),
                Longitude = 30.63530285186829,
                Latitude = 36.99537943130328
            };

            this._vehicleHandheldPairRepository
                .Setup(vh => vh.Get(It.IsAny<string>()))
                .ReturnsAsync(vehicleHandheldPair);

            this._vehicleLocationRepo
                .Setup(v => v.Get(It.IsAny<string>()))
                .ReturnsAsync(vehicleLocation);

            this._mediator.Setup(m => m.Publish(It.IsAny<HandheldAlertEvent>(), CancellationToken.None))
                .Returns(Task.CompletedTask)
                .Verifiable();


            var handheldAlertCommandHandler = new HandheldAlertCommandHandler(this._vehicleLocationRepo.Object, _vehicleHandheldPairRepository.Object, _mediator.Object, _loggerService.Object);

            // Act
            var result = await handheldAlertCommandHandler.Handle(handheldLocation, CancellationToken.None);

            // Assert
            result.Should().BeOfType<ApiResponse>();
            result.Success.Should().BeTrue();
            result.Message.Should().Contain("50mApartDelivery");

            this._mediator.Verify(m => m.Publish(It.Is<HandheldAlertEvent>(a =>
                a.AlertType!.Equals("50mApartDelivery") &&
                a.VehicleId!.Equals(vehicleLocation.VehicleId) &&
                a.HandheldId!.Equals(handheldLocation.HandheldId)
            ), CancellationToken.None), Times.Once());

            this._vehicleHandheldPairRepository
                .Verify(vh => vh.Get(It.IsAny<string>(
            )), Times.Once());

            this._vehicleLocationRepo
                .Verify(vh => vh.Get(It.IsAny<string>(
            )), Times.Once());

        }

        [Fact]
        public async void HandheldAlertCommandHandler_WithEmptyHandheldId_ShouldReturnUnSuccessfullResponse()
        {
            // Arrange
            var handheldLocation = new HandheldAlertCommand
            {
                HandheldId = "",
                Timestamp = DateTime.Parse("2022-10-10T16:45:00Z"),
                Longitude = 30.63530285186829,
                Latitude = 36.86537943130328
            };

            var vehicleHandheldPair = new VehicleHandheldPair
            {
                HandheldMacAddress = "HH:BB:BB:BB:01",
                VehicleMacAddress = "VV:AA:AA:AA:01"
            };

            var vehicleLocation = new VehicleLocation
            {
                VehicleId = "VV:AA:AA:AA:01",
                Timestamp = DateTime.Parse("2022-10-10T16:45:15Z"),
                Longitude = 30.63530285186829,
                Latitude = 36.99537943130328
            };

            this._vehicleHandheldPairRepository
                .Setup(vh => vh.Get(It.IsAny<string>()))
                .ReturnsAsync(vehicleHandheldPair);

            var handheldAlertCommandHandler = new HandheldAlertCommandHandler(this._vehicleLocationRepo.Object, _vehicleHandheldPairRepository.Object, _mediator.Object, _loggerService.Object);

            // Act
            var result = await handheldAlertCommandHandler.Handle(handheldLocation, CancellationToken.None);

            // Assert
            result.Should().BeOfType<ApiResponse>();
            result.Success.Should().BeFalse();
            result.Message.Should().Be("HandheldId Not Found!");

            this._vehicleHandheldPairRepository
                .Verify(vh => vh.Get(It.IsAny<string>(
            )), Times.Never());
        }

        [Fact]
        public async void HandheldAlertCommandHandler_WithNoDeviceMacPairs_ShouldReturnUnSuccessfullResponse()
        {
            // Arrange
            var handheldLocation = new HandheldAlertCommand
            {
                HandheldId = "HH:BB:BB:BB:01",
                Timestamp = DateTime.Parse("2022-10-10T16:45:00Z"),
                Longitude = 30.63530285186829,
                Latitude = 36.86537943130328
            };


            var vehicleLocation = new VehicleLocation
            {
                VehicleId = "VV:AA:AA:AA:01",
                Timestamp = DateTime.Parse("2022-10-10T16:45:15Z"),
                Longitude = 30.63530285186829,
                Latitude = 36.99537943130328
            };

            this._vehicleHandheldPairRepository
                .Setup(vh => vh.Get(It.IsAny<string>()))
                .Returns(Task.FromResult<VehicleHandheldPair>(null));

            var handheldAlertCommandHandler = new HandheldAlertCommandHandler(this._vehicleLocationRepo.Object, _vehicleHandheldPairRepository.Object, _mediator.Object, _loggerService.Object);

            // Act
            var result = await handheldAlertCommandHandler.Handle(handheldLocation, CancellationToken.None);

            // Assert
            result.Should().BeOfType<ApiResponse>();
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Handheld-Device Pairs Not Found!");

            this._vehicleHandheldPairRepository
                .Verify(vh => vh.Get(It.IsAny<string>(
            )), Times.Once());
        }

        [Fact]
        public async void HandheldAlertCommandHandler_WithNoVehicleLocation_ShouldReturnUnSuccessfullResponse()
        {
            // Arrange
            var handheldLocation = new HandheldAlertCommand
            {
                HandheldId = "HH:BB:BB:BB:01",
                Timestamp = DateTime.Parse("2022-10-10T16:45:00Z"),
                Longitude = 30.63530285186829,
                Latitude = 36.86537943130328
            };

            var vehicleHandheldPair = new VehicleHandheldPair
            {
                HandheldMacAddress = "HH:BB:BB:BB:01",
                VehicleMacAddress = "VV:AA:AA:AA:01"
            };

            this._vehicleHandheldPairRepository
                .Setup(vh => vh.Get(It.IsAny<string>()))
                .ReturnsAsync(vehicleHandheldPair);

            this._vehicleLocationRepo
                .Setup(v => v.Get(It.IsAny<string>()))
                .Returns(Task.FromResult<VehicleLocation>(null));

            var handheldAlertCommandHandler = new HandheldAlertCommandHandler(this._vehicleLocationRepo.Object, _vehicleHandheldPairRepository.Object, _mediator.Object, _loggerService.Object);

            // Act
            var result = await handheldAlertCommandHandler.Handle(handheldLocation, CancellationToken.None);

            // Assert
            result.Should().BeOfType<ApiResponse>();
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Vehicle Location Not Found!");

            this._vehicleHandheldPairRepository
                .Verify(vh => vh.Get(It.IsAny<string>(
            )), Times.Once());

            this._vehicleLocationRepo
                .Verify(vh => vh.Get(It.IsAny<string>(
            )), Times.Once());
        }

        [Fact]
        public async void HandheldAlertCommandHandler_WithFalseProximityCheck_ShouldReturnUnSuccessfullResponse()
        {
            // Arrange
            var handheldLocation = new HandheldAlertCommand
            {
                HandheldId = "HH:BB:BB:BB:01",
                Timestamp = DateTime.Parse("2022-10-10T16:45:00Z"),
                Longitude = 30.63530285186829,
                Latitude = 36.86537943130328
            };

            var vehicleHandheldPair = new VehicleHandheldPair
            {
                HandheldMacAddress = "HH:BB:BB:BB:01",
                VehicleMacAddress = "VV:AA:AA:AA:01"
            };

            var vehicleLocation = new VehicleLocation
            {
                VehicleId = "VV:AA:AA:AA:01",
                Timestamp = DateTime.Parse("2022-10-10T16:45:15Z"),
                Longitude = 30.63530285186829,
                Latitude = 36.86537943130328
            };

            this._vehicleHandheldPairRepository
                .Setup(vh => vh.Get(It.IsAny<string>()))
                .ReturnsAsync(vehicleHandheldPair);

            this._vehicleLocationRepo
                .Setup(v => v.Get(It.IsAny<string>()))
                .ReturnsAsync(vehicleLocation);

            var handheldAlertCommandHandler = new HandheldAlertCommandHandler(this._vehicleLocationRepo.Object, _vehicleHandheldPairRepository.Object, _mediator.Object, _loggerService.Object);

            // Act
            var result = await handheldAlertCommandHandler.Handle(handheldLocation, CancellationToken.None);

            // Assert
            result.Should().BeOfType<ApiResponse>();
            result.Success.Should().BeFalse();
            result.Message.Should().ContainAll("Proximity in meters", "VehicleId", "HandheldId", "Time Gap");

            this._vehicleHandheldPairRepository
                .Verify(vh => vh.Get(It.IsAny<string>(
            )), Times.Once());

            this._vehicleLocationRepo
                .Verify(vh => vh.Get(It.IsAny<string>(
            )), Times.Once());
        }
    }
}