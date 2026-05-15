namespace NotificationsAPI.Infrastructure.Configuration;

public class QueueSettings
{
    public string UserCreatedQueue { get; init; } = default!;
    public string PaymentProcessedQueue { get; init; } = default!;
}