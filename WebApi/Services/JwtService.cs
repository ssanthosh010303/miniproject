using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebApi.Services;

public interface IJwtService
{
    string GenerateToken(string username, string role);
}

public class JwtService(IConfiguration configuration) : IJwtService
{
    private readonly string _secretKey = configuration["Jwt:SecretKey"]
        ?? throw new NullReferenceException("JWT Secret Key is not configured.");
    private readonly string _issuer = configuration["Jwt:Issuer"]
        ?? throw new NullReferenceException("JWT Issuer is not configured.");
    private readonly string _audience = configuration["Jwt:Audience"]
        ?? throw new NullReferenceException("JWT Audience is not configured.");

    public string GenerateToken(string username, string role)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, username),
            new Claim(ClaimTypes.Role, role)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.Now.AddHours(6),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
