using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using MediatR;
using SampleIoTApp.Application.Interfaces;
using SampleIoTApp.Application.Services;
using SampleIoTApp.Application.Utilities;
using SampleIoTApp.Domain;

namespace SampleIoTApp.Application.Commands
{
    public class VehicleLocationCommand : IRequest<ApiResponse>
    {
        public string? VehicleId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
    public class VehicleLocationCommandHandler : IRequestHandler<VehicleLocationCommand, ApiResponse>
    {
        private readonly IVehicleLocationRepository _vehicleLocationRepository;
        private readonly ILoggerService _loggerService;
        public VehicleLocationCommandHandler(IVehicleLocationRepository vehicleLocationRepository, ILoggerService loggerService)
        {
            this._vehicleLocationRepository = vehicleLocationRepository;
            this._loggerService = loggerService;
        }
        public async Task<ApiResponse> Handle(VehicleLocationCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(request.VehicleId))
                return new ApiResponse(false, "VehicleId Not Found!");

            var vehicleLocation = new VehicleLocation
            {
                VehicleId = request.VehicleId,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Timestamp = request.Timestamp
            };

            await _vehicleLocationRepository.Save(vehicleLocation);
            return new ApiResponse(true, "Vehicle Location Successfully Added.");
        }
    }
}