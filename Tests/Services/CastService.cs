using WebApi.Services;

public class CryptographyServiceTests
{
    private readonly ICryptographyService _cryptographyService;

    public CryptographyServiceTests()
    {
        _cryptographyService = new CryptographyService();
    }

    [Fact]
    public void HashPassword_ReturnsNonEmptyString()
    {
        string password = "password";
        string hashedPassword = _cryptographyService.HashPassword(password);

        Assert.That(hashedPassword, Is.Not.Empty);
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
    {
        string password = "password";
        string hashedPassword = _cryptographyService.HashPassword(password);

        bool result = _cryptographyService.VerifyPassword(password, hashedPassword);
        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
    {
        string password = "password";
        string incorrectPassword = "incorrectPassword";
        string hashedPassword = _cryptographyService.HashPassword(password);

        bool result = _cryptographyService.VerifyPassword(incorrectPassword, hashedPassword);

        Assert.That(result, Is.False);
    }
}

internal class FactAttribute : Attribute
{
}
