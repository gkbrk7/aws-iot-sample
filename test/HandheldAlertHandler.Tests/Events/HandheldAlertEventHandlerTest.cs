using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Moq;
using SampleIoTApp.Application.Events;
using Xunit;

namespace HandheldAlertHandler.Tests.Events
{
    public class HandheldAlertEventHandlerTest
    {
        private readonly Mock<IAmazonSimpleNotificationService> _amazonSimpleNotificationService;
        public HandheldAlertEventHandlerTest()
        {
            this._amazonSimpleNotificationService = new();
        }

        [Fact]
        public async void HandheldAlertEventHandler_WithInvalidAlertTopic_ChecksNullTopicReceived()
        {
            // Arrange
            var handheldAlertEvent = new HandheldAlertEvent
            {
                AlertTopic = "test",
                AlertType = "50mApartDelivery",
                HandheldId = "HH:00:00:00:01",
                VehicleId = "VV:00:00:00:01",
                Latitude = 36.86537943130328,
                Longitude = 30.63530285186829
            };

            var topic = this._amazonSimpleNotificationService
                .Setup(s => s.FindTopicAsync(handheldAlertEvent.AlertTopic))
                .Returns(Task.FromResult<Topic>(null!));

            var handheldAlertEventHandler = new HandheldAlertEventHandler(_amazonSimpleNotificationService.Object);

            // Act
            await handheldAlertEventHandler.Handle(handheldAlertEvent, CancellationToken.None);

            // Assert
            this._amazonSimpleNotificationService.Verify(s => s.PublishAsync(It.IsAny<PublishRequest>(), CancellationToken.None), Times.Never());
        }

        [Fact]
        public async void HandheldAlertEventHandler_WithInvalidAlertTopic_ChecksEmptyTopicArnReceived()
        {
            // Arrange
            var handheldAlertEvent = new HandheldAlertEvent
            {
                AlertTopic = "test",
                AlertType = "50mApartDelivery",
                HandheldId = "HH:00:00:00:01",
                VehicleId = "VV:00:00:00:01",
                Latitude = 36.86537943130328,
                Longitude = 30.63530285186829
            };

            var topic = this._amazonSimpleNotificationService
                .Setup(s => s.FindTopicAsync(handheldAlertEvent.AlertTopic))
                .ReturnsAsync(new Topic());

            var handheldAlertEventHandler = new HandheldAlertEventHandler(_amazonSimpleNotificationService.Object);

            // Act
            await handheldAlertEventHandler.Handle(handheldAlertEvent, CancellationToken.None);

            // Assert
            this._amazonSimpleNotificationService.Verify(s => s.PublishAsync(It.IsAny<PublishRequest>(), CancellationToken.None), Times.Never());
        }

        [Fact]
        public async void HandheldAlertEventHandler_WithValidAlertTopic_PublishesAlert()
        {
            // Arrange
            var handheldAlertEvent = new HandheldAlertEvent
            {
                AlertTopic = "test",
                AlertType = "50mApartDelivery",
                HandheldId = "HH:00:00:00:01",
                VehicleId = "VV:00:00:00:01",
                Latitude = 36.86537943130328,
                Longitude = 30.63530285186829
            };

            var serializedAlert = JsonSerializer.Serialize(handheldAlertEvent);

            var topic = this._amazonSimpleNotificationService
                .Setup(s => s.FindTopicAsync(handheldAlertEvent.AlertTopic))
                .ReturnsAsync(new Topic { TopicArn = "test" });

            this._amazonSimpleNotificationService
                .Setup(s => s.PublishAsync(It.IsAny<PublishRequest>(), CancellationToken.None))
                .ReturnsAsync(new PublishResponse())
                .Verifiable();

            var handheldAlertEventHandler = new HandheldAlertEventHandler(_amazonSimpleNotificationService.Object);

            // Act
            await handheldAlertEventHandler.Handle(handheldAlertEvent, CancellationToken.None);

            // Assert
            this._amazonSimpleNotificationService.Verify(s => s.PublishAsync(It.IsAny<PublishRequest>(), CancellationToken.None), Times.Once());
            this._amazonSimpleNotificationService.Verify(s => s.PublishAsync(It.Is<PublishRequest>(r => r.Message.Equals(serializedAlert)), CancellationToken.None));
        }
    }
}