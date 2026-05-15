using MassTransit;
using NotificationsAPI;
using NotificationsAPI.Application.Ports;
using NotificationsAPI.Application.UseCases;
using NotificationsAPI.Domain.Services;
using NotificationsAPI.Infrastructure.Configuration;
using NotificationsAPI.Infrastructure.Email;
using NotificationsAPI.Infrastructure.Messaging.Consumers;
using RabbitMQ.Client;
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
            shared: true
        );
});

builder.Services.AddHostedService<Worker>();

builder.Services.AddScoped<INotificationDomainService, NotificationDomainService>();

builder.Services.AddScoped<SendWelcomeEmailUseCase>();
builder.Services.AddScoped<SendPurchaseConfirmationUseCase>();

builder.Services.AddScoped<IEmailSender, ConsoleEmailSender>();

var queueSettings = QueueSettingsFactory.FromEnvironment();

builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<UserCreatedIntegrationEventConsumer>();
    x.AddConsumer<PaymentProcessedConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(
            Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "localhost",
            Environment.GetEnvironmentVariable("RABBITMQ_VHOST") ?? "/",
            h =>
            {
                h.Username(
                    Environment.GetEnvironmentVariable("RABBITMQ_USERNAME") ?? "guest");
                h.Password(
                    Environment.GetEnvironmentVariable("RABBITMQ_PASSWORD") ?? "guest");
            });

        var userNotificationQueue =
            Environment.GetEnvironmentVariable("QUEUE_USER_CREATED")
            ?? "notification-queue";
        var userNotificationExchange =
            Environment.GetEnvironmentVariable("USERS_EVENTS_EXCHANGE")
            ?? "fiap.events";
        var userNotificationRoutingKey =
            Environment.GetEnvironmentVariable("USERS_EVENTS_ROUTING_KEY")
            ?? "notification-queue";

        cfg.ReceiveEndpoint(userNotificationQueue, e =>
        {
            e.ConfigureConsumeTopology = false;
            e.UseRawJsonDeserializer(isDefault: true);
            e.Bind(userNotificationExchange, s =>
            {
                s.ExchangeType = ExchangeType.Topic;
                s.RoutingKey = userNotificationRoutingKey;
            });
            e.ConfigureConsumer<UserCreatedIntegrationEventConsumer>(context);
        });

        // Configure explicit entity name for PaymentProcessedEvent
        cfg.Message<Shared.Contracts.Events.PaymentProcessedEvent>(m =>
        {
            m.SetEntityName("fcg.payment-processed-event");
        });

        // Bind to existing exchange/queue created by PaymentsAPI (producer)
        // Removendo routing key para evitar mensagens em _skipped
        cfg.ReceiveEndpoint("fcg.notifications.payment-processed", e =>
        {
            e.ConfigureConsumeTopology = false;
            e.Bind("fcg.payment-processed-event");
            e.ConfigureConsumer<PaymentProcessedConsumer>(context);
        });
    });
});

var host = builder.Build();
host.Run();
