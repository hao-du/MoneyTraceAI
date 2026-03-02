using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using MoneyTrace.Application.Authentication;
using MoneyTrace.Domain.Entities;

namespace MoneyTrace.Infrastructure.Authentication;

internal sealed class JwtProvider : IJwtProvider
{
    private readonly IConfiguration _configuration;

    public JwtProvider(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GenerateToken(User user)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Username)
        };

        // Standard dummy key and fallback limits for MVP
        // Note: Production would use injected secrets.
        var secret = _configuration["Jwt:Secret"] ?? "super-secret-key-that-needs-to-be-at-least-256-bits-long-!";
        var issuer = _configuration["Jwt:Issuer"] ?? "MoneyTraceApi";
        var audience = _configuration["Jwt:Audience"] ?? "MoneyTraceClient";

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            null,
            DateTime.UtcNow.AddHours(2),
            signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
