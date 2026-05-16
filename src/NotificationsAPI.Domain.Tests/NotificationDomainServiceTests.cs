using NotificationsAPI.Domain.Services;

public class NotificationDomainServiceTests
{
    [Theory]
    [InlineData("teste@exemplo.com")]
    [InlineData("usuario.nome+tag@dominio.com.br")]
    public void IsValidEmail_ShouldReturnTrue_WhenEmailIsValid(string email)
    {
        var service = new NotificationDomainService();

        var result = service.IsValidEmail(email);

        Assert.True(result);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("email-invalido")]
    [InlineData("teste@")]
    [InlineData("@exemplo.com")]
    public void IsValidEmail_ShouldReturnFalse_WhenEmailIsInvalid(string email)
    {
        var service = new NotificationDomainService();

        var result = service.IsValidEmail(email);

        Assert.False(result);
    }

    [Fact]
    public void CreateWelcomeNotification_ShouldCreateWelcomeNotification()
    {
        var service = new NotificationDomainService();

        var notification = service.CreateWelcomeNotification("teste@exemplo.com");

        Assert.Equal("WELCOME", notification.Type);
        Assert.Equal("teste@exemplo.com", notification.Recipient);
        Assert.NotEqual(Guid.Empty, notification.Id);
    }

    [Fact]
    public void CreatePurchaseNotification_ShouldCreatePurchaseConfirmedNotification_WhenPaymentIsApproved()
    {
        var service = new NotificationDomainService();

        var notification = service.CreatePurchaseNotification("teste@exemplo.com", "Approved");

        Assert.NotNull(notification);
        Assert.Equal("PURCHASE_CONFIRMED", notification.Type);
        Assert.Equal("teste@exemplo.com", notification.Recipient);
    }

    [Fact]
    public void CreatePurchaseNotification_ShouldReturnNull_WhenPaymentIsNotApproved()
    {
        var service = new NotificationDomainService();

        var notification = service.CreatePurchaseNotification("teste@exemplo.com", "Rejected");

        Assert.Null(notification);
    }
}
