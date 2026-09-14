using Azure.Messaging.ServiceBus;
using AutoOps.Contracts.Events;
using System.Text.Json;

namespace AutoOps.Worker
{
    public class Worker(ILogger<Worker> logger, ServiceBusClient serviceBusClient) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ServiceBusProcessor processor = serviceBusClient.CreateProcessor("automation-events", "heartbeat");
            processor.ProcessMessageAsync += ProcessMessageAsync;
            processor.ProcessErrorAsync += ProcessErrorAsync;

            await processor.StartProcessingAsync(stoppingToken);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private Task ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            HeartbeatRequested? heartbeatRequested = JsonSerializer.Deserialize<HeartbeatRequested>(body);

            if (heartbeatRequested is null)
            {
                logger.LogWarning("Could not deserialize message: {Body}", body);
                return Task.CompletedTask;
            }

            logger.LogInformation(
                "Heartbeat request received. EventId: {EventId}, Target: {Target}, RequestedAtUtc: {RequestedAtUtc}",
                heartbeatRequested.EventId,
                heartbeatRequested.Target,
                heartbeatRequested.RequestedAtUtc);

            return Task.CompletedTask;
        }

        private Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            logger.LogError(args.Exception, "Service Bus error. Source: {ErrorSource}", args.ErrorSource);

            return Task.CompletedTask;
        }
    }
}
