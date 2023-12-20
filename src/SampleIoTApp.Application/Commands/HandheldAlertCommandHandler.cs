using MediatR;
using SampleIoTApp.Application.Events;
using SampleIoTApp.Application.Extensions;
using SampleIoTApp.Application.Interfaces;
using SampleIoTApp.Application.Services;
using SampleIoTApp.Application.Utilities;

namespace SampleIoTApp.Application.Handlers
{
    public class HandheldAlertCommand : IRequest<ApiResponse>
    {
        public string? HandheldId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }

    }
    public class HandheldAlertCommandHandler : IRequestHandler<HandheldAlertCommand, ApiResponse>
    {
        private readonly IVehicleLocationRepository _vehicleRepository;
        private readonly IVehicleHandheldPairRepository _vehicleHandheldPairRepository;
        private readonly IMediator _mediator;
        private readonly ILoggerService _loggerService;
        private readonly double MAX_TIME_DIFFERENCE_THRESHOLD;
        private readonly string SNS_ALERT_TOPIC_NAME;

        public HandheldAlertCommandHandler(IVehicleLocationRepository vehicleRepository, IVehicleHandheldPairRepository vehicleHandheldPairRepository, IMediator mediator, ILoggerService loggerService)
        {
            this._loggerService = loggerService;
            this._mediator = mediator;
            this._vehicleRepository = vehicleRepository;
            this._vehicleHandheldPairRepository = vehicleHandheldPairRepository;
            this.MAX_TIME_DIFFERENCE_THRESHOLD = double.TryParse(Environment.GetEnvironmentVariable("MAX_TIME_DIFFERENCE_THRESHOLD"), out MAX_TIME_DIFFERENCE_THRESHOLD) ? MAX_TIME_DIFFERENCE_THRESHOLD == 0 ? 30.0 : MAX_TIME_DIFFERENCE_THRESHOLD : MAX_TIME_DIFFERENCE_THRESHOLD;
            this.SNS_ALERT_TOPIC_NAME = Environment.GetEnvironmentVariable("SNS_ALERT_TOPIC_NAME") ?? "50mApartDeliveryTopic";
        }
        public async Task<ApiResponse> Handle(HandheldAlertCommand handheldLocation, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(handheldLocation.HandheldId))
            {
                this._loggerService.LogInfo("HandheldId Not Found!");
                return new ApiResponse(false, "HandheldId Not Found!");
            }

            var deviceMacPairs = await _vehicleHandheldPairRepository.Get(handheldLocation.HandheldId);

            if (deviceMacPairs is null)
            {
                this._loggerService.LogInfo("Handheld-Device Pairs Not Found!");
                return new ApiResponse(false, "Handheld-Device Pairs Not Found!");
            }

            var vehicleLocation = await _vehicleRepository.Get(deviceMacPairs.VehicleMacAddress!);

            if (vehicleLocation is null)
            {
                this._loggerService.LogInfo("Vehicle Pair Not Found!");
                return new ApiResponse(false, "Vehicle Pair Not Found!");
            }

            var timeDiff = vehicleLocation.Timestamp.GetTimeDifferenceInSeconds(handheldLocation.Timestamp);
            var distanceInMeters = DistanceUtilities
                .CalculateHaversineDistanceInMeters(vehicleLocation.Latitude, vehicleLocation.Longitude, handheldLocation.Latitude, handheldLocation.Longitude);

            if (distanceInMeters < 50 || timeDiff is null || timeDiff > this.MAX_TIME_DIFFERENCE_THRESHOLD)
            {
                this._loggerService.LogInfo($"Proximity in meters: {distanceInMeters}, VehicleId: {vehicleLocation.VehicleId}, HandheldId: {handheldLocation.HandheldId}, Time Gap: {timeDiff.GetValueOrDefault()}");
                return new ApiResponse(false, $"Proximity in meters: {distanceInMeters}, VehicleId: {vehicleLocation.VehicleId}, HandheldId: {handheldLocation.HandheldId}, Time Gap: {timeDiff.GetValueOrDefault()}");
            }

            var handheldAlertEvent = new HandheldAlertEvent
            {
                AlertTopic = this.SNS_ALERT_TOPIC_NAME,
                AlertType = "50mApartDelivery",
                HandheldId = handheldLocation.HandheldId,
                VehicleId = vehicleLocation.VehicleId,
                Latitude = vehicleLocation.Latitude,
                Longitude = vehicleLocation.Longitude
            };

            await _mediator.Publish(handheldAlertEvent, cancellationToken);
            return new ApiResponse(true, $"{handheldAlertEvent.AlertType} Alert Has Been Sent!");
        }
    }
}