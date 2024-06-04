/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace WebApi.Authentication;
using WebApi.Exceptions;
using WebApi.Models;
using WebApi.Services;

public class BasicAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        ILoggerFactory logger,
        UrlEncoder encoder,
        IUserAccountService userService)
    : AuthenticationHandler<AuthenticationSchemeOptions>(
        options, logger, encoder)
{
    private readonly IUserAccountService _userService = userService;

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.ContainsKey("Authorization"))
        {
            return AuthenticateResult.Fail("Authorization header is missing.");
        }

        string? authorizationHeader = Request.Headers.Authorization;

        if (string.IsNullOrEmpty(authorizationHeader))
        {
            return AuthenticateResult.Fail("Authorization header is empty.");
        }

        if (!authorizationHeader.StartsWith("Basic ", StringComparison.OrdinalIgnoreCase))
        {
            return AuthenticateResult.Fail("Invalid authorization header.");
        }

        var token = authorizationHeader.Substring(6);
        var credentialAsString = Encoding.UTF8.GetString(Convert.FromBase64String(token));
        var credentials = credentialAsString.Split(":");

        if (credentials?.Length != 2)
        {
            return AuthenticateResult.Fail("Invalid authorization header.");
        }

        var username = credentials[0];
        var password = credentials[1];
        UserAccount user;

        try
        {
            user = await _userService.ValidateUser(username, password);
        }
        catch (ServiceException)
        {
            return AuthenticateResult.Fail("Invalid username or password.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, username),
            new Claim(ClaimTypes.Role, user.Role)
        };

        var identity = new ClaimsIdentity(claims, "Basic");
        var claimsPrincipal = new ClaimsPrincipal(identity); // Create HTTP Context
        return AuthenticateResult.Success(new AuthenticationTicket(claimsPrincipal, Scheme.Name));
    }
}
