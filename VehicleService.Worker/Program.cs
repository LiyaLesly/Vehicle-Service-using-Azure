using Azure.Communication.Email;
using Azure.Messaging.ServiceBus;
using VehicleService.Messaging;
using VehicleService.Worker;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton(new ServiceBusClient(
    builder.Configuration["ServiceBus:ConnectionString"]));

if (builder.Configuration.GetValue<bool>("Email:UseAcs"))
{
    builder.Services.AddSingleton(new EmailClient(
        builder.Configuration["Acs:ConnectionString"]));
    builder.Services.AddSingleton<IEmailService, AcsEmailService>();
}
else
{
    builder.Services.AddSingleton<IEmailService, LogEmailService>();
}

builder.Services.AddHostedService<BookingCreatedWorker>();

builder.Build().Run();