/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using WebApi.Exceptions;

namespace WebApi.Services;

public interface IJwtService
{
    string[] GenerateToken(
        string username, string role, bool createRefreshToken = false,
        int accessTokenExpirationHours = 6, int refreshTokenExpirationMonths = 1,
        params Claim[] additionalClaims);
    List<Claim> ValidateTokenAndGetClaims(string jwt);
}

public class JwtService(
    IConfiguration configuration, ILogger<JwtService> logger) : IJwtService
{
    private readonly string _secretKey = configuration["Jwt:SecretKey"]
        ?? throw new NullReferenceException("JWT Secret Key is not configured.");
    private readonly string _issuer = configuration["Jwt:Issuer"]
        ?? throw new NullReferenceException("JWT Issuer is not configured.");
    private readonly string _audience = configuration["Jwt:Audience"]
        ?? throw new NullReferenceException("JWT Audience is not configured.");
    private readonly ILogger<JwtService> _logger = logger;

    public string[] GenerateToken(
        string username, string role, bool createRefreshToken = false,
        int accessTokenExpirationHours = 6, int refreshTokenExpirationMonths = 1,
        params Claim[] additionalClaims)
    {
        var result = new string[2];
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, username),
            new(ClaimTypes.Role, role)
        };

        foreach (Claim claim in additionalClaims)
        {
            claims.Add(claim);
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var accessToken = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(accessTokenExpirationHours),
            signingCredentials: creds);

        result[0] = new JwtSecurityTokenHandler().WriteToken(accessToken);

        if (createRefreshToken)
        {
            var refreshToken = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMonths(refreshTokenExpirationMonths),
                signingCredentials: creds);

            result[1] = new JwtSecurityTokenHandler().WriteToken(refreshToken);
        }

        _logger.LogInformation("Token generated successfully.");
        return result;
    }

    public List<Claim> ValidateTokenAndGetClaims(string jwt)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_secretKey);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _issuer,
            ValidAudience = _audience,
            IssuerSigningKey = new SymmetricSecurityKey(key)
        };
        ClaimsPrincipal claimsPrincipal;

        try
        {
            claimsPrincipal = tokenHandler.ValidateToken(jwt, validationParameters, out _);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token validation failed.");
            throw new ServiceException("Token validation failed.", ex);
        }

        var claims = claimsPrincipal.Claims.ToList();

        _logger.LogInformation("Token validated successfully.");
        return claims;
    }
}
