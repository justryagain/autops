using AutoOps.Worker;
using Azure.Messaging.ServiceBus;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

string serviceBusConnectionString = builder.Configuration.GetConnectionString("ServiceBusKey")
    ?? throw new InvalidOperationException("Service bus connection string is not configured.");

builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton(new ServiceBusClient(serviceBusConnectionString));
builder.Services.AddHttpClient();

IHost host = builder.Build();
host.Run();
