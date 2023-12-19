# aws-iot-sample

A sample architecture and solution for iot devices

```
aws dynamodb get-item \
    --table-name Vehicle2HandheldTable \
    --key '{"handheldMacAddress": {"S": "HH:BB:BB:BB:01"}}' \
    --endpoint-url http://localhost:8000
```

```Vehicle Table
aws dynamodb create-table \
    --table-name VehicleLocationTable \
    --attribute-definitions \
        AttributeName=vehicleId,AttributeType=S \
        AttributeName=timestamp,AttributeType=S \
    --key-schema \
        AttributeName=vehicleId,KeyType=HASH \
        AttributeName=timestamp,KeyType=RANGE \
    --provisioned-throughput \
        ReadCapacityUnits=5,WriteCapacityUnits=5 \
    --table-class STANDARD \
    --endpoint-url http://localhost:8000
```

```Vehicle2Handheld Table
aws dynamodb create-table \
    --table-name Vehicle2HandheldTable \
    --attribute-definitions \
        AttributeName=handheldMacAddress,AttributeType=S \
        AttributeName=vehicleMacAddress,AttributeType=S \
    --key-schema \
        AttributeName=handheldMacAddress,KeyType=HASH \
    --provisioned-throughput \
        ReadCapacityUnits=5,WriteCapacityUnits=5 \
    --global-secondary-indexes \
        "[
            {
                \"IndexName\": \"VehicleMacAddressIndex\",
                \"KeySchema\": [{\"AttributeName\":\"vehicleMacAddress\",\"KeyType\":\"HASH\"}],
                \"Projection\":{
                    \"ProjectionType\":\"ALL\"
                },
                \"ProvisionedThroughput\": {
                    \"ReadCapacityUnits\": 5,
                    \"WriteCapacityUnits\": 5
                }
            }
        ]" \
    --endpoint-url http://localhost:8000
```

```
aws dynamodb list-tables \
    --endpoint-url http://localhost:8000
```

```Vehicle Table
aws dynamodb put-item \
    --table-name VehicleLocationTable \
    --item '{
      "vehicleId": {"S": "VV:AA:AA:AA:01"},
      "timestamp": {"S": "2022-10-10T16:45:33Z"},
      "latitude": {"N": "53.236545"},
      "longitude": {"N": "5.693435"}
    }' \
    --endpoint-url http://localhost:8000
```

```Vehicle2Handheld Table
aws dynamodb put-item \
    --table-name Vehicle2HandheldTable \
    --item '{
      "vehicleMacAddress": {"S": "VV:AA:AA:AA:01"},
      "handheldMacAddress": {"S": "HH:BB:BB:BB:01"}
    }' \
    --endpoint-url http://localhost:8000
```

```
aws dynamodb execute-statement \
    --statement "SELECT * FROM VehicleLocationTable WHERE vehicleId='VV:AA:AA:AA:01'" \
    --endpoint-url http://localhost:8000
```

```
aws dynamodb execute-statement \
    --statement "SELECT * FROM Vehicle2HandheldTable" \
    --endpoint-url http://localhost:8000
```

```
aws dynamodb delete-table \
    --table-name VehicleLocationTable \
    --endpoint-url http://localhost:8000
```

```Vehicle Sample
{
  "latitude": 53.236545,
  "longitude": 5.693435,
  "timestamp": "2023-12-17T23:24:43.999Z",
  "vehicleId": "VV:AA:AA:AA:01"
}
```

```Handheld Sample
{
  "latitude": 53.236545,
  "longitude": 5.693921,
  "timestamp": "2023-12-17T23:24:49.999Z",
  "handheldId": "HH:BB:BB:BB:01"
}
```
