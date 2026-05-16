using MassTransit;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NotificationsAPI.Application.Ports;
using NotificationsAPI.Application.UseCases;
using NotificationsAPI.Domain.Entities;
using NotificationsAPI.Domain.Services;
using NotificationsAPI.Infrastructure.Messaging.Consumers;
using Shared.Contracts.Events;

public class PaymentProcessedConsumerTests
{
    private readonly Mock<IEmailSender> _emailSenderMock = new();
    private readonly Mock<INotificationDomainService> _domainServiceMock = new();

    [Fact]
    public async Task Consume_ShouldSendPurchaseConfirmation_WhenPaymentIsApproved()
    {
        const string expectedEmail = "user@email.com";
        const string status = "Approved";
        _domainServiceMock
            .Setup(x => x.CreatePurchaseNotification(expectedEmail, status))
            .Returns(new Notification("PURCHASE_CONFIRMED", expectedEmail));
        var consumer = CreateConsumer();
        var context = CreateContext(status);

        await consumer.Consume(context.Object);

        _emailSenderMock.Verify(
            x => x.SendAsync(
                expectedEmail,
                "Compra confirmada",
                "Seu jogo foi adicionado à sua biblioteca."),
            Times.Once);
    }

    [Fact]
    public async Task Consume_ShouldNotSendEmail_WhenPaymentIsNotApproved()
    {
        _domainServiceMock
            .Setup(x => x.CreatePurchaseNotification(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((Notification?)null);
        var consumer = CreateConsumer();
        var context = CreateContext("Rejected");

        await consumer.Consume(context.Object);

        _emailSenderMock.Verify(
            x => x.SendAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task Consume_ShouldRethrow_WhenUseCaseFails()
    {
        _domainServiceMock
            .Setup(x => x.CreatePurchaseNotification(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(new Notification("PURCHASE_CONFIRMED", "user@email.com"));
        _emailSenderMock
            .Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("SMTP unavailable"));
        var consumer = CreateConsumer();
        var context = CreateContext("Approved");

        await Assert.ThrowsAsync<InvalidOperationException>(() => consumer.Consume(context.Object));
    }

    private PaymentProcessedConsumer CreateConsumer()
    {
        var useCase = new SendPurchaseConfirmationUseCase(
            _domainServiceMock.Object,
            _emailSenderMock.Object);

        return new PaymentProcessedConsumer(
            useCase,
            NullLogger<PaymentProcessedConsumer>.Instance);
    }

    private static Mock<ConsumeContext<PaymentProcessedEvent>> CreateContext(string status)
    {
        var contextMock = new Mock<ConsumeContext<PaymentProcessedEvent>>();
        contextMock
            .Setup(x => x.Message)
            .Returns(new PaymentProcessedEvent(
                Guid.NewGuid(),
                Guid.NewGuid(),
                Guid.NewGuid(),
                status));
        contextMock.Setup(x => x.CorrelationId).Returns(Guid.NewGuid());

        return contextMock;
    }
}
