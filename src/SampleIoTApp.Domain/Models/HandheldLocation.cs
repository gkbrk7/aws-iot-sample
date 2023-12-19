using Amazon.DynamoDBv2.DataModel;

namespace SampleIoTApp.Domain;

[DynamoDBTable("HandheldLocationTable")]
public class HandheldLocation
{
  [DynamoDBHashKey("handheldId")]
  public string? HandheldId { get; set; }

  [DynamoDBProperty("latitude")]
  public double Latitude { get; set; }

  [DynamoDBProperty("longitude")]
  public double Longitude { get; set; }

  [DynamoDBProperty("timestamp")]
  public DateTime Timestamp { get; set; } = DateTime.Now;
}
