using Moq;
using NotificationsAPI.Application.Ports;
using NotificationsAPI.Application.UseCases;
using NotificationsAPI.Domain.Entities;
using NotificationsAPI.Domain.Services;

public class SendPurchaseConfirmationUseCaseTests
{
    private readonly Mock<IEmailSender> _emailSenderMock = new();
    private readonly Mock<INotificationDomainService> _domainServiceMock = new();

    [Fact]
    public async Task ExecuteAsync_ShouldSendPurchaseConfirmation_WhenNotificationIsCreated()
    {
        const string email = "teste@exemplo.com";
        const string status = "Approved";
        _domainServiceMock
            .Setup(x => x.CreatePurchaseNotification(email, status))
            .Returns(new Notification("PURCHASE_CONFIRMED", email));

        var useCase = new SendPurchaseConfirmationUseCase(
            _domainServiceMock.Object,
            _emailSenderMock.Object);

        await useCase.ExecuteAsync(email, status);

        _emailSenderMock.Verify(
            x => x.SendAsync(
                email,
                "Compra confirmada",
                "Seu jogo foi adicionado à sua biblioteca."),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotSendEmail_WhenNotificationIsNotCreated()
    {
        _domainServiceMock
            .Setup(x => x.CreatePurchaseNotification(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((Notification?)null);

        var useCase = new SendPurchaseConfirmationUseCase(
            _domainServiceMock.Object,
            _emailSenderMock.Object);

        await useCase.ExecuteAsync("teste@exemplo.com", "Rejected");

        _emailSenderMock.Verify(
            x => x.SendAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }
}
