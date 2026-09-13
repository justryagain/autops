using AutoOps.Api.Services;
using Azure.Messaging.ServiceBus;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
string serviceBusConnectionString = builder.Configuration.GetConnectionString("ServiceBusKey") 
    ?? throw new InvalidOperationException("Service bus connection string is not configured.");

builder.Services.AddSingleton(new ServiceBusClient(serviceBusConnectionString));
builder.Services.AddSingleton<ServiceBusPublisher>();

builder.Services.AddControllers();
builder.Services.AddOpenApi();

WebApplication app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
