using AutoOps.Contracts.Events;
using Azure;
using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace AutoOps.Worker
{
    public class Worker(
        ILogger<Worker> logger,
        ServiceBusClient serviceBusClient,
        HttpClient httpClient) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            ServiceBusProcessor processor = serviceBusClient.CreateProcessor("automation-events", "heartbeat");
            processor.ProcessMessageAsync += ProcessMessageAsync;
            processor.ProcessErrorAsync += ProcessErrorAsync;

            await processor.StartProcessingAsync(stoppingToken);
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
        {
            string body = args.Message.Body.ToString();
            HeartbeatRequested? heartbeatRequested = JsonSerializer.Deserialize<HeartbeatRequested>(body);

            if (heartbeatRequested is null)
            {
                logger.LogWarning("Could not deserialize message: {Body}", body);
                return;
            }

            try
            {
                HttpResponseMessage response = await httpClient.GetAsync(heartbeatRequested.Target);
                logger.LogInformation(
                    "Heartbeat completed. Target: {Target}, StatusCode: {StatusCode}, IsSuccess: {IsSuccess}",
                    heartbeatRequested.Target,
                    (int)response.StatusCode,
                    response.IsSuccessStatusCode);
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(
                    "Heartbeat failed. Target: {Target}, Error: {Error}",
                    heartbeatRequested.Target,
                    ex.Message);
            }
        }

        private Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            logger.LogError(args.Exception, "Service Bus error. Source: {ErrorSource}", args.ErrorSource);

            return Task.CompletedTask;
        }
    }
}
