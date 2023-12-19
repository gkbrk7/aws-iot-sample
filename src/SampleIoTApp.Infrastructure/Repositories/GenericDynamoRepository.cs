using Amazon.DynamoDBv2.DataModel;
using SampleIoTApp.Application.Interfaces;

namespace SampleIoTApp.Infrastructure.Repositories
{
    public class GenericDynamoRepository<T> : IGenericDynamoRepository<T> where T : class
    {
        protected readonly IDynamoDBContext dynamoDBContext;

        public GenericDynamoRepository(IDynamoDBContext dynamoDBContext)
        {
            this.dynamoDBContext = dynamoDBContext;
        }

        public Task<T> Get(object hashKey)
        {
            return dynamoDBContext.LoadAsync<T>(hashKey);
        }

        public async Task Save(T value)
        {
            await dynamoDBContext.SaveAsync(value);
        }
    }
}