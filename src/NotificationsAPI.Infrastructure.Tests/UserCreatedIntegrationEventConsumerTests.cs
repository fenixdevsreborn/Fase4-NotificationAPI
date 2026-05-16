using MassTransit;
using Moq;
using NotificationsAPI.Application.Ports;
using NotificationsAPI.Application.UseCases;
using NotificationsAPI.Domain.Entities;
using NotificationsAPI.Domain.Services;
using NotificationsAPI.Infrastructure.Messaging.Consumers;
using Shared.Contracts.Events;

public class UserCreatedIntegrationEventConsumerTests
{
    private readonly Mock<IEmailSender> _emailSenderMock = new();
    private readonly Mock<INotificationDomainService> _domainServiceMock = new();

    [Fact]
    public async Task Consume_ShouldSendWelcomeEmail_WhenUserCreatedEventHasValidEmail()
    {
        const string email = "novo.usuario@exemplo.com";
        _domainServiceMock.Setup(x => x.IsValidEmail(email)).Returns(true);
        _domainServiceMock
            .Setup(x => x.CreateWelcomeNotification(email))
            .Returns(new Notification("WELCOME", email));
        var consumer = CreateConsumer();
        var contextMock = new Mock<ConsumeContext<UserCreatedIntegrationEvent>>();
        contextMock
            .Setup(x => x.Message)
            .Returns(new UserCreatedIntegrationEvent(Guid.NewGuid(), email, DateTime.UtcNow));

        await consumer.Consume(contextMock.Object);

        _emailSenderMock.Verify(
            x => x.SendAsync(
                email,
                "Bem-vindo à FIAP Cloud Games",
                "Olá! Seja bem-vindo à plataforma."),
            Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldSendWelcomeEmail_WhenEmailNotificationEventHasValidRecipient()
    {
        const string email = "destino@exemplo.com";
        _domainServiceMock.Setup(x => x.IsValidEmail(email)).Returns(true);
        _domainServiceMock
            .Setup(x => x.CreateWelcomeNotification(email))
            .Returns(new Notification("WELCOME", email));
        var consumer = CreateConsumer();
        var contextMock = new Mock<ConsumeContext<EmailNotificationEvent>>();
        contextMock
            .Setup(x => x.Message)
            .Returns(new EmailNotificationEvent { Recipient = email });

        await consumer.Consume(contextMock.Object);

        _emailSenderMock.Verify(
            x => x.SendAsync(
                email,
                "Bem-vindo à FIAP Cloud Games",
                "Olá! Seja bem-vindo à plataforma."),
            Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldNotSendEmail_WhenUserCreatedEventHasInvalidEmail()
    {
        const string email = "email-invalido";
        _domainServiceMock.Setup(x => x.IsValidEmail(email)).Returns(false);
        var consumer = CreateConsumer();
        var contextMock = new Mock<ConsumeContext<UserCreatedIntegrationEvent>>();
        contextMock
            .Setup(x => x.Message)
            .Returns(new UserCreatedIntegrationEvent(Guid.NewGuid(), email, DateTime.UtcNow));

        await consumer.Consume(contextMock.Object);

        _emailSenderMock.Verify(
            x => x.SendAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    private UserCreatedIntegrationEventConsumer CreateConsumer()
    {
        var useCase = new SendWelcomeEmailUseCase(
            _domainServiceMock.Object,
            _emailSenderMock.Object);

        return new UserCreatedIntegrationEventConsumer(useCase);
    }
}
