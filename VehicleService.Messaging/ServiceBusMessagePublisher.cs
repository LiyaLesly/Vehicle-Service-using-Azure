using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace VehicleService.Messaging
{
    public class ServiceBusMessagePublisher : IMessagePublisher
    {
        private readonly ServiceBusSender _sender;

        public ServiceBusMessagePublisher(ServiceBusClient client, IConfiguration config)
        {
            _sender = client.CreateSender(config["ServiceBus:QueueName"]);
        }

        public async Task PublishAsync<T>(T message, CancellationToken ct = default)
        {
            var json = JsonSerializer.Serialize(message);
            await _sender.SendMessageAsync(new ServiceBusMessage(json), ct);
        }
    }
}