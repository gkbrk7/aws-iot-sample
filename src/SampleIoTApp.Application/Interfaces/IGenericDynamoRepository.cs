namespace SampleIoTApp.Application.Interfaces
{
    public interface IGenericDynamoRepository<T> where T : class
    {
        Task<T> Get(object hashKey);
        Task Save(T value);
    }
}