using SampleIoTApp.Domain;

namespace SampleIoTApp.Application.Interfaces
{
    public interface IHandheldLocationRepository : IGenericDynamoRepository<HandheldLocation>
    {

    }
}