using FluentAssertions;
using Moq;
using SampleIoTApp.Application.Commands;
using SampleIoTApp.Application.Interfaces;
using SampleIoTApp.Application.Services;
using SampleIoTApp.Domain;
using Xunit;

namespace VehicleLocationWriter.Tests.Commands
{
    public class VehicleLocationCommandHandlerTest
    {
        private readonly Mock<IVehicleLocationRepository> _vehicleLocationRepo;
        private readonly Mock<ILoggerService> _loggerService;
        public VehicleLocationCommandHandlerTest()
        {
            this._vehicleLocationRepo = new();
            this._loggerService = new();
        }

        [Fact]
        public async void VehicleLocationCommandHandler_WithEmptyVehicleId_ReturnsVehicleNotFoundMessage()
        {
            // Arrange
            var command = new VehicleLocationCommand
            {
                Timestamp = DateTime.Now,
                Longitude = 30.63530285186829,
                Latitude = 36.86537943130328
            };

            var vehicleLocationCommandHandler = new VehicleLocationCommandHandler(this._vehicleLocationRepo.Object, _loggerService.Object);

            // Act
            var result = await vehicleLocationCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("VehicleId Not Found!");

        }

        [Fact]
        public async void VehicleLocationCommandHandler_WithVehicleId_ReturnsVehicleSuccessfullMessage()
        {
            // Arrange
            var command = new VehicleLocationCommand
            {
                Timestamp = DateTime.Now,
                Longitude = 30.63530285186829,
                Latitude = 36.86537943130328,
                VehicleId = "VV:00:00:00:01"
            };

            this._vehicleLocationRepo
                .Setup(v => v.Save(It.IsAny<VehicleLocation>()))
                .Returns(Task.CompletedTask);

            var vehicleLocationCommandHandler = new VehicleLocationCommandHandler(this._vehicleLocationRepo.Object, _loggerService.Object);

            // Act
            var result = await vehicleLocationCommandHandler.Handle(command, CancellationToken.None);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Vehicle Location Successfully Added.");
            this._vehicleLocationRepo.Verify(v => v.Save(It.IsAny<VehicleLocation>()), Times.Once());
            this._vehicleLocationRepo.Verify(v => v.Save(It.Is<VehicleLocation>(l => l.VehicleId!.Equals(command.VehicleId))), Times.Once());
            this._vehicleLocationRepo.Verify(v => v.Save(It.Is<VehicleLocation>(l => l.Latitude!.Equals(command.Latitude))), Times.Once());
            this._vehicleLocationRepo.Verify(v => v.Save(It.Is<VehicleLocation>(l => l.Longitude!.Equals(command.Longitude))), Times.Once());
        }
    }
}