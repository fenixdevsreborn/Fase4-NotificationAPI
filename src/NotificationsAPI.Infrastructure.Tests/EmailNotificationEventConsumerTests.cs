using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NotificationsAPI.Infrastructure.Email;
using NotificationsAPI.Infrastructure.Messaging.Consumers;
using Shared.Contracts.Events;

public class EmailNotificationEventConsumerTests
{
    private readonly Mock<IEmailSender> _emailSenderMock = new();

    [Fact]
    public async Task Consume_ShouldSendEmail_WithMessageData()
    {
        var consumer = new EmailNotificationEventConsumer(
            _emailSenderMock.Object,
            NullLogger<EmailNotificationEventConsumer>.Instance);
        var contextMock = new Mock<ConsumeContext<EmailNotificationEvent>>();
        contextMock
            .Setup(x => x.Message)
            .Returns(new EmailNotificationEvent
            {
                Recipient = "user@example.com",
                Title = "Compra confirmada",
                Subtitle = "Pedido aprovado",
                Body = "Seu jogo foi adicionado a sua biblioteca."
            });

        await consumer.Consume(contextMock.Object);

        _emailSenderMock.Verify(
            x => x.SendAsync(
                "user@example.com",
                "Compra confirmada",
                $"Pedido aprovado{Environment.NewLine}{Environment.NewLine}Seu jogo foi adicionado a sua biblioteca."),
            Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldSendOnlyBody_WhenSubtitleIsEmpty()
    {
        var consumer = new EmailNotificationEventConsumer(
            _emailSenderMock.Object,
            NullLogger<EmailNotificationEventConsumer>.Instance);
        var contextMock = new Mock<ConsumeContext<EmailNotificationEvent>>();
        contextMock
            .Setup(x => x.Message)
            .Returns(new EmailNotificationEvent
            {
                Recipient = "user@example.com",
                Title = "Notificacao",
                Body = "Mensagem simples."
            });

        await consumer.Consume(contextMock.Object);

        _emailSenderMock.Verify(
            x => x.SendAsync(
                "user@example.com",
                "Notificacao",
                "Mensagem simples."),
            Times.Once);
    }
}
