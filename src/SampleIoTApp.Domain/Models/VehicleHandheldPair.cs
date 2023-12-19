using Amazon.DynamoDBv2.DataModel;

namespace SampleIoTApp.Domain;

[DynamoDBTable("Vehicle2HandheldTable")]
public class VehicleHandheldPair
{
  [DynamoDBHashKey("handheldMacAddress")]
  public string? HandheldMacAddress { get; set; }

  [DynamoDBGlobalSecondaryIndexHashKey("VehicleMacAddressIndex")]
  [DynamoDBProperty("vehicleMacAddress")]
  public string? VehicleMacAddress { get; set; }
}
