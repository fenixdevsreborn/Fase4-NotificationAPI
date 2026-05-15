using NotificationsAPI.Application.Ports;
using NotificationsAPI.Domain.Services;

namespace NotificationsAPI.Application.UseCases;

public class SendPurchaseConfirmationUseCase
{
    private readonly INotificationDomainService _domainService;
    private readonly IEmailSender _emailSender;

    public SendPurchaseConfirmationUseCase(
        INotificationDomainService domainService,
        IEmailSender emailSender)
    {
        _domainService = domainService;
        _emailSender = emailSender;
    }

    public async Task ExecuteAsync(string email, string paymentStatus)
    {
        var notification =
            _domainService.CreatePurchaseNotification(email, paymentStatus);

        if (notification is null)
            return;

        await _emailSender.SendAsync(
            email,
            "Compra confirmada",
            "Seu jogo foi adicionado à sua biblioteca."
        );
    }
}