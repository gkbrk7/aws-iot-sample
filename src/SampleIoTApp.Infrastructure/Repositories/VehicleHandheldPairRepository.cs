using Amazon.DynamoDBv2.DataModel;
using SampleIoTApp.Application.Interfaces;
using SampleIoTApp.Domain;

namespace SampleIoTApp.Infrastructure.Repositories
{
    public class VehicleHandheldPairRepository : GenericDynamoRepository<VehicleHandheldPair>, IVehicleHandheldPairRepository
    {
        public VehicleHandheldPairRepository(IDynamoDBContext dynamoDBContext) : base(dynamoDBContext)
        {
        }
    }
}