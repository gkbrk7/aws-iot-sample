using Amazon.DynamoDBv2.DataModel;

namespace SampleIoTApp.Domain;

[DynamoDBTable("VehicleLocationTable")]
public class VehicleLocation
{
  [DynamoDBHashKey("vehicleId")]
  public string? VehicleId { get; set; }

  [DynamoDBProperty("latitude")]
  public double Latitude { get; set; }

  [DynamoDBProperty("longitude")]
  public double Longitude { get; set; }

  [DynamoDBProperty("timestamp")]
  public DateTime Timestamp { get; set; } = DateTime.Now;
}
