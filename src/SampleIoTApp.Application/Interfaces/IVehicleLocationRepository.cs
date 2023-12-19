using SampleIoTApp.Domain;

namespace SampleIoTApp.Application.Interfaces
{
    public interface IVehicleLocationRepository : IGenericDynamoRepository<VehicleLocation>
    {
    }
}