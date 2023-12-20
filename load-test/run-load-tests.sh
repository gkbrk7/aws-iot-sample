STACK_NAME=aws-iot-sample-stack
REGION=eu-central-1

# Retrieve the URLs for HandheldAlertHandlerFunction and VehicleLocationWriterFunction
HANDHELD_ALERT_HANDLER_URL=$(aws cloudformation describe-stacks --stack-name $STACK_NAME --region $REGION \
  --query 'Stacks[0].Outputs[?OutputKey==`HandheldAlertHandlerFunctionUrl`].OutputValue' \
  --output text)

VEHICLE_LOCATION_WRITER_URL=$(aws cloudformation describe-stacks --stack-name $STACK_NAME --region $REGION \
  --query 'Stacks[0].Outputs[?OutputKey==`VehicleLocationWriterFunctionUrl`].OutputValue' \
  --output text)

artillery run vehicle-location-writer-load-test.yml --target "$VEHICLE_LOCATION_WRITER_URL"
artillery run handheld-alert-handler-load-test.yml --target "$HANDHELD_ALERT_HANDLER_URL"