using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WebApi.Exceptions;
using WebApi.Models.DataTransferObjects;
using WebApi.Services;

namespace WebApi.Controllers;

[Route("/api/seat")]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[ApiController]
public class SeatController : ControllerBase
{
    private readonly ISeatService _service;

    public SeatController(ISeatService service)
    {
        _service = service;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(SeatAddUpdateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] SeatAddUpdateDto dtoEntity)
    {
        try
        {
            return Ok(await _service.Add(dtoEntity));
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

    [HttpDelete("{sid}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(
        [FromRoute] string sid, [FromBody] SeatRequestDto dtoEntity)
    {
        try
        {
            await _service.Delete(dtoEntity.TheaterId, dtoEntity.ScreenId, sid);
            return NoContent();
        }
        catch (ServiceException ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "EntityNotFound"
            });
        }
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<SeatListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll([FromQuery] int tid, [FromQuery] int scid)
    {
        try
        {
            return Ok(await _service.GetAll(tid, scid));
        }
        catch (ServiceException ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "DatabaseFetchFailed"
            });
        }
    }

    [HttpGet("{sid}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(SeatGetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById(
        [FromRoute] string sid, [FromQuery] int tid, [FromQuery] int scid)
    {
        try
        {
            return Ok(await _service.GetById(
                tid, scid, sid));
        }
        catch (ServiceException ex)
        {
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "EntityNotFound"
            });
        }
    }
}
