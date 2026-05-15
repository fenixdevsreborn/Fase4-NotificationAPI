using NotificationsAPI.Domain.Services;
using Xunit;

public class NotificationDomainServiceTests
{
    [Fact]
    public void Should_validate_email_correctly()
    {
        var service = new NotificationDomainService();

        var result = service.IsValidEmail("teste@exemplo.com");

        Assert.True(result);
    }

    [Fact]
    public void Should_return_false_for_invalid_email()
    {
        var service = new NotificationDomainService();

        var result = service.IsValidEmail("email-invalido");

        Assert.False(result);
    }
}