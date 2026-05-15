namespace Shared.Contracts.Events
{
    public class PaymentProcessedEvent
    {
        public Guid OrderId { get; private set; }
        public Guid UserId { get; private set; }
        public Guid GameId { get; private set; }
        public decimal Price { get; private set; }
        public string Status { get; private set; } = string.Empty;
        public DateTime ProcessedAt { get; private set; }

        protected PaymentProcessedEvent() { }

        public PaymentProcessedEvent(
            Guid orderId,
            Guid userId,
            Guid gameId,
            decimal price,
            string status,
            DateTime processedAt)
        {
            OrderId = orderId;
            UserId = userId;
            GameId = gameId;
            Price = price;
            Status = status;
            ProcessedAt = processedAt;
        }
    }

}
