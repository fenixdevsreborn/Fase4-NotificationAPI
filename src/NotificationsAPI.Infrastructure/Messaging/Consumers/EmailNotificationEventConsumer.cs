using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationsAPI.Infrastructure.Email;
using Shared.Contracts.Events;

namespace NotificationsAPI.Infrastructure.Messaging.Consumers;

public class EmailNotificationEventConsumer : IConsumer<EmailNotificationEvent>
{
    private readonly IEmailSender _emailSender;
    private readonly ILogger<EmailNotificationEventConsumer> _logger;

    public EmailNotificationEventConsumer(
        IEmailSender emailSender,
        ILogger<EmailNotificationEventConsumer> logger)
    {
        _emailSender = emailSender;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<EmailNotificationEvent> context)
    {
        var message = context.Message;

        _logger.LogInformation(
            "[NotificationsAPI] EmailNotificationEvent received. Recipient: {Recipient}, Title: {Title}",
            message.Recipient,
            message.Title);

        await _emailSender.SendAsync(
            message.Recipient,
            message.Title,
            BuildBody(message));
    }

    private static string BuildBody(EmailNotificationEvent message)
    {
        if (string.IsNullOrWhiteSpace(message.Subtitle))
            return message.Body;

        return $"{message.Subtitle}{Environment.NewLine}{Environment.NewLine}{message.Body}";
    }
}
