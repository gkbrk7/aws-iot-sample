using Amazon.Lambda.Core;
using SampleIoTApp.Application.Handlers;
using SampleIoTApp.Application.Utilities;
using SampleIoTApp.Domain;
using SampleIoTApp.HandheldAlertHandler;

namespace SampleIoTApp.Functions.HandheldAlertHandler;

public class Function : Startup
{
    public async Task<ApiResponse> FunctionHandler(HandheldLocation handheldLocation, ILambdaContext context)
    {
        var command = new HandheldAlertCommand
        {
            HandheldId = handheldLocation.HandheldId,
            Latitude = handheldLocation.Latitude,
            Longitude = handheldLocation.Longitude,
            Timestamp = handheldLocation.Timestamp
        };
        var result = await Mediator.Send(command);
        return result;
    }
}
