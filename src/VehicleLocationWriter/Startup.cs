using Amazon.Lambda.Core;
using Amazon.XRay.Recorder.Core;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleIoTApp.Application;
using SampleIoTApp.Application.Services;
using SampleIoTApp.Infrastructure;

[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace SampleIoTApp.VehicleLocationWriter;

public class Startup
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IMediator _mediator;
    private readonly ILoggerService _loggerService;

    public Startup()
    {
        var configuration = new ConfigurationBuilder()
                                   .SetBasePath(Directory.GetCurrentDirectory())
                                   .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                                   .Build();

        var collection = new ServiceCollection()
            .RegisterInfrastructureServices(configuration)
            .RegisterApplicationServices(configuration);

        this._serviceProvider = collection.BuildServiceProvider();
        this._mediator = this._serviceProvider.GetRequiredService<IMediator>();
        this._loggerService = this._serviceProvider.GetRequiredService<ILoggerService>();

        if (AWSXRayRecorder.IsLambda())
        {
            this._loggerService.AddTraceId(AWSXRayRecorder.Instance.GetEntity().TraceId);
        }
    }

    protected IMediator Mediator => this._mediator;
}