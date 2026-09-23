namespace Quizer.Abstractions.Auth;

/// <summary>
/// Настройки JWT-токена.
/// </summary>
public class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    public string SecretKey { get; set; } = string.Empty;

    public int ExpiresInMinutes { get; set; } = 60;
}
