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
    [Authorize(AuthenticationSchemes = "Basic")]
    [ProducesResponseType(typeof(JwtGetDto), StatusCodes.Status200OK)]
    public IActionResult GenerateToken()
    {
        try
        {
            var username = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new AuthenticationFailureException("Username not found in JWT.");
            var role = User.FindFirstValue(ClaimTypes.Role)
                ?? throw new AuthenticationFailureException("Role not found in JWT.");

            _logger.LogInformation("Generating a new JWT token.");

            return Ok(new JwtGetDto
            {
                Token = _service.GenerateToken(username, role)
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
