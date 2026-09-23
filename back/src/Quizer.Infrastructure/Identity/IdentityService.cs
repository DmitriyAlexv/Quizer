using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Quizer.Abstractions.Auth;

namespace Quizer.Infrastructure.Identity;

/// <summary>
/// Реализация сервиса Identity на основе ASP.NET Core Identity и JWT.
/// </summary>
public class IdentityService(
    UserManager<IdentityUser<Guid>> userManager,
    IOptions<JwtOptions> jwtOptions) : IIdentityService
{
    public async Task<Guid> RegisterAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = new IdentityUser<Guid>
        {
            UserName = email,
            Email = email,
            Id = Guid.NewGuid()
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException(
                $"Не удалось зарегистрировать пользователя: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }

        return user.Id;
    }

    public async Task<string> AuthorizeAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email)
                   ?? throw new InvalidOperationException("Пользователь с указанным email не найден.");

        var isValidPassword = await userManager.CheckPasswordAsync(user, password);
        if (!isValidPassword)
        {
            throw new InvalidOperationException("Неверный пароль.");
        }

        return GenerateToken(user);
    }

    private string GenerateToken(IdentityUser<Guid> user)
    {
        var options = jwtOptions.Value;

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(options.ExpiresInMinutes),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
