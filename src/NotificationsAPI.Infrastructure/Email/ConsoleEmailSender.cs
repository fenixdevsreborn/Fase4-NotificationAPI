using Microsoft.Extensions.Logging;

namespace NotificationsAPI.Infrastructure.Email;

public class ConsoleEmailSender : IEmailSender
{
    private readonly ILogger<ConsoleEmailSender> _logger;

    public ConsoleEmailSender(ILogger<ConsoleEmailSender> logger)
    {
        _logger = logger;
    }

    public Task SendAsync(string to, string subject, string body)
    {
        _logger.LogInformation(
            "Email sent to {To} | Subject: {Subject} | Body: {Body}",
            to,
            subject,
            body);

        return Task.CompletedTask;
    }
}
