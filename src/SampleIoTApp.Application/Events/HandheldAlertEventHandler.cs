using System.Text.Json;
using System.Text.Json.Serialization;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using MediatR;

namespace SampleIoTApp.Application.Events
{
    public class HandheldAlertEvent : INotification
    {
        [JsonIgnore]
        public string? AlertTopic { get; set; }

        [JsonPropertyName("alertType")]
        public string? AlertType { get; set; }

        [JsonPropertyName("handheldId")]
        public string? HandheldId { get; set; }

        [JsonPropertyName("vehicleId")]
        public string? VehicleId { get; set; }

        [JsonPropertyName("latitude")]
        public double Latitude { get; set; }

        [JsonPropertyName("longitude")]
        public double Longitude { get; set; }
    }
    public class HandheldAlertEventHandler : INotificationHandler<HandheldAlertEvent>
    {
        private readonly IAmazonSimpleNotificationService _notificationService;

        public HandheldAlertEventHandler(IAmazonSimpleNotificationService notificationService)
        {
            this._notificationService = notificationService;
        }
        public async Task Handle(HandheldAlertEvent notification, CancellationToken cancellationToken)
        {
            var topic = await _notificationService.FindTopicAsync(notification.AlertTopic);
            if (topic is null || string.IsNullOrEmpty(topic.TopicArn))
                return;

            var publishRequest = new PublishRequest
            {
                Message = JsonSerializer.Serialize(notification),
                TopicArn = topic.TopicArn,
                Subject = $"{notification.AlertType}Alert"
            };
            await _notificationService.PublishAsync(publishRequest, cancellationToken);
        }
    }
}