using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WebApi.Models.DataTransferObjects;
using WebApi.Services;

namespace WebApi.Controllers;

[Route("/api/jwt")]
[ApiController]
public class JwtController(IJwtService service) : ControllerBase
{
    private readonly IJwtService _service = service;

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

            return Ok(new JwtGetDto
            {
                Token = _service.GenerateToken(username, role)
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "TokenGenerationFailed"
            });
        }
    }
}
