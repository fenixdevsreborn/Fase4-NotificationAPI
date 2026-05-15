using NotificationsAPI.Domain.Entities;

namespace NotificationsAPI.Domain.Services;

public interface INotificationDomainService
{
    Notification CreateWelcomeNotification(string email);
    Notification? CreatePurchaseNotification(string email, string paymentStatus);

    bool IsValidEmail(string email);
}