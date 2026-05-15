using NotificationsAPI.Domain.Entities;
using System.Text.RegularExpressions;

namespace NotificationsAPI.Domain.Services;

public class NotificationDomainService : INotificationDomainService
{
    public Notification CreateWelcomeNotification(string email)
        => new("WELCOME", email);

    public Notification? CreatePurchaseNotification(string email, string paymentStatus)
    {
        if (paymentStatus != "Approved")
            return null;

        return new Notification("PURCHASE_CONFIRMED", email);
    }

    public bool IsValidEmail(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return false;

        var pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, pattern, RegexOptions.IgnoreCase);
    }
}