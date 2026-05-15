using NotificationsAPI.Application.Ports;
using NotificationsAPI.Domain.Services;

namespace NotificationsAPI.Application.UseCases;

public class SendWelcomeEmailUseCase
{
    private readonly INotificationDomainService _domainService;
    private readonly IEmailSender _emailSender;

    public SendWelcomeEmailUseCase(
        INotificationDomainService domainService,
        IEmailSender emailSender)
    {
        _domainService = domainService;
        _emailSender = emailSender;
    }

    public async Task ExecuteAsync(string email)
    {
        if (!_domainService.IsValidEmail(email))
            return;

        var notification = _domainService.CreateWelcomeNotification(email);

        await _emailSender.SendAsync(
            email,
            "Bem-vindo à FIAP Cloud Games",
            "Olá! Seja bem-vindo à plataforma."
        );
    }
}