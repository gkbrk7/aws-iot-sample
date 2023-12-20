using Amazon.Lambda.Core;
using SampleIoTApp.Application.Commands;
using SampleIoTApp.Application.Utilities;
using SampleIoTApp.Domain;
using SampleIoTApp.VehicleLocationWriter;

namespace SampleIoTApp.Functions.VehicleLocationWriter;

public class Function : Startup
{
    public async Task<ApiResponse> FunctionHandler(VehicleLocation vehicleLocation, ILambdaContext context)
    {
        var command = new VehicleLocationCommand
        {
            VehicleId = vehicleLocation.VehicleId,
            Latitude = vehicleLocation.Latitude,
            Longitude = vehicleLocation.Longitude,
            Timestamp = vehicleLocation.Timestamp
        };
        var result = await Mediator.Send(command);
        return result;
    }
}
