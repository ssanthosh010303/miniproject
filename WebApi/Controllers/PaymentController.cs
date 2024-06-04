/*
 * Author: Sakthi Santhosh
 * Created on: 01/05/2024
 */
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WebApi.Exceptions;
using WebApi.Models.DataTransferObjects;
using WebApi.Services;

namespace WebApi.Controllers;

[Route("/api/payment")]
[ApiController]
public class PaymentController(IPaymentService service) : ControllerBase
{
    private readonly IPaymentService _service = service;

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "User")]
    [ProducesResponseType(typeof(PaymentAddDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] PaymentAddDto dtoEntity)
    {
        try
        {
            var result = await _service.RegisterPayment(User.FindFirstValue(
                ClaimTypes.NameIdentifier
            ) ?? throw new ServiceException("User not found."), dtoEntity);

            return Ok(result);
        }
        catch (ServiceValidationException ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "ValidationFailed"
            });
        }
        catch (ServiceException ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "UpdateError"
            });
        }
    }
}
