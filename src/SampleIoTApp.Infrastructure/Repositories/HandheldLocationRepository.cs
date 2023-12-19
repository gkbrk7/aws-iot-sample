using Amazon.DynamoDBv2.DataModel;
using SampleIoTApp.Application.Interfaces;
using SampleIoTApp.Domain;

namespace SampleIoTApp.Infrastructure.Repositories
{
    public class HandheldLocationRepository : GenericDynamoRepository<HandheldLocation>, IHandheldLocationRepository
    {
        public HandheldLocationRepository(IDynamoDBContext dynamoDBContext) : base(dynamoDBContext)
        {
        }
    }
}