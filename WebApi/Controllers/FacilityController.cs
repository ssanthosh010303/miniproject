using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using WebApi.Exceptions;
using WebApi.Models.DataTransferObjects;
using WebApi.Services;

namespace WebApi.Controllers;

[Route("/api/facility")]
[ApiController]
public class FacilityController : ControllerBase
{
    private readonly IFacilityService _service;
    private readonly ILogger<FacilityController> _logger;

    public FacilityController(IFacilityService service, ILogger<FacilityController> logger)
    {
        _service = service;
        _logger = logger;
    }

    [HttpPost]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ProducesResponseType(typeof(FacilityAddUpdateDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FacilityAddUpdateDto), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FacilityAddUpdateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Add([FromBody] FacilityAddUpdateDto dtoEntity)
    {
        try
        {
            _logger.LogInformation("Adding a new facility.");
            return Ok(await _service.Add(dtoEntity));
        }
        catch (ServiceValidationException ex)
        {
            _logger.LogError(ex, "Validation failed while adding a new facility.");
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "ValidationFailed"
            });
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Failed to add a new facility.");
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "UpdateError"
            });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] int id)
    {
        try
        {
            _logger.LogInformation("Deleting a facility.");
            await _service.Delete(id);
            return NoContent();
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Failed to delete a facility.");
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "EntityNotFound"
            });
        }
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(IEnumerable<FacilityListDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            _logger.LogInformation("Fetching all facilities.");
            return Ok(await _service.GetAll());
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Failed to fetch all facilities.");
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "DatabaseFetchFailed"
            });
        }
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(FacilityGetDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetById([FromRoute] int id)
    {
        try
        {
            _logger.LogInformation("Fetching a facility by ID.");
            return Ok(await _service.GetById(id));
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Failed to fetch a facility by ID.");
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "EntityNotFound"
            });
        }
    }

    [HttpPut("{id}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
    [ProducesResponseType(typeof(FacilityAddUpdateDto), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(FacilityAddUpdateDto), StatusCodes.Status403Forbidden)]
    [ProducesResponseType(typeof(FacilityAddUpdateDto), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update([FromRoute] int id, [FromBody] FacilityAddUpdateDto dtoEntity)
    {
        try
        {
            _logger.LogInformation("Updating a facility.");
            return Ok(await _service.Update(id, dtoEntity));
        }
        catch (ServiceValidationException ex)
        {
            _logger.LogError(ex, "Validation failed while updating a facility.");
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "ValidationFailed"
            });
        }
        catch (ServiceException ex)
        {
            _logger.LogError(ex, "Failed to update a facility.");
            return BadRequest(new ErrorResponseDto
            {
                Message = ex.Message,
                ErrorCode = "EntityNotFound"
            });
        }
    }
}
