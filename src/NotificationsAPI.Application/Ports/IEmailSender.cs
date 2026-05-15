namespace NotificationsAPI.Application.Ports;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string body);
}