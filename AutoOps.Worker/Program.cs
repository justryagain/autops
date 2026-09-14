using AutoOps.Worker;
using Azure.Messaging.ServiceBus;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<Worker>();

string serviceBusConnectionString = builder.Configuration.GetConnectionString("ServiceBusKey")
    ?? throw new InvalidOperationException("Service bus connection string is not configured.");

builder.Services.AddSingleton(new ServiceBusClient(serviceBusConnectionString));

IHost host = builder.Build();
host.Run();
