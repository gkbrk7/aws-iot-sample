using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.Runtime.Internal;
using MediatR;
using SampleIoTApp.Application.Interfaces;
using SampleIoTApp.Domain;

namespace SampleIoTApp.Application.Commands
{
    public class VehicleLocationCommand : IRequest<string>
    {
        public string? VehicleId { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public DateTime Timestamp { get; set; }
    }
    public class VehicleLocationCommandHandler : IRequestHandler<VehicleLocationCommand, string>
    {
        private readonly IVehicleLocationRepository _vehicleLocationRepository;
        private readonly IMediator _mediator;
        public VehicleLocationCommandHandler(IVehicleLocationRepository vehicleLocationRepository, IMediator mediator)
        {
            this._vehicleLocationRepository = vehicleLocationRepository;
            this._mediator = mediator;
        }
        public async Task<string> Handle(VehicleLocationCommand request, CancellationToken cancellationToken)
        {
            if (request.VehicleId is null)
                return Guid.NewGuid().ToString();

            var vehicleLocation = new VehicleLocation
            {
                VehicleId = request.VehicleId,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Timestamp = request.Timestamp
            };

            await _vehicleLocationRepository.Save(vehicleLocation);
            return Guid.NewGuid().ToString();
        }
    }
}