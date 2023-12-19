using SampleIoTApp.Domain;

namespace SampleIoTApp.Application.Interfaces
{
    public interface IVehicleHandheldPairRepository : IGenericDynamoRepository<VehicleHandheldPair>
    {

    }
}