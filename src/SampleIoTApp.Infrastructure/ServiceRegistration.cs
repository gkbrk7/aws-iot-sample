using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SampleIoTApp.Application.Interfaces;
using SampleIoTApp.Infrastructure.Repositories;

namespace SampleIoTApp.Infrastructure
{
    public static class ServiceRegistration
    {
        public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection collection, IConfiguration configuration)
        {
            collection.AddDefaultAWSOptions(configuration.GetAWSOptions());
            collection.AddAWSService<IAmazonDynamoDB>();
            collection.AddSingleton(_ =>
            {
                var serviceUrl = configuration["AppSettings:DynamoDb:EndpointUrl"];
                var dynamoDbConfig = new AmazonDynamoDBConfig();

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

            return collection;
        }
    }
}