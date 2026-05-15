namespace Shared.Contracts.Events;

public record EmailNotificationEvent
{
    public string Title { get; init; } = string.Empty;

    public string Subtitle { get; init; } = string.Empty;

    public string Body { get; init; } = string.Empty;

    public string Recipient { get; init; } = string.Empty;

    public string? Sender { get; init; }
}
