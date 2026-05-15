namespace NotificationsAPI.Domain.Entities;

public class Notification
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Type { get; }
    public string Recipient { get; }
    public DateTime SentAt { get; }

    public Notification(string type, string recipient)
    {
        Type = type;
        Recipient = recipient;
        SentAt = DateTime.UtcNow;
    }
}