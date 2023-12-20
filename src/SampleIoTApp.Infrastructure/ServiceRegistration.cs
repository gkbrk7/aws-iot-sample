using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.XRay.Recorder.Handlers.AwsSdk;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleIoTApp.Application.Interfaces;
using SampleIoTApp.Application.Services;
using SampleIoTApp.Infrastructure.Repositories;
using SampleIoTApp.Infrastructure.Services;

namespace SampleIoTApp.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection collection, IConfiguration configuration)
        {
            AWSSDKHandler.RegisterXRayForAllServices();

            collection.AddDefaultAWSOptions(configuration.GetAWSOptions());
            collection.AddAWSService<IAmazonDynamoDB>();
            collection.AddSingleton(_ =>
            {
                var dynamoDbConfig = new AmazonDynamoDBConfig();
                var serviceUrl = configuration["AppSettings:DynamoDb:EndpointUrl"] ?? Environment.GetEnvironmentVariable("DYNAMODB_LOCAL_URL");

                if (serviceUrl is not null)
                    dynamoDbConfig.ServiceURL = serviceUrl;

                return new AmazonDynamoDBClient(dynamoDbConfig);
            });
            collection.AddSingleton<IDynamoDBContext, DynamoDBContext>(serviceProvider =>
            {
                var dynamoDbClient = serviceProvider.GetRequiredService<AmazonDynamoDBClient>();
                return new DynamoDBContext(dynamoDbClient);
            });

            collection.AddSingleton(typeof(IGenericDynamoRepository<>), typeof(GenericDynamoRepository<>));
            collection.AddSingleton<IHandheldLocationRepository, HandheldLocationRepository>();
            collection.AddSingleton<IVehicleHandheldPairRepository, VehicleHandheldPairRepository>();
            collection.AddSingleton<IVehicleLocationRepository, VehicleLocationRepository>();
            collection.AddSingleton<ILoggerService, LoggerService>();

            return collection;
        }
    }
}