using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace AutoOps.Api.Services;

public class ServiceBusPublisher
{
    private readonly ServiceBusSender _sender;

    public ServiceBusPublisher(ServiceBusClient serviceBusClient)
    {
        _sender = serviceBusClient.CreateSender("automation-events");
    }

    public async Task PublishAsync<T>(T message)
    {
        string messageJson = JsonSerializer.Serialize(message);
        ServiceBusMessage serviceBusMessage = new(messageJson);

        await _sender.SendMessageAsync(serviceBusMessage);
    }
}