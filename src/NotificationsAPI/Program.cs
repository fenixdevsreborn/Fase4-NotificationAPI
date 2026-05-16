using MassTransit;
using NotificationsAPI;
using NotificationsAPI.Infrastructure.Email;
using NotificationsAPI.Infrastructure.Messaging.Consumers;
using Serilog;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.ClearProviders();

builder.Services.AddSerilog((services, loggerConfig) =>
{
    loggerConfig
        .ReadFrom.Configuration(builder.Configuration)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File(
            path: "Logs/notifications-worker-.log",
            rollingInterval: RollingInterval.Day,
            retainedFileCountLimit: 7,
            shared: true);
});

builder.Services.AddHostedService<Worker>();
builder.Services.AddScoped<IEmailSender, ConsoleEmailSender>();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<EmailNotificationEventConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        var rabbitMqHost = Environment.GetEnvironmentVariable("RabbitMq__Host") ?? "localhost";
        var rabbitMqPort = ushort.Parse(Environment.GetEnvironmentVariable("RabbitMq__Port") ?? "5672");
        var rabbitMqVirtualHost = Environment.GetEnvironmentVariable("RabbitMq__VirtualHost") ?? "fiap";

        cfg.Host(rabbitMqHost, rabbitMqPort, rabbitMqVirtualHost, h =>
        {
            h.Username(Environment.GetEnvironmentVariable("RabbitMq__Username") ?? "guest");
            h.Password(Environment.GetEnvironmentVariable("RabbitMq__Password") ?? "guest");
        });

        var notificationQueue =
            Environment.GetEnvironmentVariable("RabbitMq__NotificationQueueName")
            ?? "notification-queue";
        var exchangeName =
            Environment.GetEnvironmentVariable("RabbitMq__ExchangeName")
            ?? "fiap.events";

        cfg.ReceiveEndpoint(notificationQueue, endpoint =>
        {
            endpoint.ConfigureConsumeTopology = false;
            endpoint.UseRawJsonDeserializer(isDefault: true);
            endpoint.Bind(exchangeName, binding =>
            {
                binding.ExchangeType = "topic";
                binding.RoutingKey = notificationQueue;
            });
            endpoint.ConfigureConsumer<EmailNotificationEventConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();
