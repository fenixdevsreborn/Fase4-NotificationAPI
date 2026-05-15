using Moq;
using NotificationsAPI.Application.UseCases;
using NotificationsAPI.Application.Ports;
using NotificationsAPI.Domain.Services;
using Xunit;

public class SendWelcomeEmailUseCaseTests
{
    private readonly Mock<IEmailSender> _emailSenderMock;
    private readonly Mock<INotificationDomainService> _domainServiceMock;

    public SendWelcomeEmailUseCaseTests()
    {
        _emailSenderMock = new Mock<IEmailSender>();
        _domainServiceMock = new Mock<INotificationDomainService>();
    }

    [Fact]
    public async Task Should_send_welcome_email_when_email_is_valid()
    {
        _domainServiceMock
            .Setup(x => x.IsValidEmail(It.IsAny<string>()))
            .Returns(true);

        var useCase = new SendWelcomeEmailUseCase(
            _domainServiceMock.Object,
            _emailSenderMock.Object);

        await useCase.ExecuteAsync(
            "teste@exemplo.com");

        _emailSenderMock.Verify(
            x => x.SendAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Once);
    }
}