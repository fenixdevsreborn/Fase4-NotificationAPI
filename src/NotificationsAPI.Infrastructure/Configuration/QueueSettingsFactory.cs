namespace NotificationsAPI.Infrastructure.Configuration;

public static class QueueSettingsFactory
{
    public static QueueSettings FromEnvironment()
    {
        return new QueueSettings
        {
            UserCreatedQueue =
                Environment.GetEnvironmentVariable("QUEUE_USER_CREATED")
                ?? "fcg.notifications.user-created",

            PaymentProcessedQueue =
                Environment.GetEnvironmentVariable("QUEUE_PAYMENT_PROCESSED")
                ?? "fcg.notifications.payment-processed"
        };
    }
}