using Amazon.DynamoDBv2.DataModel;
using SampleIoTApp.Application.Interfaces;
using SampleIoTApp.Domain;

namespace SampleIoTApp.Infrastructure.Repositories
{
    public class VehicleLocationRepository : GenericDynamoRepository<VehicleLocation>, IVehicleLocationRepository
    {
        public VehicleLocationRepository(IDynamoDBContext dynamoDBContext) : base(dynamoDBContext)
        {
        }
    }
}