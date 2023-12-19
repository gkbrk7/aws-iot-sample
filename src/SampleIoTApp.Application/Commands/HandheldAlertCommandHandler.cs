using MediatR;
using SampleIoTApp.Application.Events;
using SampleIoTApp.Application.Extensions;
using SampleIoTApp.Application.Interfaces;
using SampleIoTApp.Application.Utilities;

namespace SampleIoTApp.Application.Handlers
{
    public class HandheldAlertCommand : IRequest<string>
    {
        public string? HandheldId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }

    }
    public class HandheldAlertCommandHandler : IRequestHandler<HandheldAlertCommand, string>
    {
        private readonly IVehicleLocationRepository _vehicleRepository;
        private readonly IVehicleHandheldPairRepository _vehicleHandheldPairRepository;
        private readonly IMediator _mediator;

        public HandheldAlertCommandHandler(IVehicleLocationRepository vehicleRepository, IVehicleHandheldPairRepository vehicleHandheldPairRepository, IMediator mediator)
        {
            this._mediator = mediator;
            this._vehicleRepository = vehicleRepository;
            this._vehicleHandheldPairRepository = vehicleHandheldPairRepository;
        }
        public async Task<string> Handle(HandheldAlertCommand handheldLocation, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(handheldLocation.HandheldId))
                return Guid.NewGuid().ToString();

            var deviceMacPairs = await _vehicleHandheldPairRepository.Get(handheldLocation.HandheldId);

            if (deviceMacPairs is null)
                return Guid.NewGuid().ToString();

            var vehicleLocation = await _vehicleRepository.Get(deviceMacPairs.VehicleMacAddress!);

            if (vehicleLocation is null)
                return Guid.NewGuid().ToString();

            var timeDiff = vehicleLocation.Timestamp.GetTimeDifferenceInSeconds(handheldLocation.Timestamp);
            var distanceInMeters = DistanceUtilities
                .CalculateHaversineDistanceInMeters(vehicleLocation.Latitude, vehicleLocation.Longitude, handheldLocation.Latitude, handheldLocation.Longitude);

            if (distanceInMeters > 50 && timeDiff is not null && timeDiff < 60.0)
            {
                var handheldAlertEvent = new HandheldAlertEvent
                {
                    AlertTopic = "50mApartDeliveryTopic",
                    AlertType = "50mApartDelivery",
                    HandheldId = handheldLocation.HandheldId,
                    VehicleId = vehicleLocation.VehicleId,
                    Latitude = vehicleLocation.Latitude,
                    Longitude = vehicleLocation.Longitude
                };
                await _mediator.Publish(handheldAlertEvent, cancellationToken);
                return vehicleLocation.VehicleId!;
            }

            return Guid.NewGuid().ToString();
        }
    }
}