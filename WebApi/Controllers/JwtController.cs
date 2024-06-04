/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WebApi.Models.DataTransferObjects;
using WebApi.Services;

namespace WebApi.Controllers;

[Route("/api/jwt")]
[ApiController]
public class JwtController(IJwtService service, ILogger<JwtController> logger) : ControllerBase
{
    private readonly IJwtService _service = service;
    private readonly ILogger<JwtController> _logger = logger;

    [HttpGet]
    [Authorize(AuthenticationSchemes = "Basic", Roles = "Admin,User,PaymentGateway")]
    [ProducesResponseType(typeof(JwtGetDto), StatusCodes.Status200OK)]
    public IActionResult GenerateToken()
    {
        try
        {
            var username = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new AuthenticationFailureException("Username not found in JWT.");
            var role = User.FindFirstValue(ClaimTypes.Role)
                ?? throw new AuthenticationFailureException("Role not found in JWT.");

            _logger.LogInformation("Generating a new JWT token pair.");

            var tokens = _service.GenerateToken(username, role, true);

            return Ok(new JwtGetDto
            {
                AccessToken = tokens[0],
                RefreshToken = tokens[1]
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate a new JWT token.");
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "TokenGenerationFailed"
            });
        }
    }

    [HttpGet("refresh")]
    [Authorize(AuthenticationSchemes = "Bearer", Roles = "Admin,User")]
    [ProducesResponseType(typeof(JwtRefreshDto), StatusCodes.Status200OK)]
    public IActionResult RefreshToken()
    {
        try
        {
            var username = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new AuthenticationFailureException("Username not found in JWT.");
            var role = User.FindFirstValue(ClaimTypes.Role)
                ?? throw new AuthenticationFailureException("Role not found in JWT.");

            _logger.LogInformation("Generating a new JWT token.");
            return Ok(new JwtRefreshDto
            {
                AccessToken = _service.GenerateToken(username, role)[0]
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate a new JWT token.");
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "TokenGenerationFailed"
            });
        }
    }

}
