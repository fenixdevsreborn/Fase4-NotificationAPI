using NotificationsAPI.Infrastructure.Configuration;

public class QueueSettingsFactoryTests : IDisposable
{
    private readonly string? _originalUserCreatedQueue;
    private readonly string? _originalPaymentProcessedQueue;

    public QueueSettingsFactoryTests()
    {
        _originalUserCreatedQueue = Environment.GetEnvironmentVariable("QUEUE_USER_CREATED");
        _originalPaymentProcessedQueue = Environment.GetEnvironmentVariable("QUEUE_PAYMENT_PROCESSED");
    }

    [Fact]
    public void FromEnvironment_ShouldReturnDefaultQueueNames_WhenEnvironmentVariablesAreMissing()
    {
        Environment.SetEnvironmentVariable("QUEUE_USER_CREATED", null);
        Environment.SetEnvironmentVariable("QUEUE_PAYMENT_PROCESSED", null);

        var settings = QueueSettingsFactory.FromEnvironment();

        Assert.Equal("fcg.notifications.user-created", settings.UserCreatedQueue);
        Assert.Equal("fcg.notifications.payment-processed", settings.PaymentProcessedQueue);
    }

    [Fact]
    public void FromEnvironment_ShouldReturnConfiguredQueueNames_WhenEnvironmentVariablesExist()
    {
        Environment.SetEnvironmentVariable("QUEUE_USER_CREATED", "custom.user-created");
        Environment.SetEnvironmentVariable("QUEUE_PAYMENT_PROCESSED", "custom.payment-processed");

        var settings = QueueSettingsFactory.FromEnvironment();

        Assert.Equal("custom.user-created", settings.UserCreatedQueue);
        Assert.Equal("custom.payment-processed", settings.PaymentProcessedQueue);
    }

    public void Dispose()
    {
        Environment.SetEnvironmentVariable("QUEUE_USER_CREATED", _originalUserCreatedQueue);
        Environment.SetEnvironmentVariable("QUEUE_PAYMENT_PROCESSED", _originalPaymentProcessedQueue);
    }
}
