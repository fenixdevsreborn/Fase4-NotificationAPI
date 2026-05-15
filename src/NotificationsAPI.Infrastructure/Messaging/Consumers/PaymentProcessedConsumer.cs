using MassTransit;
using Microsoft.Extensions.Logging;
using NotificationsAPI.Application.UseCases;
using Shared.Contracts.Events;

namespace NotificationsAPI.Infrastructure.Messaging.Consumers;

public class PaymentProcessedConsumer : IConsumer<PaymentProcessedEvent>
{
    private readonly SendPurchaseConfirmationUseCase _useCase;
    private readonly ILogger<PaymentProcessedConsumer> _logger;

    public PaymentProcessedConsumer(
        SendPurchaseConfirmationUseCase useCase,
        ILogger<PaymentProcessedConsumer> logger)
    {
        _useCase = useCase;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<PaymentProcessedEvent> context)
    {
        var message = context.Message;
        var correlationId = context.CorrelationId ?? context.ConversationId;

        _logger.LogInformation(
            "[NotificationsAPI] PaymentProcessedEvent recebido. OrderId: {OrderId}, Status: {Status}, UserId: {UserId}, GameId: {GameId}, CorrelationId: {CorrelationId}",
            message.OrderId, message.Status, message.UserId, message.GameId, correlationId);

        try
        {
            await _useCase.ExecuteAsync(
                email: "user@email.com",
                paymentStatus: message.Status
            );

            _logger.LogInformation(
                "[NotificationsAPI] Notificação de pagamento processada com sucesso. OrderId: {OrderId}, Status: {Status}, CorrelationId: {CorrelationId}",
                message.OrderId, message.Status, correlationId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "[NotificationsAPI] Erro ao processar notificação de pagamento. OrderId: {OrderId}, Status: {Status}, CorrelationId: {CorrelationId}",
                message.OrderId, message.Status, correlationId);
            throw;
        }
    }
}