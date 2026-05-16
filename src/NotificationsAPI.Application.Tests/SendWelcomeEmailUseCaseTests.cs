using Moq;
using NotificationsAPI.Application.Ports;
using NotificationsAPI.Application.UseCases;
using NotificationsAPI.Domain.Entities;
using NotificationsAPI.Domain.Services;

public class SendWelcomeEmailUseCaseTests
{
    private readonly Mock<IEmailSender> _emailSenderMock = new();
    private readonly Mock<INotificationDomainService> _domainServiceMock = new();

    [Fact]
    public async Task ExecuteAsync_ShouldSendWelcomeEmail_WhenEmailIsValid()
    {
        const string email = "teste@exemplo.com";
        _domainServiceMock
            .Setup(x => x.IsValidEmail(email))
            .Returns(true);
        _domainServiceMock
            .Setup(x => x.CreateWelcomeNotification(email))
            .Returns(new Notification("WELCOME", email));

        var useCase = new SendWelcomeEmailUseCase(
            _domainServiceMock.Object,
            _emailSenderMock.Object);

        await useCase.ExecuteAsync(email);

        _domainServiceMock.Verify(x => x.CreateWelcomeNotification(email), Times.Once);
        _emailSenderMock.Verify(
            x => x.SendAsync(
                email,
                "Bem-vindo à FIAP Cloud Games",
                "Olá! Seja bem-vindo à plataforma."),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ShouldNotSendEmail_WhenEmailIsInvalid()
    {
        const string email = "email-invalido";
        _domainServiceMock
            .Setup(x => x.IsValidEmail(email))
            .Returns(false);

        var useCase = new SendWelcomeEmailUseCase(
            _domainServiceMock.Object,
            _emailSenderMock.Object);

        await useCase.ExecuteAsync(email);

        _domainServiceMock.Verify(x => x.CreateWelcomeNotification(It.IsAny<string>()), Times.Never);
        _emailSenderMock.Verify(
            x => x.SendAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }
}
