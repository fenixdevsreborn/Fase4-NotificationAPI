namespace NotificationsAPI.Infrastructure.Configuration;

public static class QueueSettingsFactory
{
    public static QueueSettings FromEnvironment()
    {
        return new QueueSettings
        {
            UserCreatedQueue =
                Environment.GetEnvironmentVariable("RabbitMq__UserCreatedQueueName")
                ?? "fcg.notifications.user-created",

            PaymentProcessedQueue =
                Environment.GetEnvironmentVariable("QUEUE_PAYMENT_PROCESSED")
                ?? "fcg.notifications.payment-processed"
        };
    }
}