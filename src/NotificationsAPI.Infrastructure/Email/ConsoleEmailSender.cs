using Microsoft.Extensions.Logging;
using NotificationsAPI.Application.Ports;

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
            "📧 Email enviado para {To} | Assunto: {Subject} | Conteúdo: {Body}",
            to,
            subject,
            body
        );

        return Task.CompletedTask;
    }
}